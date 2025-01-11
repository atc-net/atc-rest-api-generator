namespace Atc.Rest.ApiGenerator.Framework.Mvc.ContentGenerators;

public sealed class ContentGeneratorServerController : IContentGenerator
{
    private readonly GeneratedCodeHeaderGenerator codeHeaderGenerator;
    private readonly GeneratedCodeAttributeGenerator codeAttributeGenerator;
    private readonly CodeDocumentationTagsGenerator codeDocumentationTagsGenerator;
    private readonly ContentGeneratorServerEndpointParameters parameters;
    private readonly bool useProblemDetailsAsDefaultResponseBody;

    public ContentGeneratorServerController(
        GeneratedCodeHeaderGenerator codeHeaderGenerator,
        GeneratedCodeAttributeGenerator codeAttributeGenerator,
        CodeDocumentationTagsGenerator codeDocumentationTagsGenerator,
        ContentGeneratorServerEndpointParameters parameters,
        bool useProblemDetailsAsDefaultResponseBody)
    {
        this.codeHeaderGenerator = codeHeaderGenerator;
        this.codeAttributeGenerator = codeAttributeGenerator;
        this.codeDocumentationTagsGenerator = codeDocumentationTagsGenerator;
        this.parameters = parameters;
        this.useProblemDetailsAsDefaultResponseBody = useProblemDetailsAsDefaultResponseBody;
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

        if (parameters.Authorization is not null)
        {
            sb.AppendLine(parameters.Authorization.UseAllowAnonymous
                ? "[AllowAnonymous]"
                : "[Authorize]");
        }

        sb.AppendLine("[ApiController]");
        sb.AppendLine($"[Route(\"{parameters.RouteBase}\")]");
        sb.AppendLine(codeAttributeGenerator.Generate());

        sb.AppendLine($"{parameters.DeclarationModifier.GetDescription()} {parameters.ApiGroupName}Controller : ControllerBase");
        sb.AppendLine("{");
        AppendContent(sb);
        sb.Append('}');

        return sb.ToString();
    }

    private void AppendContent(
        StringBuilder sb)
    {
        for (var i = 0; i < parameters.MethodParameters.Count; i++)
        {
            var item = parameters.MethodParameters[i];

            if (codeDocumentationTagsGenerator.ShouldGenerateTags(item.DocumentationTags))
            {
                sb.Append(codeDocumentationTagsGenerator.GenerateTags(4, item.DocumentationTags));
            }

            StringBuilderEndpointHelper.AppendMethodContentAuthorizationIfNeeded(
                sb,
                parameters.Authorization,
                item.Authorization);

            sb.AppendLine(4, string.IsNullOrEmpty(item.RouteSuffix)
                ? $"[Http{item.OperationTypeRepresentation}]"
                : $"[Http{item.OperationTypeRepresentation}(\"{item.RouteSuffix}\")]");

            if (item.MultipartBodyLengthLimit.HasValue)
            {
                sb.AppendLine(4, item.MultipartBodyLengthLimit.Value == long.MaxValue
                    ? "[RequestFormLimits(MultipartBodyLengthLimit = long.MaxValue)]"
                    : $"[RequestFormLimits(MultipartBodyLengthLimit = {item.MultipartBodyLengthLimit.Value})]");
            }

            if (useProblemDetailsAsDefaultResponseBody)
            {
                AppendProducesWithProblemDetails(sb, item);
            }
            else
            {
                AppendProducesWithoutProblemDetails(sb, item);
            }

            sb.AppendLine(4, $"public async Task<ActionResult> {item.Name}(");

            if (!string.IsNullOrEmpty(item.ParameterTypeName))
            {
                sb.AppendLine(8, $"{item.ParameterTypeName} parameters,");
            }

            sb.AppendLine(8, $"[FromServices] {item.InterfaceName} handler,");
            sb.AppendLine(8, "CancellationToken cancellationToken)");

            sb.AppendLine(8, !string.IsNullOrEmpty(item.ParameterTypeName)
                ? "=> await handler.ExecuteAsync(parameters, cancellationToken);"
                : "=> await handler.ExecuteAsync(cancellationToken);");

            if (i < parameters.MethodParameters.Count - 1)
            {
                sb.AppendLine();
            }
        }
    }

