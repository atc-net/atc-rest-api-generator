// ReSharper disable MergeIntoPattern
// ReSharper disable ConvertIfStatementToReturnStatement
namespace Atc.Rest.ApiGenerator.Framework.Factories.Parameters.ServerClient;

public static class ContentGeneratorServerClientModelParametersFactory
{
    public static ClassParameters Create(
        string headerContent,
        string @namespace,
        AttributeParameters codeGeneratorAttribute,
        string modelName,
        OpenApiSchema apiSchemaModel)
    {
        ArgumentNullException.ThrowIfNull(apiSchemaModel);

        var documentationTags = apiSchemaModel.ExtractDocumentationTags($"{modelName}.");

        var propertiesParameters = ExtractPropertiesParameters(apiSchemaModel);

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
            AccessModifiers.Public,
            ClassTypeName: modelName,
            GenericTypeName: genericTypeName,
            InheritedClassTypeName: null,
            InheritedGenericClassTypeName: null,
            InheritedInterfaceTypeName: null,
            Constructors: null,
            Properties: propertiesParameters,
            Methods: null,
            GenerateToStringMethode: true);
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
        IOpenApiAny? initializer,
        string? dataTypeForList)
    {
        if (initializer is not null ||
            string.IsNullOrEmpty(dataTypeForList))
        {
            return initializer.GetDefaultValueAsString();
        }

        if ("List".Equals(dataTypeForList, StringComparison.Ordinal))
        {
            return null;
        }

        return $"new List<{dataTypeForList}>()";
    }

    private static List<PropertyParameters> ExtractPropertiesParameters(
        OpenApiSchema apiSchemaModel)
    {
        var hasAnyPropertiesAsArrayWithFormatTypeBinary = apiSchemaModel.HasAnyPropertiesAsArrayWithFormatTypeBinary();

        var propertiesParameters = new List<PropertyParameters>();

        if (apiSchemaModel.Properties.Count == 0)
        {
            var childModelName = apiSchemaModel.Items.GetModelName();

            var documentationTags = new CodeDocumentationTags($"A list of {childModelName}.");

            propertiesParameters.Add(
                new PropertyParameters(
                    documentationTags,
                    Attributes: null,
                    AccessModifier: AccessModifiers.Public,
                    GenericTypeName: "List",
                    IsGenericListType: true,
                    TypeName: childModelName,
                    IsReferenceType: false,
                    Name: childModelName + "List",
                    DefaultValue: $"new List<{childModelName}>()",
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
                    dataType = openApiParameter.AnyOf.Count == 1
                        ? openApiParameter.AnyOf[0].GetDataType()
                        : openApiParameter.GetDataType();
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
                    dataTypeForList = "List";
                }

                if (openApiParameter.Nullable)
                {
                    dataType += "?";
                }

                var defaultValue = GetDefaultValue(openApiParameter.Default, dataTypeForList);

                if (dataType.Equals(dataTypeForList, StringComparison.Ordinal))
                {
                    dataTypeForList = "List";
                }

                propertiesParameters.Add(
                    new PropertyParameters(
                        documentationTags,
                        Attributes: ExtractAttributeParameters(
                            apiSchemaModel.Required,
                            apiSchema.Key,
                            hasAnyPropertiesAsArrayWithFormatTypeBinary,
                            openApiParameter),
                        AccessModifier: AccessModifiers.Public,
                        GenericTypeName: dataTypeForList,
                        IsGenericListType: !string.IsNullOrEmpty(dataTypeForList),
                        TypeName: dataType,
                        IsReferenceType: !isSimpleType,
                        Name: apiSchema.Key.EnsureFirstCharacterToUpper(),
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
}