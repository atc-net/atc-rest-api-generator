namespace Atc.Rest.ApiGenerator.Framework.Minimal.Factories.Parameters.Server;

public static class ContentGeneratorServerHandlerInterfaceParametersFactory
{
    public static InterfaceParameters Create(
        bool useProblemDetails,
        string headerContent,
        string @namespace,
        AttributeParameters codeGeneratorAttribute,
        OpenApiPathItem openApiPath,
        OpenApiOperation openApiOperation)
    {
        ArgumentNullException.ThrowIfNull(openApiOperation);

        var operationName = openApiOperation.GetOperationName();

        var methodParametersAttributes = new Dictionary<string, string>(StringComparer.Ordinal);
        var methodParametersParameters = new List<ParameterBaseParameters>();
        if (openApiPath.HasParameters() ||
            openApiOperation.HasParametersOrRequestBody())
        {
            methodParametersAttributes.Add("parameters", "The parameters.");
            methodParametersParameters.Add(
                new ParameterBaseParameters(
                    Attributes: null,
                    GenericTypeName: null,
                    IsGenericListType: false,
                    TypeName: $"{operationName}{ContentGeneratorConstants.Parameters}",
                    IsReferenceType: true,
                    Name: "parameters",
                    DefaultValue: null));
        }

        methodParametersAttributes.Add("cancellationToken", "The cancellation token.");
        methodParametersParameters.Add(
            new ParameterBaseParameters(
                Attributes: null,
                GenericTypeName: null,
                IsGenericListType: false,
                TypeName: "CancellationToken",
                IsReferenceType: true,
                Name: "cancellationToken",
                DefaultValue: "default"));

        var returnTypeName = ConstructReturnTypeName(
            useProblemDetails,
            openApiOperation.Responses);

        var methodParameters = new List<MethodParameters>
        {
            new(
                DocumentationTags: new CodeDocumentationTags(
                    "Execute method",
                    parameters: methodParametersAttributes,
                    remark: null,
                    code: null,
                    example: null,
                    exceptions: null,
                    @return: null),
                Attributes: null,
                AccessModifier: AccessModifiers.None,
                ReturnTypeName: returnTypeName,
                ReturnGenericTypeName: "Task",
                Name: "ExecuteAsync",
                Parameters: methodParametersParameters,
                AlwaysBreakDownParameters: true,
                UseExpressionBody: false,
                Content: null),
        };

        return new InterfaceParameters(
            headerContent,
            @namespace,
            DocumentationTags: openApiOperation.ExtractDocumentationTagsForHandlerInterface(),
            new List<AttributeParameters> { codeGeneratorAttribute },
            AccessModifiers.Public,
            InterfaceTypeName: $"I{operationName}{ContentGeneratorConstants.Handler}",
            InheritedInterfaceTypeName: null,
            Properties: null,
            Methods: methodParameters);
    }

    private static string ConstructReturnTypeName(
        bool useProblemDetails,
        OpenApiResponses responses)
    {
        var responseReturnTypes = ExtractResponseReturnTypes(useProblemDetails, responses);

        if (responseReturnTypes.Count == 1)
        {
            var (httpStatusCode, returnTypeName) = responseReturnTypes[0];
            var responseReturnType = httpStatusCode.ToNormalizedString();

            return returnTypeName is null
                ? responseReturnType
                : $"{responseReturnType}<{returnTypeName}>";
        }

        var result = new List<string>();
        foreach (var (httpStatusCode, returnTypeName) in responseReturnTypes)
        {
            result.Add(returnTypeName is null
                ? httpStatusCode.ToNormalizedString()
                : $"{httpStatusCode.ToNormalizedString()}<{returnTypeName}>");
        }

        return $"Results<{string.Join(", ", result)}>";
    }

    private static List<(HttpStatusCode HttpStatusCode, string? ReturnTypeName)> ExtractResponseReturnTypes(
        bool useProblemDetails,
        OpenApiResponses responses)
    {
        var result = new List<(HttpStatusCode HttpStatusCode, string? ReturnTypeName)>();

        foreach (var response in responses.OrderBy(x => x.Key, StringComparer.Ordinal))
        {
            if (!Enum.TryParse(typeof(HttpStatusCode), response.Key, out var parsedType))
            {
                continue;
            }

            var httpStatusCode = parsedType is HttpStatusCode code ? code : 0;

            var isList = responses.IsSchemaTypeArrayForStatusCode(httpStatusCode);
            var modelName = responses.GetModelNameForStatusCode(httpStatusCode);

            // TODO: IsShared..
            ////var isShared = apiOperationSchemaMappings.IsShared(modelName);
            ////modelName = OpenApiDocumentSchemaModelNameResolver.EnsureModelNameWithNamespaceIfNeeded(projectName, apiGroupName, modelName, isShared);

            switch (httpStatusCode)
            {
                case HttpStatusCode.OK:
                case HttpStatusCode.Created:
                    string? typeResponseName = null;

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
                                typeResponseName = $"IEnumerable<{dataType}>";
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

                    result.Add((httpStatusCode, typeResponseName));

                    break;
                case HttpStatusCode.NotModified:
                    result.Add((httpStatusCode, null));
                    break;
                case HttpStatusCode.Accepted:
                case HttpStatusCode.NoContent:
                case HttpStatusCode.Unauthorized:
                case HttpStatusCode.Forbidden:
                    result.Add(useProblemDetails
                        ? (httpStatusCode, "ProblemDetails")
                        : (httpStatusCode, null));
                    break;
                case HttpStatusCode.BadRequest:
                    result.Add(useProblemDetails
                        ? (httpStatusCode, "ValidationProblemDetails")
                        : (httpStatusCode, "string"));
                    break;
                case HttpStatusCode.NotFound:
                case HttpStatusCode.MethodNotAllowed:
                case HttpStatusCode.Conflict:
                case HttpStatusCode.InternalServerError:
                case HttpStatusCode.NotImplemented:
                case HttpStatusCode.BadGateway:
                case HttpStatusCode.ServiceUnavailable:
                case HttpStatusCode.GatewayTimeout:
                    result.Add(useProblemDetails
                        ? (httpStatusCode, "ProblemDetails")
                        : (httpStatusCode, "string"));
                    break;
                default:
                    throw new NotImplementedException($"ResponseType for {(int)httpStatusCode} - {httpStatusCode} is missing.");
            }
        }

        return result;
    }
}