// ReSharper disable SwitchStatementHandlesSomeKnownEnumValuesWithDefault
// ReSharper disable SwitchStatementMissingSomeEnumCasesNoDefault
// ReSharper disable ConvertIfStatementToReturnStatement
namespace Atc.Rest.ApiGenerator.Framework.Mvc.ContentGenerators.Server;

public class ContentGeneratorServerResult : IContentGenerator
{
    private readonly GeneratedCodeHeaderGenerator codeHeaderGenerator;
    private readonly GeneratedCodeAttributeGenerator codeAttributeGenerator;
    private readonly ContentGeneratorServerResultParameters parameters;

    public ContentGeneratorServerResult(
        GeneratedCodeHeaderGenerator codeHeaderGenerator,
        GeneratedCodeAttributeGenerator codeAttributeGenerator,
        ContentGeneratorServerResultParameters parameters)
    {
        this.codeHeaderGenerator = codeHeaderGenerator;
        this.codeAttributeGenerator = codeAttributeGenerator;
        this.parameters = parameters;
    }

    public string Generate()
    {
        var sb = new StringBuilder();

        sb.Append(codeHeaderGenerator.Generate());
        sb.AppendLine($"namespace {parameters.Namespace};");
        sb.AppendLine();
        sb.AppendLine("/// <summary>");
        sb.AppendLine("/// Results for operation request.");
        sb.AppendLine($"/// Description: {parameters.Description}");
        sb.AppendLine($"/// Operation: {parameters.OperationName}.");
        sb.AppendLine("/// </summary>");
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

        sb.AppendLine("}");

        return sb.ToString();
    }

    private static void AppendMethodContent(
        StringBuilder sb,
        ContentGeneratorServerResultMethodParameters methodParameter,
        string resultName)
    {
        AppendMethodSummary(sb, methodParameter.HttpStatusCode);

        if (methodParameter.HttpStatusCode == HttpStatusCode.OK)
        {
            AppendMethodContentStatusCodeOk(sb, methodParameter, resultName);
            return;
        }

        if (methodParameter.UsesProblemDetails)
        {
            AppendMethodContentWithProblemDetails(sb, methodParameter, resultName);
        }
        else
        {
            AppendMethodContentWithoutProblemDetails(sb, methodParameter, resultName);
        }
    }

    private static void AppendImplicitOperatorContent(
        StringBuilder sb,
        ContentGeneratorServerResultParameters parameters)
    {
        sb.AppendLine();
        sb.AppendLine(4, "/// <summary>");
        sb.AppendLine(4, $"/// Performs an implicit conversion from {parameters.ResultName} to ActionResult.");
        sb.AppendLine(4, "/// </summary>");

        if (string.IsNullOrEmpty(parameters.ImplicitOperatorParameters!.ModelName))
        {
            switch (parameters.ImplicitOperatorParameters.SchemaType)
            {
                case SchemaType.None:
                    sb.AppendLine(4, $"public static implicit operator {parameters.ResultName}(string response)");
                    break;
                case SchemaType.SimpleType:
                    sb.AppendLine(4, $"public static implicit operator {parameters.ResultName}({parameters.ImplicitOperatorParameters.SimpleDataTypeName} response)");
                    break;
                case SchemaType.SimpleTypeList:
                    sb.AppendLine(4, $"public static implicit operator {parameters.ResultName}(List<{parameters.ImplicitOperatorParameters.SimpleDataTypeName}> response)");
                    break;
                case SchemaType.SimpleTypePagedList:
                    sb.AppendLine(4, $"public static implicit operator {parameters.ResultName}(Pagination<{parameters.ImplicitOperatorParameters.SimpleDataTypeName}> response)");
                    break;
                default:
                    sb.AppendLine("//// TODO: This is unexpected when we do not have a model-name!"); // TODO: Remove eventually
                    break;
            }
        }
        else
        {
            switch (parameters.ImplicitOperatorParameters.SchemaType)
            {
                case SchemaType.ComplexType:
                    sb.AppendLine(4, $"public static implicit operator {parameters.ResultName}({parameters.ImplicitOperatorParameters.ModelName} response)");
                    break;
                case SchemaType.ComplexTypeList:
                    sb.AppendLine(4, $"public static implicit operator {parameters.ResultName}(List<{parameters.ImplicitOperatorParameters.ModelName}> response)");
                    break;
                case SchemaType.ComplexTypePagedList:
                    sb.AppendLine(4, $"public static implicit operator {parameters.ResultName}(Pagination<{parameters.ImplicitOperatorParameters.ModelName}> response)");
                    break;
                default:
                    sb.AppendLine("//// TODO: This is unexpected when we have a model-name!"); // TODO: Remove eventually
                    break;
            }
        }

        sb.AppendLine(8, "=> Ok(response);");
    }

