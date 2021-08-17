using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Atc.Data.Models;
using Atc.Rest.ApiGenerator.Models;
using Atc.Rest.ApiGenerator.SyntaxGenerators;
using Microsoft.OpenApi.Models;

// ReSharper disable ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
// ReSharper disable LoopCanBeConvertedToQuery
// ReSharper disable UseDeconstruction
// ReSharper disable SuggestBaseTypeForParameter
namespace Atc.Rest.ApiGenerator.Helpers.XunitTest
{
    public static class GenerateServerApiXunitTestEndpointTestHelper
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
            AppendConstructor(sb, endpointMethodMetadata);
            AppendTestMethod(sb, endpointMethodMetadata);

            if (endpointMethodMetadata.IsContractParameterRequestBodyUsedAsMultipartFormData())
            {
                AppendGetMultipartFormDataContentRequestMethod(sb, endpointMethodMetadata, endpointMethodMetadata.IsContractParameterRequestBodyUsedAsMultipartFormDataAndHasInlineSchemaFile());
            }
            else if (endpointMethodMetadata.IsContractParameterRequestBodyUsedAsMultipartOctetStreamData())
            {
                AppendGetSingleFormDataContentRequestMethod(sb, endpointMethodMetadata);
            }

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
            sb.AppendLine(4, "[Collection(\"Sequential-Endpoints\")]");
            sb.AppendLine(4, $"public class {endpointMethodMetadata.MethodName}Tests : WebApiControllerBaseTest");
            sb.AppendLine(4, "{");
        }

        private static void AppendConstructor(
            StringBuilder sb,
            EndpointMethodMetadata endpointMethodMetadata)
        {
            sb.AppendLine(8, $"public {endpointMethodMetadata.MethodName}Tests(WebApiStartupFactory fixture) : base(fixture) {{ }}");
        }

        private static void AppendTestMethod(StringBuilder sb, EndpointMethodMetadata endpointMethodMetadata)
        {
            foreach (var contractReturnTypeName in endpointMethodMetadata.ContractReturnTypeNames)
            {
                switch (contractReturnTypeName.StatusCode)
                {
                    case HttpStatusCode.OK:
                        AppendTest200Ok(sb, endpointMethodMetadata, contractReturnTypeName);
                        break;
                    case HttpStatusCode.Created:
                        AppendTest201Created(sb, endpointMethodMetadata, contractReturnTypeName);
                        break;
                    case HttpStatusCode.BadRequest:
                        AppendTest400BadRequestInPath(sb, endpointMethodMetadata, contractReturnTypeName);
                        AppendTest400BadRequestInHeader(sb, endpointMethodMetadata, contractReturnTypeName);
                        AppendTest400BadRequestInQuery(sb, endpointMethodMetadata, contractReturnTypeName);
                        AppendTest400BadRequestInBody(sb, endpointMethodMetadata, contractReturnTypeName);
                        break;
                }
            }
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
            var fileName = $"{endpointMethodMetadata.MethodName}Tests.cs";
            var file = new FileInfo(Path.Combine(pathC, fileName));
            return TextFileHelper.Save(file, sb.ToString());
        }

        private static List<string> GetUsingStatements(HostProjectOptions hostProjectOptions, EndpointMethodMetadata endpointMethodMetadata)
        {
            var systemList = new List<string>
            {
                "System.CodeDom.Compiler",
                "System.Net",
                "System.Threading.Tasks",
            };

            if (endpointMethodMetadata.IsContractParameterRequestBodyUsingSystemNamespace())
            {
                systemList.Add("System");
            }

            if (endpointMethodMetadata.IsContractParameterRequestBodyUsingStringBuilder())
            {
                systemList.Add("System.Text");
            }

            if (endpointMethodMetadata.IsContractReturnTypeUsingList() ||
                endpointMethodMetadata.IsContractParameterRequestBodyUsingSystemCollectionGenericNamespace())
            {
                systemList.Add("System.Collections.Generic");
            }

            var list = new List<string>
            {
                "FluentAssertions",
                "Xunit",
            };

            if (endpointMethodMetadata.IsContractReturnTypeUsingPagination())
            {
                list.Add("Atc.Rest.Results");
            }

            if (endpointMethodMetadata.HasSharedModelOrEnumInContractParameterRequestBody() ||
                endpointMethodMetadata.HasSharedModelInContractReturnType(false))
            {
                list.Add($"{hostProjectOptions.ProjectName}.Generated.Contracts");
            }

            if (endpointMethodMetadata.IsContractParameterRequestBodyUsed() ||
                endpointMethodMetadata.HasContractReturnTypeAsComplexAndNotSharedModel())
            {
                systemList.Add("System.Net.Http");

                if (!endpointMethodMetadata.IsContractParameterRequestBodyUsedAsMultipartOctetStreamData())
                {
                    list.Add($"{hostProjectOptions.ProjectName}.Generated.Contracts.{endpointMethodMetadata.SegmentName}");
                }
            }
            else if (endpointMethodMetadata.HasContractReturnTypeAsComplexAsListOrPagination())
            {
                systemList.Add("System.Net.Http");
            }

            if (endpointMethodMetadata.IsContractParameterRequestBodyUsedAsMultipartFormData() ||
                endpointMethodMetadata.IsContractParameterRequestBodyUsedAsMultipartOctetStreamData())
            {
                list.Add("Microsoft.AspNetCore.Http");
            }

            return systemList
                .OrderBy(x => x)
                .Concat(list
                    .OrderBy(x => x))
                .ToList();
        }

        private static void AppendTest200Ok(StringBuilder sb, EndpointMethodMetadata endpointMethodMetadata, ResponseTypeNameAndItemSchema contractReturnTypeName)
        {
            var renderRelativeRefs = RenderRelativeRefsForQuery(endpointMethodMetadata);
            if (renderRelativeRefs.Count == 0)
            {
                return;
            }

            sb.AppendLine();
            sb.AppendLine(8, "[Theory]");
            foreach (var renderRelativeRef in renderRelativeRefs)
            {
                sb.AppendLine(8, $"[InlineData(\"{renderRelativeRef}\")]");
            }

            sb.AppendLine(8, $"public async {OpenApiDocumentSchemaModelNameHelper.EnsureTaskNameWithNamespaceIfNeeded(contractReturnTypeName.FullModelName)} {endpointMethodMetadata.MethodName}_Ok(string relativeRef)");
            AppendTextContent(sb, endpointMethodMetadata, HttpStatusCode.OK, contractReturnTypeName);
        }

        private static void AppendTest201Created(StringBuilder sb, EndpointMethodMetadata endpointMethodMetadata, ResponseTypeNameAndItemSchema contractReturnTypeName)
        {
            var renderRelativeRefs = RenderRelativeRefsForQuery(endpointMethodMetadata);
            if (renderRelativeRefs.Count == 0)
            {
                return;
            }

            sb.AppendLine();
            sb.AppendLine(8, "[Theory]");
            foreach (var renderRelativeRef in renderRelativeRefs)
            {
                sb.AppendLine(8, $"[InlineData(\"{renderRelativeRef}\")]");
            }

            sb.AppendLine(8, $"public async {OpenApiDocumentSchemaModelNameHelper.EnsureTaskNameWithNamespaceIfNeeded(contractReturnTypeName.FullModelName)} {endpointMethodMetadata.MethodName}_Created(string relativeRef)");
            AppendTextContent(sb, endpointMethodMetadata, HttpStatusCode.Created, contractReturnTypeName);
        }

        private static void AppendTest400BadRequestInPath(StringBuilder sb, EndpointMethodMetadata endpointMethodMetadata, ResponseTypeNameAndItemSchema contractReturnTypeName)
        {
            var renderRelativeRefs = RenderRelativeRefsForBadRequestInPath(endpointMethodMetadata, true);
            if (renderRelativeRefs.Count == 0)
            {
                return;
            }

            sb.AppendLine();
            sb.AppendLine(8, "[Theory]");
            foreach (var renderRelativeRef in renderRelativeRefs)
            {
                sb.AppendLine(8, $"[InlineData(\"{renderRelativeRef}\")]");
            }

            sb.AppendLine(8, $"public async {OpenApiDocumentSchemaModelNameHelper.EnsureTaskNameWithNamespaceIfNeeded(contractReturnTypeName.FullModelName)} {endpointMethodMetadata.MethodName}_BadRequest_InPath(string relativeRef)");
            AppendTextContent(sb, endpointMethodMetadata, HttpStatusCode.BadRequest, contractReturnTypeName);
        }

        private static void AppendTest400BadRequestInHeader(StringBuilder sb, EndpointMethodMetadata endpointMethodMetadata, ResponseTypeNameAndItemSchema contractReturnTypeName)
        {
            var headerRequiredParameters = endpointMethodMetadata.GetHeaderRequiredParameters();
            var testForParameters = headerRequiredParameters
                .Where(x => x.Schema.GetDataType() != OpenApiDataTypeConstants.String)
                .ToList();
            if (headerRequiredParameters.Count == 0)
            {
                return;
            }

            var relativeRef = RenderRelativeRef(endpointMethodMetadata);
            foreach (var testForParameter in testForParameters)
            {
                sb.AppendLine();
                sb.AppendLine(8, "[Theory]");
                sb.AppendLine(8, $"[InlineData(\"{relativeRef}\")]");
                sb.AppendLine(8, $"public async {OpenApiDocumentSchemaModelNameHelper.EnsureTaskNameWithNamespaceIfNeeded(contractReturnTypeName.FullModelName)} {endpointMethodMetadata.MethodName}_BadRequest_InHeader_{testForParameter.Name.EnsureFirstCharacterToUpper()}(string relativeRef)");
                sb.AppendLine(8, "{");
                sb.AppendLine(12, "// Arrange");
                if (headerRequiredParameters.Count > 0)
                {
                    foreach (var headerParameter in headerRequiredParameters)
                    {
                        var useInvalidData = headerParameter.Name == testForParameter.Name;
                        string propertyValueGenerated = PropertyValueGenerator(headerParameter, endpointMethodMetadata.ComponentsSchemas, useInvalidData, null);
                        sb.AppendLine(
                            12,
                            $"HttpClient.DefaultRequestHeaders.Add(\"{headerParameter.Name}\", \"{propertyValueGenerated}\");");
                    }

                    sb.AppendLine();
                }

                AppendNewRequestModel(12, sb, endpointMethodMetadata, contractReturnTypeName.StatusCode);
                sb.AppendLine();
                AppendActHttpClientOperation(12, sb, endpointMethodMetadata.HttpOperation, true);
                sb.AppendLine();
                sb.AppendLine(12, "// Assert");
                sb.AppendLine(12, "response.Should().NotBeNull();");
                sb.AppendLine(12, $"response.StatusCode.Should().Be(HttpStatusCode.{HttpStatusCode.BadRequest});");
                sb.AppendLine(8, "}");
            }
        }

        private static void AppendTest400BadRequestInQuery(StringBuilder sb, EndpointMethodMetadata endpointMethodMetadata, ResponseTypeNameAndItemSchema contractReturnTypeName)
        {
            var renderRelativeRefs = RenderRelativeRefsForQuery(endpointMethodMetadata, true);
            if (renderRelativeRefs.Count == 0)
            {
                return;
            }

            sb.AppendLine();
            sb.AppendLine(8, "[Theory]");
            foreach (var renderRelativeRef in renderRelativeRefs)
            {
                sb.AppendLine(8, $"[InlineData(\"{renderRelativeRef}\")]");
            }

            sb.AppendLine(8, $"public async {OpenApiDocumentSchemaModelNameHelper.EnsureTaskNameWithNamespaceIfNeeded(contractReturnTypeName.FullModelName)} {endpointMethodMetadata.MethodName}_BadRequest_InQuery(string relativeRef)");
            AppendTextContent(sb, endpointMethodMetadata, HttpStatusCode.BadRequest, contractReturnTypeName);
        }

        private static void AppendTest400BadRequestInBody(StringBuilder sb, EndpointMethodMetadata endpointMethodMetadata, ResponseTypeNameAndItemSchema contractReturnTypeName)
        {
            if (!endpointMethodMetadata.HasContractParameterRequestBody())
            {
                return;
            }

            if (endpointMethodMetadata.IsContractParameterRequestBodyUsedAsMultipartFormData())
            {
                return;
            }

            var schema = endpointMethodMetadata.ContractParameter?.ApiOperation.RequestBody?.Content.GetSchemaByFirstMediaType();
            if (schema == null)
            {
                return;
            }

            var modelName = schema.GetModelName();
            if (string.IsNullOrEmpty(modelName))
            {
                return;
            }

            var modelSchema = endpointMethodMetadata.ComponentsSchemas.GetSchemaByModelName(modelName);
            var relevantSchemas = endpointMethodMetadata.GetRelevantSchemasForBadRequestBodyParameters(modelSchema);

            var headerRequiredParameters = endpointMethodMetadata.GetHeaderRequiredParameters();

            var relativeRef = RenderRelativeRef(endpointMethodMetadata);
            foreach (var testForSchema in relevantSchemas)
            {
                sb.AppendLine();
                sb.AppendLine(8, "[Theory]");
                sb.AppendLine(8, $"[InlineData(\"{relativeRef}\")]");
                sb.AppendLine(8, $"public async {OpenApiDocumentSchemaModelNameHelper.EnsureTaskNameWithNamespaceIfNeeded(contractReturnTypeName.FullModelName)} {endpointMethodMetadata.MethodName}_BadRequest_InBody_{testForSchema.Key.EnsureFirstCharacterToUpper()}(string relativeRef)");
                sb.AppendLine(8, "{");
                sb.AppendLine(12, "// Arrange");
                if (headerRequiredParameters.Count > 0)
                {
                    foreach (var headerParameter in headerRequiredParameters)
                    {
                        string propertyValueGenerated = PropertyValueGenerator(headerParameter, endpointMethodMetadata.ComponentsSchemas, false, null);
                        sb.AppendLine(
                            12,
                            $"HttpClient.DefaultRequestHeaders.Add(\"{headerParameter.Name}\", \"{propertyValueGenerated}\");");
                    }

                    sb.AppendLine();
                }

                AppendNewRequestModelForBadRequest(12, sb, endpointMethodMetadata, contractReturnTypeName.StatusCode, testForSchema);
                sb.AppendLine();
                AppendActHttpClientOperation(12, sb, endpointMethodMetadata.HttpOperation, true, true);
                sb.AppendLine();
                sb.AppendLine(12, "// Assert");
                sb.AppendLine(12, "response.Should().NotBeNull();");
                sb.AppendLine(12, $"response.StatusCode.Should().Be(HttpStatusCode.{HttpStatusCode.BadRequest});");
                sb.AppendLine(8, "}");
            }
        }

        private static void AppendTextContent(
            StringBuilder sb,
            EndpointMethodMetadata endpointMethodMetadata,
            HttpStatusCode testExpectedHttpStatusCode,
            ResponseTypeNameAndItemSchema contractReturnTypeName)
        {
            sb.AppendLine(8, "{");
            if (endpointMethodMetadata.HasContractParameterAnyParametersOrRequestBody())
            {
                sb.AppendLine(12, "// Arrange");
                var headerParameters = endpointMethodMetadata.GetHeaderParameters();
                if (headerParameters.Count > 0)
                {
                    foreach (var headerParameter in headerParameters)
                    {
                        string propertyValueGenerated = PropertyValueGenerator(headerParameter, endpointMethodMetadata.ComponentsSchemas, false, null);
                        sb.AppendLine(
                            12,
                            $"HttpClient.DefaultRequestHeaders.Add(\"{headerParameter.Name}\", \"{propertyValueGenerated}\");");
                    }

                    sb.AppendLine();
                }

                var isContractParameterRequestBodyUsedAsMultipartOctetStreamData = endpointMethodMetadata.IsContractParameterRequestBodyUsedAsMultipartOctetStreamData();
                if (isContractParameterRequestBodyUsedAsMultipartOctetStreamData)
                {
                    sb.AppendLine(12, "var data = GetTestFile();");
                }
                else
                {
                    var isModelCreated = AppendNewRequestModel(12, sb, endpointMethodMetadata, contractReturnTypeName.StatusCode);
                    if (!isModelCreated && endpointMethodMetadata.HttpOperation.IsRequestBodySupported())
                    {
                        sb.AppendLine(12, "var data = \"{ }\";");
                    }
                }

                sb.AppendLine();

                if (endpointMethodMetadata.IsContractParameterRequestBodyUsedAsMultipartFormData())
                {
                    AppendActHttpClientOperationForMultipartFormData(
                        12,
                        sb,
                        endpointMethodMetadata.HttpOperation,
                        endpointMethodMetadata.GetRequestBodyModelName()!);
                }
                else if (isContractParameterRequestBodyUsedAsMultipartOctetStreamData)
                {
                    AppendActHttpClientOperationForMultipartFormData(
                        12,
                        sb,
                        endpointMethodMetadata.HttpOperation,
                        $"{endpointMethodMetadata.MethodName}{NameConstants.Request}");
                }
                else
                {
                    AppendActHttpClientOperation(12, sb, endpointMethodMetadata.HttpOperation, true);
                }
            }
            else
            {
                AppendActHttpClientOperation(12, sb, endpointMethodMetadata.HttpOperation);
            }

            sb.AppendLine();
            sb.AppendLine(12, "// Assert");
            sb.AppendLine(12, "response.Should().NotBeNull();");
            sb.AppendLine(12, $"response.StatusCode.Should().Be(HttpStatusCode.{testExpectedHttpStatusCode});");

            if (testExpectedHttpStatusCode == HttpStatusCode.OK &&
                !string.IsNullOrEmpty(contractReturnTypeName.FullModelName) &&
                contractReturnTypeName.Schema != null &&
                !contractReturnTypeName.Schema.IsSimpleDataType() &&
                !(endpointMethodMetadata.IsContractParameterRequestBodyUsedAsMultipartOctetStreamData() || endpointMethodMetadata.IsContractParameterRequestBodyUsedAsMultipartFormData()))
            {
                var modelName = OpenApiDocumentSchemaModelNameHelper.EnsureModelNameWithNamespaceIfNeeded(endpointMethodMetadata, contractReturnTypeName.FullModelName);

                sb.AppendLine();
                sb.AppendLine(12, $"var responseData = await response.DeserializeAsync<{modelName}>(JsonSerializerOptions);");
                sb.AppendLine(12, "responseData.Should().NotBeNull();");
            }

            sb.AppendLine(8, "}");
        }

        private static void AppendActHttpClientOperation(int indentSpaces, StringBuilder sb, OperationType operationType, bool useData = false, bool isDataJson = false)
        {
            sb.AppendLine(indentSpaces, "// Act");
            switch (operationType)
            {
                case OperationType.Get:
                case OperationType.Delete:
                    sb.AppendLine(12, $"var response = await HttpClient.{operationType}Async(relativeRef);");
                    break;
                case OperationType.Post:
                case OperationType.Put:
                case OperationType.Patch:
                    if (isDataJson)
                    {
                        sb.AppendLine(
                            indentSpaces,
                            useData
                                ? $"var response = await HttpClient.{operationType}Async(relativeRef, Json(data));"
                                : $"var response = await HttpClient.{operationType}Async(relativeRef, Json(\"{{}}\"));");
                    }
                    else
                    {
                        sb.AppendLine(
                            indentSpaces,
                            useData
                                ? $"var response = await HttpClient.{operationType}Async(relativeRef, ToJson(data));"
                                : $"var response = await HttpClient.{operationType}Async(relativeRef, ToJson(new {{}}));");
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(operationType), operationType, null);
            }
        }

        private static void AppendActHttpClientOperationForMultipartFormData(
            int indentSpaces,
            StringBuilder sb,
            OperationType operationType,
            string modelName)
        {
            sb.AppendLine(indentSpaces, "// Act");
            switch (operationType)
            {
                case OperationType.Post:
                    sb.AppendLine(
                        indentSpaces,
                        string.IsNullOrEmpty(modelName)
                            ? $"var response = await HttpClient.{operationType}Async(relativeRef, await GetMultipartFormDataContentFromFiles(data));"
                            : $"var response = await HttpClient.{operationType}Async(relativeRef, await GetMultipartFormDataContentFrom{modelName}(data));");

                    break;
                case OperationType.Get:
                case OperationType.Delete:
                case OperationType.Put:
                case OperationType.Patch:
                    throw new NotSupportedException("Append-MultipartFormData");
                default:
                    throw new ArgumentOutOfRangeException(nameof(operationType), operationType, message: null);
            }
        }

        private static void AppendGetMultipartFormDataContentRequestMethod(
            StringBuilder sb,
            EndpointMethodMetadata endpointMethodMetadata,
            bool useIFormFileDirectly)
        {
            var modelSchema = endpointMethodMetadata.GetRequestBodySchema()!;
            var modelName = modelSchema.GetModelName();

            sb.AppendLine();

            sb.AppendLine(
                8,
                string.IsNullOrEmpty(modelName)
                    ? "private async Task<MultipartFormDataContent> GetMultipartFormDataContentFromFiles(List<IFormFile> request)"
                    : $"private async Task<MultipartFormDataContent> GetMultipartFormDataContentFrom{modelName}({modelName} request)");

            sb.AppendLine(8, "{");
            sb.AppendLine(12, "var formDataContent = new MultipartFormDataContent();");

            if (OpenApiDataTypeConstants.Array.Equals(modelSchema.Type, StringComparison.OrdinalIgnoreCase))
            {
                sb.AppendLine(12, "if (request is not null)");
                sb.AppendLine(12, "{");
                sb.AppendLine(16, "foreach (var item in request)");
                sb.AppendLine(16, "{");
                sb.AppendLine(20, "var bytesContent = new ByteArrayContent(await item.GetBytes());");
                sb.AppendLine(20, "formDataContent.Add(bytesContent, \"Request\", item.FileName);");
                sb.AppendLine(16, "}");
                sb.AppendLine(12, "}");
                sb.AppendLine();
            }
            else
            {
                foreach (var schemaProperty in modelSchema.Properties)
                {
                    var propertyName = schemaProperty.Key.EnsureFirstCharacterToUpper();
                    if (schemaProperty.Value.IsFormatTypeOfBinary())
                    {
                        sb.AppendLine(12, $"if (request.{propertyName} is not null)");
                        sb.AppendLine(12, "{");
                        sb.AppendLine(16, $"var bytesContent = new ByteArrayContent(await request.{propertyName}.GetBytes());");

                        sb.AppendLine(
                            16,
                            useIFormFileDirectly
                                ? $"formDataContent.Add(bytesContent, \"Request.{propertyName}\", request.FileName);"
                                : $"formDataContent.Add(bytesContent, \"Request.{propertyName}\", request.{propertyName}.FileName);");

                        sb.AppendLine(12, "}");
                        sb.AppendLine();
                    }
                    else if (schemaProperty.Value.IsItemsOfFormatTypeBinary())
                    {
                        sb.AppendLine(12, $"if (request.{propertyName} is not null)");
                        sb.AppendLine(12, "{");
                        sb.AppendLine(16, $"foreach (var item in request.{propertyName})");
                        sb.AppendLine(16, "{");
                        sb.AppendLine(20, "var bytesContent = new ByteArrayContent(await item.GetBytes());");
                        sb.AppendLine(20, $"formDataContent.Add(bytesContent, \"Request.{propertyName}\", item.FileName);");
                        sb.AppendLine(16, "}");
                        sb.AppendLine(12, "}");
                        sb.AppendLine();
                    }
                    else if (schemaProperty.Value.IsDataTypeOfList())
                    {
                        sb.AppendLine(12, $"if (request.{propertyName} is not null && request.{propertyName}.Count > 0)");
                        sb.AppendLine(12, "{");
                        sb.AppendLine(16, $"foreach (var item in request.{propertyName})");
                        sb.AppendLine(16, "{");
                        sb.AppendLine(20, $"formDataContent.Add(new StringContent(item), \"Request.{propertyName}\");");
                        sb.AppendLine(16, "}");
                        sb.AppendLine(12, "}");
                        sb.AppendLine();
                    }
                    else
                    {
                        sb.AppendLine(12, $"formDataContent.Add(new StringContent(request.{propertyName}), \"Request.{propertyName}\");");
                    }
                }
            }

            sb.AppendLine(12, "return formDataContent;");
            sb.AppendLine(8, "}");
        }

        private static void AppendGetSingleFormDataContentRequestMethod(StringBuilder sb, EndpointMethodMetadata endpointMethodMetadata)
        {
            var modelName = $"{endpointMethodMetadata.MethodName}{NameConstants.Request}";

            sb.AppendLine();
            sb.AppendLine(8, $"private async Task<MultipartFormDataContent> GetMultipartFormDataContentFrom{modelName}(IFormFile request)");
            sb.AppendLine(8, "{");
            sb.AppendLine(12, "var formDataContent = new MultipartFormDataContent();");

            sb.AppendLine(12, "if (request is not null)");
            sb.AppendLine(12, "{");
            sb.AppendLine(16, "var bytesContent = new ByteArrayContent(await request.GetBytes());");
            sb.AppendLine(16, "formDataContent.Add(bytesContent, \"Request\", request.FileName);");
            sb.AppendLine(12, "}");
            sb.AppendLine();

            sb.AppendLine(12, "return formDataContent;");
            sb.AppendLine(8, "}");
        }

        private static bool AppendNewRequestModel(
            int indentSpaces,
            StringBuilder sb,
            EndpointMethodMetadata endpointMethodMetadata,
            HttpStatusCode httpStatusCode)
        {
            var schema = endpointMethodMetadata.ContractParameter?.ApiOperation.RequestBody?.Content.GetSchemaByFirstMediaType();
            if (schema == null)
            {
                return false;
            }

            GenerateXunitTestHelper.AppendVarDataModelOrListOfModel(
                indentSpaces,
                sb,
                endpointMethodMetadata,
                schema,
                httpStatusCode,
                SchemaMapLocatedAreaType.RequestBody);

            return true;
        }

        private static void AppendNewRequestModelForBadRequest(
            int indentSpaces,
            StringBuilder sb,
            EndpointMethodMetadata endpointMethodMetadata,
            HttpStatusCode httpStatusCode,
            KeyValuePair<string, OpenApiSchema> badPropertySchema)
        {
            var schema = endpointMethodMetadata.ContractParameter?.ApiOperation.RequestBody?.Content.GetSchemaByFirstMediaType();
            if (schema == null)
            {
                return;
            }

            GenerateXunitTestHelper.AppendVarDataModelOrListOfModel(
                indentSpaces,
                sb,
                endpointMethodMetadata,
                schema,
                httpStatusCode,
                SchemaMapLocatedAreaType.RequestBody,
                badPropertySchema,
                asJsonBody: true);
        }

        private static List<string> RenderRelativeRefsForBadRequestInPath(EndpointMethodMetadata endpointMethodMetadata, bool useForBadRequest = false)
        {
            var renderRelativeRefs = new List<string>();

            var allRouteParameters = endpointMethodMetadata.GetRouteParameters();
            var badRequestRouteParameters = FindBadRequestRouteParameters(allRouteParameters);

            if (badRequestRouteParameters.Count <= 0)
            {
                return renderRelativeRefs;
            }

            var combinationOfRouteParameters = ParameterCombinationHelper.GetCombination(badRequestRouteParameters, useForBadRequest);
            foreach (var parameters in combinationOfRouteParameters)
            {
                renderRelativeRefs.Add(RenderRelativeRefsForPathHelper(endpointMethodMetadata, allRouteParameters, parameters, useForBadRequest));
            }

            return renderRelativeRefs;
        }

        private static List<OpenApiParameter> FindBadRequestRouteParameters(List<OpenApiParameter> parameters)
        {
            return parameters
                .Where(x => x.Schema.GetDataType() != OpenApiDataTypeConstants.String)
                .ToList();
        }

        private static string RenderRelativeRefsForPathHelper(EndpointMethodMetadata endpointMethodMetadata, List<OpenApiParameter> allRouteParameters, List<OpenApiParameter> badRouteParameters, bool useForBadRequest)
        {
            var route = endpointMethodMetadata.Route;
            if (endpointMethodMetadata.ContractParameter == null)
            {
                return route;
            }

            string relativeRefPath = RenderRelativeRefPath(route, allRouteParameters, badRouteParameters, endpointMethodMetadata.ComponentsSchemas, useForBadRequest);

            if (allRouteParameters.Count == 0)
            {
                return relativeRefPath;
            }

            var queryRequiredParameters = endpointMethodMetadata.GetQueryRequiredParameters();
            if (queryRequiredParameters.Count == 0)
            {
                return relativeRefPath;
            }

            string relativeRefQuery = RenderRelativeRefQuery(queryRequiredParameters, endpointMethodMetadata.ComponentsSchemas, false);
            return $"{relativeRefPath}{relativeRefQuery}";
        }

        private static string RenderRelativeRef(EndpointMethodMetadata endpointMethodMetadata)
        {
            var queryParameters = endpointMethodMetadata.GetQueryParameters();
            return RenderRelativeRefsForQueryHelper(endpointMethodMetadata, queryParameters, false);
        }

        private static List<string> RenderRelativeRefsForQuery(EndpointMethodMetadata endpointMethodMetadata, bool useForBadRequest = false)
        {
            var renderRelativeRefs = new List<string>();

            var queryRequiredParameters = endpointMethodMetadata.GetQueryRequiredParameters();
            if (queryRequiredParameters.Count == 0)
            {
                if (!useForBadRequest)
                {
                    // Create without queryParameters
                    renderRelativeRefs.Add(RenderRelativeRefsForQueryHelper(endpointMethodMetadata, null, useForBadRequest));
                }
            }
            else
            {
                var queryParameters = endpointMethodMetadata.GetQueryParameters();
                var combinationOfQueryParameters = ParameterCombinationHelper.GetCombination(queryParameters, useForBadRequest);
                foreach (var parameters in combinationOfQueryParameters)
                {
                    renderRelativeRefs.Add(RenderRelativeRefsForQueryHelper(endpointMethodMetadata, parameters, useForBadRequest));
                }
            }

            return renderRelativeRefs;
        }

        private static string RenderRelativeRefsForQueryHelper(EndpointMethodMetadata endpointMethodMetadata, List<OpenApiParameter>? queryParameters, bool useForBadRequest)
        {
            var route = endpointMethodMetadata.Route;
            if (endpointMethodMetadata.ContractParameter == null)
            {
                return route;
            }

            var routeParameters = endpointMethodMetadata.GetRouteParameters();
            string relativeRefPath = RenderRelativeRefPath(route, routeParameters, routeParameters, endpointMethodMetadata.ComponentsSchemas, false);

            if (queryParameters == null || queryParameters.Count == 0)
            {
                return relativeRefPath;
            }

            string relativeRefQuery = RenderRelativeRefQuery(queryParameters, endpointMethodMetadata.ComponentsSchemas, useForBadRequest);
            return $"{relativeRefPath}{relativeRefQuery}";
        }

        private static string RenderRelativeRefPath(string route, List<OpenApiParameter> allRouteParameters, List<OpenApiParameter> badRouteParameters, IDictionary<string, OpenApiSchema> componentsSchemas, bool useForBadRequest)
        {
            var sa = route.Split('/');
            for (var i = 0; i < sa.Length; i++)
            {
                if (!sa[i].Contains("{", StringComparison.Ordinal))
                {
                    continue;
                }

                var pn = sa[i]
                    .Replace("{", string.Empty, StringComparison.Ordinal)
                    .Replace("}", string.Empty, StringComparison.Ordinal);

                var fromRoute = badRouteParameters.Find(x => x.Name.Equals(pn, StringComparison.OrdinalIgnoreCase));
                if (fromRoute == null)
                {
                    fromRoute = allRouteParameters.Find(x => x.Name.Equals(pn, StringComparison.OrdinalIgnoreCase));
                    sa[i] = PropertyValueGenerator(fromRoute, componentsSchemas, false, null);
                }
                else
                {
                    sa[i] = PropertyValueGenerator(fromRoute, componentsSchemas, useForBadRequest, null);
                }
            }

            return string.Join('/', sa);
        }

        private static string RenderRelativeRefQuery(List<OpenApiParameter> queryParameters, IDictionary<string, OpenApiSchema> componentsSchemas, bool useForBadRequest)
        {
            var sb = new StringBuilder();
            sb.Append('?');
            foreach (var queryParameter in queryParameters)
            {
                var val = PropertyValueGenerator(queryParameter, componentsSchemas, useForBadRequest, null);
                if ("null".Equals(val, StringComparison.Ordinal))
                {
                    val = string.Empty;
                }

                sb.Append($"&{queryParameter.Name}={val}");
            }

            return sb.ToString().Replace("?&", "?", StringComparison.Ordinal);
        }

        private static string PropertyValueGenerator(OpenApiParameter parameter, IDictionary<string, OpenApiSchema> componentsSchemas, bool useForBadRequest, string? customValue)
        {
            // Match on OpenApiSchemaExtensions->GetDataType
            return parameter.Schema.GetDataType() switch
            {
                "double" => ValueTypeTestPropertiesHelper.Number(parameter.Name, parameter.Schema, useForBadRequest),
                "long" => ValueTypeTestPropertiesHelper.Number(parameter.Name, parameter.Schema, useForBadRequest),
                "int" => ValueTypeTestPropertiesHelper.Number(parameter.Name, parameter.Schema, useForBadRequest),
                "bool" => ValueTypeTestPropertiesHelper.CreateValueBool(useForBadRequest),
                "string" => ValueTypeTestPropertiesHelper.CreateValueString(parameter.Name, parameter.Schema, parameter.In, useForBadRequest, 0, customValue),
                "DateTimeOffset" => ValueTypeTestPropertiesHelper.CreateValueDateTimeOffset(useForBadRequest),
                "Guid" => ValueTypeTestPropertiesHelper.CreateValueGuid(useForBadRequest),
                "Uri" => ValueTypeTestPropertiesHelper.CreateValueUri(useForBadRequest),
                "Email" => ValueTypeTestPropertiesHelper.CreateValueEmail(useForBadRequest),
                "Array" when parameter.In == ParameterLocation.Query => ValueTypeTestPropertiesHelper.CreateValueArray(parameter.Name, parameter.Schema.Items, parameter.In, useForBadRequest, 3),
                _ => PropertyValueGeneratorTypeResolver(parameter, componentsSchemas, useForBadRequest)
            };
        }

        private static string PropertyValueGeneratorTypeResolver(OpenApiParameter parameter, IDictionary<string, OpenApiSchema> componentsSchemas, bool useForBadRequest)
        {
            var name = parameter.Name.EnsureFirstCharacterToUpper();
            var schemaForDataType = componentsSchemas.FirstOrDefault(x => x.Key.Equals(parameter.Schema.GetDataType(), StringComparison.OrdinalIgnoreCase));
            if (schemaForDataType.Key != null && schemaForDataType.Value.IsSchemaEnumOrPropertyEnum())
            {
                return ValueTypeTestPropertiesHelper.CreateValueEnum(name, schemaForDataType, useForBadRequest);
            }

            throw new NotSupportedException($"PropertyValueGenerator: {parameter.Name} - {parameter.Schema.GetDataType()}");
        }
    }
}