    private static void AppendProducesWithProblemDetails(
        StringBuilder sb,
        ContentGeneratorServerEndpointMethodParameters item)
    {
        var responseModels = item.ResponseModels
            .AppendUnauthorizedIfNeeded(item.Authorization, item.IsAuthorizationRequiredFromPath)
            .AppendForbiddenIfNeeded(item.Authorization)
            .AppendBadRequestIfNeeded(item.ParameterTypeName)
            .OrderBy(x => x.StatusCode)
            .ToList();

        foreach (var responseModel in responseModels)
        {
            switch (responseModel.StatusCode)
            {
                case HttpStatusCode.OK:
                    AppendProducesForOk(sb, responseModel);
                    break;
                case HttpStatusCode.NotFound:
                    sb.AppendLine(4, $"[ProducesResponseType(typeof(string), StatusCodes.{responseModel.StatusCode.ToStatusCodesConstant()})]");
                    break;
                case HttpStatusCode.BadRequest:
                    sb.AppendLine(4, $"[ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.{responseModel.StatusCode.ToStatusCodesConstant()})]");
                    break;
                case HttpStatusCode.EarlyHints:
                    sb.AppendLine(4, $"[ProducesResponseType(typeof(ProblemDetails), {(int)HttpStatusCode.EarlyHints})]");
                    break;
                case HttpStatusCode.Continue:
                case HttpStatusCode.SwitchingProtocols:
                case HttpStatusCode.Processing:
                case HttpStatusCode.Created:
                case HttpStatusCode.Accepted:
                case HttpStatusCode.NonAuthoritativeInformation:
                case HttpStatusCode.NoContent:
                case HttpStatusCode.ResetContent:
                case HttpStatusCode.PartialContent:
                case HttpStatusCode.MultiStatus:
                case HttpStatusCode.AlreadyReported:
                case HttpStatusCode.IMUsed:
                case HttpStatusCode.MultipleChoices:
                case HttpStatusCode.MovedPermanently:
                case HttpStatusCode.Found:
                case HttpStatusCode.SeeOther:
                case HttpStatusCode.NotModified:
                case HttpStatusCode.UseProxy:
                case HttpStatusCode.Unused:
                case HttpStatusCode.RedirectKeepVerb:
                case HttpStatusCode.PermanentRedirect:
                case HttpStatusCode.Unauthorized:
                case HttpStatusCode.PaymentRequired:
                case HttpStatusCode.Forbidden:
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
                    sb.AppendLine(4, $"[ProducesResponseType(typeof(ProblemDetails), StatusCodes.{responseModel.StatusCode.ToStatusCodesConstant()})]");
                    break;
                default:
                    sb.AppendLine(4, $"// TODO: Not Implemented for {responseModel.StatusCode}.");
                    break;
            }
        }
    }

