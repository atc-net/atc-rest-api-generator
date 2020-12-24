using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Atc.Data.Models;
using Atc.Rest.ApiGenerator.Models;
using Microsoft.OpenApi.Models;

// ReSharper disable SwitchStatementHandlesSomeKnownEnumValuesWithDefault
// ReSharper disable UseDeconstruction
// ReSharper disable SwitchStatementMissingSomeEnumCasesNoDefault
namespace Atc.Rest.ApiGenerator.Helpers.XunitTest
{
    public static class GenerateServerApiXunitTestEndpointTestHelper
    {
        public static LogKeyValueItem Generate(HostProjectOptions hostProjectOptions, EndpointMethodMetadata endpointMethodMetadata)
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
            foreach (var statement in GetUsingStatements(hostProjectOptions, endpointMethodMetadata))
            {
                sb.AppendLine($"using {statement};");
            }

            sb.AppendLine();
            GenerateCodeHelper.AppendNamespaceComment(sb, hostProjectOptions.ToolNameAndVersion);
            sb.AppendLine($"namespace {hostProjectOptions.ProjectName}.Tests.Endpoints.{endpointMethodMetadata.SegmentName}.Generated");
            sb.AppendLine("{");
            GenerateCodeHelper.AppendGeneratedCodeAttribute(sb, hostProjectOptions.ToolName, hostProjectOptions.ToolVersion);
            sb.AppendLine(4, "[Collection(\"Sequential-Endpoints\")]");
            sb.AppendLine(4, $"public class {endpointMethodMetadata.MethodName}Tests : WebApiControllerBaseTest");
            sb.AppendLine(4, "{");
            sb.AppendLine(8, $"public {endpointMethodMetadata.MethodName}Tests(WebApiStartupFactory fixture) : base(fixture) {{ }}");
            foreach (var contractReturnTypeName in endpointMethodMetadata.ContractReturnTypeNames)
            {
                switch (contractReturnTypeName.Item1)
                {
                    case HttpStatusCode.OK:
                        AppendTest200Ok(sb, endpointMethodMetadata, contractReturnTypeName!);
                        break;
                    case HttpStatusCode.Created:
                        AppendTest201Created(sb, endpointMethodMetadata, contractReturnTypeName!);
                        break;
                    case HttpStatusCode.BadRequest:
                        AppendTest400BadRequestInPath(sb, endpointMethodMetadata, contractReturnTypeName!);
                        AppendTest400BadRequestInHeader(sb, endpointMethodMetadata, contractReturnTypeName!);
                        AppendTest400BadRequestInQuery(sb, endpointMethodMetadata, contractReturnTypeName!);
                        AppendTest400BadRequestInBody(sb, endpointMethodMetadata, contractReturnTypeName!);
                        break;
                }
            }

            sb.AppendLine(4, "}");
            sb.AppendLine("}");

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
                "System",
                "System.CodeDom.Compiler",
                "System.Collections.Generic",
                "System.Net",
                "System.Net.Http",
                "System.Text",
                "System.Threading.Tasks",
            }.OrderBy(x => x);

            var generalList = new List<string>
            {
                "FluentAssertions",
                "Xunit",
                $"{hostProjectOptions.ProjectName}.Generated.Contracts",
                $"{hostProjectOptions.ProjectName}.Generated.Contracts.{endpointMethodMetadata.SegmentName}",
            };
            if (endpointMethodMetadata.IsPaginationUsed())
            {
                generalList.Add("Atc.Rest.Results");
            }

