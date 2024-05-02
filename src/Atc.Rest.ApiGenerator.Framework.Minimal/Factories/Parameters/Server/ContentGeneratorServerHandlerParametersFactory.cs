namespace Atc.Rest.ApiGenerator.Framework.Minimal.Factories.Parameters.Server;

public static class ContentGeneratorServerHandlerParametersFactory
{
    public static ClassParameters Create(
        string @namespace,
        string contractNamespace,
        OpenApiPathItem openApiPath,
        OpenApiOperation openApiOperation)
    {
        ArgumentNullException.ThrowIfNull(openApiOperation);

        var operationName = openApiOperation.GetOperationName();

        var hasParameters = openApiPath.HasParameters() ||
                            openApiOperation.HasParametersOrRequestBody();

        var methodParametersParameters = new List<ParameterBaseParameters>();
        if (hasParameters)
        {
            methodParametersParameters.Add(
                new ParameterBaseParameters(
                    Attributes: null,
                    GenericTypeName: null,
                    IsGenericListType: false,
                    TypeName: $"{operationName}{ContentGeneratorConstants.Parameters}",
                    IsNullableType: false,
                    IsReferenceType: true,
                    Name: "parameters",
                    DefaultValue: null));
        }

        methodParametersParameters.Add(
            new ParameterBaseParameters(
                Attributes: null,
                GenericTypeName: null,
                IsGenericListType: false,
                TypeName: "CancellationToken",
                IsNullableType: false,
                IsReferenceType: true,
                Name: "cancellationToken",
                DefaultValue: "default"));

        var returnTypeName = ConstructReturnTypeName(openApiOperation.Responses, contractNamespace);

        var methodParameters = new List<MethodParameters>
        {
            new(
                DocumentationTags: null,
                Attributes: null,
                AccessModifier: AccessModifiers.Public,
                ReturnTypeName: returnTypeName,
                ReturnGenericTypeName: "Task",
                Name: "ExecuteAsync",
                Parameters: methodParametersParameters,
                AlwaysBreakDownParameters: true,
                UseExpressionBody: false,
                Content: GenerateContentExecuteMethod(hasParameters, operationName)),
        };

        return new ClassParameters(
            HeaderContent: null,
            @namespace,
            openApiOperation.ExtractDocumentationTagsForHandler(),
            Attributes: null,
            AccessModifiers.PublicClass,
            ClassTypeName: $"{operationName}{ContentGeneratorConstants.Handler}",
            GenericTypeName: null,
            InheritedClassTypeName: $"I{operationName}{ContentGeneratorConstants.Handler}",
            InheritedGenericClassTypeName: null,
            InheritedInterfaceTypeName: null,
            Constructors: null,
            Properties: null,
            Methods: methodParameters,
            GenerateToStringMethod: false);
    }

    // TODO: Duplicate - copied from ContentGeneratorServerHandlerInterfaceParametersFactory
    private static string ConstructReturnTypeName(
        OpenApiResponses responses,
        string contractNamespace)
    {
        var responseReturnTypes = ExtractResponseReturnTypes(responses, contractNamespace);

        if (responseReturnTypes.Count == 1)
        {
            var (httpStatusCode, returnTypeName) = responseReturnTypes[0];
            if (httpStatusCode == HttpStatusCode.BadGateway)
            {
                return "ProblemHttpResult";
            }

            var responseReturnType = httpStatusCode.ToNormalizedString();

            return returnTypeName is null
                ? responseReturnType
                : $"{responseReturnType}<{returnTypeName}>";
        }

        var result = new List<string>();
        foreach (var (httpStatusCode, returnTypeName) in responseReturnTypes)
        {
            if (httpStatusCode == HttpStatusCode.BadGateway)
            {
                result.Add("ProblemHttpResult");
            }
            else
            {
                result.Add(returnTypeName is null
                    ? httpStatusCode.ToNormalizedString()
                    : $"{httpStatusCode.ToNormalizedString()}<{returnTypeName}>");
            }
        }

        return $"Results<{string.Join(", ", result)}>";
    }

