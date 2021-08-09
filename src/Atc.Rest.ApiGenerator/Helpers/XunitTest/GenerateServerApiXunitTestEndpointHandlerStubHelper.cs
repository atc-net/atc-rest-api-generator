using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Atc.Data.Models;
using Atc.Rest.ApiGenerator.Models;
using Microsoft.OpenApi.Models;

// ReSharper disable UseDeconstruction
// ReSharper disable SuggestBaseTypeForParameter
namespace Atc.Rest.ApiGenerator.Helpers.XunitTest
{
    public static class GenerateServerApiXunitTestEndpointHandlerStubHelper
    {
        public static LogKeyValueItem Generate(
            HostProjectOptions hostProjectOptions,
            EndpointMethodMetadata endpointMethodMetadata)
        {
            if (hostProjectOptions == null)
            {
                throw new ArgumentNullException(nameof(hostProjectOptions));
            }

            if (endpointMethodMetadata == null)
            {
                throw new ArgumentNullException(nameof(endpointMethodMetadata));
            }

            var sb = new StringBuilder();
            AppendUsingStatements(sb, hostProjectOptions, endpointMethodMetadata);
            sb.AppendLine();
            GenerateCodeHelper.AppendGeneratedCodeWarningComment(sb, hostProjectOptions.ToolNameAndVersion);
            AppendNamespaceAndClassStart(sb, hostProjectOptions, endpointMethodMetadata);
            AppendMethodExecuteAsyncStart(sb, endpointMethodMetadata);
            AppendMethodExecuteAsyncContent(sb, endpointMethodMetadata);
            AppendMethodExecuteAsyncEnd(sb);
            AppendNamespaceAndClassEnd(sb);
            return SaveFile(sb, hostProjectOptions, endpointMethodMetadata);
        }

        private static void AppendUsingStatements(
            StringBuilder sb,
            HostProjectOptions hostProjectOptions,
            EndpointMethodMetadata endpointMethodMetadata)
        {
            foreach (var statement in GetUsingStatements(hostProjectOptions, endpointMethodMetadata))
            {
                sb.AppendLine($"using {statement};");
            }
        }

        private static void AppendNamespaceAndClassStart(
            StringBuilder sb,
            HostProjectOptions hostProjectOptions,
            EndpointMethodMetadata endpointMethodMetadata)
        {
            sb.AppendLine($"namespace {hostProjectOptions.ProjectName}.Tests.Endpoints.{endpointMethodMetadata.SegmentName}.Generated");
            sb.AppendLine("{");

            GenerateCodeHelper.AppendGeneratedCodeAttribute(sb, hostProjectOptions.ToolName, hostProjectOptions.ToolVersion);
            sb.AppendLine(4, $"public class {endpointMethodMetadata.MethodName}HandlerStub : {endpointMethodMetadata.ContractInterfaceHandlerTypeName}");
            sb.AppendLine(4, "{");
        }

        private static void AppendMethodExecuteAsyncStart(
            StringBuilder sb,
            EndpointMethodMetadata endpointMethodMetadata)
        {
            sb.AppendLine(8, endpointMethodMetadata.ContractParameterTypeName == null
                ? $"public Task<{endpointMethodMetadata.ContractResultTypeName}> ExecuteAsync(CancellationToken cancellationToken = default)"
                : $"public Task<{endpointMethodMetadata.ContractResultTypeName}> ExecuteAsync({endpointMethodMetadata.ContractParameterTypeName} parameters, CancellationToken cancellationToken = default)");
            sb.AppendLine(8, "{");
        }

