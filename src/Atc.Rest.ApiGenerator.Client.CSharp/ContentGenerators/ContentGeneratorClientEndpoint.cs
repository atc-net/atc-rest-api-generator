// ReSharper disable SwitchExpressionHandlesSomeKnownEnumValuesWithExceptionInDefault
// ReSharper disable ConvertIfStatementToSwitchStatement
// ReSharper disable ConvertIfStatementToConditionalTernaryExpression
namespace Atc.Rest.ApiGenerator.Client.CSharp.ContentGenerators;

public class ContentGeneratorClientEndpoint : IContentGenerator
{
    private readonly GeneratedCodeHeaderGenerator codeHeaderGenerator;
    private readonly GeneratedCodeAttributeGenerator codeAttributeGenerator;
    private readonly CodeDocumentationTagsGenerator codeDocumentationTagsGenerator;
    private readonly ContentGeneratorClientEndpointParameters parameters;
    private readonly bool useProblemDetailsAsDefaultResponseBody;
    private readonly string? customErrorResponseModel;

    public ContentGeneratorClientEndpoint(
        GeneratedCodeHeaderGenerator codeHeaderGenerator,
        GeneratedCodeAttributeGenerator codeAttributeGenerator,
        CodeDocumentationTagsGenerator codeDocumentationTagsGenerator,
        ContentGeneratorClientEndpointParameters parameters,
        bool useProblemDetailsAsDefaultResponseBody,
        string? customErrorResponseModel)
    {
        this.codeHeaderGenerator = codeHeaderGenerator;
        this.codeAttributeGenerator = codeAttributeGenerator;
        this.codeDocumentationTagsGenerator = codeDocumentationTagsGenerator;
        this.parameters = parameters;
        this.useProblemDetailsAsDefaultResponseBody = useProblemDetailsAsDefaultResponseBody;
        this.customErrorResponseModel = customErrorResponseModel;
    }

