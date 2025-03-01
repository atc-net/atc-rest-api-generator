// ReSharper disable SwitchExpressionHandlesSomeKnownEnumValuesWithExceptionInDefault
namespace Atc.Rest.ApiGenerator.Client.CSharp.ContentGenerators;

public class ContentGeneratorClientEndpointResult : IContentGenerator
{
    private readonly GeneratedCodeHeaderGenerator codeHeaderGenerator;
    private readonly GeneratedCodeAttributeGenerator codeAttributeGenerator;
    private readonly CodeDocumentationTagsGenerator codeDocumentationTagsGenerator;
    private readonly ContentGeneratorClientEndpointResultParameters parameters;
    private readonly bool useProblemDetailsAsDefaultResponseBody;

    public ContentGeneratorClientEndpointResult(
        GeneratedCodeHeaderGenerator codeHeaderGenerator,
        GeneratedCodeAttributeGenerator codeAttributeGenerator,
        CodeDocumentationTagsGenerator codeDocumentationTagsGenerator,
        ContentGeneratorClientEndpointResultParameters parameters,
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

        sb.AppendLine(codeAttributeGenerator.Generate());
        sb.AppendLine($"{parameters.DeclarationModifier.GetDescription()} {parameters.EndpointResultName} : {parameters.InheritClassName}, {parameters.EndpointResultInterfaceName}");
        sb.AppendLine("{");
        sb.AppendLine(4, $"public {parameters.EndpointResultName}(EndpointResponse response)");
        sb.AppendLine(8, ": base(response)");
        sb.AppendLine(4, "{");
        sb.AppendLine(4, "}");
        AppendContent(sb);
        sb.Append('}');

        return sb.ToString();
    }

    private void AppendContent(
        StringBuilder sb)
    {
        AppendContentIsStatus(sb);

        if (useProblemDetailsAsDefaultResponseBody)
        {
            AppendContentWithProblemDetails(sb);
        }
        else
        {
            AppendContentWithoutProblemDetails(sb);
        }
    }

    private void AppendContentIsStatus(
        StringBuilder sb)
    {
        var responseModels = parameters.ResponseModels
            .AppendUnauthorizedIfNeeded(parameters.Authorization, parameters.IsAuthorizationRequiredFromPath)
            .AppendForbiddenIfNeeded(parameters.Authorization)
            .AppendBadRequestIfNeeded(parameters.HasParameterType)
            .OrderBy(x => x.StatusCode)
            .ToList();

        foreach (var responseModel in responseModels)
        {
            switch (responseModel.StatusCode)
            {
                case HttpStatusCode.Continue:
                case HttpStatusCode.SwitchingProtocols:
                case HttpStatusCode.Processing:
                case HttpStatusCode.EarlyHints:
                case HttpStatusCode.OK:
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
                case HttpStatusCode.BadRequest:
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
                    sb.AppendLine();
                    sb.AppendLine(4, $"public bool Is{responseModel.StatusCode.ToNormalizedString()}");
                    sb.AppendLine(8, $"=> StatusCode == HttpStatusCode.{responseModel.StatusCode};");
                    break;
                default:
                    sb.AppendLine(4, $"// TODO: Not Implemented for {responseModel.StatusCode}.");
                    break;
            }
        }
    }

    private void AppendMethodContentStatusCodeOk(
        StringBuilder sb,
        ApiOperationResponseModel responseModel)
    {
        if (responseModel.MediaType == MediaTypeNames.Application.Octet)
        {
            sb.AppendLine(4, "public byte[] OkContent");
            sb.AppendLine(8, "=> IsOk && ContentObject is byte[] result");
            sb.AppendLine(12, "? result");
            sb.AppendLine(12, ": throw new InvalidOperationException(\"Content is not the expected type - please use the IsOk property first.\");");
            return;
        }

        if (string.IsNullOrEmpty(responseModel.DataType))
        {
            sb.AppendLine(4, "public string? OkContent");
            sb.AppendLine(8, "=> IsOk && ContentObject is string result");
            sb.AppendLine(12, "? result");
            sb.AppendLine(12, ": throw new InvalidOperationException(\"Content is not the expected type - please use the IsOk property first.\");");
            return;
        }

        var dataType = responseModel.GetQualifiedDataType();

        if (responseModel.CollectionDataType is null)
        {
            sb.AppendLine(4, $"public {dataType} OkContent");
            sb.AppendLine(8, $"=> IsOk && ContentObject is {dataType} result");
            sb.AppendLine(12, "? result");
            sb.AppendLine(12, ": throw new InvalidOperationException(\"Content is not the expected type - please use the IsOk property first.\");");
            return;
        }

        if (responseModel.UseAsyncEnumerable)
        {
            if (responseModel.CollectionDataType == NameConstants.List)
            {
                sb.AppendLine(4, $"public IAsyncEnumerable<{dataType}> OkContent");
                sb.AppendLine(8, $"=> IsOk && ContentObject is IAsyncEnumerable<{dataType}> result");
                sb.AppendLine(12, "? result");
                sb.AppendLine(12, ": throw new InvalidOperationException(\"Content is not the expected type - please use the IsOk property first.\");");
            }
            else
            {
                sb.AppendLine(4, $"public IAsyncEnumerable<{responseModel.CollectionDataType}<{dataType}>> OkContent");
                sb.AppendLine(8, $"=> IsOk && ContentObject is IAsyncEnumerable<{responseModel.CollectionDataType}<{dataType}>> result");
                sb.AppendLine(12, "? result");
                sb.AppendLine(12, ": throw new InvalidOperationException(\"Content is not the expected type - please use the IsOk property first.\");");
            }
        }
        else
        {
            if (responseModel.CollectionDataType == NameConstants.List)
            {
                sb.AppendLine(4, $"public IEnumerable<{dataType}> OkContent");
                sb.AppendLine(8, $"=> IsOk && ContentObject is IEnumerable<{dataType}> result");
                sb.AppendLine(12, "? result");
                sb.AppendLine(12, ": throw new InvalidOperationException(\"Content is not the expected type - please use the IsOk property first.\");");
            }
            else
            {
                sb.AppendLine(4, $"public {responseModel.CollectionDataType}<{dataType}> OkContent");
                sb.AppendLine(8, $"=> IsOk && ContentObject is {responseModel.CollectionDataType}<{dataType}> result");
                sb.AppendLine(12, "? result");
                sb.AppendLine(12, ": throw new InvalidOperationException(\"Content is not the expected type - please use the IsOk property first.\");");
            }
        }
    }

