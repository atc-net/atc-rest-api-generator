// ReSharper disable SwitchStatementHandlesSomeKnownEnumValuesWithDefault
// ReSharper disable SwitchStatementMissingSomeEnumCasesNoDefault
// ReSharper disable ConvertIfStatementToReturnStatement
namespace Atc.Rest.ApiGenerator.Framework.Mvc.ContentGenerators;

public sealed class ContentGeneratorServerResult : IContentGenerator
{
    private readonly GeneratedCodeHeaderGenerator codeHeaderGenerator;
    private readonly GeneratedCodeAttributeGenerator codeAttributeGenerator;
    private readonly CodeDocumentationTagsGenerator codeDocumentationTagsGenerator;
    private readonly ContentGeneratorServerResultParameters parameters;
    private readonly bool useProblemDetailsAsDefaultResponseBody;

    public ContentGeneratorServerResult(
        GeneratedCodeHeaderGenerator codeHeaderGenerator,
        GeneratedCodeAttributeGenerator codeAttributeGenerator,
        CodeDocumentationTagsGenerator codeDocumentationTagsGenerator,
        ContentGeneratorServerResultParameters parameters,
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
        sb.AppendLine($"public class {parameters.ResultName} : {nameof(Results.ResultBase)}");
        sb.AppendLine("{");
        sb.AppendLine(4, $"private {parameters.ResultName}(ActionResult result) : base(result) {{ }}");
        sb.AppendLine();

        for (var i = 0; i < parameters.MethodParameters.Count; i++)
        {
            var item = parameters.MethodParameters[i];

            AppendMethodContent(sb, item, parameters.ResultName);

            if (i < parameters.MethodParameters.Count - 1)
            {
                sb.AppendLine();
            }
        }

        if (parameters.ImplicitOperatorParameters is not null)
        {
            AppendImplicitOperatorContent(sb, parameters);
        }

        sb.Append('}');

        return sb.ToString();
    }

    private void AppendMethodContent(
        StringBuilder sb,
        ContentGeneratorServerResultMethodParameters item,
        string resultName)
    {
        if (codeDocumentationTagsGenerator.ShouldGenerateTags(item.DocumentationTags))
        {
            sb.Append(codeDocumentationTagsGenerator.GenerateTags(4, item.DocumentationTags));
        }

        if (item.ResponseModel.StatusCode == HttpStatusCode.OK)
        {
            AppendMethodContentStatusCodeOk(sb, item, resultName);
        }
        else
        {
            if (useProblemDetailsAsDefaultResponseBody)
            {
                AppendMethodContentForOtherStatusCodesThenOkWithProblemDetails(sb, item, resultName);
            }
            else
            {
                AppendMethodContentForOtherStatusCodesThenOkWithoutProblemDetails(sb, item, resultName);
            }
        }
    }

