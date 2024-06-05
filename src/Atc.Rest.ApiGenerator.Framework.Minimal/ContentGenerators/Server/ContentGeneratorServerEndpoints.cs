namespace Atc.Rest.ApiGenerator.Framework.Minimal.ContentGenerators.Server;

public sealed class ContentGeneratorServerEndpoints : IContentGenerator
{
    private readonly GeneratedCodeHeaderGenerator codeHeaderGenerator;
    private readonly GeneratedCodeAttributeGenerator codeAttributeGenerator;
    private readonly CodeDocumentationTagsGenerator codeDocumentationTagsGenerator;
    private readonly ContentGeneratorServerEndpointParameters parameters;
    private readonly bool useProblemDetailsAsDefaultResponseBody;

    public ContentGeneratorServerEndpoints(
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

        sb.AppendLine(codeAttributeGenerator.Generate());

        sb.AppendLine($"public sealed class {parameters.ApiGroupName}EndpointDefinition : IEndpointDefinition");
        sb.AppendLine("{");
        AppendContent(sb);
        sb.Append('}');

        return sb.ToString();
    }

    private void AppendContent(
        StringBuilder sb)
    {
        sb.AppendLine(4, $"internal const string ApiRouteBase = \"{parameters.RouteBase}\";");
        sb.AppendLine();

        AppendDefineEndpoints(sb);

        AppendRouteHandlers(sb);
    }

    private void AppendDefineEndpoints(
        StringBuilder sb)
    {
        var routeGroupBuilderName = parameters.ApiGroupName.EnsureFirstCharacterToLower();

        sb.AppendLine(4, "public void DefineEndpoints(");
        sb.AppendLine(8, "WebApplication app)");
        sb.AppendLine(4, "{");
        sb.AppendLine(8, $"var {routeGroupBuilderName} = app");
        sb.AppendLine(12, $".NewVersionedApi(\"{parameters.ApiGroupName}\")");
        sb.AppendLine(12, ".MapGroup(ApiRouteBase);");
        sb.AppendLine();

        for (var i = 0; i < parameters.MethodParameters.Count; i++)
        {
            var item = parameters.MethodParameters[i];

            AppendRoute(sb, routeGroupBuilderName, item);

            if (i < parameters.MethodParameters.Count - 1)
            {
                sb.AppendLine();
            }
        }

        sb.AppendLine(4, "}");
        sb.AppendLine();
    }

    private void AppendRoute(
        StringBuilder sb,
        string routeGroupBuilderName,
        ContentGeneratorServerEndpointMethodParameters item)
    {
        sb.AppendLine(8, routeGroupBuilderName);
        sb.AppendLine(
            12,
            item.RouteSuffix is null
                ? $".Map{item.OperationTypeRepresentation}(\"/\", {item.Name})"
                : $".Map{item.OperationTypeRepresentation}(\"{item.RouteSuffix}\", {item.Name})");
        sb.AppendLine(12, $".WithName(\"{item.Name}\")");

        if (!string.IsNullOrEmpty(item.DocumentationTags.Summary))
        {
            var summary = item.DocumentationTags.Summary;
            var indexOfOperation = summary.IndexOf("Operation: ", StringComparison.Ordinal);
            if (indexOfOperation > 0)
            {
                summary = summary[..indexOfOperation];
            }

            summary = summary
                .Replace("Description: ", string.Empty, StringComparison.Ordinal)
                .Replace(Environment.NewLine, string.Empty, StringComparison.Ordinal)
                .Trim()
                .EnsureEndsWithDot();

            sb.AppendLine(12, $".WithSummary(\"{summary.EnsureEndsWithDot()}\")");
        }

        if (!string.IsNullOrEmpty(item.Description))
        {
            var description = item.Description
                .Replace(Environment.NewLine, string.Empty, StringComparison.Ordinal)
                .Trim()
                .EnsureEndsWithDot();

            sb.AppendLine(12, $".WithDescription(\"{description}\")");
        }

        if (!string.IsNullOrEmpty(item.ParameterTypeName))
        {
            sb.AppendLine(12, $".AddEndpointFilter<ValidationFilter<{item.ParameterTypeName}>>()");
        }

        if (useProblemDetailsAsDefaultResponseBody)
        {
            AppendProducesWithProblemDetails(sb, item);
        }
        else
        {
            AppendProducesWithoutProblemDetails(sb, item);
        }
    }