    private void AppendContentWithProblemDetails(
        StringBuilder sb)
    {
        var responseModels = parameters.ResponseModels
            .AppendUnauthorizedIfNeeded(parameters.Authorization, parameters.IsAuthorizationRequiredFromPath)
            .AppendForbiddenIfNeeded(parameters.Authorization)
            .AppendBadRequestIfNeeded(parameters.HasParameterType)
            .OrderBy(x => x.StatusCode)
            .ToList();

        foreach (var responseModel in responseModels)
        {
            switch (responseModel.StatusCode)
            {
                case HttpStatusCode.OK:
                    sb.AppendLine();
                    AppendMethodContentStatusCodeOk(sb, responseModel);
                    break;
                case HttpStatusCode.NotFound:
                    sb.AppendLine();
                    sb.AppendLine(4, $"public string? {responseModel.StatusCode.ToNormalizedString()}Content");
                    sb.AppendLine(8, $"=> Is{responseModel.StatusCode.ToNormalizedString()} && ContentObject is string result");
                    sb.AppendLine(12, "? result");
                    sb.AppendLine(12, $": throw new InvalidOperationException(\"Content is not the expected type - please use the Is{responseModel.StatusCode.ToNormalizedString()} property first.\");");
                    break;
                case HttpStatusCode.BadRequest:
                    sb.AppendLine();
                    sb.AppendLine(4, $"public ValidationProblemDetails {responseModel.StatusCode.ToNormalizedString()}Content");
                    sb.AppendLine(8, $"=> Is{responseModel.StatusCode.ToNormalizedString()} && ContentObject is ValidationProblemDetails result");
                    sb.AppendLine(12, "? result");
                    sb.AppendLine(12, $": throw new InvalidOperationException(\"Content is not the expected type - please use the Is{responseModel.StatusCode.ToNormalizedString()} property first.\");");
                    break;
                case HttpStatusCode.Continue:
                case HttpStatusCode.SwitchingProtocols:
                case HttpStatusCode.Processing:
                case HttpStatusCode.Created:
                case HttpStatusCode.Accepted:
                case HttpStatusCode.EarlyHints:
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
                    sb.AppendLine();
                    sb.AppendLine(4, $"public ProblemDetails {responseModel.StatusCode.ToNormalizedString()}Content");
                    sb.AppendLine(8, $"=> Is{responseModel.StatusCode.ToNormalizedString()} && ContentObject is ProblemDetails result");
                    sb.AppendLine(12, "? result");
                    sb.AppendLine(12, $": throw new InvalidOperationException(\"Content is not the expected type - please use the Is{responseModel.StatusCode.ToNormalizedString()} property first.\");");
                    break;
                default:
                    sb.AppendLine(4, $"// TODO: Not Implemented for {responseModel.StatusCode}.");
                    break;
            }
        }
    }

    private void AppendContentWithoutProblemDetails(
        StringBuilder sb)
    {
        var responseModels = parameters.ResponseModels
            .AppendUnauthorizedIfNeeded(parameters.Authorization, parameters.IsAuthorizationRequiredFromPath)
            .AppendForbiddenIfNeeded(parameters.Authorization)
            .AppendBadRequestIfNeeded(parameters.HasParameterType)
            .OrderBy(x => x.StatusCode)
            .ToList();

        foreach (var responseModel in responseModels)
        {
            switch (responseModel.StatusCode)
            {
                case HttpStatusCode.OK:
                    sb.AppendLine();
                    AppendMethodContentStatusCodeOk(sb, responseModel);
                    break;
                case HttpStatusCode.Continue:
                case HttpStatusCode.SwitchingProtocols:
                case HttpStatusCode.Processing:
                case HttpStatusCode.Created:
                case HttpStatusCode.Accepted:
                case HttpStatusCode.EarlyHints:
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
                case HttpStatusCode.BadRequest:
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
                    sb.AppendLine();
                    sb.AppendLine(4, $"public string? {responseModel.StatusCode.ToNormalizedString()}Content");
                    sb.AppendLine(8, $"=> Is{responseModel.StatusCode.ToNormalizedString()} && ContentObject is string result");
                    sb.AppendLine(12, "? result");
                    sb.AppendLine(12, $": throw new InvalidOperationException(\"Content is not the expected type - please use the Is{responseModel.StatusCode.ToNormalizedString()} property first.\");");
                    break;
                default:
                    sb.AppendLine(4, $"// TODO: Not Implemented for {responseModel.StatusCode}.");
                    break;
            }
        }
    }
}