    private void AppendMethodContentStatusCodeOk(
        StringBuilder sb,
        ContentGeneratorServerResultMethodParameters item,
        string resultName)
    {
        if (item.ResponseModel.MediaType == MediaTypeNames.Application.Octet)
        {
            sb.AppendLine(4, $"public static {resultName} Ok(byte[] bytes, string fileName)");
            sb.AppendLine(8, $"=> new {resultName}(ResultFactory.CreateFileContentResult(bytes, fileName));");
            return;
        }

        if (string.IsNullOrEmpty(item.ResponseModel.DataType))
        {
            sb.AppendLine(4, $"public static {resultName} Ok(string? message = null)");
            sb.AppendLine(8, $"=> new {resultName}(new OkObjectResult(message));");
            return;
        }

        if (item.ResponseModel.CollectionDataType is null)
        {
            sb.AppendLine(4, $"public static {resultName} Ok({item.ResponseModel.DataType} response)");
            sb.AppendLine(8, $"=> new {resultName}(new OkObjectResult(response));");
            return;
        }

        if (item.ResponseModel.UseAsyncEnumerable)
        {
            if (item.ResponseModel.CollectionDataType == NameConstants.List)
            {
                sb.AppendLine(4, $"public static {resultName} Ok(IAsyncEnumerable<{item.ResponseModel.DataType}> response)");
                sb.AppendLine(8, $"=> new {resultName}(new OkObjectResult(response ?? AsyncEnumerableFactory.Empty<{item.ResponseModel.DataType}>()));");
            }
            else
            {
                sb.AppendLine(4, $"public static {resultName} Ok(IAsyncEnumerable<{item.ResponseModel.CollectionDataType}<{item.ResponseModel.DataType}>> response)");
                sb.AppendLine(8, $"=> new {resultName}(new OkObjectResult(response));");
            }
        }
        else
        {
            if (item.ResponseModel.CollectionDataType == NameConstants.List)
            {
                sb.AppendLine(4, $"public static {resultName} Ok(IEnumerable<{item.ResponseModel.DataType}> response)");
                sb.AppendLine(8, $"=> new {resultName}(new OkObjectResult(response ?? Enumerable.Empty<{item.ResponseModel.DataType}>()));");
            }
            else
            {
                sb.AppendLine(4, $"public static {resultName} Ok({item.ResponseModel.CollectionDataType}<{item.ResponseModel.DataType}> response)");
                sb.AppendLine(8, $"=> new {resultName}(new OkObjectResult(response));");
            }
        }
    }

    private void AppendMethodContentForOtherStatusCodesThenOkWithProblemDetails(
        StringBuilder sb,
        ContentGeneratorServerResultMethodParameters item,
        string resultName)
    {
        switch (item.ResponseModel.StatusCode)
        {
            case HttpStatusCode.OK:
                throw new InvalidOperationException("This should not happen.");
            case HttpStatusCode.Accepted:
            case HttpStatusCode.Created:
                sb.AppendLine(4, $"public static {resultName} {item.ResponseModel.StatusCode.ToNormalizedString()}(string? uri = null)");
                sb.AppendLine(8, $"=> new {resultName}({nameof(Results.ResultFactory)}.{nameof(Results.ResultFactory.CreateContentResult)}({nameof(HttpStatusCode)}.{item.ResponseModel.StatusCode}, uri));");
                break;
            case HttpStatusCode.BadRequest:
                sb.AppendLine(4, $"public static {resultName} {item.ResponseModel.StatusCode.ToNormalizedString()}(string? message = null)");
                sb.AppendLine(8, $"=> new {resultName}({nameof(Results.ResultFactory)}.{nameof(Results.ResultFactory.CreateContentResultWithValidationProblemDetails)}({nameof(HttpStatusCode)}.{item.ResponseModel.StatusCode}, message));");
                break;
            case HttpStatusCode.EarlyHints:
                sb.AppendLine(4, $"public static {resultName} {item.ResponseModel.StatusCode.ToNormalizedString()}()");
                sb.AppendLine(8, $"=> new {resultName}({nameof(Results.ResultFactory)}.{nameof(Results.ResultFactory.CreateContentResultWithProblemDetails)}({nameof(HttpStatusCode)}.{item.ResponseModel.StatusCode}));");
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
                sb.AppendLine(4, $"public static {resultName} {item.ResponseModel.StatusCode.ToNormalizedString()}(string? message = null)");
                sb.AppendLine(8, $"=> new {resultName}({nameof(Results.ResultFactory)}.{nameof(Results.ResultFactory.CreateContentResultWithProblemDetails)}({nameof(HttpStatusCode)}.{item.ResponseModel.StatusCode}, message));");
                break;
            default:
                sb.AppendLine(4, $"// TODO: Not Implemented for {item.ResponseModel.StatusCode}.");
                break;
        }
    }

