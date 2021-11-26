using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atc.Rest.ApiGenerator.Models;
using Microsoft.OpenApi.Models;

namespace Atc.Rest.ApiGenerator.Helpers.XunitTest
{
    public static class GenerateXunitTestPartsHelper
    {
        public static int AppendModelSimpleProperty(
            int indentSpaces,
            StringBuilder sb,
            EndpointMethodMetadata endpointMethodMetadata,
            KeyValuePair<string, OpenApiSchema> schemaProperty,
            string dataType,
            bool isRequired,
            string propertyValueGenerated,
            int countString,
            bool asJsonBody,
            int depthHierarchy,
            TrailingCharType trailingChar)
        {
            var propertyName = schemaProperty.Key.EnsureFirstCharacterToUpper();

            var isHandled = false;
            if (isRequired && OpenApiDataTypeConstants.Array.Equals(dataType, StringComparison.OrdinalIgnoreCase))
            {
                var itemsDataType = schemaProperty.Value.Items.GetDataType();

                if (asJsonBody)
                {
                    switch (itemsDataType)
                    {
                        case OpenApiDataTypeConstants.String:
                            sb.AppendLine(indentSpaces, $"sb.AppendLine(\"  {WrapInQuotes(propertyName)}: [ {WrapInQuotes("Hallo")}, {WrapInQuotes("World")} ]{GenerateCodeHelper.GetTrailingChar(trailingChar)}\");");
                            isHandled = true;
                            break;
                        case OpenApiDataTypeConstants.Integer:
                            sb.AppendLine(indentSpaces, $"sb.AppendLine(\"  {WrapInQuotes(propertyName)}: [ 42, 17 ]{GenerateCodeHelper.GetTrailingChar(trailingChar)}\");");
                            isHandled = true;
                            break;
                        case OpenApiDataTypeConstants.Boolean:
                            sb.AppendLine(indentSpaces, $"sb.AppendLine(\"  {WrapInQuotes(propertyName)}: [ true, false, true ]{GenerateCodeHelper.GetTrailingChar(trailingChar)}\");");
                            isHandled = true;
                            break;
                        case "IFormFile":
                            throw new NotSupportedException("IFormFile is not supported when working with Json.");
                    }
                }
                else
                {
                    switch (itemsDataType)
                    {
                        case OpenApiDataTypeConstants.String:
                            sb.AppendLine(indentSpaces + 4, $"{propertyName} = new List<string>() {{ \"Hallo\", \"World\" }},");
                            isHandled = true;
                            break;
                        case OpenApiDataTypeConstants.Integer:
                            sb.AppendLine(indentSpaces + 4, $"{propertyName} = new List<int>() {{ 42, 17 }},");
                            isHandled = true;
                            break;
                        case OpenApiDataTypeConstants.Boolean:
                            sb.AppendLine(indentSpaces + 4, $"{propertyName} = new List<bool>() {{ true, false, true }},");
                            isHandled = true;
                            break;
                        case "IFormFile":
                            sb.AppendLine(indentSpaces + 4, $"{propertyName} = GetTestFiles(),");
                            isHandled = true;
                            break;
                    }
                }
            }

            if (isHandled)
            {
                return countString;
            }

            switch (dataType)
            {
                case "string":
                    countString = AppendModelSimplePropertyForString(
                        indentSpaces,
                        sb,
                        propertyName,
                        schemaProperty,
                        propertyValueGenerated,
                        countString,
                        asJsonBody,
                        depthHierarchy,
                        trailingChar);
                    break;
                case "DateTimeOffset":
                    AppendModelSimplePropertyForDateTimeOffset(
                        indentSpaces,
                        sb,
                        propertyName,
                        propertyValueGenerated,
                        asJsonBody,
                        depthHierarchy,
                        trailingChar);
                    break;
                case "Guid":
                    AppendModelSimplePropertyForGuid(
                        indentSpaces,
                        sb,
                        propertyName,
                        propertyValueGenerated,
                        asJsonBody,
                        depthHierarchy,
                        trailingChar);
                    break;
                case "Uri":
                    AppendModelSimplePropertyForUri(
                        indentSpaces,
                        sb,
                        propertyName,
                        propertyValueGenerated,
                        asJsonBody,
                        depthHierarchy,
                        trailingChar);
                    break;
                case "IFormFile":
                    AppendModelSimplePropertyForIFormFile(
                        indentSpaces,
                        sb,
                        propertyName,
                        propertyValueGenerated,
                        asJsonBody,
                        depthHierarchy,
                        trailingChar);
                    break;
                default:
                    var enumDataType = GetDataTypeIfEnum(schemaProperty, endpointMethodMetadata.ComponentsSchemas);
                    if (enumDataType == null)
                    {
                        AppendModelSimplePropertyDefault(
                            indentSpaces,
                            sb,
                            propertyName,
                            propertyValueGenerated,
                            asJsonBody,
                            depthHierarchy,
                            trailingChar);
                    }
                    else
                    {
                        if (propertyValueGenerated.Contains('=', StringComparison.Ordinal))
                        {
                            propertyValueGenerated = propertyValueGenerated.Split('=').First().Trim();
                        }

                        if (asJsonBody)
                        {
                            sb.AppendLine(
                                indentSpaces,
                                WrapInStringBuilderAppendLineWithKeyAndValueQuotes(depthHierarchy, propertyName, propertyValueGenerated, trailingChar));
                        }
                        else
                        {
                            sb.AppendLine(
                                indentSpaces + 4,
                                $"{propertyName} = {enumDataType}.{propertyValueGenerated},");
                        }
                    }

                    break;
            }

            return countString;
        }

