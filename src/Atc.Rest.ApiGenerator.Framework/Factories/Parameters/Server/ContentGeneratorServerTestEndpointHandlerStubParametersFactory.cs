namespace Atc.Rest.ApiGenerator.Framework.Factories.Parameters.Server;

public static class ContentGeneratorServerTestEndpointHandlerStubParametersFactory
{
    public static ClassParameters Create(
        string headerContent,
        string @namespace,
        AttributeParameters codeGeneratorAttribute,
        OpenApiPathItem openApiPath,
        OpenApiOperation openApiOperation,
        string contractNamespace,
        AspNetOutputType aspNetOutputType)
    {
        ArgumentNullException.ThrowIfNull(@namespace);
        ArgumentNullException.ThrowIfNull(openApiOperation);
        ArgumentNullException.ThrowIfNull(contractNamespace);

        var operationName = openApiOperation.GetOperationName();

        var hasParameters = openApiPath.HasParameters() ||
                            openApiOperation.HasParametersOrRequestBody();

        var inheritedClassTypeName = EnsureFullNamespaceIfNeeded($"I{operationName}{ContentGeneratorConstants.Handler}", contractNamespace);
        var returnTypeName = EnsureFullNamespaceIfNeeded($"{operationName}{ContentGeneratorConstants.Result}", contractNamespace);

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

        var methodParameters = new List<MethodParameters>();

        var responseModel = openApiOperation
            .ExtractApiOperationResponseModels()
            .FirstOrDefault(x => x.StatusCode == HttpStatusCode.OK);

        if (responseModel is not null && responseModel.UseAsyncEnumerable)
        {
            methodParameters.Add(
                new(
                    DocumentationTags: null,
                    Attributes: null,
                    DeclarationModifier: DeclarationModifiers.Public,
                    ReturnTypeName: returnTypeName,
                    ReturnGenericTypeName: "Task",
                    Name: "ExecuteAsync",
                    Parameters: methodParametersParameters,
                    AlwaysBreakDownParameters: true,
                    UseExpressionBody: false,
                    Content: GenerateContentExecuteMethod(
                        contractNamespace,
                        returnTypeName,
                        openApiOperation,
                        hasParameters,
                        asAsyncEnumerable: true,
                        aspNetOutputType)));

            var parameterDataType = $"{responseModel.CollectionDataType}<{responseModel.GetQualifiedDataType()}>";
            var returnDataType = responseModel.CollectionDataType == NameConstants.List
                ? responseModel.GetQualifiedDataType()
                : $"{responseModel.CollectionDataType}<{responseModel.GetQualifiedDataType()}>";

            var methodParametersParametersForDataAsync = new List<ParameterBaseParameters>
                {
                    new(
                        Attributes: null,
                        GenericTypeName: null,
                        IsGenericListType: false,
                        TypeName: parameterDataType,
                        IsNullableType: false,
                        IsReferenceType: true,
                        Name: "data",
                        DefaultValue: null),
                };

            methodParameters.Add(
                new(
                    DocumentationTags: null,
                    Attributes: null,
                    DeclarationModifier: DeclarationModifiers.PrivateStaticAsync,
                    ReturnTypeName: returnDataType,
                    ReturnGenericTypeName: "IAsyncEnumerable",
                    Name: "GetDataAsync",
                    Parameters: methodParametersParametersForDataAsync,
                    AlwaysBreakDownParameters: true,
                    UseExpressionBody: false,
                    Content: GenerateContentAsyncEnumerableMethod(responseModel.CollectionDataType == NameConstants.List)));
        }
        else
        {
            methodParameters.Add(
                new(
                    DocumentationTags: null,
                    Attributes: null,
                    DeclarationModifier: DeclarationModifiers.Public,
                    ReturnTypeName: returnTypeName,
                    ReturnGenericTypeName: "Task",
                    Name: "ExecuteAsync",
                    Parameters: methodParametersParameters,
                    AlwaysBreakDownParameters: true,
                    UseExpressionBody: false,
                    Content: GenerateContentExecuteMethod(
                        contractNamespace,
                        returnTypeName,
                        openApiOperation,
                        hasParameters,
                        asAsyncEnumerable: false,
                        aspNetOutputType)));
        }

        return new ClassParameters(
            headerContent,
            @namespace,
            DocumentationTags: null,
            new List<AttributeParameters> { codeGeneratorAttribute },
            DeclarationModifiers.PublicClass,
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
        bool hasParameters,
        bool asAsyncEnumerable,
        AspNetOutputType aspNetOutputType)
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
                sb.Append($"return {GetTaskName(@namespace)}.FromResult({contractResultTypeName}.Ok(bytes, \"{MediaTypeNames.Text.Plain}\", \"dummy.txt\"));");
                break;
            default:
                if (returnSchema is not null)
                {
                    AppendGenerateContentForComplexData(
                        sb,
                        @namespace,
                        contractResultTypeName,
                        returnSchema,
                        hasParameters,
                        asAsyncEnumerable,
                        aspNetOutputType);
                }

                break;
        }

        return sb.ToString();
    }

    private static string GenerateContentAsyncEnumerableMethod(
        bool useList)
    {
        var sb = new StringBuilder();
        if (useList)
        {
            sb.AppendLine("foreach (var item in data)");
            sb.AppendLine("{");
            sb.AppendLine(4, "yield return item;");
            sb.AppendLine("}");
        }
        else
        {
            sb.AppendLine("yield return data;");
        }

        sb.AppendLine();
        sb.AppendLine("await Task.CompletedTask;");
        return sb.ToString();
    }

    private static void AppendGenerateContentForComplexData(
        StringBuilder sb,
        string @namespace,
        string contractResultTypeName,
        OpenApiSchema returnSchema,
        bool hasParameters,
        bool asAsyncEnumerable,
        AspNetOutputType aspNetOutputType)
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
                        if (aspNetOutputType == AspNetOutputType.Mvc)
                        {
                            sb.AppendLine($"var paginationData = new {genericDataTypeName}<{returnName}>");
                            sb.AppendLine("{");
                            sb.AppendLine(4, "PageSize = 10,");
                            sb.AppendLine(
                                4,
                                customPaginationSchema.Properties.Keys.Contains("results", StringComparer.OrdinalIgnoreCase)
                                    ? "Results = data,"
                                    : "Items = data,");
                            sb.AppendLine("};");
                        }
                        else
                        {
                            sb.AppendLine($"var paginationData = new {genericDataTypeName}<{returnName}>(10, null, data);");
                        }

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

                if (asAsyncEnumerable)
                {
                    sb.Append($"return {GetTaskName(@namespace)}.FromResult({contractResultTypeName}.Ok(GetDataAsync(paginationData)));");
                }
                else
                {
                    sb.Append($"return {GetTaskName(@namespace)}.FromResult({contractResultTypeName}.Ok(paginationData));");
                }
            }
            else
            {
                if (asAsyncEnumerable)
                {
                    sb.Append($"return {GetTaskName(@namespace)}.FromResult({contractResultTypeName}.Ok(GetDataAsync(data)));");
                }
                else
                {
                    sb.Append($"return {GetTaskName(@namespace)}.FromResult({contractResultTypeName}.Ok(data));");
                }
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

        var index = @namespace.IndexOf("Generated", StringComparison.Ordinal);
        if (index != -1)
        {
            @namespace = @namespace[index..];
        }

        return $"{@namespace}.{value}";
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