using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Atc.Rest.ApiGenerator.Models;
using Microsoft.OpenApi.Models;

// ReSharper disable SwitchStatementMissingSomeEnumCasesNoDefault
namespace Atc.Rest.ApiGenerator.Helpers.XunitTest
{
    public static class GenerateXunitTestHelper
    {
        public static void AppendNewModelOrListOfModel(
            int indentSpaces,
            StringBuilder sb,
            EndpointMethodMetadata endpointMethodMetadata,
            OpenApiSchema schema,
            HttpStatusCode httpStatusCode,
            SchemaMapLocatedAreaType locatedArea,
            string variableName = "data")
        {
            if (sb == null)
            {
                throw new ArgumentNullException(nameof(sb));
            }

            if (endpointMethodMetadata == null)
            {
                throw new ArgumentNullException(nameof(endpointMethodMetadata));
            }

            if (schema == null)
            {
                throw new ArgumentNullException(nameof(schema));
            }

            switch (locatedArea)
            {
                case SchemaMapLocatedAreaType.Response:
                {
                    var contractReturnTypeName = endpointMethodMetadata.ContractReturnTypeNames.First(x => x.Item1 == httpStatusCode);
                    if (!string.IsNullOrEmpty(contractReturnTypeName.Item2) && (
                        contractReturnTypeName.Item2.StartsWith(Microsoft.OpenApi.Models.NameConstants.Pagination, StringComparison.Ordinal) ||
                        contractReturnTypeName.Item2.StartsWith(Microsoft.OpenApi.Models.NameConstants.List, StringComparison.Ordinal)))
                    {
                        if (contractReturnTypeName.Item2.StartsWith(Microsoft.OpenApi.Models.NameConstants.Pagination, StringComparison.Ordinal))
                        {
                            var listDataType = contractReturnTypeName.Item2.Replace(Microsoft.OpenApi.Models.NameConstants.Pagination, Microsoft.OpenApi.Models.NameConstants.List, StringComparison.Ordinal);
                            sb.AppendLine(indentSpaces, $"var {variableName} = new {listDataType}");
                        }
                        else
                        {
                            sb.AppendLine(indentSpaces, $"var {variableName} = new {contractReturnTypeName.Item2}");
                        }

                        sb.AppendLine(indentSpaces, "{");
                        for (var i = 0; i < 3; i++)
                        {
                            AppendNewModel(indentSpaces + 4, sb, endpointMethodMetadata, schema, null, i + 1, null);
                        }

                        sb.AppendLine(indentSpaces, "};");
                    }
                    else
                    {
                        AppendNewModel(indentSpaces, sb, endpointMethodMetadata, schema, null, 0, variableName);
                    }

                    break;
                }

                case SchemaMapLocatedAreaType.RequestBody when schema.Type == OpenApiDataTypeConstants.Array:
                {
                    var modelSchemaName = schema.GetModelName();
                    var modelSchema = endpointMethodMetadata.ComponentsSchemas.GetSchemaByModelName(modelSchemaName);
                    var modelName = OpenApiDocumentSchemaModelNameHelper.EnsureModelNameWithNamespaceIfNeeded(endpointMethodMetadata, schema.GetModelName());
                    sb.AppendLine(indentSpaces, $"var {variableName} = new List<{modelName}>");

                    sb.AppendLine(indentSpaces, "{");
                    for (var i = 0; i < 3; i++)
                    {
                        AppendNewModel(indentSpaces + 4, sb, endpointMethodMetadata, modelSchema, null, i + 1, null);
                    }

                    sb.AppendLine(indentSpaces, "};");
                    break;
                }

                case SchemaMapLocatedAreaType.RequestBody:
                    AppendNewModel(indentSpaces, sb, endpointMethodMetadata, schema, null, 0, variableName);
                    break;
            }
        }

