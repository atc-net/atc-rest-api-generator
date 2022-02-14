// ReSharper disable ConvertIfStatementToSwitchStatement
// ReSharper disable SwitchStatementMissingSomeEnumCasesNoDefault
// ReSharper disable UseDeconstructionOnParameter
namespace Atc.Rest.ApiGenerator.Helpers.XunitTest;

public static class GenerateXunitTestHelper
{
    public static void AppendVarDataListSimpleType(
        int indentSpaces,
        StringBuilder sb,
        string simpleDataType,
        int maxItemsForList = 3)
    {
        sb.AppendLine(indentSpaces, $"var data = new List<{simpleDataType}>()");
        sb.AppendLine(indentSpaces, "{");
        for (var i = 0; i < maxItemsForList; i++)
        {
            switch (simpleDataType)
            {
                case "string":
                    sb.AppendLine(indentSpaces + 4, $"\"Hallo {i}\",");
                    break;
                case "double":
                case "long":
                case "int":
                    sb.AppendLine(indentSpaces + 4, $"{i},");
                    break;
                case "bool":
                    sb.AppendLine(indentSpaces + 4, i.IsEven() ? "true," : "false,");
                    break;
                case "guid":
                    var x = i;
                    if (x is > 9 or < 0)
                    {
                        x = 9;
                    }

                    sb.AppendLine(indentSpaces + 4, $"\"a6ed4bde-0f0e-4f58-b432-a668e21c800{x}\",");
                    break;
            }
        }

        sb.AppendLine(indentSpaces, "};");
    }