        private static void AppendMethodExecuteAsyncContent(
            StringBuilder sb,
            EndpointMethodMetadata endpointMethodMetadata)
        {
            if (endpointMethodMetadata.ContractReturnTypeNames.FirstOrDefault(x => x.StatusCode == HttpStatusCode.OK) != null)
            {
                AppendContentForExecuteAsynchronous(sb, endpointMethodMetadata, HttpStatusCode.OK);
            }
            else if (endpointMethodMetadata.ContractReturnTypeNames.FirstOrDefault(x => x.StatusCode == HttpStatusCode.Created) != null)
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
                            ? $"return {OpenApiDocumentSchemaModelNameHelper.EnsureTaskNameWithNamespaceIfNeeded(contractReturnTypeName.FullModelName)}.FromResult({endpointMethodMetadata.ContractResultTypeName}.{httpStatusCode.ToNormalizedString()}());"
                            : $"return {OpenApiDocumentSchemaModelNameHelper.EnsureTaskNameWithNamespaceIfNeeded(contractReturnTypeName.FullModelName)}.FromResult({endpointMethodMetadata.ContractResultTypeName}.{httpStatusCode.ToNormalizedString()}(\"Hallo world\"));");
                    break;
                case "bool":
                    sb.AppendLine(
                        12,
                        httpStatusCode == HttpStatusCode.Created
                            ? $"return {OpenApiDocumentSchemaModelNameHelper.EnsureTaskNameWithNamespaceIfNeeded(contractReturnTypeName.FullModelName)}.FromResult({endpointMethodMetadata.ContractResultTypeName}.{httpStatusCode.ToNormalizedString()}());"
                            : $"return {OpenApiDocumentSchemaModelNameHelper.EnsureTaskNameWithNamespaceIfNeeded(contractReturnTypeName.FullModelName)}.FromResult({endpointMethodMetadata.ContractResultTypeName}.{httpStatusCode.ToNormalizedString()}(true));");
                    break;
                case "int":
                case "long":
                    sb.AppendLine(
                        12,
                        httpStatusCode == HttpStatusCode.Created
                            ? $"return {OpenApiDocumentSchemaModelNameHelper.EnsureTaskNameWithNamespaceIfNeeded(contractReturnTypeName.FullModelName)}.FromResult({endpointMethodMetadata.ContractResultTypeName}.{httpStatusCode.ToNormalizedString()}());"
                            : $"return {OpenApiDocumentSchemaModelNameHelper.EnsureTaskNameWithNamespaceIfNeeded(contractReturnTypeName.FullModelName)}.FromResult({endpointMethodMetadata.ContractResultTypeName}.{httpStatusCode.ToNormalizedString()}(42));");
                    break;
                case "float":
                case "double":
                    sb.AppendLine(
                        12,
                        httpStatusCode == HttpStatusCode.Created
                            ? $"return {OpenApiDocumentSchemaModelNameHelper.EnsureTaskNameWithNamespaceIfNeeded(contractReturnTypeName.FullModelName)}.FromResult({endpointMethodMetadata.ContractResultTypeName}.{httpStatusCode.ToNormalizedString()}());"
                            : $"return {OpenApiDocumentSchemaModelNameHelper.EnsureTaskNameWithNamespaceIfNeeded(contractReturnTypeName.FullModelName)}.FromResult({endpointMethodMetadata.ContractResultTypeName}.{httpStatusCode.ToNormalizedString()}(42.2));");
                    break;
                default:
                {
                    var singleReturnTypeName = OpenApiDocumentSchemaModelNameHelper.GetRawModelName(returnTypeName);
                    var modelSchema = endpointMethodMetadata.ComponentsSchemas.GetSchemaByModelName(singleReturnTypeName);

                    GenerateXunitTestHelper.AppendVarDataModelOrListOfModel(
                        12,
                        sb,
                        endpointMethodMetadata,
                        modelSchema,
                        httpStatusCode,
                        SchemaMapLocatedAreaType.Response);
                    sb.AppendLine();

                    if (contractReturnTypeName.Schema == null ||
                        GenerateXunitTestPartsHelper.IsListKind(returnTypeName))
                    {
                        if (returnTypeName.StartsWith(Microsoft.OpenApi.Models.NameConstants.Pagination, StringComparison.Ordinal))
                        {
                            if (endpointMethodMetadata.ContractParameter != null)
                            {
                                var queryParameters = endpointMethodMetadata.ContractParameter.ApiOperation.Parameters.GetAllFromQuery();
                                var sPageSize = "10";
                                if (queryParameters.FirstOrDefault(x => x.Name.Equals("PageSize", StringComparison.OrdinalIgnoreCase)) != null)
                                {
                                    sPageSize = "parameters.PageSize";
                                }

                                var sQueryString = "null";
                                if (queryParameters.FirstOrDefault(x => x.Name.Equals("QueryString", StringComparison.OrdinalIgnoreCase)) != null)
                                {
                                    sQueryString = "parameters.QueryString";
                                }

                                var sContinuationToken = "null";
                                if (queryParameters.FirstOrDefault(x => x.Name.Equals("ContinuationToken", StringComparison.OrdinalIgnoreCase)) != null)
                                {
                                    sContinuationToken = "parameters.ContinuationToken";
                                }

                                sb.AppendLine(12, $"var paginationData = new {contractReturnTypeName.FullModelName}(data, {sPageSize}, {sQueryString}, {sContinuationToken});");
                            }
                            else
                            {
                                sb.AppendLine(12, $"var paginationData = new {contractReturnTypeName.FullModelName}(data, 10, null, null);");
                            }

                            sb.AppendLine(12, $"return {OpenApiDocumentSchemaModelNameHelper.EnsureTaskNameWithNamespaceIfNeeded(contractReturnTypeName.FullModelName)}.FromResult({endpointMethodMetadata.ContractResultTypeName}.{httpStatusCode.ToNormalizedString()}(paginationData));");
                        }
                        else
                        {
                            sb.AppendLine(12, $"return {OpenApiDocumentSchemaModelNameHelper.EnsureTaskNameWithNamespaceIfNeeded(contractReturnTypeName.FullModelName)}.FromResult({endpointMethodMetadata.ContractResultTypeName}.{httpStatusCode.ToNormalizedString()}(data));");
                        }
                    }
                    else
                    {
                        sb.AppendLine(12, $"return {OpenApiDocumentSchemaModelNameHelper.EnsureTaskNameWithNamespaceIfNeeded(contractReturnTypeName.FullModelName)}.FromResult({endpointMethodMetadata.ContractResultTypeName}.{httpStatusCode.ToNormalizedString()}(data));");
                    }

                    break;
                }
            }
        }

        private static void AppendMethodExecuteAsyncEnd(StringBuilder sb)
        {
            sb.AppendLine(8, "}");
        }

        private static void AppendNamespaceAndClassEnd(StringBuilder sb)
        {
            sb.AppendLine(4, "}");
            sb.AppendLine("}");
        }

        private static LogKeyValueItem SaveFile(
            StringBuilder sb,
            HostProjectOptions hostProjectOptions,
            EndpointMethodMetadata endpointMethodMetadata)
        {
            var pathA = Path.Combine(hostProjectOptions.PathForTestGenerate!.FullName, "Endpoints");
            var pathB = Path.Combine(pathA, endpointMethodMetadata.SegmentName);
            var pathC = Path.Combine(pathB, "Generated");
            var fileName = $"{endpointMethodMetadata.ContractInterfaceHandlerTypeName.Substring(1)}Stub.cs";
            var file = new FileInfo(Path.Combine(pathC, fileName));
            return TextFileHelper.Save(file, sb.ToString());
        }

        private static List<string> GetUsingStatements(HostProjectOptions hostProjectOptions, EndpointMethodMetadata endpointMethodMetadata)
        {
            var systemList = new List<string>
            {
                "System.CodeDom.Compiler",
                "System.Threading",
                "System.Threading.Tasks",
            };

            if (endpointMethodMetadata.IsContractReturnTypeUsingSystemNamespace())
            {
                systemList.Add("System");
            }

            if (endpointMethodMetadata.IsContractReturnTypeUsingPaginationOrListUsed())
            {
                systemList.Add("System.Collections.Generic");
            }

            var list = new List<string>();

            if (endpointMethodMetadata.IsContractReturnTypeUsingPagination())
            {
                list.Add("Atc.Rest.Results");
            }

            if (endpointMethodMetadata.HasSharedModelInContractReturnType())
            {
                list.Add($"{hostProjectOptions.ProjectName}.Generated.Contracts");
            }

            list.Add($"{hostProjectOptions.ProjectName}.Generated.Contracts.{endpointMethodMetadata.SegmentName}");

            return systemList
                .OrderBy(x => x)
                .Concat(list
                    .OrderBy(x => x))
                .ToList();
        }
    }
}