// ReSharper disable MergeIntoPattern
// ReSharper disable ConvertIfStatementToReturnStatement
// ReSharper disable InvertIf
namespace Atc.Rest.ApiGenerator.Framework.Factories.Parameters.ServerClient;

public static class ContentGeneratorServerClientModelParametersFactory
{
    public static ClassParameters CreateForClass(
        string headerContent,
        string @namespace,
        AttributeParameters codeGeneratorAttribute,
        string modelName,
        OpenApiSchema apiSchemaModel,
        bool usePartialClass = false,
        bool includeDeprecated = false)
    {
        ArgumentNullException.ThrowIfNull(apiSchemaModel);
        ArgumentNullException.ThrowIfNull(modelName);

        var documentationTags = apiSchemaModel.ExtractDocumentationTags($"{modelName}.");

        var propertiesParameters = ExtractPropertiesParameters(apiSchemaModel, modelName, includeDeprecated);

        string? genericTypeName = null;
        if (propertiesParameters.Any(x => x.TypeName == "T"))
        {
            genericTypeName = "T";
        }

        return new ClassParameters(
            headerContent,
            @namespace,
            documentationTags,
            new List<AttributeParameters> { codeGeneratorAttribute },
            usePartialClass ? DeclarationModifiers.PublicPartialClass : DeclarationModifiers.PublicClass,
            ClassTypeName: modelName,
            GenericTypeName: genericTypeName,
            InheritedClassTypeName: null,
            InheritedGenericClassTypeName: null,
            InheritedInterfaceTypeName: null,
            Constructors: null,
            Properties: propertiesParameters,
            Methods: null,
            GenerateToStringMethod: true);
    }

    public static RecordsParameters CreateForRecord(
        string headerContent,
        string @namespace,
        AttributeParameters codeGeneratorAttribute,
        string modelName,
        OpenApiSchema apiSchemaModel,
        bool usePartialRecord = false,
        bool includeDeprecated = false)
    {
        ArgumentNullException.ThrowIfNull(apiSchemaModel);

        var documentationTags = apiSchemaModel.ExtractDocumentationTags($"{modelName}.");

        var recordParameters = ExtractRecordParameters(modelName, apiSchemaModel, includeDeprecated);

        return new RecordsParameters(
            headerContent,
            @namespace,
            documentationTags,
            new List<AttributeParameters> { codeGeneratorAttribute },
            usePartialRecord ? DeclarationModifiers.PublicPartialRecord : DeclarationModifiers.PublicRecord,
            Parameters: recordParameters);
    }

    public static ClassParameters CreateForCustomErrorResponseModel(
        string codeGeneratorContentHeader,
        string fullNamespace,
        AttributeParameters codeGeneratorAttribute,
        CustomErrorResponseModel customErrorResponseModel,
        bool usePartialClass)
    {
        ArgumentNullException.ThrowIfNull(customErrorResponseModel);

        var summery = "Represents an error response.";
        var documentationTags = new CodeDocumentationTags(summery);
        if (!string.IsNullOrEmpty(customErrorResponseModel.Description))
        {
            documentationTags = new CodeDocumentationTags(
                summary: customErrorResponseModel.Description,
                parameters: null,
                remark: null,
                code: null,
                example: null,
                exceptions: null,
                @return: null);
        }

        var properties = new List<PropertyParameters>();
        foreach (var schema in customErrorResponseModel.Schema)
        {
            CodeDocumentationTags? documentationTag = null;

            string dataType;
            var isNullableType = false;
            string? defaultValue = null;

            if (schema.Value.DataType.EndsWith('?'))
            {
                dataType = schema.Value.DataType.Replace("?", string.Empty, StringComparison.Ordinal);
                isNullableType = true;
            }
            else
            {
                dataType = schema.Value.DataType;
                if (dataType.Equals("string", StringComparison.OrdinalIgnoreCase))
                {
                    defaultValue = "string.Empty";
                }
            }

            properties.Add(
                new PropertyParameters(
                    documentationTag,
                    Attributes: null,
                    DeclarationModifiers.Public,
                    GenericTypeName: null,
                    TypeName: dataType,
                    IsNullableType: isNullableType,
                    Name: schema.Key.EnsureFirstCharacterToUpper(),
                    JsonName: null,
                    DefaultValue: defaultValue,
                    IsReferenceType: false,
                    IsGenericListType: false,
                    UseAutoProperty: true,
                    UseGet: true,
                    UseSet: true,
                    UseExpressionBody: false,
                    Content: null));
        }

        return new ClassParameters(
            HeaderContent: null,
            fullNamespace,
            documentationTags,
            new List<AttributeParameters> { codeGeneratorAttribute },
            usePartialClass ? DeclarationModifiers.PublicPartialClass : DeclarationModifiers.PublicClass,
            ClassTypeName: customErrorResponseModel.Name.EnsureFirstCharacterToUpper(),
            GenericTypeName: null,
            InheritedClassTypeName: null,
            InheritedGenericClassTypeName: null,
            InheritedInterfaceTypeName: null,
            Constructors: null,
            Properties: properties,
            Methods: null,
            GenerateToStringMethod: true);
    }