        public static void AppendNewModelOrListOfModelForBadRequest(
            int indentSpaces,
            StringBuilder sb,
            EndpointMethodMetadata endpointMethodMetadata,
            OpenApiSchema schema,
            HttpStatusCode httpStatusCode,
            KeyValuePair<string, OpenApiSchema> badPropertySchema,
            string variableName = "data")
        {
            if (sb == null)
            {
                throw new ArgumentNullException(nameof(sb));
            }

            if (endpointMethodMetadata == null)
            {
                throw new ArgumentNullException(nameof(endpointMethodMetadata));
            }

            if (schema == null)
            {
                throw new ArgumentNullException(nameof(schema));
            }

            var contractReturnTypeName = endpointMethodMetadata.ContractReturnTypeNames.First(x => x.Item1 == httpStatusCode);
            if (!string.IsNullOrEmpty(contractReturnTypeName.Item2) && (
                contractReturnTypeName.Item2.StartsWith(Microsoft.OpenApi.Models.NameConstants.Pagination, StringComparison.Ordinal) ||
                contractReturnTypeName.Item2.StartsWith(Microsoft.OpenApi.Models.NameConstants.List, StringComparison.Ordinal)))
            {
                // TO-DO: Imp this.
            }
            else
            {
                AppendNewModelAsJson(indentSpaces, sb, endpointMethodMetadata, schema, badPropertySchema.Key, 0, variableName);
            }
        }

        private static void AppendNewModel(
            int indentSpaces,
            StringBuilder sb,
            EndpointMethodMetadata endpointMethodMetadata,
            OpenApiSchema schema,
            string? badRequestPropertyName,
            int itemNumber,
            string? variableName)
        {
            if (sb == null)
            {
                throw new ArgumentNullException(nameof(sb));
            }

            if (schema == null)
            {
                throw new ArgumentNullException(nameof(schema));
            }

            var countString = 0;
            if (itemNumber != -1 || variableName != null)
            {
                var modelName = OpenApiDocumentSchemaModelNameHelper.EnsureModelNameWithNamespaceIfNeeded(endpointMethodMetadata, schema.GetModelName());

                sb.AppendLine(
                    indentSpaces,
                    variableName == null
                        ? $"new {modelName}"
                        : $"var {variableName} = new {modelName}");
            }

            sb.AppendLine(indentSpaces, "{");

            foreach (var schemaProperty in schema.Properties)
            {
                var useForBadRequest = !string.IsNullOrEmpty(badRequestPropertyName) &&
                                       schemaProperty.Key.Equals(badRequestPropertyName, StringComparison.Ordinal);
                string dataType = schemaProperty.Value.GetDataType();
                string propertyValueGenerated = PropertyValueGenerator(schemaProperty, endpointMethodMetadata.ComponentsSchemas, useForBadRequest, itemNumber, null);

                if ("NEW-INSTANCE".Equals(propertyValueGenerated, StringComparison.Ordinal))
                {
                    var schemaForDataType = endpointMethodMetadata.ComponentsSchemas.FirstOrDefault(x => x.Key.Equals(dataType, StringComparison.OrdinalIgnoreCase));
                    sb.AppendLine(
                        indentSpaces + 4,
                        $"{schemaProperty.Key.EnsureFirstCharacterToUpper()} = new {schemaForDataType.Value.GetModelName()}");
                    AppendNewModel(indentSpaces + 4, sb, endpointMethodMetadata, schemaForDataType.Value, badRequestPropertyName, -1, null);
                }
                else
                {
                    switch (dataType)
                    {
                        case "string":
                            if (!schemaProperty.Value.IsFormatTypeOfEmail() &&
                                !schemaProperty.Value.IsRuleValidationString())
                            {
                                if (countString > 0)
                                {
                                    propertyValueGenerated = $"{propertyValueGenerated}{countString}";
                                }

                                countString++;
                            }

                            sb.AppendLine(
                                indentSpaces + 4,
                                $"{schemaProperty.Key.EnsureFirstCharacterToUpper()} = \"{propertyValueGenerated}\",");
                            break;
                        case "DateTimeOffset":
                            sb.AppendLine(
                                indentSpaces + 4,
                                $"{schemaProperty.Key.EnsureFirstCharacterToUpper()} = DateTimeOffset.Parse(\"{propertyValueGenerated}\"),");
                            break;
                        case "Guid":
                            sb.AppendLine(
                                indentSpaces + 4,
                                $"{schemaProperty.Key.EnsureFirstCharacterToUpper()} = Guid.Parse(\"{propertyValueGenerated}\"),");
                            break;
                        case "Uri":
                            sb.AppendLine(
                                indentSpaces + 4,
                                $"{schemaProperty.Key.EnsureFirstCharacterToUpper()} = new Uri(\"{propertyValueGenerated}\"),");
                            break;
                        default:
                            var enumDataType = GetDataTypeIfEnum(schemaProperty, endpointMethodMetadata.ComponentsSchemas);
                            if (enumDataType == null)
                            {
                                sb.AppendLine(
                                    indentSpaces + 4,
                                    $"{schemaProperty.Key.EnsureFirstCharacterToUpper()} = {propertyValueGenerated},");
                            }
                            else
                            {
                                if (propertyValueGenerated.Contains("=", StringComparison.Ordinal))
                                {
                                    propertyValueGenerated = propertyValueGenerated.Split('=').First().Trim();
                                }

                                sb.AppendLine(
                                    indentSpaces + 4,
                                    $"{schemaProperty.Key.EnsureFirstCharacterToUpper()} = {enumDataType}.{propertyValueGenerated},");
                            }

                            break;
                    }
                }
            }

            sb.AppendLine(
                indentSpaces,
                variableName == null
                    ? "},"
                    : "};");
        }