    // TODO: Duplicate - copied from ContentGeneratorServerHandlerInterfaceParametersFactory
    private static List<(HttpStatusCode HttpStatusCode, string? ReturnTypeName)> ExtractResponseReturnTypes(
        OpenApiResponses responses,
        string contractNamespace)
    {
        var result = new List<(HttpStatusCode HttpStatusCode, string? ReturnTypeName)>();

        foreach (var response in responses.OrderBy(x => x.Key, StringComparer.Ordinal))
        {
            if (!Enum.TryParse(typeof(HttpStatusCode), response.Key, out var parsedType))
            {
                continue;
            }

            var httpStatusCode = (HttpStatusCode)parsedType;

            var isList = responses.IsSchemaTypeArrayForStatusCode(httpStatusCode);
            var modelName = responses.GetModelNameForStatusCode(httpStatusCode);

            // TODO: IsShared..
            ////var isShared = apiOperationSchemaMappings.IsShared(modelName);
            ////modelName = OpenApiDocumentSchemaModelNameResolver.EnsureModelNameWithNamespaceIfNeeded(projectName, apiGroupName, modelName, isShared);
            if (EndsWithWellKnownSystemTypeName(modelName))
            {
                // TODO: Hack for now, need to fix this in the future with EnsureModelNameWithNamespaceIfNeeded
                modelName = $"{contractNamespace}.{modelName}";
            }

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
                                            ////dataType = OpenApiDocumentSchemaModelNameResolver.EnsureModelNameWithNamespaceIfNeeded(projectName, apiGroupName, dataType, isShared); // TODO: FIx isshared
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
                case HttpStatusCode.Accepted:
                case HttpStatusCode.NoContent:
                case HttpStatusCode.Unauthorized:
                case HttpStatusCode.Forbidden:
                    result.Add((httpStatusCode, null));
                    break;
                case HttpStatusCode.BadRequest:
                case HttpStatusCode.NotFound:
                case HttpStatusCode.MethodNotAllowed:
                case HttpStatusCode.Conflict:
                case HttpStatusCode.InternalServerError:
                case HttpStatusCode.NotImplemented:
                case HttpStatusCode.BadGateway:
                case HttpStatusCode.ServiceUnavailable:
                case HttpStatusCode.GatewayTimeout:
                    result.Add((httpStatusCode, "string"));
                    break;
                default:
                    throw new NotImplementedException($"ResponseType for {(int)httpStatusCode} - {httpStatusCode} is missing.");
            }
        }

        return result;
    }

    public static ClassParameters CreateForCustomTest(
        string @namespace,
        OpenApiOperation openApiOperation)
    {
        var operationName = openApiOperation.GetOperationName();

        var methodParameters = new List<MethodParameters>
        {
            new(
                DocumentationTags: null,
                Attributes: AttributesParametersFactory.Create("Fact", "Skip = \"Change this to a real test\""),
                AccessModifier: AccessModifiers.Public,
                ReturnTypeName: "void",
                ReturnGenericTypeName: null,
                Name: "Sample",
                Parameters: null,
                AlwaysBreakDownParameters: false,
                UseExpressionBody: false,
                Content: GenerateContentTestSample()),
        };

        return new ClassParameters(
            HeaderContent: null,
            @namespace,
            DocumentationTags: null,
            Attributes: null,
            AccessModifiers.PublicClass,
            ClassTypeName: $"{operationName}{ContentGeneratorConstants.HandlerTests}",
            GenericTypeName: null,
            InheritedClassTypeName: null,
            InheritedGenericClassTypeName: null,
            InheritedInterfaceTypeName: null,
            Constructors: null,
            Properties: null,
            Methods: methodParameters,
            GenerateToStringMethod: false);
    }

    private static string GenerateContentExecuteMethod(
        bool hasParameters,
        string operationName)
    {
        var sb = new StringBuilder();
        if (hasParameters)
        {
            sb.AppendLine("ArgumentNullException.ThrowIfNull(parameters);");
            sb.AppendLine();
        }

        sb.Append($"throw new NotImplementedException(\"Add logic here for {operationName}{ContentGeneratorConstants.Handler}\");");
        return sb.ToString();
    }

    private static string GenerateContentTestSample()
    {
        var sb = new StringBuilder();
        sb.AppendLine(4, "{");
        sb.AppendLine(8, "// Arrange");
        sb.AppendLine();
        sb.AppendLine(8, "// Act");
        sb.AppendLine();
        sb.AppendLine(8, "// Assert");
        sb.Append(4, "}");
        return sb.ToString();
    }

    private static bool EndsWithWellKnownSystemTypeName(
        string value)
        => value.EndsWith("Task", StringComparison.Ordinal) ||
           value.EndsWith("Tasks", StringComparison.Ordinal) ||
           value.EndsWith("Endpoint", StringComparison.Ordinal) ||
           value.EndsWith("EventArgs", StringComparison.Ordinal);
}