    private static void AppendMethodContentStatusCodeOk(
        StringBuilder sb,
        ContentGeneratorServerResultMethodParameters methodParameter,
        string resultName)
    {
        if (methodParameter.UsesBinaryResponse.HasValue &&
            methodParameter.UsesBinaryResponse.Value)
        {
            AppendMethodContentStatusCodeOkForBinary(sb, resultName);
        }
        else
        {
            if (string.IsNullOrEmpty(methodParameter.ModelName))
            {
                switch (methodParameter.SchemaType)
                {
                    case SchemaType.None:
                        sb.AppendLine(4, $"public static {resultName} Ok(string? message = null)");
                        ////sb.AppendLine(8, $"=> new {resultName}({nameof(Results.ResultFactory)}.{nameof(Results.ResultFactory.CreateContentResult)}(HttpStatusCode.OK, message));");
                        sb.AppendLine(8, $"=> new {resultName}(new OkObjectResult(message));");
                        break;
                    case SchemaType.SimpleType:
                        sb.AppendLine(4, $"public static {resultName} Ok({methodParameter.SimpleDataTypeName} response)");
                        sb.AppendLine(8, $"=> new {resultName}(new OkObjectResult(response));");
                        break;
                    case SchemaType.SimpleTypeList:
                        sb.AppendLine(4, $"public static {resultName} Ok(IEnumerable<{methodParameter.SimpleDataTypeName}> response)");
                        sb.AppendLine(8, $"=> new {resultName}(new OkObjectResult(response ?? Enumerable.Empty<{methodParameter.SimpleDataTypeName}>()));");
                        break;
                    case SchemaType.SimpleTypePagedList:
                        sb.AppendLine(4, $"public static {resultName} Ok(Pagination<{methodParameter.SimpleDataTypeName}> response)");
                        sb.AppendLine(8, $"=> new {resultName}(new OkObjectResult(response));");
                        break;
                    default:
                        sb.AppendLine("//// TODO: This is unexpected when we do not have a model-name!"); // TODO: Remove eventually
                        break;
                }
            }
            else
            {
                switch (methodParameter.SchemaType)
                {
                    case SchemaType.ComplexType:
                        sb.AppendLine(4, $"public static {resultName} Ok({methodParameter.ModelName} response)");
                        sb.AppendLine(8, $"=> new {resultName}(new OkObjectResult(response));");
                        break;
                    case SchemaType.ComplexTypeList:
                        sb.AppendLine(4, $"public static {resultName} Ok(IEnumerable<{methodParameter.ModelName}> response)");
                        sb.AppendLine(8, $"=> new {resultName}(new OkObjectResult(response ?? Enumerable.Empty<{methodParameter.ModelName}>()));");
                        break;
                    case SchemaType.ComplexTypePagedList:
                        sb.AppendLine(4, $"public static {resultName} Ok(Pagination<{methodParameter.ModelName}> response)");
                        sb.AppendLine(8, $"=> new {resultName}(new OkObjectResult(response));");
                        break;
                    default:
                        sb.AppendLine("//// TODO: This is unexpected when we have a model-name!"); // TODO: Remove eventually
                        break;
                }
            }
        }
    }