        private static void AppendNewModelAsJson(int indentSpaces, StringBuilder sb, EndpointMethodMetadata endpointMethodMetadata, OpenApiSchema schema, string? badRequestPropertyName, int itemNumber, string? variableName, int jsonIndentLevel = 0)
        {
            if (sb == null)
            {
                throw new ArgumentNullException(nameof(sb));
            }

            if (endpointMethodMetadata == null)
            {
                throw new ArgumentNullException(nameof(endpointMethodMetadata));
            }

            if (schema == null)
            {
                throw new ArgumentNullException(nameof(schema));
            }

            var countString = 0;
            var jsonSpaces = string.Empty.PadLeft(jsonIndentLevel * 2);
            if (jsonIndentLevel == 0)
            {
                sb.AppendLine(indentSpaces, "var sb = new StringBuilder();");
                sb.AppendLine(indentSpaces, WrapInAppendLine("{"));
            }

            foreach (var schemaProperty in schema.Properties)
            {
                var trailingChar = ",";
                if (schema.Properties.Last().Key == schemaProperty.Key)
                {
                    trailingChar = string.Empty;
                }

                var useForBadRequest = !string.IsNullOrEmpty(badRequestPropertyName) &&
                                       schemaProperty.Key.Equals(badRequestPropertyName, StringComparison.Ordinal);
                string dataType = schemaProperty.Value.GetDataType();
                string propertyValueGenerated =
                    WrapInQuotes(
                        PropertyValueGenerator(schemaProperty, endpointMethodMetadata.ComponentsSchemas, useForBadRequest, itemNumber, null),
                        schemaProperty);
                if ("NEW-INSTANCE".Equals(propertyValueGenerated, StringComparison.Ordinal))
                {
                    var schemaForDataType = endpointMethodMetadata.ComponentsSchemas.FirstOrDefault(x => x.Key.Equals(dataType, StringComparison.OrdinalIgnoreCase));
                    sb.AppendLine(
                        indentSpaces,
                        WrapInAppendLine($"{jsonSpaces}  {WrapInQuotes(schemaProperty.Key.EnsureFirstCharacterToUpper())}: {{"));
                    AppendNewModelAsJson(indentSpaces, sb, endpointMethodMetadata, schemaForDataType.Value, badRequestPropertyName, -1, null, jsonIndentLevel + 1);
                }
                else
                {
                    if (dataType == "string" && !schemaProperty.Value.IsFormatTypeOfEmail())
                    {
                        if (countString > 0 && !propertyValueGenerated.Equals("null", StringComparison.Ordinal))
                        {
                            propertyValueGenerated = $"{propertyValueGenerated}{countString}";
                        }

                        countString++;
                    }

                    sb.AppendLine(
                               indentSpaces,
                               WrapInAppendLine($"{jsonSpaces}  {WrapInQuotes(schemaProperty.Key.EnsureFirstCharacterToUpper())}: {propertyValueGenerated}{trailingChar}"));
                }
            }

            if (jsonIndentLevel == 0)
            {
                sb.AppendLine(indentSpaces, WrapInAppendLine($"}}"));
                sb.AppendLine(indentSpaces, $"var {variableName} = sb.ToString();");
            }
            else
            {
                sb.AppendLine(indentSpaces, WrapInAppendLine($"{jsonSpaces}}}"));
            }
        }

