// ReSharper disable UseDeconstruction
// ReSharper disable SuggestBaseTypeForParameter
namespace Atc.Rest.ApiGenerator.Helpers.XunitTest;

[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:Parameter should not span multiple lines", Justification = "OK.")]
public static class GenerateServerApiXunitTestEndpointHandlerStubHelper
{
    public static void Generate(
        ILogger logger,
        HostProjectOptions hostProjectOptions,
        EndpointMethodMetadata endpointMethodMetadata,
        string apiGroupName,
        ContentGeneratorServerControllerMethodParameters methodParameters)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(hostProjectOptions);
        ArgumentNullException.ThrowIfNull(endpointMethodMetadata);
        ArgumentNullException.ThrowIfNull(methodParameters);

        var sb = new StringBuilder();

        GenerateCodeHelper.AppendGeneratedCodeWarningComment(sb, hostProjectOptions.ApiGeneratorNameAndVersion);
        AppendNamespaceAndClassStart(sb, hostProjectOptions, apiGroupName, methodParameters);
        AppendMethodExecuteAsyncStart(sb, hostProjectOptions, apiGroupName, methodParameters);
        AppendMethodExecuteAsyncContent(sb, hostProjectOptions, endpointMethodMetadata, apiGroupName, methodParameters);
        AppendMethodExecuteAsyncEnd(sb);
        AppendNamespaceAndClassEnd(sb);
        SaveFile(logger, sb, hostProjectOptions, apiGroupName, methodParameters);
    }

    private static void AppendNamespaceAndClassStart(
        StringBuilder sb,
        HostProjectOptions hostProjectOptions,
        string apiGroupName,
        ContentGeneratorServerControllerMethodParameters methodParameters)
    {
        sb.AppendLine($"namespace {hostProjectOptions.ProjectName}.Tests.Endpoints.{apiGroupName}.Generated");
        sb.AppendLine("{");

        GenerateCodeHelper.AppendGeneratedCodeAttribute(sb, hostProjectOptions.ApiGeneratorName, hostProjectOptions.ApiGeneratorVersion);
        sb.AppendLine(
            4,
            "Tasks".Equals(apiGroupName, StringComparison.OrdinalIgnoreCase)
                ? $"public class {methodParameters.Name}HandlerStub : {hostProjectOptions.ProjectName}.Generated.Contracts.{apiGroupName}.{methodParameters.InterfaceName}"
                : $"public class {methodParameters.Name}HandlerStub : {methodParameters.InterfaceName}");

        sb.AppendLine(4, "{");
    }

    private static void AppendMethodExecuteAsyncStart(
        StringBuilder sb,
        HostProjectOptions hostProjectOptions,
        string apiGroupName,
        ContentGeneratorServerControllerMethodParameters methodParameters)
    {
        var contractResultTypeName = methodParameters.Name + "Result";

        if ("Tasks".Equals(apiGroupName, StringComparison.OrdinalIgnoreCase))
        {
            sb.AppendLine(8, methodParameters.ParameterTypeName is null
                ? $"public Task<{hostProjectOptions.ProjectName}.Generated.Contracts.{apiGroupName}.{contractResultTypeName}> ExecuteAsync(CancellationToken cancellationToken = default)"
                : $"public Task<{hostProjectOptions.ProjectName}.Generated.Contracts.{apiGroupName}.{contractResultTypeName}> ExecuteAsync({methodParameters.ParameterTypeName} parameters, CancellationToken cancellationToken = default)");
        }
        else
        {
            sb.AppendLine(8, methodParameters.ParameterTypeName is null
                ? $"public Task<{contractResultTypeName}> ExecuteAsync(CancellationToken cancellationToken = default)"
                : $"public Task<{contractResultTypeName}> ExecuteAsync({methodParameters.ParameterTypeName} parameters, CancellationToken cancellationToken = default)");
        }

        sb.AppendLine(8, "{");
    }

    private static void AppendMethodExecuteAsyncContent(
        StringBuilder sb,
        HostProjectOptions hostProjectOptions,
        EndpointMethodMetadata endpointMethodMetadata,
        string apiGroupName,
        ContentGeneratorServerControllerMethodParameters methodParameters)
    {
        if (endpointMethodMetadata.ContractReturnTypeNames.Find(x => x.StatusCode == HttpStatusCode.OK) is not null)
        {
            AppendContentForExecuteAsynchronous(sb, hostProjectOptions, endpointMethodMetadata, apiGroupName, methodParameters, HttpStatusCode.OK);
        }
        else if (endpointMethodMetadata.ContractReturnTypeNames.Find(x => x.StatusCode == HttpStatusCode.Created) is not null)
        {
            AppendContentForExecuteAsynchronous(sb, hostProjectOptions, endpointMethodMetadata, apiGroupName, methodParameters, HttpStatusCode.Created);
        }
        else
        {
            sb.AppendLine(12, "throw new System.NotImplementedException();");
        }
    }

    private static void AppendContentForExecuteAsynchronous(
        StringBuilder sb,
        HostProjectOptions hostProjectOptions,
        EndpointMethodMetadata endpointMethodMetadata,
        string apiGroupName,
        ContentGeneratorServerControllerMethodParameters methodParameters,
        HttpStatusCode httpStatusCode)
    {
        var contractReturnTypeName = endpointMethodMetadata.ContractReturnTypeNames.First(x => x.StatusCode == httpStatusCode);
        var returnTypeName = contractReturnTypeName.FullModelName;

        var contractResultTypeName = methodParameters.Name + "Result";
        if ("Tasks".Equals(apiGroupName, StringComparison.OrdinalIgnoreCase))
        {
            contractResultTypeName = $"{hostProjectOptions.ProjectName}.Generated.Contracts.{apiGroupName}.{contractResultTypeName}";
        }

        switch (returnTypeName)
        {
            case "string":
                sb.AppendLine(
                    12,
                    httpStatusCode == HttpStatusCode.Created
                        ? $"return {OpenApiDocumentSchemaModelNameHelper.EnsureTaskNameWithNamespaceIfNeeded(contractReturnTypeName)}.FromResult({contractResultTypeName}.{httpStatusCode.ToNormalizedString()}());"
                        : $"return {OpenApiDocumentSchemaModelNameHelper.EnsureTaskNameWithNamespaceIfNeeded(contractReturnTypeName)}.FromResult({contractResultTypeName}.{httpStatusCode.ToNormalizedString()}(\"Hallo world\"));");
                break;
            case "bool":
                sb.AppendLine(
                    12,
                    httpStatusCode == HttpStatusCode.Created
                        ? $"return {OpenApiDocumentSchemaModelNameHelper.EnsureTaskNameWithNamespaceIfNeeded(contractReturnTypeName)}.FromResult({contractResultTypeName}.{httpStatusCode.ToNormalizedString()}());"
                        : $"return {OpenApiDocumentSchemaModelNameHelper.EnsureTaskNameWithNamespaceIfNeeded(contractReturnTypeName)}.FromResult({contractResultTypeName}.{httpStatusCode.ToNormalizedString()}(true));");
                break;
            case "int":
            case "long":
                sb.AppendLine(
                    12,
                    httpStatusCode == HttpStatusCode.Created
                        ? $"return {OpenApiDocumentSchemaModelNameHelper.EnsureTaskNameWithNamespaceIfNeeded(contractReturnTypeName)}.FromResult({contractResultTypeName}.{httpStatusCode.ToNormalizedString()}());"
                        : $"return {OpenApiDocumentSchemaModelNameHelper.EnsureTaskNameWithNamespaceIfNeeded(contractReturnTypeName)}.FromResult({contractResultTypeName}.{httpStatusCode.ToNormalizedString()}(42));");
                break;
            case "float":
            case "double":
                sb.AppendLine(
                    12,
                    httpStatusCode == HttpStatusCode.Created
                        ? $"return {OpenApiDocumentSchemaModelNameHelper.EnsureTaskNameWithNamespaceIfNeeded(contractReturnTypeName)}.FromResult({contractResultTypeName}.{httpStatusCode.ToNormalizedString()}());"
                        : $"return {OpenApiDocumentSchemaModelNameHelper.EnsureTaskNameWithNamespaceIfNeeded(contractReturnTypeName)}.FromResult({contractResultTypeName}.{httpStatusCode.ToNormalizedString()}(42.2));");
                break;
            case "Guid":
                sb.AppendLine(
                    12,
                    httpStatusCode == HttpStatusCode.Created
                        ? $"return {OpenApiDocumentSchemaModelNameHelper.EnsureTaskNameWithNamespaceIfNeeded(contractReturnTypeName)}.FromResult({contractResultTypeName}.{httpStatusCode.ToNormalizedString()}());"
                        : $"return {OpenApiDocumentSchemaModelNameHelper.EnsureTaskNameWithNamespaceIfNeeded(contractReturnTypeName)}.FromResult({contractResultTypeName}.{httpStatusCode.ToNormalizedString()}(System.Guid.NewGuid()));");
                break;
            case "byte[]":
                sb.AppendLine(12, "var bytes = System.Text.Encoding.UTF8.GetBytes(\"Hello World\");");
                sb.AppendLine(12, $"return {OpenApiDocumentSchemaModelNameHelper.EnsureTaskNameWithNamespaceIfNeeded(contractReturnTypeName)}.FromResult({contractResultTypeName}.{httpStatusCode.ToNormalizedString()}(bytes, \"dummy.txt\"));");
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
                        if (endpointMethodMetadata.ApiOperation is not null)
                        {
                            var queryParameters = endpointMethodMetadata.ApiOperation.Parameters.GetAllFromQuery();
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

                        sb.AppendLine(12, $"return {OpenApiDocumentSchemaModelNameHelper.EnsureTaskNameWithNamespaceIfNeeded(contractReturnTypeName)}.FromResult({contractResultTypeName}.{httpStatusCode.ToNormalizedString()}(paginationData));");
                    }
                    else
                    {
                        sb.AppendLine(12, $"return {OpenApiDocumentSchemaModelNameHelper.EnsureTaskNameWithNamespaceIfNeeded(contractReturnTypeName)}.FromResult({contractResultTypeName}.{httpStatusCode.ToNormalizedString()}(data));");
                    }
                }
                else
                {
                    sb.AppendLine(12, $"return {OpenApiDocumentSchemaModelNameHelper.EnsureTaskNameWithNamespaceIfNeeded(contractReturnTypeName)}.FromResult({contractResultTypeName}.{httpStatusCode.ToNormalizedString()}(data));");
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
        string apiGroupName,
        ContentGeneratorServerControllerMethodParameters methodParameters)
    {
        var pathA = Path.Combine(hostProjectOptions.PathForTestGenerate!.FullName, "Endpoints");
        var pathB = Path.Combine(pathA, apiGroupName);
        var pathC = Path.Combine(pathB, "Generated");
        var fileName = $"{methodParameters.InterfaceName.Substring(1)}Stub.cs";
        var file = new FileInfo(Path.Combine(pathC, fileName));

        var contentWriter = new ContentWriter(logger);
        contentWriter.Write(
            hostProjectOptions.PathForTestGenerate,
            file,
            ContentWriterArea.Test,
            sb.ToString());
    }
}