        public static int AppendModelSimplePropertyForString(
            int indentSpaces,
            StringBuilder sb,
            string propertyName,
            KeyValuePair<string, OpenApiSchema> schemaProperty,
            string propertyValueGenerated,
            int countString,
            bool asJsonBody,
            int depthHierarchy,
            TrailingCharType trailingChar)
        {
            if ("null".Equals(propertyValueGenerated, StringComparison.Ordinal))
            {
                if (asJsonBody)
                {
                    sb.AppendLine(
                        indentSpaces,
                        WrapInStringBuilderAppendLineWithKeyQuotes(depthHierarchy, propertyName, "null", trailingChar));
                }
                else
                {
                    sb.AppendLine(
                        indentSpaces + 4,
                        $"{propertyName} = null,");
                }
            }
            else
            {
                if (!schemaProperty.Value.IsFormatTypeEmail() &&
                    !schemaProperty.Value.IsRuleValidationString())
                {
                    if (countString > 0)
                    {
                        propertyValueGenerated = $"{propertyValueGenerated}{countString}";
                    }

                    countString++;
                }

                if (asJsonBody)
                {
                    sb.AppendLine(
                        indentSpaces,
                        WrapInStringBuilderAppendLineWithKeyAndValueQuotes(depthHierarchy, propertyName, propertyValueGenerated, trailingChar));
                }
                else
                {
                    sb.AppendLine(
                        indentSpaces + 4,
                        $"{propertyName} = \"{propertyValueGenerated}\",");
                }
            }

            return countString;
        }

        public static void AppendModelSimplePropertyForDateTimeOffset(
            int indentSpaces,
            StringBuilder sb,
            string propertyName,
            string propertyValueGenerated,
            bool asJsonBody,
            int depthHierarchy,
            TrailingCharType trailingChar)
        {
            if (asJsonBody)
            {
                sb.AppendLine(
                    indentSpaces,
                    WrapInStringBuilderAppendLineWithKeyAndValueQuotes(depthHierarchy, propertyName, propertyValueGenerated, trailingChar));
            }
            else
            {
                sb.AppendLine(
                    indentSpaces + 4,
                    $"{propertyName} = DateTimeOffset.Parse(\"{propertyValueGenerated}\"),");
            }
        }