    public string Generate()
    {
        var sb = new StringBuilder();

        sb.Append(codeHeaderGenerator.Generate());
        sb.AppendLine($"namespace {parameters.Namespace};");
        sb.AppendLine();
        if (codeDocumentationTagsGenerator.ShouldGenerateTags(parameters.DocumentationTags))
        {
            sb.Append(codeDocumentationTagsGenerator.GenerateTags(0, parameters.DocumentationTags));
        }

        sb.AppendLine(codeAttributeGenerator.Generate());
        sb.AppendLine($"public class {parameters.EndpointName} : {parameters.InterfaceName}");
        sb.AppendLine("{");
        sb.AppendLine(4, "private readonly IHttpClientFactory factory;");
        sb.AppendLine(4, "private readonly IHttpMessageFactory httpMessageFactory;");
        sb.AppendLine();
        sb.AppendLine(4, $"public {parameters.EndpointName}(");
        sb.AppendLine(8, "IHttpClientFactory factory,");
        sb.AppendLine(8, "IHttpMessageFactory httpMessageFactory)");
        sb.AppendLine(4, "{");
        sb.AppendLine(8, "this.factory = factory;");
        sb.AppendLine(8, "this.httpMessageFactory = httpMessageFactory;");
        sb.AppendLine(4, "}");
        sb.AppendLine();
        sb.AppendLine(4, $"public async Task<{parameters.ResultName}> ExecuteAsync(");
        if (parameters.ParameterName is not null)
        {
            sb.AppendLine(8, $"{parameters.ParameterName} parameters,");
        }

        sb.AppendLine(8, $"string httpClientName = \"{parameters.HttpClientName}\",");
        sb.AppendLine(8, "CancellationToken cancellationToken = default)");
        sb.AppendLine(4, "{");
        sb.AppendLine(8, "var client = factory.CreateClient(httpClientName);");
        sb.AppendLine();
        sb.AppendLine(8, $"var requestBuilder = httpMessageFactory.FromTemplate(\"{parameters.UrlPath}\");");

        if (parameters.Parameters is not null && parameters.Parameters.Any())
        {
            for (var index = 0; index < parameters.Parameters.Count; index++)
            {
                var item = parameters.Parameters[index];
                var isLastParameter = index == parameters.Parameters.Count - 1;

                switch (item.ParameterLocationType)
                {
                    case ParameterLocationType.Query:
                        AppendParameterForParameterLocation(sb, item, item.ParameterLocationType, isLastParameter);
                        break;
                    case ParameterLocationType.Header:
                        AppendParameterForParameterLocation(sb, item, item.ParameterLocationType, isLastParameter);
                        break;
                    case ParameterLocationType.Route:
                        sb.AppendLine(8, $"requestBuilder.WithPathParameter(\"{item.Name}\", parameters.{item.ParameterName});");
                        break;
                    case ParameterLocationType.Body:
                        sb.AppendLine(8, "requestBuilder.WithBody(parameters.Request);");
                        break;
                    case ParameterLocationType.Form:
                        sb.AppendLine(8, "// TODO: Imp. With-Form");
                        break;
                    default:
                        throw new SwitchCaseDefaultException(item.ParameterLocationType);
                }
            }
        }

        sb.AppendLine();
        sb.AppendLine(8, $"using var requestMessage = requestBuilder.Build(HttpMethod.{parameters.HttpMethod});");
        sb.AppendLine(8, "using var response = await client.SendAsync(requestMessage, cancellationToken);");
        sb.AppendLine();
        sb.AppendLine(8, "var responseBuilder = httpMessageFactory.FromResponse(response);");

        var responseModels = parameters.ResponseModels
            .AppendUnauthorizedIfNeeded(parameters.Authorization)
            .AppendForbiddenIfNeeded(parameters.Authorization)
            .AppendBadRequestIfNeeded(parameters.ParameterName)
            .OrderBy(x => x.StatusCode)
            .ToList();

        foreach (var responseModel in responseModels)
        {
            switch (responseModel.StatusCode)
            {
                case HttpStatusCode.OK:
                    AppendAddSuccessResponseForStatusCodeOk(sb, responseModel);
                    break;
                case HttpStatusCode.BadRequest:
                    if (string.IsNullOrEmpty(customErrorResponseModel))
                    {
                        sb.AppendLine(8, $"responseBuilder.AddErrorResponse<ValidationProblemDetails>(HttpStatusCode.{responseModel.StatusCode.ToNormalizedString()});");
                    }
                    else
                    {
                        sb.AppendLine(8, $"responseBuilder.AddErrorResponse<{customErrorResponseModel}>(HttpStatusCode.{responseModel.StatusCode.ToNormalizedString()});");
                    }

                    break;
                case HttpStatusCode.Continue:
                case HttpStatusCode.SwitchingProtocols:
                case HttpStatusCode.Processing:
                case HttpStatusCode.EarlyHints:
                case HttpStatusCode.Created:
                case HttpStatusCode.Accepted:
                case HttpStatusCode.NonAuthoritativeInformation:
                case HttpStatusCode.NoContent:
                case HttpStatusCode.ResetContent:
                case HttpStatusCode.PartialContent:
                case HttpStatusCode.MultiStatus:
                case HttpStatusCode.AlreadyReported:
                case HttpStatusCode.IMUsed:
                case HttpStatusCode.Ambiguous:
                case HttpStatusCode.Moved:
                case HttpStatusCode.Found:
                case HttpStatusCode.RedirectMethod:
                case HttpStatusCode.NotModified:
                case HttpStatusCode.UseProxy:
                case HttpStatusCode.Unused:
                case HttpStatusCode.RedirectKeepVerb:
                case HttpStatusCode.PermanentRedirect:
                case HttpStatusCode.Unauthorized:
                case HttpStatusCode.PaymentRequired:
                case HttpStatusCode.Forbidden:
                case HttpStatusCode.NotFound:
                case HttpStatusCode.MethodNotAllowed:
                case HttpStatusCode.NotAcceptable:
                case HttpStatusCode.ProxyAuthenticationRequired:
                case HttpStatusCode.RequestTimeout:
                case HttpStatusCode.Conflict:
                case HttpStatusCode.Gone:
                case HttpStatusCode.LengthRequired:
                case HttpStatusCode.PreconditionFailed:
                case HttpStatusCode.RequestEntityTooLarge:
                case HttpStatusCode.RequestUriTooLong:
                case HttpStatusCode.UnsupportedMediaType:
                case HttpStatusCode.RequestedRangeNotSatisfiable:
                case HttpStatusCode.ExpectationFailed:
                case HttpStatusCode.MisdirectedRequest:
                case HttpStatusCode.UnprocessableEntity:
                case HttpStatusCode.Locked:
                case HttpStatusCode.FailedDependency:
                case HttpStatusCode.UpgradeRequired:
                case HttpStatusCode.PreconditionRequired:
                case HttpStatusCode.TooManyRequests:
                case HttpStatusCode.RequestHeaderFieldsTooLarge:
                case HttpStatusCode.UnavailableForLegalReasons:
                case HttpStatusCode.InternalServerError:
                case HttpStatusCode.NotImplemented:
                case HttpStatusCode.BadGateway:
                case HttpStatusCode.ServiceUnavailable:
                case HttpStatusCode.GatewayTimeout:
                case HttpStatusCode.HttpVersionNotSupported:
                case HttpStatusCode.VariantAlsoNegotiates:
                case HttpStatusCode.InsufficientStorage:
                case HttpStatusCode.LoopDetected:
                case HttpStatusCode.NotExtended:
                case HttpStatusCode.NetworkAuthenticationRequired:
                    if (string.IsNullOrEmpty(customErrorResponseModel))
                    {
                        sb.AppendLine(
                            8,
                            useProblemDetailsAsDefaultResponseBody
                                ? $"responseBuilder.AddErrorResponse<ProblemDetails>(HttpStatusCode.{responseModel.StatusCode});"
                                : $"responseBuilder.AddErrorResponse<string>(HttpStatusCode.{responseModel.StatusCode});");
                    }
                    else
                    {
                        sb.AppendLine(8, $"responseBuilder.AddErrorResponse<{customErrorResponseModel}>(HttpStatusCode.{responseModel.StatusCode});");
                    }

                    break;
                default:
                    sb.AppendLine(4, $"// TODO: Not Implemented for {responseModel.StatusCode}.");
                    break;
            }
        }

        sb.AppendLine(8, $"return await responseBuilder.BuildResponseAsync(x => new {parameters.ResultName}(x), cancellationToken);");
        sb.AppendLine(4, "}");
        sb.Append('}');

        return sb.ToString();
    }