    private static void AppendMethodContentStatusCodeOkForBinary(
        StringBuilder sb,
        string resultName)
    {
        sb.AppendLine(4, $"public static {resultName} Ok(byte[] bytes, string fileName)");
        sb.AppendLine(8, $"=> new {resultName}({nameof(Results.ResultFactory)}.{nameof(Results.ResultFactory.CreateFileContentResult)}(bytes, fileName));");
    }

    private static void AppendMethodContentWithProblemDetails(
        StringBuilder sb,
        ContentGeneratorServerResultMethodParameters methodParameter,
        string resultName)
    {
        switch (methodParameter.HttpStatusCode)
        {
            case HttpStatusCode.Created:
                sb.AppendLine(4, $"public static {resultName} Created()");
                sb.AppendLine(8, $"=> new {resultName}({nameof(Results.ResultFactory)}.{nameof(Results.ResultFactory.CreateContentResult)}(HttpStatusCode.Created, null));");
                break;
            case HttpStatusCode.Accepted:
                sb.AppendLine(4, $"public static {resultName} Accepted()");
                sb.AppendLine(8, $"=> new {resultName}({nameof(Results.ResultFactory)}.{nameof(Results.ResultFactory.CreateContentResultWithProblemDetails)}(HttpStatusCode.Accepted, null));");
                break;
            case HttpStatusCode.NoContent:
                sb.AppendLine(4, $"public static {resultName} NoContent()");
                sb.AppendLine(8, $"=> new {resultName}({nameof(Results.ResultFactory)}.{nameof(Results.ResultFactory.CreateContentResultWithProblemDetails)}(HttpStatusCode.NoContent, null));");
                break;
            case HttpStatusCode.NotModified:
                sb.AppendLine(4, $"public static {resultName} NotModified()");
                sb.AppendLine(8, $"=> new {resultName}({nameof(Results.ResultFactory)}.{nameof(Results.ResultFactory.CreateContentResultWithProblemDetails)}(HttpStatusCode.NotModified, null));");
                break;
            case HttpStatusCode.BadRequest:
                sb.AppendLine(4, $"public static {resultName} BadRequest(string message)");
                sb.AppendLine(8, $"=> new {resultName}({nameof(Results.ResultFactory)}.{nameof(Results.ResultFactory.CreateContentResultWithValidationProblemDetails)}(HttpStatusCode.BadRequest, message));");
                break;
            case HttpStatusCode.Unauthorized:
                sb.AppendLine(4, $"public static {resultName} Unauthorized()");
                sb.AppendLine(8, $"=> new {resultName}({nameof(Results.ResultFactory)}.{nameof(Results.ResultFactory.CreateContentResultWithProblemDetails)}(HttpStatusCode.Unauthorized, null));");
                break;
            case HttpStatusCode.Forbidden:
                sb.AppendLine(4, $"public static {resultName} Forbidden()");
                sb.AppendLine(8, $"=> new {resultName}({nameof(Results.ResultFactory)}.{nameof(Results.ResultFactory.CreateContentResultWithProblemDetails)}(HttpStatusCode.Forbidden, null));");
                break;
            case HttpStatusCode.NotFound:
                sb.AppendLine(4, $"public static {resultName} NotFound(string? message = null)");
                sb.AppendLine(8, $"=> new {resultName}({nameof(Results.ResultFactory)}.{nameof(Results.ResultFactory.CreateContentResultWithProblemDetails)}(HttpStatusCode.NotFound, message));");
                break;
            case HttpStatusCode.Conflict:
                sb.AppendLine(4, $"public static {resultName} Conflict(object? error = null)");
                sb.AppendLine(8, $"=> new {resultName}({nameof(Results.ResultFactory)}.{nameof(Results.ResultFactory.CreateContentResultWithProblemDetails)}(HttpStatusCode.Conflict, error));");
                break;
            case HttpStatusCode.MethodNotAllowed:
            case HttpStatusCode.InternalServerError:
            case HttpStatusCode.NotImplemented:
            case HttpStatusCode.BadGateway:
            case HttpStatusCode.ServiceUnavailable:
            case HttpStatusCode.GatewayTimeout:
                sb.AppendLine(4, $"public static {resultName} {methodParameter.HttpStatusCode}(string? message = null)");
                sb.AppendLine(8, $"=> new {resultName}({nameof(Results.ResultFactory)}.{nameof(Results.ResultFactory.CreateContentResultWithProblemDetails)}(HttpStatusCode.{methodParameter.HttpStatusCode}, message));");
                break;
            default:
                sb.AppendLine($"// TODO: Not Implemented with WithProblemDetails for {methodParameter.HttpStatusCode}.");
                break;
        }
    }