    private static bool GetRequired(
        ICollection<string> required,
        string name,
        bool hasAnyPropertiesAsArrayWithFormatTypeBinary)
    {
        if (required.Count == 0 || hasAnyPropertiesAsArrayWithFormatTypeBinary)
        {
            return false;
        }

        return required.Contains(name, StringComparer.OrdinalIgnoreCase);
    }

    private static string? GetDefaultValue(
        OpenApiSchema schema,
        string? dataTypeForList)
    {
        if (schema.Default is not null ||
            string.IsNullOrEmpty(dataTypeForList))
        {
            if (schema.IsSchemaEnum())
            {
                var value = schema.Default.GetDefaultValueAsString();
                return value is null
                    ? null
                    : $"{schema.GetModelName()}.{value.EnsureFirstCharacterToUpper()}";
            }

            return schema.Default.GetDefaultValueAsString();
        }

        if (NameConstants.List.Equals(dataTypeForList, StringComparison.Ordinal))
        {
            return null;
        }

        return $"new List<{dataTypeForList}>()";
    }

    private static List<PropertyParameters> ExtractPropertiesParameters(
        OpenApiSchema apiSchemaModel,
        string modelName,
        bool includeDeprecated)
    {
        var propertiesParameters = new List<PropertyParameters>();

        var hasAnyPropertiesAsArrayWithFormatTypeBinary = apiSchemaModel.HasAnyPropertiesAsArrayWithFormatTypeBinary();

        if (apiSchemaModel.Properties.Count == 0)
        {
            var childModelName = apiSchemaModel.Items is null
                ? apiSchemaModel.GetModelName()
                : apiSchemaModel.Items.GetModelName();

            var documentationTags = new CodeDocumentationTags($"A list of {childModelName}.");

            propertiesParameters.Add(
                new PropertyParameters(
                    documentationTags,
                    Attributes: null,
                    DeclarationModifier: DeclarationModifiers.Public,
                    GenericTypeName: NameConstants.List,
                    IsGenericListType: true,
                    TypeName: childModelName,
                    IsNullableType: false,
                    IsReferenceType: false,
                    Name: childModelName + NameConstants.List,
                    JsonName: null,
                    DefaultValue: $"new {NameConstants.List}<{childModelName}>()",
                    UseAutoProperty: true,
                    UseGet: true,
                    UseSet: true,
                    UseExpressionBody: false,
                    Content: null));
        }
        else
        {
            foreach (var apiSchema in apiSchemaModel.Properties)
            {
                var openApiParameter = apiSchema.Value;

                if (openApiParameter.Deprecated && !includeDeprecated)
                {
                    continue;
                }

                var useListForDataType = openApiParameter.IsTypeArray();

                string? dataType = null;
                if (useListForDataType)
                {
                    if (apiSchema.Key.IsNamedAsItemsOrResult() &&
                        string.IsNullOrEmpty(openApiParameter.Items.Type))
                    {
                        dataType = "T";
                    }

                    dataType ??= openApiParameter.Items.GetDataType();

                    if ("Object".Equals(dataType, StringComparison.Ordinal))
                    {
                        dataType = "object";
                    }
                }
                else
                {
                    dataType = openApiParameter.GetDataType();

                    if ("Object".Equals(dataType, StringComparison.Ordinal))
                    {
                        if (openApiParameter.AdditionalProperties is not null)
                        {
                            // A defined Object with AdditionalProperties is a Dictionary - https://swagger.io/docs/specification/data-models/dictionaries/
                            var additionalPropertiesDataType = openApiParameter.AdditionalProperties.GetDataType();
                            dataType = $"Dictionary<string, {additionalPropertiesDataType}>";
                        }
                        else
                        {
                            dataType = "object";
                        }
                    }
                }

                var isSimpleType = useListForDataType
                    ? openApiParameter.Items.IsSimpleDataType() || openApiParameter.Items.IsSchemaEnumOrPropertyEnum() || openApiParameter.Items.IsFormatTypeBinary()
                    : openApiParameter.IsSimpleDataType() || openApiParameter.IsSchemaEnumOrPropertyEnum() || openApiParameter.IsFormatTypeBinary();

                string? dataTypeForList = null;
                string? description = null;
                if (hasAnyPropertiesAsArrayWithFormatTypeBinary)
                {
                    description = "A list of File(s).";

                    dataTypeForList = dataType;
                }
                else if (useListForDataType && !string.IsNullOrEmpty(openApiParameter.Items.Title))
                {
                    description = $"A list of {openApiParameter.Items.Title}.";

                    if (openApiParameter.Default is null &&
                        !GetRequired(apiSchemaModel.Required, apiSchema.Key, hasAnyPropertiesAsArrayWithFormatTypeBinary))
                    {
                        dataTypeForList = dataType;
                    }
                }

                var documentationTags = openApiParameter.ExtractDocumentationTags(description);

                if (dataTypeForList is null &&
                    openApiParameter.Default is null &&
                    useListForDataType &&
                    !GetRequired(apiSchemaModel.Required, apiSchema.Key, hasAnyPropertiesAsArrayWithFormatTypeBinary))
                {
                    dataTypeForList = dataType;
                }

                if (dataTypeForList is null && useListForDataType)
                {
                    dataTypeForList = NameConstants.List;
                }

                var defaultValue = GetDefaultValue(openApiParameter, dataTypeForList);

                if (dataType.Equals(dataTypeForList, StringComparison.Ordinal))
                {
                    dataTypeForList = NameConstants.List;
                }

                var name = apiSchema.Key.EnsureFirstCharacterToUpper();
                string? jsonName = null;
                if (modelName.Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    name = $"{name}Property";
                    jsonName = apiSchema.Key;
                }

                propertiesParameters.Add(
                    new PropertyParameters(
                        documentationTags,
                        Attributes: ExtractAttributeParameters(
                            apiSchemaModel.Required,
                            apiSchema.Key,
                            hasAnyPropertiesAsArrayWithFormatTypeBinary,
                            openApiParameter),
                        DeclarationModifier: DeclarationModifiers.Public,
                        GenericTypeName: dataTypeForList,
                        IsGenericListType: !string.IsNullOrEmpty(dataTypeForList),
                        TypeName: dataType,
                        IsNullableType: openApiParameter.Nullable,
                        IsReferenceType: !isSimpleType,
                        Name: name,
                        JsonName: jsonName,
                        DefaultValue: defaultValue,
                        UseAutoProperty: true,
                        UseGet: true,
                        UseSet: true,
                        UseExpressionBody: false,
                        Content: null));
            }
        }

        return propertiesParameters;
    }

