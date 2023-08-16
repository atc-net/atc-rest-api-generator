namespace Atc.Rest.ApiGenerator.Framework.Factories.Parameters.Server;

public static class ContentGeneratorServerTestEndpointHandlerStubParametersFactory
{
    public static ClassParameters Create(
        string headerContent,
        string @namespace,
        AttributeParameters codeGeneratorAttribute,
        OpenApiPathItem openApiPath,
        OpenApiOperation openApiOperation)
    {
        ArgumentNullException.ThrowIfNull(@namespace);
        ArgumentNullException.ThrowIfNull(openApiOperation);

        var operationName = openApiOperation.GetOperationName();

        var hasParameters = openApiPath.HasParameters() ||
                            openApiOperation.HasParametersOrRequestBody();

        var inheritedClassTypeName = EnsureFullNamespaceIfNeeded($"I{operationName}{ContentGeneratorConstants.Handler}", @namespace);
        var returnTypeName = EnsureFullNamespaceIfNeeded($"{operationName}{ContentGeneratorConstants.Result}", @namespace);

        var methodParametersParameters = new List<ParameterBaseParameters>();
        if (hasParameters)
        {
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

        methodParametersParameters.Add(
            new ParameterBaseParameters(
                Attributes: null,
                GenericTypeName: null,
                IsGenericListType: false,
                TypeName: "CancellationToken",
                IsReferenceType: true,
                Name: "cancellationToken",
                DefaultValue: "default"));

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
                Content: GenerateContentExecuteMethod(
                    @namespace,
                    returnTypeName,
                    openApiOperation,
                    hasParameters)),
        };

