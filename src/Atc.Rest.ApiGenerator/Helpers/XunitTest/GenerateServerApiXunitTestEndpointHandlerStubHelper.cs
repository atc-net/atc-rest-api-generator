using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Atc.Data.Models;
using Atc.Rest.ApiGenerator.Models;
using Microsoft.OpenApi.Models;

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
            sb.AppendLine("using System;");
            sb.AppendLine("using System.CodeDom.Compiler;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using System.Threading;");
            sb.AppendLine("using System.Threading.Tasks;");
            if (endpointMethodMetadata.IsPaginationUsed())
            {
                sb.AppendLine("using Atc.Rest.Results;");
            }

            sb.AppendLine($"using {hostProjectOptions.ProjectName}.Generated.Contracts;");
            sb.AppendLine($"using {hostProjectOptions.ProjectName}.Generated.Contracts.{endpointMethodMetadata.SegmentName};");
            sb.AppendLine();
            GenerateCodeHelper.AppendNamespaceComment(sb, hostProjectOptions.ToolNameAndVersion);
            sb.AppendLine($"namespace {hostProjectOptions.ProjectName}.Tests.Endpoints.{endpointMethodMetadata.SegmentName}.Generated");
            sb.AppendLine("{");
            GenerateCodeHelper.AppendGeneratedCodeAttribute(sb, hostProjectOptions.ToolName, hostProjectOptions.ToolVersion);
            sb.AppendLine(4, $"public class {endpointMethodMetadata.MethodName}HandlerStub : {endpointMethodMetadata.ContractInterfaceHandlerTypeName}");
            sb.AppendLine(4, "{");
            sb.AppendLine(8, endpointMethodMetadata.ContractParameterTypeName == null
                ? $"public Task<{endpointMethodMetadata.ContractResultTypeName}> ExecuteAsync(CancellationToken cancellationToken = default)"
                : $"public Task<{endpointMethodMetadata.ContractResultTypeName}> ExecuteAsync({endpointMethodMetadata.ContractParameterTypeName} parameters, CancellationToken cancellationToken = default)");
            sb.AppendLine(8, "{");

            if (endpointMethodMetadata.ContractReturnTypeNames.FirstOrDefault(x => x.Item1 == HttpStatusCode.OK) != null)
            {
                AppendContentForExecuteAsynchronous(sb, endpointMethodMetadata, HttpStatusCode.OK);
            }
            else if (endpointMethodMetadata.ContractReturnTypeNames.FirstOrDefault(x => x.Item1 == HttpStatusCode.Created) != null)
            {
                AppendContentForExecuteAsynchronous(sb, endpointMethodMetadata, HttpStatusCode.Created);
            }
            else
            {
                sb.AppendLine(12, "throw new System.NotImplementedException();");
            }

            sb.AppendLine(8, "}");
            sb.AppendLine(4, "}");
            sb.AppendLine("}");

            var pathA = Path.Combine(hostProjectOptions.PathForTestGenerate!.FullName, "Endpoints");
            var pathB = Path.Combine(pathA, endpointMethodMetadata.SegmentName);
            var pathC = Path.Combine(pathB, "Generated");
            var fileName = $"{endpointMethodMetadata.ContractInterfaceHandlerTypeName.Substring(1)}Stub.cs";
            var file = new FileInfo(Path.Combine(pathC, fileName));
            return TextFileHelper.Save(file, sb.ToString());
        }

        private static void AppendContentForExecuteAsynchronous(StringBuilder sb, EndpointMethodMetadata endpointMethodMetadata, HttpStatusCode httpStatusCode)
        {
            var contractReturnTypeName = endpointMethodMetadata.ContractReturnTypeNames.First(x => x.Item1 == httpStatusCode);
            var returnTypeName = contractReturnTypeName.Item2;

            if (returnTypeName == "string")
            {
                sb.AppendLine(
                    12,
                    httpStatusCode == HttpStatusCode.Created
                        ? $"return Task.FromResult({endpointMethodMetadata.ContractResultTypeName}.{httpStatusCode.ToNormalizedString()}());"
                        : $"return Task.FromResult({endpointMethodMetadata.ContractResultTypeName}.{httpStatusCode.ToNormalizedString()}(\"Hallo world\"));");
            }
            else if (returnTypeName == "bool")
            {
                sb.AppendLine(
                    12,
                    httpStatusCode == HttpStatusCode.Created
                        ? $"return Task.FromResult({endpointMethodMetadata.ContractResultTypeName}.{httpStatusCode.ToNormalizedString()}());"
                        : $"return Task.FromResult({endpointMethodMetadata.ContractResultTypeName}.{httpStatusCode.ToNormalizedString()}(true));");
            }
            else if (returnTypeName == "int" || returnTypeName == "long")
            {
                sb.AppendLine(
                    12,
                    httpStatusCode == HttpStatusCode.Created
                        ? $"return Task.FromResult({endpointMethodMetadata.ContractResultTypeName}.{httpStatusCode.ToNormalizedString()}());"
                        : $"return Task.FromResult({endpointMethodMetadata.ContractResultTypeName}.{httpStatusCode.ToNormalizedString()}(42));");
            }
            else if (returnTypeName == "float" || returnTypeName == "double")
            {
                sb.AppendLine(
                    12,
                    httpStatusCode == HttpStatusCode.Created
                        ? $"return Task.FromResult({endpointMethodMetadata.ContractResultTypeName}.{httpStatusCode.ToNormalizedString()}());"
                        : $"return Task.FromResult({endpointMethodMetadata.ContractResultTypeName}.{httpStatusCode.ToNormalizedString()}(42.2));");
            }
            else
            {
                if (contractReturnTypeName.Item3 == null ||
                    returnTypeName.StartsWith(Microsoft.OpenApi.Models.NameConstants.Pagination, StringComparison.Ordinal) ||
                    returnTypeName.StartsWith(Microsoft.OpenApi.Models.NameConstants.List, StringComparison.Ordinal))
                {
                    var singleReturnTypeName = OpenApiDocumentSchemaModelNameHelper.GetRawModelName(returnTypeName);
                    var modelSchema = endpointMethodMetadata.ComponentsSchemas.GetSchemaByModelName(singleReturnTypeName);
                    GenerateXunitTestHelper.AppendNewModelOrListOfModel(12, sb, endpointMethodMetadata, modelSchema, httpStatusCode, SchemaMapLocatedAreaType.Response);
                    sb.AppendLine();
                    if (contractReturnTypeName.Item2.StartsWith(Microsoft.OpenApi.Models.NameConstants.Pagination, StringComparison.Ordinal))
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

                            sb.AppendLine(12, $"var paginationData = new {contractReturnTypeName.Item2}(data, {sPageSize}, {sQueryString}, {sContinuationToken});");
                        }
                        else
                        {
                            sb.AppendLine(12, $"var paginationData = new {contractReturnTypeName.Item2}(data, 10, null, null);");
                        }

                        sb.AppendLine();
                        sb.AppendLine(12, $"return Task.FromResult({endpointMethodMetadata.ContractResultTypeName}.{httpStatusCode.ToNormalizedString()}(paginationData));");
                    }
                    else
                    {
                        sb.AppendLine(12, $"return Task.FromResult({endpointMethodMetadata.ContractResultTypeName}.{httpStatusCode.ToNormalizedString()}(data));");
                    }
                }
                else
                {
                    GenerateXunitTestHelper.AppendNewModelOrListOfModel(12, sb, endpointMethodMetadata, contractReturnTypeName.Item3, httpStatusCode, SchemaMapLocatedAreaType.Response);
                    sb.AppendLine();
                    sb.AppendLine(12, $"return Task.FromResult({endpointMethodMetadata.ContractResultTypeName}.{httpStatusCode.ToNormalizedString()}(data));");
                }
            }
        }
    }
}