    private void AppendMethodContentForOtherStatusCodesThenOkWithoutProblemDetails(
        StringBuilder sb,
        ContentGeneratorServerResultMethodParameters item,
        string resultName)
    {
        switch (item.ResponseModel.StatusCode)
        {
            case HttpStatusCode.OK:
                throw new InvalidOperationException("This should not happen.");
            case HttpStatusCode.Accepted:
            case HttpStatusCode.Created:
                sb.AppendLine(4, $"public static {resultName} {item.ResponseModel.StatusCode.ToNormalizedString()}(string? uri = null)");
                sb.AppendLine(8, $"=> new {resultName}({nameof(Results.ResultFactory)}.{nameof(Results.ResultFactory.CreateContentResult)}({nameof(HttpStatusCode)}.{item.ResponseModel.StatusCode}, uri));");
                break;
            case HttpStatusCode.BadRequest:
            case HttpStatusCode.NotFound:
            case HttpStatusCode.Conflict:
                sb.AppendLine(4, $"public static {resultName} {item.ResponseModel.StatusCode.ToNormalizedString()}(string? message = null)");
                sb.AppendLine(8, $"=> new {resultName}(new {item.ResponseModel.StatusCode.ToNormalizedString()}ObjectResult(message));");
                break;
            case HttpStatusCode.EarlyHints:
                sb.AppendLine(4, $"public static {resultName} {item.ResponseModel.StatusCode.ToNormalizedString()}()");
                sb.AppendLine(8, $"=> new {resultName}({nameof(Results.ResultFactory)}.{nameof(Results.ResultFactory.CreateContentResult)}({nameof(HttpStatusCode)}.{item.ResponseModel.StatusCode}, string? message = null));");
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
                sb.AppendLine(4, $"public static {resultName} {item.ResponseModel.StatusCode.ToNormalizedString()}(string? message = null)");
                sb.AppendLine(8, $"=> new {resultName}({nameof(Results.ResultFactory)}.{nameof(Results.ResultFactory.CreateContentResult)}({nameof(HttpStatusCode)}.{item.ResponseModel.StatusCode}, message));");
                break;
            default:
                sb.AppendLine(4, $"// TODO: Not Implemented for {item.ResponseModel.StatusCode}.");
                break;
        }
    }

    private static void AppendImplicitOperatorContent(
        StringBuilder sb,
        ContentGeneratorServerResultParameters item)
    {
        if (item.ImplicitOperatorParameters is null)
        {
            return;
        }

        sb.AppendLine();
        sb.AppendLine(4, "/// <summary>");
        sb.AppendLine(4, $"/// Performs an implicit conversion from {item.ResultName} to ActionResult.");
        sb.AppendLine(4, "/// </summary>");
        if (item.ImplicitOperatorParameters.DataType is null)
        {
            sb.AppendLine(4, $"public static implicit operator {item.ResultName}(string response)");
            sb.AppendLine(8, "=> Ok(response);");
        }
        else
        {
            sb.AppendLine(
                4,
                item.ImplicitOperatorParameters.CollectionDataType is null
                    ? $"public static implicit operator {item.ResultName}({item.ImplicitOperatorParameters.DataType} response)"
                    : $"public static implicit operator {item.ResultName}({item.ImplicitOperatorParameters.CollectionDataType}<{item.ImplicitOperatorParameters.DataType}> response)");

            if (item.ImplicitOperatorParameters.UseAsyncEnumerable)
            {
                if (item.ImplicitOperatorParameters.CollectionDataType is null)
                {
                    sb.AppendLine(8, "=> Ok(AsyncEnumerableFactory.FromSingleItem(response));");
                }
                else
                {
                    sb.AppendLine(
                        8,
                        item.ImplicitOperatorParameters.CollectionDataType == NameConstants.List
                            ? "=> Ok(response.ToAsyncEnumerable());"
                            : "=> Ok(AsyncEnumerableFactory.FromSingleItem(response));");
                }
            }
            else
            {
                sb.AppendLine(8, "=> Ok(response);");
            }
        }
    }
}