        return new ClassParameters(
            headerContent,
            @namespace,
            DocumentationTags: null,
            new List<AttributeParameters> { codeGeneratorAttribute },
            AccessModifiers.PublicClass,
            ClassTypeName: $"{operationName}{ContentGeneratorConstants.HandlerStub}",
            GenericTypeName: null,
            InheritedClassTypeName: inheritedClassTypeName,
            InheritedGenericClassTypeName: null,
            InheritedInterfaceTypeName: null,
            Constructors: null,
            Properties: null,
            Methods: methodParameters,
            GenerateToStringMethod: false);
    }

    [SuppressMessage("Performance", "CA1854:Prefer the 'IDictionary.TryGetValue(TKey, out TValue)' method", Justification = "OK.")]
    private static string GenerateContentExecuteMethod(
        string @namespace,
        string contractResultTypeName,
        OpenApiOperation openApiOperation,
        bool hasParameters)
    {
        var responseStatusCodes = openApiOperation.Responses.GetHttpStatusCodes();
        if (!responseStatusCodes.Any())
        {
            return "throw new NotImplementedException();";
        }

        if (!responseStatusCodes.Contains(HttpStatusCode.OK))
        {
            return responseStatusCodes.Contains(HttpStatusCode.Created)
                ? $"return {GetTaskName(@namespace)}.FromResult({contractResultTypeName}.Created());"
                : "throw new NotImplementedException();";
        }

        var returnType = string.Empty;
        var returnSchema = openApiOperation.Responses.GetSchemaForStatusCode(HttpStatusCode.OK);
        if (returnSchema is null)
        {
            if (responseStatusCodes.Contains(HttpStatusCode.OK) &&
                openApiOperation.Responses["200"].Content.ContainsKey(MediaTypeNames.Application.Octet))
            {
                returnType = "byte[]";
            }
        }
        else
        {
            if (returnSchema.IsTypeCustomPagination())
            {
                var customPaginationSchema = returnSchema.GetCustomPaginationSchema();
                var customPaginationItemsSchema = returnSchema.GetCustomPaginationItemsSchema();
                if (customPaginationSchema is not null &&
                    customPaginationItemsSchema is not null)
                {
                    returnType = customPaginationItemsSchema.IsSimpleDataType()
                        ? customPaginationItemsSchema.GetDataType()
                        : customPaginationItemsSchema.GetModelName();
                }
            }
            else
            {
                returnType = returnSchema.IsSimpleDataType()
                    ? returnSchema.GetDataType()
                    : returnSchema.GetModelType();
            }
        }

        var sb = new StringBuilder();
        switch (returnType)
        {
            case "":
            case null:
            case "string":
                sb.Append($"return {GetTaskName(@namespace)}.FromResult({contractResultTypeName}.Ok(\"Hallo world\"));");
                break;
            case "bool":
                sb.Append($"return {GetTaskName(@namespace)}.FromResult({contractResultTypeName}.Ok(true));");
                break;
            case "int":
            case "long":
                sb.Append($"return {GetTaskName(@namespace)}.FromResult({contractResultTypeName}.Ok(42));");
                break;
            case "float":
            case "double":
                sb.Append($"return {GetTaskName(@namespace)}.FromResult({contractResultTypeName}.Ok(42.2));");
                break;
            case "Guid":
                sb.Append($"return {GetTaskName(@namespace)}.FromResult({contractResultTypeName}.Ok(Guid.NewGuid()));");
                break;
            case "byte[]":
                sb.AppendLine("var bytes = Encoding.UTF8.GetBytes(\"Hello World\");");
                sb.Append($"return {GetTaskName(@namespace)}.FromResult({contractResultTypeName}.Ok(bytes, \"dummy.txt\"));");
                break;
            default:
                if (returnSchema is not null)
                {
                    AppendGenerateContentForComplexData(
                        sb,
                        @namespace,
                        contractResultTypeName,
                        returnSchema,
                        hasParameters);
                }

                break;
        }

        return sb.ToString();
    }

    private static void AppendGenerateContentForComplexData(
        StringBuilder sb,
        string @namespace,
        string contractResultTypeName,
        OpenApiSchema returnSchema,
        bool hasParameters)
    {
        var isTypeCustomPagination = returnSchema.IsTypeCustomPagination();

        var returnName = EnsureFullNamespaceIfNeeded(
            returnSchema.GetModelName(),
            @namespace);

        if (string.IsNullOrEmpty(returnName) &&
            (isTypeCustomPagination || returnSchema.IsTypeArrayOrPagination()))
        {
            if (isTypeCustomPagination)
            {
                var customPaginationItemsSchema = returnSchema.GetCustomPaginationItemsSchema();
                if (customPaginationItemsSchema is not null)
                {
                    if (customPaginationItemsSchema.IsSimpleDataType())
                    {
                        returnName = customPaginationItemsSchema.GetDataType();
                    }
                    else
                    {
                        returnName = EnsureFullNamespaceIfNeeded(
                            customPaginationItemsSchema.GetModelName(),
                            @namespace);
                    }
                }
            }
            else if (returnSchema.IsTypeArray())
            {
                returnName = returnSchema.GetSimpleDataTypeFromArray();
            }
            else if (returnSchema.IsTypePagination())
            {
                returnName = returnSchema.GetSimpleDataTypeFromPagination();
            }
        }

        if (isTypeCustomPagination ||
            returnSchema.IsTypeArrayOrPagination())
        {
            sb.AppendLine($"var data = new Fixture().Create<List<{returnName}>>();");
            sb.AppendLine();
            if (isTypeCustomPagination ||
                returnSchema.IsTypePagination())
            {
                if (isTypeCustomPagination)
                {
                    var customPaginationSchema = returnSchema.GetCustomPaginationSchema();
                    if (customPaginationSchema is not null)
                    {
                        var genericDataTypeName = customPaginationSchema.GetModelName();
                        sb.AppendLine($"var paginationData = new {genericDataTypeName}<{returnName}>();");
                        sb.AppendLine();
                    }
                }
                else
                {
                    sb.AppendLine($"var paginationData = new Pagination<{returnName}>(");
                    sb.AppendLine(4, "data,");
                    if (hasParameters)
                    {
                        var paginationParameters = returnSchema.GetPaginationParameters();
                        if (HasParametersForAtcRestPagination(paginationParameters))
                        {
                            sb.AppendLine(4, "parameters.PageSize,");
                            sb.AppendLine(4, "parameters.QueryString,");
                            sb.AppendLine(4, "parameters.ContinuationToken);");
                        }
                        else
                        {
                            sb.AppendLine(4, "pageSize: 10,");
                            sb.AppendLine(4, "queryString: null,");
                            sb.AppendLine(4, "pageIndex: 1,");
                            sb.AppendLine(4, "totalCount: 0);");
                        }

                        sb.AppendLine();
                    }
                    else
                    {
                        sb.AppendLine(4, "pageSize: 10,");
                        sb.AppendLine(4, "queryString: null,");
                        sb.AppendLine(4, "continuationToken: null);");
                        sb.AppendLine();
                    }
                }

                sb.Append($"return {GetTaskName(@namespace)}.FromResult({contractResultTypeName}.Ok(paginationData));");
            }
            else
            {
                sb.Append($"return {GetTaskName(@namespace)}.FromResult({contractResultTypeName}.Ok(data));");
            }
        }
        else
        {
            if (string.IsNullOrEmpty(returnName))
            {
                returnName = "string";
            }

            sb.AppendLine($"var data = new Fixture().Create<{returnName}>();");
            sb.AppendLine();
            sb.Append($"return {GetTaskName(@namespace)}.FromResult({contractResultTypeName}.Ok(data));");
        }
    }

    private static bool HasParametersForAtcRestPagination(
        IDictionary<string, OpenApiSchema> paginationParameters)
    {
        if (!paginationParameters.Any())
        {
            return false;
        }

        var keys = paginationParameters.Keys;
        return keys.Contains("PageSize", StringComparer.OrdinalIgnoreCase) &&
               keys.Contains("QueryString", StringComparer.OrdinalIgnoreCase) &&
               keys.Contains("ContinuationToken", StringComparer.OrdinalIgnoreCase);
    }

    private static string EnsureFullNamespaceIfNeeded(
        string value,
        string @namespace)
    {
        // TODO: Use OpenApiDocumentSchemaModelNameResolver
        string valueToTest;
        if (value.EndsWith("Handler", StringComparison.Ordinal))
        {
            valueToTest = value[..(value.LastIndexOf("Handler", StringComparison.Ordinal) - 1)];
        }
        else if (value.EndsWith("Result", StringComparison.Ordinal))
        {
            valueToTest = value[..(value.LastIndexOf("Result", StringComparison.Ordinal) - 1)];
        }
        else
        {
            valueToTest = value;
        }

        var namespaceComponents = @namespace.Split('.');

        if (!EndsWithWellKnownContract(@namespace, valueToTest) &&
            !EndsWithWellKnownSystemTypeName(valueToTest) &&
            !namespaceComponents.Contains(valueToTest, StringComparer.Ordinal))
        {
            return value;
        }

        var s1 = @namespace.Replace(".Generated", string.Empty, StringComparison.Ordinal);
        var s2 = s1.Replace("Tests.Endpoints.", "Generated.Contracts.", StringComparison.Ordinal);

        return $"{s2}.{value}";
    }

    private static bool EndsWithWellKnownContract(
        string @namespace,
        string valueToTest)
        => @namespace.EndsWith("." + valueToTest, StringComparison.Ordinal);

    private static bool EndsWithWellKnownSystemTypeName(
        string value)
        => value.EndsWith("Task", StringComparison.Ordinal) ||
           value.EndsWith("Tasks", StringComparison.Ordinal) ||
           value.EndsWith("Endpoint", StringComparison.Ordinal) ||
           value.EndsWith("EventArgs", StringComparison.Ordinal);

    private static string GetTaskName(
        string @namespace)
    {
        var taskName = "Task";
        var namespaceComponents = @namespace.Split('.');
        if (namespaceComponents.Contains("Task", StringComparer.Ordinal))
        {
            taskName = "System.Threading.Tasks.Task";
        }

        return taskName;
    }
}