        public static void AppendModelSimplePropertyForGuid(
            int indentSpaces,
            StringBuilder sb,
            string propertyName,
            string propertyValueGenerated,
            bool asJsonBody,
            int depthHierarchy,
            TrailingCharType trailingChar)
        {
            if (asJsonBody)
            {
                sb.AppendLine(
                    indentSpaces,
                    WrapInStringBuilderAppendLineWithKeyAndValueQuotes(depthHierarchy, propertyName, propertyValueGenerated, trailingChar));
            }
            else
            {
                sb.AppendLine(
                    indentSpaces + 4,
                    $"{propertyName} = Guid.Parse(\"{propertyValueGenerated}\"),");
            }
        }

        public static void AppendModelSimplePropertyForUri(
            int indentSpaces,
            StringBuilder sb,
            string propertyName,
            string propertyValueGenerated,
            bool asJsonBody,
            int depthHierarchy,
            TrailingCharType trailingChar)
        {
            if (asJsonBody)
            {
                sb.AppendLine(
                    indentSpaces,
                    WrapInStringBuilderAppendLineWithKeyAndValueQuotes(depthHierarchy, propertyName, propertyValueGenerated, trailingChar));
            }
            else
            {
                sb.AppendLine(
                    indentSpaces + 4,
                    $"{propertyName} = new Uri(\"{propertyValueGenerated}\"),");
            }
        }

        public static void AppendModelSimplePropertyForIFormFile(
            int indentSpaces,
            StringBuilder sb,
            string propertyName,
            string propertyValueGenerated,
            bool asJsonBody,
            int depthHierarchy,
            TrailingCharType trailingChar)
        {
            if (asJsonBody)
            {
                throw new NotSupportedException("IFormFile");
            }
            else
            {
                sb.AppendLine(
                    indentSpaces + 4,
                    $"{propertyName} = GetTestFile(),");
            }
        }

        public static void AppendModelSimplePropertyDefault(
            int indentSpaces,
            StringBuilder sb,
            string propertyName,
            string propertyValueGenerated,
            bool asJsonBody,
            int depthHierarchy,
            TrailingCharType trailingChar)
        {
            if (asJsonBody)
            {
                sb.AppendLine(
                    indentSpaces,
                    WrapInStringBuilderAppendLineWithKeyQuotes(depthHierarchy, propertyName, propertyValueGenerated, trailingChar));
            }
            else
            {
                sb.AppendLine(
                    indentSpaces + 4,
                    $"{propertyName} = {propertyValueGenerated},");
            }
        }

        public static bool IsListKind(string typeName)
        {
            return !string.IsNullOrEmpty(typeName) &&
                   (typeName.StartsWith(Microsoft.OpenApi.Models.NameConstants.Pagination + "<", StringComparison.Ordinal) ||
                    typeName.StartsWith(Microsoft.OpenApi.Models.NameConstants.List + "<", StringComparison.Ordinal));
        }

        public static void AppendPartDataNew(int indentSpaces, StringBuilder sb)
        {
            var value = "new ";
            sb.Append(value.PadLeft(value.Length + indentSpaces));
        }

        public static void AppendPartDataEqualNew(int indentSpaces, StringBuilder sb, string variableName)
        {
            var value = $"{variableName} = new ";
            sb.Append(value.PadLeft(value.Length + indentSpaces));
        }

        public static void AppendPartVarDataEqualNew(int indentSpaces, StringBuilder sb, string variableName = "data")
        {
            var value = $"var {variableName} = new ";
            sb.Append(value.PadLeft(value.Length + indentSpaces));
        }