    private void AppendAddSuccessResponseForStatusCodeOk(
        StringBuilder sb,
        ApiOperationResponseModel responseModel)
    {
        if (responseModel.MediaType == MediaTypeNames.Application.Octet)
        {
            sb.AppendLine(8, "responseBuilder.AddSuccessResponse<byte[]>(HttpStatusCode.OK);");
            return;
        }

        if (string.IsNullOrEmpty(responseModel.DataType))
        {
            sb.AppendLine(8, "responseBuilder.AddSuccessResponse<string?>(HttpStatusCode.OK);");
            return;
        }

        var dataType = responseModel.GetQualifiedDataType();

        if (responseModel.CollectionDataType is null)
        {
            sb.AppendLine(8, $"responseBuilder.AddSuccessResponse<{responseModel.DataType}>(HttpStatusCode.OK);");
            return;
        }

        if (responseModel.CollectionDataType == NameConstants.List)
        {
            sb.AppendLine(8, $"responseBuilder.AddSuccessResponse<IEnumerable<{dataType}>>(HttpStatusCode.OK);");
        }
        else
        {
            sb.AppendLine(8, $"responseBuilder.AddSuccessResponse<{responseModel.CollectionDataType}<{dataType}>>(HttpStatusCode.OK);");
        }
    }

    private static void AppendParameterForParameterLocation(
        StringBuilder sb,
        ContentGeneratorClientEndpointParametersParameters item,
        ParameterLocationType parameterLocationType,
        bool isLastParameter)
    {
        if (item.IsList)
        {
            sb.AppendLine(8, $"if (parameters.{item.ParameterName}.Any())");
            sb.AppendLine(8, "{");
            sb.AppendLine(12, $"requestBuilder.With{parameterLocationType}Parameter(\"{item.Name}\", parameters.{item.ParameterName});");
            sb.AppendLine(8, "}");

            if (!isLastParameter)
            {
                sb.AppendLine();
            }
        }
        else
        {
            sb.AppendLine(8, $"requestBuilder.With{parameterLocationType}Parameter(\"{item.Name}\", parameters.{item.ParameterName});");
        }
    }
}