    private static void AppendProducesWithoutProblemDetails(
        StringBuilder sb,
        ContentGeneratorServerEndpointMethodParameters item)
    {
        var responseModels = item.ResponseModels
            .AppendUnauthorizedIfNeeded(item.Authorization, item.IsAuthorizationRequiredFromPath)
            .AppendForbiddenIfNeeded(item.Authorization)
            .AppendBadRequestIfNeeded(item.ParameterTypeName)
            .OrderBy(x => x.StatusCode)
            .ToList();

        foreach (var responseModel in responseModels)
        {
            switch (responseModel.StatusCode)
            {
                case HttpStatusCode.OK:
                    AppendProducesForOk(sb, responseModel);
                    break;
                case HttpStatusCode.BadRequest:
                    sb.AppendLine(4, $"[ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.{responseModel.StatusCode.ToStatusCodesConstant()})]");
                    break;
                case HttpStatusCode.Accepted:
                case HttpStatusCode.Created:
                case HttpStatusCode.NotFound:
                case HttpStatusCode.Conflict:
                case HttpStatusCode.InternalServerError:
                    sb.AppendLine(4, $"[ProducesResponseType(typeof(string), StatusCodes.{responseModel.StatusCode.ToStatusCodesConstant()})]");
                    break;
                case HttpStatusCode.EarlyHints:
                    sb.AppendLine(4, $"[ProducesResponseType({(int)HttpStatusCode.EarlyHints})]");
                    break;
                case HttpStatusCode.Continue:
                case HttpStatusCode.SwitchingProtocols:
                case HttpStatusCode.Processing:
                case HttpStatusCode.NonAuthoritativeInformation:
                case HttpStatusCode.NoContent:
                case HttpStatusCode.ResetContent:
                case HttpStatusCode.PartialContent:
                case HttpStatusCode.MultiStatus:
                case HttpStatusCode.AlreadyReported:
                case HttpStatusCode.IMUsed:
                case HttpStatusCode.MultipleChoices:
                case HttpStatusCode.MovedPermanently:
                case HttpStatusCode.Found:
                case HttpStatusCode.SeeOther:
                case HttpStatusCode.NotModified:
                case HttpStatusCode.UseProxy:
                case HttpStatusCode.Unused:
                case HttpStatusCode.RedirectKeepVerb:
                case HttpStatusCode.PermanentRedirect:
                case HttpStatusCode.Unauthorized:
                case HttpStatusCode.PaymentRequired:
                case HttpStatusCode.Forbidden:
                case HttpStatusCode.MethodNotAllowed:
                case HttpStatusCode.NotAcceptable:
                case HttpStatusCode.ProxyAuthenticationRequired:
                case HttpStatusCode.RequestTimeout:
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
                    sb.AppendLine(4, $"[ProducesResponseType(StatusCodes.{responseModel.StatusCode.ToStatusCodesConstant()})]");
                    break;
                default:
                    sb.AppendLine(4, $"// TODO: Not Implemented for {responseModel.StatusCode}.");
                    break;
            }
        }
    }

    private static void AppendProducesForOk(
        StringBuilder sb,
        ApiOperationResponseModel responseModel)
    {
        if (responseModel.MediaType == MediaTypeNames.Application.Octet)
        {
            sb.AppendLine(4, $"[ProducesResponseType(typeof(byte[]), StatusCodes.{responseModel.StatusCode.ToStatusCodesConstant()})]");
        }
        else if (responseModel.DataType is null)
        {
            sb.AppendLine(4, $"[ProducesResponseType(typeof(string), StatusCodes.{responseModel.StatusCode.ToStatusCodesConstant()})]");
        }
        else
        {
            var dataType = responseModel.GetQualifiedDataType();

            if (string.IsNullOrEmpty(responseModel.CollectionDataType))
            {
                sb.AppendLine(4, $"[ProducesResponseType(typeof({dataType}), StatusCodes.{responseModel.StatusCode.ToStatusCodesConstant()})]");
            }
            else
            {
                if (responseModel.UseAsyncEnumerable)
                {
                    sb.AppendLine(
                        4,
                        responseModel.CollectionDataType == NameConstants.List
                            ? $"[ProducesResponseType(typeof(IAsyncEnumerable<{dataType}>), StatusCodes.{responseModel.StatusCode.ToStatusCodesConstant()})]"
                            : $"[ProducesResponseType(typeof(IAsyncEnumerable<{responseModel.CollectionDataType}<{dataType}>>), StatusCodes.{responseModel.StatusCode.ToStatusCodesConstant()})]");
                }
                else
                {
                    sb.AppendLine(
                        4,
                        responseModel.CollectionDataType == NameConstants.List
                            ? $"[ProducesResponseType(typeof(IEnumerable<{dataType}>), StatusCodes.{responseModel.StatusCode.ToStatusCodesConstant()})]"
                            : $"[ProducesResponseType(typeof({responseModel.CollectionDataType}<{dataType}>), StatusCodes.{responseModel.StatusCode.ToStatusCodesConstant()})]");
                }
            }
        }
    }
}