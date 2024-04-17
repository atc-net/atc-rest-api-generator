namespace Atc.Rest.ApiGenerator.OpenApi.Extensions;

public static class OpenApiResponsesExtensions
{
    public static List<string> GetProducesResponseAttributeParts(
        this OpenApiResponses responses,
        IList<ApiOperation> apiOperationSchemaMappings,
        string apiGroupName,
        string projectName,
        bool useProblemDetailsAsDefaultResponseBody,
        bool includeIfNotDefinedValidation,
        bool includeIfNotDefinedAuthorization,
        bool includeIfNotDefinedInternalServerError)
    {
        var responseTypes = GetResponseTypes(
            responses,
            apiOperationSchemaMappings,
            apiGroupName,
            projectName,
            useProblemDetailsAsDefaultResponseBody,
            includeEmptyResponseTypes: true,
            includeIfNotDefinedValidation,
            includeIfNotDefinedAuthorization,
            includeIfNotDefinedInternalServerError);

        return responseTypes
            .OrderBy(x => x.Item1)
            .Select(x => string.IsNullOrEmpty(x.Item2)
                ? $"ProducesResponseType(StatusCodes.Status{(int)x.Item1}{x.Item1})"
                : $"ProducesResponseType(typeof({x.Item2}), StatusCodes.Status{(int)x.Item1}{x.Item1})")
            .ToList();
    }

    public static List<Tuple<HttpStatusCode, string>> GetResponseTypes(
        this OpenApiResponses responses,
        IList<ApiOperation> apiOperationSchemaMappings,
        string apiGroupName,
        string projectName,
        bool useProblemDetailsAsDefaultResponseBody,
        bool includeEmptyResponseTypes,
        bool includeIfNotDefinedValidation,
        bool includeIfNotDefinedAuthorization,
        bool includeIfNotDefinedInternalServerError)
    {
        var result = new List<Tuple<HttpStatusCode, string>>();
        foreach (var response in responses.OrderBy(x => x.Key, StringComparer.Ordinal))
        {
            if (!Enum.TryParse(typeof(HttpStatusCode), response.Key, out var parsedType))
            {
                continue;
            }

            var httpStatusCode = parsedType is HttpStatusCode code ? code : 0;

            var isList = responses.IsSchemaTypeArrayForStatusCode(httpStatusCode);
            var modelName = responses.GetModelNameForStatusCode(httpStatusCode);

            var isShared = apiOperationSchemaMappings.IsShared(modelName);
            modelName = OpenApiDocumentSchemaModelNameResolver.EnsureModelNameWithNamespaceIfNeeded(projectName, apiGroupName, modelName, isShared);

            var useProblemDetails = responses.IsSchemaTypeProblemDetailsForStatusCode(httpStatusCode);
            if (!useProblemDetails &&
                useProblemDetailsAsDefaultResponseBody)
            {
                useProblemDetails = true;
            }

            string? typeResponseName = null;
            switch (httpStatusCode)
            {
                case HttpStatusCode.OK:
                case HttpStatusCode.Created:
                    var dataType = modelName;

                    var useBinaryResponse = responses.IsSchemaUsingBinaryFormatForOkResponse();
                    if (useBinaryResponse)
                    {
                        typeResponseName = "byte[]";
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(modelName))
                        {
                            var schema = responses.GetSchemaForStatusCode(httpStatusCode);
                            if (schema is not null)
                            {
                                if (schema.IsSimpleDataType())
                                {
                                    dataType = schema.GetDataType();
                                }
                                else if (schema.HasArrayItemsWithSimpleDataType())
                                {
                                    dataType = schema.GetSimpleDataTypeFromArray();
                                }
                                else if (schema.HasPaginationItemsWithSimpleDataType())
                                {
                                    dataType = schema.GetSimpleDataTypeFromPagination();
                                }
                                else if (schema.IsTypeCustomPagination())
                                {
                                    var customPaginationSchema = schema.GetCustomPaginationSchema();
                                    var customPaginationItemsSchema = schema.GetCustomPaginationItemsSchema();
                                    if (customPaginationSchema is not null &&
                                        customPaginationItemsSchema is not null)
                                    {
                                        var genericDataTypeName = customPaginationSchema.GetDataType();
                                        if (customPaginationItemsSchema.Items.IsSimpleDataType())
                                        {
                                            dataType = customPaginationItemsSchema.GetDataType();
                                            typeResponseName = $"{genericDataTypeName}<{dataType}>";
                                        }
                                        else
                                        {
                                            dataType = customPaginationItemsSchema.GetModelName();
                                            dataType = OpenApiDocumentSchemaModelNameResolver.EnsureModelNameWithNamespaceIfNeeded(projectName, apiGroupName, dataType, isShared);
                                            typeResponseName = $"{genericDataTypeName}<{dataType}>";
                                        }
                                    }
                                }
                            }

                            if (string.IsNullOrEmpty(dataType))
                            {
                                dataType = "string";
                            }
                        }

                        if (string.IsNullOrEmpty(typeResponseName))
                        {
                            if (isList)
                            {
                                typeResponseName = $"IEnumerable<{dataType}>"; // TODO: Verify MVC code
                            }
                            else
                            {
                                var isPagination = responses.IsSchemaTypePaginationForStatusCode(httpStatusCode);
                                typeResponseName = isPagination
                                    ? $"{NameConstants.Pagination}<{dataType}>"
                                    : dataType;
                            }
                        }
                    }

                    break;
                case HttpStatusCode.NotModified:
                    typeResponseName = null;
                    break;
                case HttpStatusCode.Accepted:
                case HttpStatusCode.NoContent:
                case HttpStatusCode.Unauthorized:
                case HttpStatusCode.Forbidden:
                    typeResponseName = useProblemDetails
                        ? "ProblemDetails"
                        : null;
                    break;
                case HttpStatusCode.BadRequest:
                    typeResponseName = useProblemDetails
                        ? "ValidationProblemDetails"
                        : "string";
                    break;
                case HttpStatusCode.NotFound:
                case HttpStatusCode.MethodNotAllowed:
                case HttpStatusCode.Conflict:
                case HttpStatusCode.InternalServerError:
                case HttpStatusCode.NotImplemented:
                case HttpStatusCode.BadGateway:
                case HttpStatusCode.ServiceUnavailable:
                case HttpStatusCode.GatewayTimeout:
                    typeResponseName = useProblemDetails
                        ? "ProblemDetails"
                        : "string";
                    break;
                default:
                    throw new NotImplementedException($"ResponseType for {(int)httpStatusCode} - {httpStatusCode} is missing.");
            }

            if (typeResponseName is not null)
            {
                result.Add(Tuple.Create(httpStatusCode, typeResponseName));
            }
            else if (includeEmptyResponseTypes)
            {
                result.Add(Tuple.Create(httpStatusCode, string.Empty));
            }
        }