    private static IList<AttributeParameters> ExtractAttributeParameters(
        ICollection<string> required,
        string apiSchemaKey,
        bool hasAnyPropertiesAsArrayWithFormatTypeBinary,
        OpenApiSchema openApiParameter)
    {
        var attributesParameters = new List<AttributeParameters>();
        if (GetRequired(required, apiSchemaKey, hasAnyPropertiesAsArrayWithFormatTypeBinary))
        {
            attributesParameters.Add(new AttributeParameters("Required", Content: null));
        }

        var validationAttributeExtractor = new ValidationAttributeExtractor();
        attributesParameters.AddRange(
            AttributesParametersFactory.Create(
                validationAttributeExtractor.Extract(openApiParameter)));

        return attributesParameters;
    }

    private static List<RecordParameters> ExtractRecordParameters(
        string modelName,
        OpenApiSchema apiSchemaModel,
        bool includeDeprecated)
    {
        var documentationTags = apiSchemaModel.ExtractDocumentationTags($"{modelName}.");

        return
        [
            new(
                documentationTags,
                DeclarationModifiers.PublicRecord,
                Name: modelName,
                Parameters: ExtractRecordParameterBaseParameters(apiSchemaModel, includeDeprecated))

        ];
    }

    private static List<ParameterBaseParameters> ExtractRecordParameterBaseParameters(
        OpenApiSchema apiSchemaModel,
        bool includeDeprecated)
    {
        var parameterBaseParameters = new List<ParameterBaseParameters>();

        var hasAnyPropertiesAsArrayWithFormatTypeBinary = apiSchemaModel.HasAnyPropertiesAsArrayWithFormatTypeBinary();

        if (apiSchemaModel.Properties.Count == 0)
        {
            var childModelName = apiSchemaModel.Items.GetModelName();

            parameterBaseParameters.Add(
             new PropertyParameters(
                 DocumentationTags: null,
                 Attributes: null,
                 DeclarationModifier: DeclarationModifiers.Public,
                 GenericTypeName: NameConstants.List,
                 IsGenericListType: true,
                 TypeName: childModelName,
                 IsNullableType: false,
                 IsReferenceType: false,
                 Name: childModelName + NameConstants.List,
                 JsonName: null,
                 DefaultValue: null,
                 UseAutoProperty: true,
                 UseGet: true,
                 UseSet: true,
                 UseExpressionBody: false,
                 Content: null));
        }
        else
        {
            foreach (var apiSchema in apiSchemaModel.Properties)
            {
                var openApiParameter = apiSchema.Value;

                if (openApiParameter.Deprecated && !includeDeprecated)
                {
                    continue;
                }

                var useListForDataType = openApiParameter.IsTypeArray();

                string? dataType = null;
                if (useListForDataType)
                {
                    if (apiSchema.Key.IsNamedAsItemsOrResult() &&
                        string.IsNullOrEmpty(openApiParameter.Items.Type))
                    {
                        dataType = "T";
                    }

                    dataType ??= openApiParameter.Items.GetDataType();
                }
                else
                {
                    dataType = openApiParameter.GetDataType();
                }

                var isSimpleType = useListForDataType
                    ? openApiParameter.Items.IsSimpleDataType() || openApiParameter.Items.IsSchemaEnumOrPropertyEnum() || openApiParameter.Items.IsFormatTypeBinary()
                    : openApiParameter.IsSimpleDataType() || openApiParameter.IsSchemaEnumOrPropertyEnum() || openApiParameter.IsFormatTypeBinary();

                string? dataTypeForList = null;
                if (hasAnyPropertiesAsArrayWithFormatTypeBinary ||
                    (useListForDataType && !string.IsNullOrEmpty(openApiParameter.Items.Title) &&
                     openApiParameter.Default is null &&
                     !GetRequired(apiSchemaModel.Required, apiSchema.Key, hasAnyPropertiesAsArrayWithFormatTypeBinary)))
                {
                    dataTypeForList = dataType;
                }

                if (dataTypeForList is null &&
                    openApiParameter.Default is null &&
                    useListForDataType &&
                    !GetRequired(apiSchemaModel.Required, apiSchema.Key, hasAnyPropertiesAsArrayWithFormatTypeBinary))
                {
                    dataTypeForList = dataType;
                }

                string? defaultValue = null;
                if (useListForDataType)
                {
                    dataTypeForList = NameConstants.List;
                }
                else
                {
                    defaultValue = GetDefaultValue(openApiParameter, dataTypeForList);
                }

                parameterBaseParameters.Add(
                    new ParameterBaseParameters(
                        Attributes: ExtractAttributeParameters(
                            apiSchemaModel.Required,
                            apiSchema.Key,
                            hasAnyPropertiesAsArrayWithFormatTypeBinary,
                            openApiParameter),
                        GenericTypeName: dataTypeForList,
                        IsGenericListType: !string.IsNullOrEmpty(dataTypeForList),
                        TypeName: dataType,
                        IsNullableType: openApiParameter.Nullable,
                        IsReferenceType: !isSimpleType,
                        Name: apiSchema.GetFormattedKey(),
                        DefaultValue: defaultValue));
            }
        }

        return SortOptionalParametersMustAppearAfterAllRequiredParameters(parameterBaseParameters);
    }

    /// <summary>
    /// CS1737 -  Sorts the optional parameters must appear after all required parameters.
    /// </summary>
    /// <param name="parameters">The parameter base parameters.</param>
    private static List<ParameterBaseParameters> SortOptionalParametersMustAppearAfterAllRequiredParameters(
        IReadOnlyCollection<ParameterBaseParameters> parameters)
    {
        var parametersWithoutDefaultValues = parameters.Where(x => x.DefaultValue is null).ToList();
        var parametersWitDefaultValues = parameters.Where(x => x.DefaultValue is not null).ToList();

        var data = new List<ParameterBaseParameters>();
        data.AddRange(parametersWithoutDefaultValues);
        data.AddRange(parametersWitDefaultValues);
        return data;
    }
}