    public static void AppendVarDataModelOrListOfModel(
        int indentSpaces,
        StringBuilder sb,
        EndpointMethodMetadata endpointMethodMetadata,
        OpenApiSchema schema,
        HttpStatusCode httpStatusCode,
        SchemaMapLocatedAreaType locatedAreaType,
        KeyValuePair<string, OpenApiSchema>? badPropertySchema = null,
        bool asJsonBody = false,
        int maxItemsForList = 3,
        int depthHierarchy = 0,
        int maxDepthHierarchy = 2)
    {
        ArgumentNullException.ThrowIfNull(endpointMethodMetadata);

        var trailingChar = TrailingCharType.SemiColon;
        if (asJsonBody)
        {
            trailingChar = TrailingCharType.None;
        }

        switch (locatedAreaType)
        {
            case SchemaMapLocatedAreaType.Parameter:
                break;
            case SchemaMapLocatedAreaType.RequestBody:
                if (schema.IsTypeArray())
                {
                    var indentSpacesForData = indentSpaces;
                    if (asJsonBody)
                    {
                        sb.AppendLine(indentSpaces, "var sb = new StringBuilder();");
                        indentSpacesForData -= 4;
                    }

                    if (schema.HasItemsWithFormatTypeBinary())
                    {
                        if (asJsonBody)
                        {
                            throw new NotSupportedException("JSON not supported when RequestBody is type Array and format type is Binary.");
                        }

                        sb.AppendLine(indentSpaces, "var data = GetTestFiles();");
                    }
                    else
                    {
                        AppendVarDataEqualNewListOfModel(
                            indentSpacesForData,
                            sb,
                            endpointMethodMetadata,
                            new KeyValuePair<string, OpenApiSchema>("data", schema),
                            trailingChar,
                            maxItemsForList,
                            depthHierarchy,
                            maxDepthHierarchy,
                            badPropertySchema,
                            asJsonBody);
                    }

                    if (asJsonBody)
                    {
                        sb.AppendLine(indentSpaces, "var data = sb.ToString();");
                    }
                }
                else
                {
                    if (asJsonBody)
                    {
                        sb.AppendLine(indentSpaces, "var sb = new StringBuilder();");
                    }
                    else
                    {
                        GenerateXunitTestPartsHelper.AppendPartVarDataEqualNew(12, sb);
                    }

                    var modelName = schema.GetModelName();
                    AppendModel(
                        indentSpaces,
                        sb,
                        endpointMethodMetadata,
                        modelName,
                        schema,
                        trailingChar,
                        0,
                        maxItemsForList,
                        depthHierarchy,
                        maxDepthHierarchy,
                        badPropertySchema,
                        asJsonBody);

                    if (asJsonBody)
                    {
                        sb.AppendLine(indentSpaces, "var data = sb.ToString();");
                    }
                }

                break;
            case SchemaMapLocatedAreaType.Response:
                var contractReturnTypeName = endpointMethodMetadata.ContractReturnTypeNames.First(x => x.StatusCode == httpStatusCode);
                if (GenerateXunitTestPartsHelper.IsListKind(contractReturnTypeName.FullModelName))
                {
                    AppendVarDataEqualNewListOfModel(
                        indentSpaces,
                        sb,
                        endpointMethodMetadata,
                        new KeyValuePair<string, OpenApiSchema>("data", contractReturnTypeName.Schema!),
                        trailingChar,
                        maxItemsForList,
                        depthHierarchy,
                        maxDepthHierarchy,
                        badPropertySchema,
                        asJsonBody);
                }
                else
                {
                    GenerateXunitTestPartsHelper.AppendPartVarDataEqualNew(12, sb);
                    AppendModel(
                        indentSpaces,
                        sb,
                        endpointMethodMetadata,
                        contractReturnTypeName.FullModelName,
                        contractReturnTypeName.Schema!,
                        trailingChar,
                        0,
                        maxItemsForList,
                        depthHierarchy,
                        maxDepthHierarchy,
                        badPropertySchema,
                        asJsonBody);
                }

                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(locatedAreaType), locatedAreaType, message: null);
        }
    }

    public static void AppendModel(
        int indentSpaces,
        StringBuilder sb,
        EndpointMethodMetadata endpointMethodMetadata,
        string modelName,
        OpenApiSchema schema,
        TrailingCharType trailingChar,
        int itemNumber,
        int maxItemsForList,
        int depthHierarchy,
        int maxDepthHierarchy,
        KeyValuePair<string, OpenApiSchema>? badPropertySchema,
        bool asJsonBody,
        string? parentModelNameToJsonBody = null)
    {
        ArgumentNullException.ThrowIfNull(sb);
        ArgumentNullException.ThrowIfNull(schema);

        var countString = 1;
        var renderModelName = OpenApiDocumentSchemaModelNameHelper.EnsureModelNameWithNamespaceIfNeeded(endpointMethodMetadata, modelName);
        var jsonSpaces = string.Empty.PadLeft(depthHierarchy * 2);
        if (asJsonBody)
        {
            sb.AppendLine(
                indentSpaces,
                string.IsNullOrEmpty(parentModelNameToJsonBody)
                    ? GenerateXunitTestPartsHelper.WrapInStringBuilderAppendLine($"{jsonSpaces}{{")
                    : GenerateXunitTestPartsHelper.WrapInStringBuilderAppendLine($"{jsonSpaces}\\\"{parentModelNameToJsonBody}\\\": {{"));
        }
        else
        {
            sb.AppendLine(renderModelName);
            sb.AppendLine(indentSpaces, "{");
        }

        foreach (var schemaProperty in schema.Properties)
        {
            var trailingCharForProperty = GenerateXunitTestPartsHelper.GetTrailingCharForProperty(asJsonBody, schemaProperty, schema.Properties);
            var useForBadRequest = badPropertySchema is not null &&
                                   schemaProperty.Key.Equals(badPropertySchema.Value.Key, StringComparison.Ordinal);

            var dataType = schemaProperty.Value.GetDataType();

            var propertyValueGenerated = GenerateXunitTestPartsHelper.PropertyValueGenerator(
                schemaProperty,
                endpointMethodMetadata.ComponentsSchemas,
                useForBadRequest,
                itemNumber,
                customValue: null);

            if ("NEW-INSTANCE-LIST".Equals(propertyValueGenerated, StringComparison.Ordinal))
            {
                AppendDataEqualNewListOfModel(
                    indentSpaces + 4,
                    sb,
                    endpointMethodMetadata,
                    schemaProperty,
                    trailingCharForProperty,
                    maxItemsForList,
                    depthHierarchy + 1,
                    maxDepthHierarchy,
                    badPropertySchema,
                    asJsonBody);
            }
            else if ("NEW-INSTANCE".Equals(propertyValueGenerated, StringComparison.Ordinal))
            {
                AppendModelComplexProperty(
                    indentSpaces,
                    sb,
                    endpointMethodMetadata,
                    schemaProperty,
                    dataType,
                    trailingCharForProperty,
                    itemNumber,
                    maxItemsForList,
                    depthHierarchy + 1,
                    maxDepthHierarchy,
                    badPropertySchema,
                    asJsonBody);
            }
            else
            {
                var countResult = GenerateXunitTestPartsHelper.AppendModelSimpleProperty(
                    indentSpaces,
                    sb,
                    endpointMethodMetadata,
                    schemaProperty,
                    dataType,
                    schema.Required.Contains(schemaProperty.Key),
                    propertyValueGenerated,
                    countString,
                    asJsonBody,
                    depthHierarchy,
                    trailingCharForProperty);

                if (countResult > 1)
                {
                    countString += 1;
                }
            }
        }

        sb.AppendLine(
            indentSpaces,
            asJsonBody
                ? GenerateXunitTestPartsHelper.WrapInStringBuilderAppendLine($"{jsonSpaces}}}{GenerateCodeHelper.GetTrailingChar(trailingChar)}")
                : $"}}{GenerateCodeHelper.GetTrailingChar(trailingChar)}");
    }

    public static void AppendModelComplexProperty(
        int indentSpaces,
        StringBuilder sb,
        EndpointMethodMetadata endpointMethodMetadata,
        KeyValuePair<string, OpenApiSchema> schemaProperty,
        string dataType,
        TrailingCharType trailingChar,
        int itemNumber,
        int maxItemsForList,
        int depthHierarchy,
        int maxDepthHierarchy,
        KeyValuePair<string, OpenApiSchema>? badPropertySchema,
        bool asJsonBody)
    {
        var propertyName = schemaProperty.Key.EnsureFirstCharacterToUpper();
        if (depthHierarchy > maxDepthHierarchy)
        {
            if (asJsonBody)
            {
                // TODO Missing Json support.
            }
            else
            {
                sb.AppendLine(
                    indentSpaces,
                    $"{propertyName} = null{GenerateCodeHelper.GetTrailingChar(trailingChar)}");
                return;
            }
        }

        if (!asJsonBody)
        {
            indentSpaces += 4;
            GenerateXunitTestPartsHelper.AppendPartDataEqualNew(
                indentSpaces,
                sb,
                propertyName);
        }

        var schemaPropertyValue = schemaProperty.Value;
        if (schemaProperty.Value.Properties.Count == 0 &&
            schemaProperty.Value.OneOf.Count > 0)
        {
            schemaPropertyValue = schemaProperty.Value.OneOf.First();
        }

        var modelName = schemaProperty.Key.EnsureFirstCharacterToUpper();

        AppendModel(
            indentSpaces,
            sb,
            endpointMethodMetadata,
            dataType,
            schemaPropertyValue,
            trailingChar,
            itemNumber,
            maxItemsForList,
            depthHierarchy,
            maxDepthHierarchy,
            badPropertySchema,
            asJsonBody,
            modelName);
    }

    public static void AppendVarDataEqualNewListOfModel(
        int indentSpaces,
        StringBuilder sb,
        EndpointMethodMetadata endpointMethodMetadata,
        KeyValuePair<string, OpenApiSchema> schemaProperty,
        TrailingCharType trailingChar,
        int maxItemsForList,
        int depthHierarchy,
        int maxDepthHierarchy,
        KeyValuePair<string, OpenApiSchema>? badPropertySchema,
        bool asJsonBody)
    {
        ArgumentNullException.ThrowIfNull(sb);

        var modelName = schemaProperty.Value.GetModelName();
        var renderModelName = OpenApiDocumentSchemaModelNameHelper.EnsureModelNameWithNamespaceIfNeeded(endpointMethodMetadata, modelName);

        if (depthHierarchy > maxDepthHierarchy)
        {
            if (asJsonBody)
            {
                // TODO Missing Json support.
            }
            else
            {
                sb.AppendLine(
                    indentSpaces,
                    $"var {schemaProperty.Key} = new List<{renderModelName}>(){GenerateCodeHelper.GetTrailingChar(trailingChar)}");
                return;
            }
        }

        if (!asJsonBody)
        {
            GenerateXunitTestPartsHelper.AppendPartVarDataEqualNewListOf(
                indentSpaces,
                sb,
                schemaProperty.Key,
                renderModelName);
            sb.AppendLine();
            sb.AppendLine(indentSpaces, "{");
        }

        var modelSchema = endpointMethodMetadata.ComponentsSchemas.GetSchemaByModelName(modelName);
        for (var i = 0; i < maxItemsForList; i++)
        {
            var trailingCharForProperty = GenerateXunitTestPartsHelper.GetTrailingCharForProperty(asJsonBody, i, maxItemsForList);
            var indentSpacesForItem = indentSpaces + 4;
            if (!asJsonBody)
            {
                indentSpacesForItem = indentSpaces + 4;
                GenerateXunitTestPartsHelper.AppendPartDataNew(indentSpacesForItem, sb);
            }

            AppendModel(
                indentSpacesForItem,
                sb,
                endpointMethodMetadata,
                modelName,
                modelSchema,
                trailingCharForProperty,
                i + 1,
                maxItemsForList,
                depthHierarchy,
                maxDepthHierarchy,
                badPropertySchema,
                asJsonBody);
        }

        if (!asJsonBody)
        {
            sb.AppendLine(
                indentSpaces,
                $"}}{GenerateCodeHelper.GetTrailingChar(trailingChar)}");
        }
    }

    public static void AppendDataEqualNewListOfModel(
        int indentSpaces,
        StringBuilder sb,
        EndpointMethodMetadata endpointMethodMetadata,
        KeyValuePair<string, OpenApiSchema> schemaProperty,
        TrailingCharType trailingChar,
        int maxItemsForList,
        int depthHierarchy,
        int maxDepthHierarchy,
        KeyValuePair<string, OpenApiSchema>? badPropertySchema,
        bool asJsonBody)
    {
        var modelName = schemaProperty.Value.GetModelName();
        var renderModelName = OpenApiDocumentSchemaModelNameHelper.EnsureModelNameWithNamespaceIfNeeded(endpointMethodMetadata, modelName);
        var propertyName = schemaProperty.Key.EnsureFirstCharacterToUpper();
        var jsonSpacesCount = (depthHierarchy * 2) + 2;

        if (depthHierarchy > maxDepthHierarchy)
        {
            if (asJsonBody)
            {
                // TODO Missing Json support.
            }
            else
            {
                sb.AppendLine(
                    indentSpaces,
                    $"{propertyName} = new List<{renderModelName}>()" + GenerateCodeHelper.GetTrailingChar(trailingChar));
                return;
            }
        }

        if (asJsonBody)
        {
            var useForBadRequest = badPropertySchema is not null &&
                                   schemaProperty.Key.Equals(badPropertySchema.Value.Key, StringComparison.Ordinal);

            if (useForBadRequest)
            {
                sb.AppendLine(
                    indentSpaces - jsonSpacesCount,
                    GenerateXunitTestPartsHelper.WrapInStringBuilderAppendLineWithKeyQuotes(depthHierarchy - 1, propertyName, "null", trailingChar));
                return;
            }

            sb.AppendLine(
                indentSpaces - jsonSpacesCount,
                GenerateXunitTestPartsHelper.WrapInStringBuilderAppendLineWithKeyQuotes(depthHierarchy - 1, propertyName, "[", TrailingCharType.None));
        }
        else
        {
            GenerateXunitTestPartsHelper.AppendPartDataEqualNewListOf(
                indentSpaces,
                sb,
                propertyName,
                modelName);
            sb.AppendLine();
            sb.AppendLine(indentSpaces, "{");
        }

        var modelSchema = endpointMethodMetadata.ComponentsSchemas.GetSchemaByModelName(modelName);
        var currentIndentSpaces = asJsonBody
            ? indentSpaces - jsonSpacesCount
            : indentSpaces + 4;

        for (int i = 0; i < maxItemsForList; i++)
        {
            var trailingCharForProperty = GenerateXunitTestPartsHelper.GetTrailingCharForProperty(asJsonBody, i, maxItemsForList);

            if (!asJsonBody)
            {
                GenerateXunitTestPartsHelper.AppendPartDataNew(currentIndentSpaces, sb);
            }

            AppendModel(
                currentIndentSpaces,
                sb,
                endpointMethodMetadata,
                modelName,
                modelSchema,
                trailingCharForProperty,
                i + 1,
                maxItemsForList,
                depthHierarchy,
                maxDepthHierarchy,
                badPropertySchema,
                asJsonBody);
        }

        if (asJsonBody)
        {
            var jsonSpaces = string.Empty.PadLeft((depthHierarchy - 1) * 2);
            sb.AppendLine(
                indentSpaces - jsonSpacesCount,
                GenerateXunitTestPartsHelper.WrapInStringBuilderAppendLine($"{jsonSpaces}  ]{GenerateCodeHelper.GetTrailingChar(trailingChar)}"));
        }
        else
        {
            sb.AppendLine(
                indentSpaces,
                $"}}{GenerateCodeHelper.GetTrailingChar(trailingChar)}");
        }
    }
}