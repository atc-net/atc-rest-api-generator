// ReSharper disable UseDeconstruction
// ReSharper disable SuggestBaseTypeForParameter
namespace Atc.Rest.ApiGenerator.Helpers.XunitTest;

[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:Parameter should not span multiple lines", Justification = "OK.")]
public static class GenerateServerApiXunitTestEndpointHandlerStubHelper
{
    public static void Generate(
        ILogger logger,
        HostProjectOptions hostProjectOptions,
        EndpointMethodMetadata endpointMethodMetadata)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(hostProjectOptions);
        ArgumentNullException.ThrowIfNull(endpointMethodMetadata);

        var sb = new StringBuilder();

        GenerateCodeHelper.AppendGeneratedCodeWarningComment(sb, hostProjectOptions.ApiGeneratorNameAndVersion);
        AppendNamespaceAndClassStart(sb, hostProjectOptions, endpointMethodMetadata);
        AppendMethodExecuteAsyncStart(sb, endpointMethodMetadata);
        AppendMethodExecuteAsyncContent(sb, endpointMethodMetadata);
        AppendMethodExecuteAsyncEnd(sb);
        AppendNamespaceAndClassEnd(sb);
        SaveFile(logger, sb, hostProjectOptions, endpointMethodMetadata);
    }

    private static void AppendNamespaceAndClassStart(
        StringBuilder sb,
        HostProjectOptions hostProjectOptions,
        EndpointMethodMetadata endpointMethodMetadata)
    {
        sb.AppendLine($"namespace {hostProjectOptions.ProjectName}.Tests.Endpoints.{endpointMethodMetadata.SegmentName}.Generated");
        sb.AppendLine("{");

        GenerateCodeHelper.AppendGeneratedCodeAttribute(sb, hostProjectOptions.ApiGeneratorName, hostProjectOptions.ApiGeneratorVersion);
        sb.AppendLine(4, $"public class {endpointMethodMetadata.MethodName}HandlerStub : {endpointMethodMetadata.ContractInterfaceHandlerTypeName}");
        sb.AppendLine(4, "{");
    }

    private static void AppendMethodExecuteAsyncStart(
        StringBuilder sb,
        EndpointMethodMetadata endpointMethodMetadata)
    {
        sb.AppendLine(8, endpointMethodMetadata.ContractParameterTypeName is null
            ? $"public Task<{endpointMethodMetadata.ContractResultTypeName}> ExecuteAsync(CancellationToken cancellationToken = default)"
            : $"public Task<{endpointMethodMetadata.ContractResultTypeName}> ExecuteAsync({endpointMethodMetadata.ContractParameterTypeName} parameters, CancellationToken cancellationToken = default)");
        sb.AppendLine(8, "{");
    }

    private static void AppendMethodExecuteAsyncContent(
        StringBuilder sb,
        EndpointMethodMetadata endpointMethodMetadata)
    {
        if (endpointMethodMetadata.ContractReturnTypeNames.Find(x => x.StatusCode == HttpStatusCode.OK) is not null)
        {
            AppendContentForExecuteAsynchronous(sb, endpointMethodMetadata, HttpStatusCode.OK);
        }
        else if (endpointMethodMetadata.ContractReturnTypeNames.Find(x => x.StatusCode == HttpStatusCode.Created) is not null)
        {
            AppendContentForExecuteAsynchronous(sb, endpointMethodMetadata, HttpStatusCode.Created);
        }
        else
        {
            sb.AppendLine(12, "throw new System.NotImplementedException();");
        }
    }

    private static void AppendContentForExecuteAsynchronous(
        StringBuilder sb,
        EndpointMethodMetadata endpointMethodMetadata,
        HttpStatusCode httpStatusCode)
    {
        var contractReturnTypeName = endpointMethodMetadata.ContractReturnTypeNames.First(x => x.StatusCode == httpStatusCode);
        var returnTypeName = contractReturnTypeName.FullModelName;

        switch (returnTypeName)
        {
            case "string":
                sb.AppendLine(
                    12,
                    httpStatusCode == HttpStatusCode.Created
                        ? $"return {OpenApiDocumentSchemaModelNameHelper.EnsureTaskNameWithNamespaceIfNeeded(contractReturnTypeName)}.FromResult({endpointMethodMetadata.ContractResultTypeName}.{httpStatusCode.ToNormalizedString()}());"
                        : $"return {OpenApiDocumentSchemaModelNameHelper.EnsureTaskNameWithNamespaceIfNeeded(contractReturnTypeName)}.FromResult({endpointMethodMetadata.ContractResultTypeName}.{httpStatusCode.ToNormalizedString()}(\"Hallo world\"));");
                break;
            case "bool":
                sb.AppendLine(
                    12,
                    httpStatusCode == HttpStatusCode.Created
                        ? $"return {OpenApiDocumentSchemaModelNameHelper.EnsureTaskNameWithNamespaceIfNeeded(contractReturnTypeName)}.FromResult({endpointMethodMetadata.ContractResultTypeName}.{httpStatusCode.ToNormalizedString()}());"
                        : $"return {OpenApiDocumentSchemaModelNameHelper.EnsureTaskNameWithNamespaceIfNeeded(contractReturnTypeName)}.FromResult({endpointMethodMetadata.ContractResultTypeName}.{httpStatusCode.ToNormalizedString()}(true));");
                break;
            case "int":
            case "long":
                sb.AppendLine(
                    12,
                    httpStatusCode == HttpStatusCode.Created
                        ? $"return {OpenApiDocumentSchemaModelNameHelper.EnsureTaskNameWithNamespaceIfNeeded(contractReturnTypeName)}.FromResult({endpointMethodMetadata.ContractResultTypeName}.{httpStatusCode.ToNormalizedString()}());"
                        : $"return {OpenApiDocumentSchemaModelNameHelper.EnsureTaskNameWithNamespaceIfNeeded(contractReturnTypeName)}.FromResult({endpointMethodMetadata.ContractResultTypeName}.{httpStatusCode.ToNormalizedString()}(42));");
                break;
            case "float":
            case "double":
                sb.AppendLine(
                    12,
                    httpStatusCode == HttpStatusCode.Created
                        ? $"return {OpenApiDocumentSchemaModelNameHelper.EnsureTaskNameWithNamespaceIfNeeded(contractReturnTypeName)}.FromResult({endpointMethodMetadata.ContractResultTypeName}.{httpStatusCode.ToNormalizedString()}());"
                        : $"return {OpenApiDocumentSchemaModelNameHelper.EnsureTaskNameWithNamespaceIfNeeded(contractReturnTypeName)}.FromResult({endpointMethodMetadata.ContractResultTypeName}.{httpStatusCode.ToNormalizedString()}(42.2));");
                break;
            case "Guid":
                sb.AppendLine(
                    12,
                    httpStatusCode == HttpStatusCode.Created
                        ? $"return {OpenApiDocumentSchemaModelNameHelper.EnsureTaskNameWithNamespaceIfNeeded(contractReturnTypeName)}.FromResult({endpointMethodMetadata.ContractResultTypeName}.{httpStatusCode.ToNormalizedString()}());"
                        : $"return {OpenApiDocumentSchemaModelNameHelper.EnsureTaskNameWithNamespaceIfNeeded(contractReturnTypeName)}.FromResult({endpointMethodMetadata.ContractResultTypeName}.{httpStatusCode.ToNormalizedString()}(System.Guid.NewGuid()));");
                break;
            case "byte[]":
                sb.AppendLine(12, "var bytes = System.Text.Encoding.UTF8.GetBytes(\"Hello World\");");
                sb.AppendLine(12, $"return {OpenApiDocumentSchemaModelNameHelper.EnsureTaskNameWithNamespaceIfNeeded(contractReturnTypeName)}.FromResult({endpointMethodMetadata.ContractResultTypeName}.{httpStatusCode.ToNormalizedString()}(bytes, \"dummy.txt\"));");
                break;
            default:
            {
                var singleReturnTypeName = OpenApiDocumentSchemaModelNameResolver.GetRawModelName(returnTypeName);
                var simpleTypePair = SimpleTypeHelper.BeautifySimpleTypeLookup.FirstOrDefault(x => x.Value == singleReturnTypeName);

                if (simpleTypePair.Key is not null)
                {
                    GenerateXunitTestHelper.AppendVarDataListSimpleType(
                        12,
                        sb,
                        simpleTypePair.Value);
                    sb.AppendLine();
                }
                else
                {
                    var modelSchema = endpointMethodMetadata.ComponentsSchemas.GetSchemaByModelName(singleReturnTypeName);

                    GenerateXunitTestHelper.AppendVarDataModelOrListOfModel(
                        12,
                        sb,
                        endpointMethodMetadata,
                        modelSchema,
                        httpStatusCode,
                        ApiSchemaMapLocatedAreaType.Response);
                    sb.AppendLine();
                }

                if (contractReturnTypeName.Schema is null ||
                    GenerateXunitTestPartsHelper.IsListKind(returnTypeName))
                {
                    if (returnTypeName.StartsWith(Microsoft.OpenApi.Models.NameConstants.Pagination, StringComparison.Ordinal))
                    {
                        if (endpointMethodMetadata.ContractParameter is not null)
                        {
                            var queryParameters = endpointMethodMetadata.ContractParameter.ApiOperation.Parameters.GetAllFromQuery();
                            var sPageSize = "10";
                            if (queryParameters.Find(x => x.Name.Equals("PageSize", StringComparison.OrdinalIgnoreCase)) is not null)
                            {
                                sPageSize = "parameters.PageSize";
                            }

                            var sQueryString = "null";
                            if (queryParameters.Find(x => x.Name.Equals("QueryString", StringComparison.OrdinalIgnoreCase)) is not null)
                            {
                                sQueryString = "parameters.QueryString";
                            }

                            var sContinuationToken = "null";
                            if (queryParameters.Find(x => x.Name.Equals("ContinuationToken", StringComparison.OrdinalIgnoreCase)) is not null)
                            {
                                sContinuationToken = "parameters.ContinuationToken";
                            }

                            sb.AppendLine(12, $"var paginationData = new {contractReturnTypeName.FullModelName}(data, {sPageSize}, {sQueryString}, {sContinuationToken});");
                        }
                        else
                        {
                            sb.AppendLine(12, $"var paginationData = new {contractReturnTypeName.FullModelName}(data, 10, null, null);");
                        }

                        sb.AppendLine(12, $"return {OpenApiDocumentSchemaModelNameHelper.EnsureTaskNameWithNamespaceIfNeeded(contractReturnTypeName)}.FromResult({endpointMethodMetadata.ContractResultTypeName}.{httpStatusCode.ToNormalizedString()}(paginationData));");
                    }
                    else
                    {
                        sb.AppendLine(12, $"return {OpenApiDocumentSchemaModelNameHelper.EnsureTaskNameWithNamespaceIfNeeded(contractReturnTypeName)}.FromResult({endpointMethodMetadata.ContractResultTypeName}.{httpStatusCode.ToNormalizedString()}(data));");
                    }
                }
                else
                {
                    sb.AppendLine(12, $"return {OpenApiDocumentSchemaModelNameHelper.EnsureTaskNameWithNamespaceIfNeeded(contractReturnTypeName)}.FromResult({endpointMethodMetadata.ContractResultTypeName}.{httpStatusCode.ToNormalizedString()}(data));");
                }

                break;
            }
        }
    }

    private static void AppendMethodExecuteAsyncEnd(
        StringBuilder sb)
        => sb.AppendLine(8, "}");

    private static void AppendNamespaceAndClassEnd(
        StringBuilder sb)
    {
        sb.AppendLine(4, "}");
        sb.AppendLine("}");
    }

    private static void SaveFile(
        ILogger logger,
        StringBuilder sb,
        HostProjectOptions hostProjectOptions,
        EndpointMethodMetadata endpointMethodMetadata)
    {
        var pathA = Path.Combine(hostProjectOptions.PathForTestGenerate!.FullName, "Endpoints");
        var pathB = Path.Combine(pathA, endpointMethodMetadata.SegmentName);
        var pathC = Path.Combine(pathB, "Generated");
        var fileName = $"{endpointMethodMetadata.ContractInterfaceHandlerTypeName.Substring(1)}Stub.cs";
        var file = new FileInfo(Path.Combine(pathC, fileName));

        var contentWriter = new ContentWriter(logger);
        contentWriter.Write(
            hostProjectOptions.PathForTestGenerate,
            file,
            ContentWriterArea.Test,
            sb.ToString());
    }
}