    private void AppendRouteHandlers(
        StringBuilder sb)
    {
        for (var i = 0; i < parameters.MethodParameters.Count; i++)
        {
            var item = parameters.MethodParameters[i];

            sb.AppendLine(4, $"internal async Task<IResult> {item.Name}(");
            sb.AppendLine(8, $"[FromServices] {item.InterfaceName} handler,");

            if (!string.IsNullOrEmpty(item.ParameterTypeName))
            {
                sb.AppendLine(8, $"[AsParameters] {item.ParameterTypeName} parameters,");
            }

            sb.AppendLine(8, "CancellationToken cancellationToken)");

            sb.AppendLine(8, $"=> {item.ResultName}.ToIResult(");
            sb.AppendLine(12, "await handler.ExecuteAsync(");
            if (!string.IsNullOrEmpty(item.ParameterTypeName))
            {
                sb.AppendLine(16, "parameters,");
            }

            sb.AppendLine(16, "cancellationToken));");

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
            .AppendUnauthorizedIfNeeded(item.Authorization)
            .AppendForbiddenIfNeeded(item.Authorization)
            .AppendBadRequestIfNeeded(item.ParameterTypeName)
            .OrderBy(x => x.StatusCode)
            .ToList();

        foreach (var responseModel in responseModels)
        {
            switch (responseModel.StatusCode)
            {
                case HttpStatusCode.OK:
                    if (responseModel.DataType is null)
                    {
                        sb.Append(12, ".Produces<string?>()");
                    }
                    else
                    {
                        sb.Append(
                            12,
                            string.IsNullOrEmpty(responseModel.CollectionDataType)
                                ? $".Produces<{responseModel.DataType}>()"
                                : $".Produces<{responseModel.CollectionDataType}<{responseModel.DataType}>>()");
                    }

                    break;
                case HttpStatusCode.BadRequest:
                    sb.Append(12, ".ProducesValidationProblem()");
                    break;
                case HttpStatusCode.Accepted:
                case HttpStatusCode.Created:
                    sb.Append(12, $".Produces<string?>(StatusCodes.{responseModel.StatusCode.ToStatusCodesConstant()})");
                    break;
                case HttpStatusCode.EarlyHints:
                    sb.Append(12, $".ProducesProblem({(int)HttpStatusCode.EarlyHints})");
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
                    sb.Append(12, $".ProducesProblem(StatusCodes.{responseModel.StatusCode.ToStatusCodesConstant()})");
                    break;
                default:
                    sb.Append(12, $"// TODO: Not Implemented for {responseModel.StatusCode}.");
                    break;
            }

            if (responseModel == responseModels[^1])
            {
                sb.AppendLine(";");
            }
            else
            {
                sb.AppendLine();
            }
        }
    }

    private static void AppendProducesWithoutProblemDetails(
        StringBuilder sb,
        ContentGeneratorServerEndpointMethodParameters item)
    {
        var responseModels = item.ResponseModels
            .AppendUnauthorizedIfNeeded(item.Authorization)
            .AppendForbiddenIfNeeded(item.Authorization)
            .AppendBadRequestIfNeeded(item.ParameterTypeName)
            .OrderBy(x => x.StatusCode)
            .ToList();

        foreach (var responseModel in responseModels)
        {
            switch (responseModel.StatusCode)
            {
                case HttpStatusCode.OK:
                    if (responseModel.DataType is null)
                    {
                        sb.Append(12, ".Produces<string?>()");
                    }
                    else
                    {
                        sb.Append(
                            12,
                            string.IsNullOrEmpty(responseModel.CollectionDataType)
                                ? $".Produces<{responseModel.DataType}>()"
                                : $".Produces<{responseModel.CollectionDataType}<{responseModel.DataType}>>()");
                    }

                    break;
                case HttpStatusCode.Accepted:
                case HttpStatusCode.Created:
                case HttpStatusCode.BadRequest:
                case HttpStatusCode.NotFound:
                case HttpStatusCode.Conflict:
                    sb.Append(12, $".Produces<string?>(StatusCodes.{responseModel.StatusCode.ToStatusCodesConstant()})");
                    break;
                case HttpStatusCode.NotImplemented:
                case HttpStatusCode.InternalServerError:
                    sb.Append(12, $".ProducesProblem(StatusCodes.{responseModel.StatusCode.ToStatusCodesConstant()})");
                    break;
                case HttpStatusCode.EarlyHints:
                    sb.Append(12, $".Produces({(int)HttpStatusCode.EarlyHints})");
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
                case HttpStatusCode.BadGateway:
                case HttpStatusCode.ServiceUnavailable:
                case HttpStatusCode.GatewayTimeout:
                case HttpStatusCode.HttpVersionNotSupported:
                case HttpStatusCode.VariantAlsoNegotiates:
                case HttpStatusCode.InsufficientStorage:
                case HttpStatusCode.LoopDetected:
                case HttpStatusCode.NotExtended:
                case HttpStatusCode.NetworkAuthenticationRequired:
                    sb.Append(12, $".Produces(StatusCodes.{responseModel.StatusCode.ToStatusCodesConstant()})");
                    break;
                default:
                    sb.Append(12, $"// TODO: Not Implemented for {responseModel.StatusCode}.");
                    break;
            }

            if (responseModel == responseModels[^1])
            {
                sb.AppendLine(";");
            }
            else
            {
                sb.AppendLine();
            }
        }
    }
}