    private static void AppendMethodContentWithoutProblemDetails(
        StringBuilder sb,
        ContentGeneratorServerResultMethodParameters methodParameter,
        string resultName)
    {
        switch (methodParameter.HttpStatusCode)
        {
            case HttpStatusCode.Created:
                sb.AppendLine(4, $"public static {resultName} Created()");
                sb.AppendLine(8, $"=> new {resultName}(new StatusCodeResult(StatusCodes.Status201Created));");
                break;
            case HttpStatusCode.Accepted:
                sb.AppendLine(4, $"public static {resultName} Accepted()");
                sb.AppendLine(8, $"=> new {resultName}(new AcceptedResult());");
                break;
            case HttpStatusCode.NoContent:
                sb.AppendLine(4, $"public static {resultName} NoContent()");
                sb.AppendLine(8, $"=> new {resultName}(new NoContentResult());");
                break;
            case HttpStatusCode.NotModified:
                sb.AppendLine(4, $"public static {resultName} NotModified()");
                sb.AppendLine(8, $"=> new {resultName}(new StatusCodeResult(StatusCodes.Status304NotModified));");
                break;
            case HttpStatusCode.BadRequest:
                sb.AppendLine(4, $"public static {resultName} BadRequest(string message)");
                sb.AppendLine(8, $"=> new {resultName}(new BadRequestObjectResult(message));");
                break;
            case HttpStatusCode.Unauthorized:
                sb.AppendLine(4, $"public static {resultName} Unauthorized()");
                sb.AppendLine(8, $"=> new {resultName}(new UnauthorizedResult());");
                break;
            case HttpStatusCode.Forbidden:
                sb.AppendLine(4, $"public static {resultName} Forbidden()");
                sb.AppendLine(8, $"=> new {resultName}(new StatusCodeResult(StatusCodes.Status403Forbidden));");
                break;
            case HttpStatusCode.NotFound:
                sb.AppendLine(4, $"public static {resultName} NotFound(string? message = null)");
                sb.AppendLine(8, $"=> new {resultName}(new NotFoundObjectResult(message));");
                break;
            case HttpStatusCode.Conflict:
                sb.AppendLine(4, $"public static {resultName} Conflict(string? error = null)");
                sb.AppendLine(8, $"=> new {resultName}(new ConflictObjectResult(error));");
                break;
            case HttpStatusCode.MethodNotAllowed:
            case HttpStatusCode.InternalServerError:
            case HttpStatusCode.NotImplemented:
            case HttpStatusCode.BadGateway:
            case HttpStatusCode.ServiceUnavailable:
            case HttpStatusCode.GatewayTimeout:
                sb.AppendLine(4, $"public static {resultName} {methodParameter.HttpStatusCode}(string? message = null)");
                sb.AppendLine(8, $"=> new {resultName}(new ContentResult {{ StatusCode = (int)HttpStatusCode.{methodParameter.HttpStatusCode}, Content = message }} );");
                break;
            default:
                sb.AppendLine($"// TODO: Not Implemented with WithoutProblemDetails for {methodParameter.HttpStatusCode}.");
                break;
        }
    }

    private static void AppendMethodSummary(
        StringBuilder sb,
        HttpStatusCode httpStatusCode)
    {
        sb.AppendLine(4, "/// <summary>");
        sb.AppendLine(4, $"/// {(int)httpStatusCode} - {httpStatusCode.ToNormalizedString()} response.");
        sb.AppendLine(4, "/// </summary>");
    }
}