        // TODO: For MinimalApi - Should we use ProblemDetails or move to "middleware generate a response body"
        if (useProblemDetailsAsDefaultResponseBody &&
            includeIfNotDefinedValidation &&
            result.TrueForAll(x => x.Item1 != HttpStatusCode.BadRequest))
        {
            result.Add(Tuple.Create(HttpStatusCode.BadRequest, "ValidationProblemDetails"));
        }

        if (includeIfNotDefinedAuthorization)
        {
            if (result.TrueForAll(x => x.Item1 != HttpStatusCode.Unauthorized))
            {
                result.Add(useProblemDetailsAsDefaultResponseBody
                    ? Tuple.Create(HttpStatusCode.Unauthorized, "ProblemDetails")
                    : Tuple.Create(HttpStatusCode.Unauthorized, string.Empty));
            }

            if (result.TrueForAll(x => x.Item1 != HttpStatusCode.Forbidden))
            {
                result.Add(useProblemDetailsAsDefaultResponseBody
                    ? Tuple.Create(HttpStatusCode.Forbidden, "ProblemDetails")
                    : Tuple.Create(HttpStatusCode.Forbidden, string.Empty));
            }
        }

        if (includeIfNotDefinedInternalServerError &&
            result.TrueForAll(x => x.Item1 != HttpStatusCode.InternalServerError))
        {
            result.Add(useProblemDetailsAsDefaultResponseBody
                ? Tuple.Create(HttpStatusCode.InternalServerError, "ProblemDetails")
                : Tuple.Create(HttpStatusCode.InternalServerError, "string"));
        }

        return result;
    }
}