            return systemList
                .OrderBy(x => x)
                .Concat(generalList
                    .OrderBy(x => x))
                .ToList();
        }

        private static void AppendTest200Ok(StringBuilder sb, EndpointMethodMetadata endpointMethodMetadata, Tuple<HttpStatusCode, string, OpenApiSchema> contractReturnTypeName)
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

            sb.AppendLine(8, $"public async Task {endpointMethodMetadata.MethodName}_Ok(string relativeRef)");
            AppendTextContent(sb, endpointMethodMetadata, HttpStatusCode.OK, contractReturnTypeName);
        }

        private static void AppendTest201Created(StringBuilder sb, EndpointMethodMetadata endpointMethodMetadata, Tuple<HttpStatusCode, string, OpenApiSchema> contractReturnTypeName)
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

            sb.AppendLine(8, $"public async Task {endpointMethodMetadata.MethodName}_Created(string relativeRef)");
            AppendTextContent(sb, endpointMethodMetadata, HttpStatusCode.Created, contractReturnTypeName);
        }

        private static void AppendTest400BadRequestInPath(StringBuilder sb, EndpointMethodMetadata endpointMethodMetadata, Tuple<HttpStatusCode, string, OpenApiSchema> contractReturnTypeName)
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

            sb.AppendLine(8, $"public async Task {endpointMethodMetadata.MethodName}_BadRequest_InPath(string relativeRef)");
            AppendTextContent(sb, endpointMethodMetadata, HttpStatusCode.BadRequest, contractReturnTypeName);
        }

        private static void AppendTest400BadRequestInHeader(StringBuilder sb, EndpointMethodMetadata endpointMethodMetadata, Tuple<HttpStatusCode, string, OpenApiSchema> contractReturnTypeName)
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
                sb.AppendLine(8, $"public async Task {endpointMethodMetadata.MethodName}_BadRequest_InHeader_{testForParameter.Name.EnsureFirstCharacterToUpper()}(string relativeRef)");
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

                AppendNewRequestModel(12, sb, endpointMethodMetadata, contractReturnTypeName.Item1);
                sb.AppendLine();
                AppendActHttpClientOperation(12, sb, endpointMethodMetadata.HttpOperation, true);
                sb.AppendLine();
                sb.AppendLine(12, "// Assert");
                sb.AppendLine(12, "response.Should().NotBeNull();");
                sb.AppendLine(12, $"response.StatusCode.Should().Be(HttpStatusCode.{HttpStatusCode.BadRequest});");
                sb.AppendLine(8, "}");
            }
        }

        private static void AppendTest400BadRequestInQuery(StringBuilder sb, EndpointMethodMetadata endpointMethodMetadata, Tuple<HttpStatusCode, string, OpenApiSchema> contractReturnTypeName)
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

            sb.AppendLine(8, $"public async Task {endpointMethodMetadata.MethodName}_BadRequest_InQuery(string relativeRef)");
            AppendTextContent(sb, endpointMethodMetadata, HttpStatusCode.BadRequest, contractReturnTypeName);
        }

        private static void AppendTest400BadRequestInBody(StringBuilder sb, EndpointMethodMetadata endpointMethodMetadata, Tuple<HttpStatusCode, string, OpenApiSchema> contractReturnTypeName)
        {
            if (!endpointMethodMetadata.HasContractParameterRequestBody())
            {
                return;
            }

            var schema = endpointMethodMetadata.ContractParameter?.ApiOperation.RequestBody?.Content.GetSchema();
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

            var headerRequiredParameters = endpointMethodMetadata.GetHeaderRequiredParameters();

            var relevantSchemas = new List<KeyValuePair<string, OpenApiSchema>>();
            foreach (var schemaProperty in modelSchema.Properties)
            {
                if (modelSchema.Required.Contains(schemaProperty.Key) ||
                    schemaProperty.Value.IsFormatTypeOfEmail() ||
                    schemaProperty.Value.IsFormatTypeOfDate() ||
                    schemaProperty.Value.IsFormatTypeOfDateTime() ||
                    schemaProperty.Value.IsFormatTypeOfTime() ||
                    schemaProperty.Value.IsFormatTypeOfTimestamp())
                {
                    relevantSchemas.Add(schemaProperty);
                }
            }

            var relativeRef = RenderRelativeRef(endpointMethodMetadata);
            foreach (var testForSchema in relevantSchemas)
            {
                sb.AppendLine();
                sb.AppendLine(8, "[Theory]");
                sb.AppendLine(8, $"[InlineData(\"{relativeRef}\")]");
                sb.AppendLine(8, $"public async Task {endpointMethodMetadata.MethodName}_BadRequest_InBody_{testForSchema.Key.EnsureFirstCharacterToUpper()}(string relativeRef)");
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

                AppendNewRequestModelForBadRequest(12, sb, endpointMethodMetadata, contractReturnTypeName.Item1, testForSchema);
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
            Tuple<HttpStatusCode, string, OpenApiSchema> contractReturnTypeName)
        {
            sb.AppendLine(8, "{");
            if (endpointMethodMetadata.HasContractParameterRequestBody())
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

                AppendNewRequestModel(12, sb, endpointMethodMetadata, contractReturnTypeName.Item1);
                sb.AppendLine();
                AppendActHttpClientOperation(12, sb, endpointMethodMetadata.HttpOperation, true);
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
                !string.IsNullOrEmpty(contractReturnTypeName.Item2) &&
                contractReturnTypeName.Item3 != null && !contractReturnTypeName.Item3.IsSimpleDataType())
            {
                var modelName = OpenApiDocumentSchemaModelNameHelper.EnsureModelNameWithNamespaceIfNeeded(endpointMethodMetadata, contractReturnTypeName.Item2);

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

        private static void AppendNewRequestModel(
            int indentSpaces,
            StringBuilder sb,
            EndpointMethodMetadata endpointMethodMetadata,
            HttpStatusCode httpStatusCode,
            string variableName = "data")
        {
            var schema = endpointMethodMetadata.ContractParameter?.ApiOperation.RequestBody?.Content.GetSchema();
            if (schema == null)
            {
                return;
            }

            GenerateXunitTestHelper.AppendNewModelOrListOfModel(indentSpaces, sb, endpointMethodMetadata, schema, httpStatusCode, SchemaMapLocatedAreaType.RequestBody, variableName);
        }

        private static void AppendNewRequestModelForBadRequest(
            int indentSpaces,
            StringBuilder sb,
            EndpointMethodMetadata endpointMethodMetadata,
            HttpStatusCode httpStatusCode,
            KeyValuePair<string, OpenApiSchema> badPropertySchema,
            string variableName = "data")
        {
            var schema = endpointMethodMetadata.ContractParameter?.ApiOperation.RequestBody?.Content.GetSchema();
            if (schema == null)
            {
                return;
            }

            GenerateXunitTestHelper.AppendNewModelOrListOfModelForBadRequest(indentSpaces, sb, endpointMethodMetadata, schema, httpStatusCode, badPropertySchema, variableName);
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