        private static string? GetDataTypeIfEnum(KeyValuePair<string, OpenApiSchema> schema, IDictionary<string, OpenApiSchema> componentsSchemas)
        {
            var schemaForDataType = componentsSchemas.FirstOrDefault(x => x.Key.Equals(schema.Value.GetDataType(), StringComparison.OrdinalIgnoreCase));
            return schemaForDataType.Key != null && schemaForDataType.Value.IsSchemaEnumOrPropertyEnum()
                ? schemaForDataType.Key
                : null;
        }

        private static string PropertyValueGenerator(KeyValuePair<string, OpenApiSchema> schema, IDictionary<string, OpenApiSchema> componentsSchemas, bool useForBadRequest, int itemNumber, string? customValue)
        {
            var name = schema.Key.EnsureFirstCharacterToUpper();

            // Match on OpenApiSchemaExtensions->GetDataType
            return schema.Value.GetDataType() switch
            {
                "double" => ValueTypeTestPropertiesHelper.Number(name, schema.Value, useForBadRequest),
                "long" => ValueTypeTestPropertiesHelper.Number(name, schema.Value, useForBadRequest),
                "int" => ValueTypeTestPropertiesHelper.Number(name, schema.Value, useForBadRequest),
                "bool" => ValueTypeTestPropertiesHelper.CreateValueBool(useForBadRequest),
                "string" => ValueTypeTestPropertiesHelper.CreateValueString(name, schema.Value, null, useForBadRequest, itemNumber, customValue),
                "DateTimeOffset" => ValueTypeTestPropertiesHelper.CreateValueDateTimeOffset(useForBadRequest),
                "Guid" => ValueTypeTestPropertiesHelper.CreateValueGuid(useForBadRequest, itemNumber),
                "Uri" => ValueTypeTestPropertiesHelper.CreateValueUri(useForBadRequest),
                "Email" => ValueTypeTestPropertiesHelper.CreateValueEmail(useForBadRequest),
                _ => PropertyValueGeneratorTypeResolver(schema, componentsSchemas, useForBadRequest)
            };
        }

        private static string PropertyValueGeneratorTypeResolver(KeyValuePair<string, OpenApiSchema> schema, IDictionary<string, OpenApiSchema> componentsSchemas, bool useForBadRequest)
        {
            var name = schema.Key.EnsureFirstCharacterToUpper();
            var schemaForDataType = componentsSchemas.FirstOrDefault(x => x.Key.Equals(schema.Value.GetDataType(), StringComparison.OrdinalIgnoreCase));
            if (schemaForDataType.Key != null)
            {
                if (schemaForDataType.Value.IsSchemaEnumOrPropertyEnum())
                {
                    return ValueTypeTestPropertiesHelper.CreateValueEnum(name, schemaForDataType, useForBadRequest);
                }

                return useForBadRequest
                    ? "null"
                    : "NEW-INSTANCE";
            }

            if (schema.Value.GetDataType() == "array")
            {
                // TO-DO: Imp. this.
                return "null";
            }

            return "null";
        }

        private static string WrapInQuotes(string typeTestValue, KeyValuePair<string, OpenApiSchema> schema)
        {
            var dataType = schema.Value.GetDataType();
            return (typeTestValue, dataType) switch
            {
                (_, "double") => typeTestValue,
                (_, "long") => typeTestValue,
                (_, "int") => typeTestValue,
                ("null", _) => typeTestValue,
                ("NEW-INSTANCE", _) => typeTestValue,
                _ => WrapInQuotes(typeTestValue)
            };
        }

        private static string WrapInQuotes(string str) => $"\\\"{str}\\\"";

        private static string WrapInAppendLine(string str) => $"sb.AppendLine(\"{str}\");";
    }
}