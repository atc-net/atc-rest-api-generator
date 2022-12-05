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

        // TODO: Implement
        ////if (methodParameter.HttpStatusCode == HttpStatusCode.OK)
        ////{
        ////    AppendMethodContentStatusCodeOk(sb, methodParameter);
        ////    return;
        ////}

        if (methodParameter.UsesProblemDetails)
        {
            AppendMethodContentWithProblemDetails(sb, methodParameter, resultName);
        }
        else
        {
            AppendMethodContentWithoutProblemDetails(sb, methodParameter, resultName);
        }

        //switch (methodParameter.HttpStatusCode)
        //{
        //    case HttpStatusCode.Created:
                //methodDeclaration = useProblemDetails
                //    ? CreateTypeRequestWithSpecifiedResultFactoryMethod("CreateContentResult", className, httpStatusCode)
                //    : CreateStatusCodeResult(className, httpStatusCode);
                ////break;
            ////case HttpStatusCode.Accepted:
            ////    methodDeclaration = useProblemDetails
            ////        ? CreateTypeRequestWithSpecifiedResultFactoryMethod("CreateContentResultWithProblemDetails", className,
            ////            httpStatusCode)
            ////        : CreateTypeRequest(className, httpStatusCode, nameof(AcceptedResult));
            ////    break;
            ////case HttpStatusCode.NoContent:
            ////    methodDeclaration = useProblemDetails
            ////        ? CreateTypeRequestWithSpecifiedResultFactoryMethod("CreateContentResultWithProblemDetails", className,
            ////            httpStatusCode)
            ////        : CreateTypeRequest(className, httpStatusCode, nameof(NoContentResult));
            ////    break;
            ////case HttpStatusCode.NotModified:
            ////    methodDeclaration = useProblemDetails
            ////        ? CreateTypeRequestWithSpecifiedResultFactoryMethod("CreateContentResultWithProblemDetails", className,
            ////            httpStatusCode)
            ////        : CreateStatusCodeResult(className, httpStatusCode);
            ////    break;
            ////case HttpStatusCode.BadRequest:
            ////    methodDeclaration = useProblemDetails
            ////        ? CreateTypeRequestWithSpecifiedResultFactoryMethodWithMessageAllowNull(
            ////            "CreateContentResultWithValidationProblemDetails", className, httpStatusCode)
            ////        : CreateTypeRequestWithMessage(className, httpStatusCode, nameof(BadRequestObjectResult));
            ////    break;
            ////case HttpStatusCode.Unauthorized:
            ////    methodDeclaration = useProblemDetails
            ////        ? CreateTypeRequestWithSpecifiedResultFactoryMethod("CreateContentResultWithProblemDetails", className,
            ////            httpStatusCode)
            ////        : CreateTypeRequest(className, httpStatusCode, nameof(UnauthorizedResult));
            ////    break;
            ////case HttpStatusCode.Forbidden:
            ////    methodDeclaration = useProblemDetails
            ////        ? CreateTypeRequestWithSpecifiedResultFactoryMethod("CreateContentResultWithProblemDetails", className,
            ////            httpStatusCode)
            ////        : CreateStatusCodeResult(className, httpStatusCode);
            ////    break;
            ////case HttpStatusCode.NotFound:
            ////    methodDeclaration = useProblemDetails
            ////        ? CreateTypeRequestWithSpecifiedResultFactoryMethodWithMessageAllowNull(
            ////            "CreateContentResultWithProblemDetails", className, httpStatusCode)
            ////        : CreateTypeRequestWithMessageAllowNull(className, httpStatusCode, nameof(NotFoundObjectResult));
            ////    break;
            ////case HttpStatusCode.Conflict:
            ////    methodDeclaration = useProblemDetails
            ////        ? CreateTypeRequestWithSpecifiedResultFactoryMethodWithMessageAllowNull(
            ////            "CreateContentResultWithProblemDetails", className, httpStatusCode, "error", false)
            ////        : CreateTypeRequestWithMessageAllowNull(className, httpStatusCode, nameof(ConflictObjectResult),
            ////            "error");
            ////    break;
            ////case HttpStatusCode.MethodNotAllowed:
            ////case HttpStatusCode.InternalServerError:
            ////case HttpStatusCode.NotImplemented:
            ////case HttpStatusCode.BadGateway:
            ////case HttpStatusCode.ServiceUnavailable:
            ////case HttpStatusCode.GatewayTimeout:
            ////    methodDeclaration = useProblemDetails
            ////        ? CreateTypeRequestWithSpecifiedResultFactoryMethodWithMessageAllowNull(
            ////            "CreateContentResultWithProblemDetails", className, httpStatusCode)
            ////        : CreateTypeRequestWithObject(className, httpStatusCode, nameof(ContentResult));
            ////    break;
            ////default:
            ////    throw new NotImplementedException("Method: " + nameof(httpStatusCode) + " " + httpStatusCode);
        //}
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
                sb.AppendLine(4, $"public static {resultName} Created() => new {resultName}({nameof(Results.ResultFactory)}.{nameof(Results.ResultFactory.CreateContentResult)}(HttpStatusCode.Created, null));");
                break;
            case HttpStatusCode.Accepted:
                sb.AppendLine($"// TODO: Implement this WithProblemDetails {methodParameter.HttpStatusCode}.");
                break;
            case HttpStatusCode.NoContent:
                sb.AppendLine($"// TODO: Implement this WithProblemDetails {methodParameter.HttpStatusCode}.");
                break;
            case HttpStatusCode.NotModified:
                sb.AppendLine($"// TODO: Implement this WithProblemDetails {methodParameter.HttpStatusCode}.");
                break;
            case HttpStatusCode.BadRequest:
                sb.AppendLine($"// TODO: Implement this WithProblemDetails {methodParameter.HttpStatusCode}.");
                break;
            case HttpStatusCode.Unauthorized:
                sb.AppendLine($"// TODO: Implement this WithProblemDetails {methodParameter.HttpStatusCode}.");
                break;
            case HttpStatusCode.Forbidden:
                sb.AppendLine($"// TODO: Implement this WithProblemDetails {methodParameter.HttpStatusCode}.");
                break;
            case HttpStatusCode.NotFound:
                sb.AppendLine($"// TODO: Implement this WithProblemDetails {methodParameter.HttpStatusCode}.");
                // public static GetOrdersResult NotFound(string? message = null) => new GetOrdersResult(ResultFactory.CreateContentResultWithProblemDetails(HttpStatusCode.NotFound, message));
                break;
            case HttpStatusCode.Conflict:
                sb.AppendLine($"// TODO: Implement this WithProblemDetails {methodParameter.HttpStatusCode}.");
                break;
            case HttpStatusCode.MethodNotAllowed:
            case HttpStatusCode.InternalServerError:
            case HttpStatusCode.NotImplemented:
            case HttpStatusCode.BadGateway:
            case HttpStatusCode.ServiceUnavailable:
            case HttpStatusCode.GatewayTimeout:
                sb.AppendLine($"// TODO: Implement this WithProblemDetails {methodParameter.HttpStatusCode}.");
                break;
            default:
                sb.AppendLine($"// TODO: Implement this WithProblemDetails {methodParameter.HttpStatusCode}.");
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
                sb.AppendLine(4, $"public static {resultName} Created(string? message = null) => new {resultName}({nameof(Results.ResultFactory)}.{nameof(Results.ResultFactory.CreateContentResult)}(HttpStatusCode.Created, message))");
                break;
            case HttpStatusCode.Accepted:
                sb.AppendLine($"// TODO: Implement this WithoutProblemDetails {methodParameter.HttpStatusCode}.");
                break;
            case HttpStatusCode.NoContent:
                sb.AppendLine($"// TODO: Implement this WithoutProblemDetails {methodParameter.HttpStatusCode}.");
                break;
            case HttpStatusCode.NotModified:
                sb.AppendLine($"// TODO: Implement this WithoutProblemDetails {methodParameter.HttpStatusCode}.");
                break;
            case HttpStatusCode.BadRequest:
                sb.AppendLine($"// TODO: Implement this WithoutProblemDetails {methodParameter.HttpStatusCode}.");
                break;
            case HttpStatusCode.Unauthorized:
                sb.AppendLine($"// TODO: Implement this WithoutProblemDetails {methodParameter.HttpStatusCode}.");
                break;
            case HttpStatusCode.Forbidden:
                sb.AppendLine($"// TODO: Implement this WithoutProblemDetails {methodParameter.HttpStatusCode}.");
                break;
            case HttpStatusCode.NotFound:
                sb.AppendLine($"// TODO: Implement this WithoutProblemDetails {methodParameter.HttpStatusCode}.");
                break;
            case HttpStatusCode.Conflict:
                sb.AppendLine($"// TODO: Implement this WithoutProblemDetails {methodParameter.HttpStatusCode}.");
                break;
            case HttpStatusCode.MethodNotAllowed:
            case HttpStatusCode.InternalServerError:
            case HttpStatusCode.NotImplemented:
            case HttpStatusCode.BadGateway:
            case HttpStatusCode.ServiceUnavailable:
            case HttpStatusCode.GatewayTimeout:
                sb.AppendLine($"// TODO: Implement this WithoutProblemDetails {methodParameter.HttpStatusCode}.");
                break;
            default:
                sb.AppendLine($"// TODO: Implement this WithoutProblemDetails {methodParameter.HttpStatusCode}.");
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