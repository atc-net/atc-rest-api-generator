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
            AccessModifiers.Public,
            ClassTypeName: $"{operationName}{ContentGeneratorConstants.HandlerStub}",
            InheritedClassTypeName: inheritedClassTypeName,
            InheritedGenericClassTypeName: null,
            InheritedInterfaceTypeName: null,
            Constructors: null,
            Properties: null,
            Methods: methodParameters,
            GenerateToStringMethode: false);
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

        if (!EndsWithWellKnownSystemTypeName(valueToTest) &&
            !EndsWithWellKnownSystemTypeNameHackForPerstore(@namespace, valueToTest))
        {
            return value;
        }

        var s1 = @namespace.Replace(".Generated", string.Empty, StringComparison.Ordinal);
        var s2 = s1.Replace("Tests.Endpoints.", "Generated.Contracts.", StringComparison.Ordinal);

        return $"{s2}.{value}";
    }

    private static bool EndsWithWellKnownSystemTypeName(
        string value)
        => value.EndsWith("Task", StringComparison.Ordinal) ||
           value.EndsWith("Tasks", StringComparison.Ordinal) ||
           value.EndsWith("Endpoint", StringComparison.Ordinal) ||
           value.EndsWith("EventArgs", StringComparison.Ordinal);

    // TODO: Fix hack later on...
    private static bool EndsWithWellKnownSystemTypeNameHackForPerstore(
        string @namespace,
        string value)
        => @namespace.Contains("Petstore", StringComparison.Ordinal) &&
           (value.EndsWith("User", StringComparison.Ordinal) ||
            value.EndsWith("Pet", StringComparison.Ordinal));

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
                ? $"return Task.FromResult({contractResultTypeName}.Created());"
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
            returnType = returnSchema.IsSimpleDataType()
                 ? returnSchema.GetDataType()
                 : returnSchema.GetModelType();
        }

        var sb = new StringBuilder();
        switch (returnType)
        {
            case "":
            case null:
            case "string":
                sb.Append($"return Task.FromResult({contractResultTypeName}.Ok(\"Hallo world\"));");
                break;
            case "bool":
                sb.Append($"return Task.FromResult({contractResultTypeName}.Ok(true));");
                break;
            case "int":
            case "long":
                sb.Append($"return Task.FromResult({contractResultTypeName}.Ok(42));");
                break;
            case "float":
            case "double":
                sb.Append($"return Task.FromResult({contractResultTypeName}.Ok(42.2));");
                break;
            case "Guid":
                sb.Append($"return Task.FromResult({contractResultTypeName}.Ok(Guid.NewGuid()));");
                break;
            case "byte[]":
                sb.AppendLine("var bytes = Encoding.UTF8.GetBytes(\"Hello World\");");
                sb.Append($"return Task.FromResult({contractResultTypeName}.Ok(bytes, \"dummy.txt\"));");
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
        var returnName = EnsureFullNamespaceIfNeeded(
            returnSchema.GetModelName(),
            @namespace);

        if (string.IsNullOrEmpty(returnName) &&
            returnSchema.IsTypeArrayOrPagination())
        {
            if (returnSchema.IsTypeArray())
            {
                returnName = returnSchema.GetSimpleDataTypeFromArray();
            }
            else if (returnSchema.IsTypePagination())
            {
                returnName = returnSchema.GetSimpleDataTypeFromPagination();
            }
        }

        if (returnSchema.IsTypeArrayOrPagination())
        {
            sb.AppendLine($"var data = new Fixture().Create<List<{returnName}>>();");
            sb.AppendLine();
            if (returnSchema.IsTypePagination())
            {
                sb.AppendLine($"var paginationData = new Pagination<{returnName}>(");
                sb.AppendLine(4, "data,");
                if (hasParameters)
                {
                    sb.AppendLine(4, "parameters.PageSize,");
                    sb.AppendLine(4, "parameters.QueryString,");
                    sb.AppendLine(4, "parameters.ContinuationToken);");
                    sb.AppendLine();
                }
                else
                {
                    sb.AppendLine(4, "pageSize: 10,");
                    sb.AppendLine(4, "queryString: null,");
                    sb.AppendLine(4, "continuationToken: null);");
                    sb.AppendLine();
                }

                sb.Append($"return Task.FromResult({contractResultTypeName}.Ok(paginationData));");
            }
            else
            {
                sb.Append($"return Task.FromResult({contractResultTypeName}.Ok(data));");
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
            sb.Append($"return Task.FromResult({contractResultTypeName}.Ok(data));");
        }
    }
}