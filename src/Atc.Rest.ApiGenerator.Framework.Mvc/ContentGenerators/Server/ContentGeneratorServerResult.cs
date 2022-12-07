// ReSharper disable SwitchStatementHandlesSomeKnownEnumValuesWithDefault
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

        // Implicit versions
        /*
            /// <summary>
            /// Performs an implicit conversion from CreateIotEdgeDeviceMappingByLocationIdResult to ActionResult.
            /// </summary>
            public static implicit operator CreateIotEdgeDeviceMappingByLocationIdResult(LocationIotEdgeDeviceMapping response) => Ok(response);
        */

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
            AppendMethodContentStatusCodeOk(sb, methodParameter);
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

    private static void AppendMethodContentStatusCodeOk(
        StringBuilder sb,
        ContentGeneratorServerResultMethodParameters methodParameter)
    {
        ////case HttpStatusCode.OK:
        ////    var useBinaryResponse = ApiOperation.Responses.IsSchemaUsingBinaryFormatForOkResponse();
        ////    if (useBinaryResponse)
        ////    {
        ////        methodDeclaration = CreateTypeRequestFileContentResult(className, httpStatusCode.ToNormalizedString());
        ////    }
        ////    else
        ////    {
        ////        var isPagination = ApiOperation.Responses.IsSchemaTypePaginationForStatusCode(httpStatusCode);
        ////        if (useProblemDetails)
        ////        {
        ////            if (string.IsNullOrEmpty(modelName))
        ////            {
        ////                if (schema is not null &&
        ////                    (schema.IsSimpleDataType() ||
        ////                     schema.IsTypeArrayOrPagination()))
        ////                {
        ////                    if (schema.IsSimpleDataType())
        ////                    {
        ////                        methodDeclaration = CreateTypeRequestObjectResult(className,
        ////                            httpStatusCode.ToNormalizedString(), schema.GetDataType(), "response", isList,
        ////                            isPagination);
        ////                    }
        ////                    else if (schema.IsTypeArray() &&
        ////                             schema.HasItemsWithSimpleDataType())
        ////                    {
        ////                        methodDeclaration = CreateTypeRequestObjectResult(className,
        ////                            httpStatusCode.ToNormalizedString(), schema.GetSimpleDataTypeFromArray(),
        ////                            "response", isList, isPagination);
        ////                    }
        ////                    else if (schema.IsTypePagination() &&
        ////                             schema.HasPaginationItemsWithSimpleDataType())
        ////                    {
        ////                        methodDeclaration = CreateTypeRequestObjectResult(className,
        ////                            httpStatusCode.ToNormalizedString(), schema.GetSimpleDataTypeFromPagination(),
        ////                            "response", isList, isPagination);
        ////                    }
        ////                    else
        ////                    {
        ////                        // This should not happen
        ////                        methodDeclaration = CreateTypeRequestObjectResult(className,
        ////                            httpStatusCode.ToNormalizedString(), "UnexpectedDataType");
        ////                    }
        ////                }
        ////                else
        ////                {
        ////                    methodDeclaration =
        ////                        CreateTypeRequestWithSpecifiedResultFactoryMethodWithMessageAllowNull(
        ////                            "CreateContentResult", className, httpStatusCode);
        ////                    HasCreateContentResult = true;
        ////                }
        ////            }
        ////            else
        ////            {
        ////                methodDeclaration = CreateTypeRequestObjectResult(className,
        ////                    httpStatusCode.ToNormalizedString(), modelName, "response", isList, isPagination);
        ////            }
        ////        }
        ////        else
        ////        {
        ////            if (string.IsNullOrEmpty(modelName))
        ////            {
        ////                if (schema is not null &&
        ////                    (schema.IsSimpleDataType() ||
        ////                     schema.IsTypeArrayOrPagination()))
        ////                {
        ////                    if (schema.IsSimpleDataType())
        ////                    {
        ////                        methodDeclaration = CreateTypeRequestObjectResult(className,
        ////                            httpStatusCode.ToNormalizedString(), schema.GetDataType(), "response", isList,
        ////                            isPagination);
        ////                    }
        ////                    else if (schema.IsTypeArray() &&
        ////                             schema.HasItemsWithSimpleDataType())
        ////                    {
        ////                        methodDeclaration = CreateTypeRequestObjectResult(className,
        ////                            httpStatusCode.ToNormalizedString(), schema.GetSimpleDataTypeFromArray(),
        ////                            "response", isList, isPagination);
        ////                    }
        ////                    else if (schema.IsTypePagination() &&
        ////                             schema.HasPaginationItemsWithSimpleDataType())
        ////                    {
        ////                        methodDeclaration = CreateTypeRequestObjectResult(className,
        ////                            httpStatusCode.ToNormalizedString(), schema.GetSimpleDataTypeFromPagination(),
        ////                            "response", isList, isPagination);
        ////                    }
        ////                    else
        ////                    {
        ////                        // This should not happen
        ////                        methodDeclaration = CreateTypeRequestObjectResult(className,
        ////                            httpStatusCode.ToNormalizedString(), "UnexpectedDataType");
        ////                    }
        ////                }
        ////                else if (schema is not null &&
        ////                         schema.IsTypeArray())
        ////                {
        ////                    methodDeclaration = CreateTypeRequestObjectResult(
        ////                        className,
        ////                        httpStatusCode.ToNormalizedString(),
        ////                        schema.Items.GetDataType(),
        ////                        "response",
        ////                        isList,
        ////                        isPagination);
        ////                }
        ////                else
        ////                {
        ////                    methodDeclaration =
        ////                        CreateTypeRequestWithMessageAllowNull(className, httpStatusCode,
        ////                            nameof(OkObjectResult));
        ////                }
        ////            }
        ////            else
        ////            {
        ////                methodDeclaration = CreateTypeRequestObjectResult(className,
        ////                    httpStatusCode.ToNormalizedString(), modelName, "response", isList, isPagination);
        ////            }
        ////        }
        ////    }
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
                sb.AppendLine(4, $"public static {resultName} BadRequest(string? message = null)");
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
                sb.AppendLine(4, $"public static {resultName} BadRequest(string? message = null)");
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