        public static void AppendPartDataEqualNewListOf(int indentSpaces, StringBuilder sb, string variableName, string dataType)
        {
            var value = $"{variableName} = new List<{dataType}>";
            sb.Append(value.PadLeft(value.Length + indentSpaces));
        }

        public static void AppendPartVarDataEqualNewListOf(int indentSpaces, StringBuilder sb, string variableName, string dataType)
        {
            var value = $"var {variableName} = new List<{dataType}>";
            sb.Append(value.PadLeft(value.Length + indentSpaces));
        }

        public static string? GetDataTypeIfEnum(KeyValuePair<string, OpenApiSchema> schema, IDictionary<string, OpenApiSchema> componentsSchemas)
        {
            var schemaForDataType = componentsSchemas.FirstOrDefault(x => x.Key.Equals(schema.Value.GetDataType(), StringComparison.OrdinalIgnoreCase));
            return schemaForDataType.Key != null && schemaForDataType.Value.IsSchemaEnumOrPropertyEnum()
                ? schemaForDataType.Key
                : null;
        }

        public static TrailingCharType GetTrailingCharForProperty(bool asJsonBody, int currentItem, int totalItems)
        {
            var trailingCharForProperty = TrailingCharType.Comma;
            if (asJsonBody && (currentItem + 1) == totalItems)
            {
                trailingCharForProperty = TrailingCharType.None;
            }

            return trailingCharForProperty;
        }

        public static TrailingCharType GetTrailingCharForProperty(bool asJsonBody, KeyValuePair<string, OpenApiSchema> currentSchema, IDictionary<string, OpenApiSchema> totalSchema)
        {
            var trailingCharForProperty = TrailingCharType.Comma;
            if (asJsonBody && totalSchema.Last().Key == currentSchema.Key)
            {
                trailingCharForProperty = TrailingCharType.None;
            }

            return trailingCharForProperty;
        }

        public static string PropertyValueGenerator(
            KeyValuePair<string, OpenApiSchema> schema,
            IDictionary<string, OpenApiSchema> componentsSchemas,
            bool useForBadRequest,
            int itemNumber,
            string? customValue)
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

        public static string PropertyValueGeneratorTypeResolver(KeyValuePair<string, OpenApiSchema> schema, IDictionary<string, OpenApiSchema> componentsSchemas, bool useForBadRequest)
        {
            var name = schema.Key.EnsureFirstCharacterToUpper();
            var schemaForDataType = componentsSchemas.FirstOrDefault(x => x.Key.Equals(schema.Value.GetDataType(), StringComparison.OrdinalIgnoreCase));

            if (schemaForDataType.Key is null && schema.Value.IsTypeArray())
            {
                var modelName = schema.Value.GetModelName();
                if (!string.IsNullOrEmpty(modelName))
                {
                    return "NEW-INSTANCE-LIST";
                }
            }

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

            return "null";
        }

        public static string WrapInQuotes(string str) => $"\\\"{str}\\\"";

        public static string WrapInStringBuilderAppendLine(string str) => $"sb.AppendLine(\"{str}\");";

        public static string WrapInStringBuilderAppendLineWithKeyQuotes(int depthHierarchy, string key, string value, TrailingCharType trailingChar)
        {
            var jsonSpaces = string.Empty.PadLeft(depthHierarchy * 2);
            return WrapInStringBuilderAppendLine($"{jsonSpaces}  {WrapInQuotes(key)}: {value}{GenerateCodeHelper.GetTrailingChar(trailingChar)}");
        }

        public static string WrapInStringBuilderAppendLineWithKeyAndValueQuotes(int depthHierarchy, string key, string value, TrailingCharType trailingChar)
        {
            var jsonSpaces = string.Empty.PadLeft(depthHierarchy * 2);
            return WrapInStringBuilderAppendLine($"{jsonSpaces}  {WrapInQuotes(key)}: {WrapInQuotes(value)}{GenerateCodeHelper.GetTrailingChar(trailingChar)}");
        }
    }
}