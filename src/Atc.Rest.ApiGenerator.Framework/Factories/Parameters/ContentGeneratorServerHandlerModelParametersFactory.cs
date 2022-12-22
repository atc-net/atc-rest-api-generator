// ReSharper disable MergeIntoPattern
namespace Atc.Rest.ApiGenerator.Framework.Factories.Parameters;

public static class ContentGeneratorServerHandlerModelParametersFactory
{
    public static ContentGeneratorServerHandlerModelParameters Create(
        string @namespace,
        string modelName,
        OpenApiSchema apiSchemaModel)
    {
        ArgumentNullException.ThrowIfNull(apiSchemaModel);

        var parameters = new List<ContentGeneratorServerHandlerModelParametersProperty>();

        var hasAnyPropertiesAsArrayWithFormatTypeBinary = apiSchemaModel.HasAnyPropertiesAsArrayWithFormatTypeBinary();

        if (apiSchemaModel.Properties.Count == 0)
        {
            var childModelName = apiSchemaModel.Items.Title.EnsureFirstCharacterToUpper();

            parameters.Add(new ContentGeneratorServerHandlerModelParametersProperty(
                "#",
                childModelName + "List",
                $"A list of {childModelName}.",
                childModelName,
                IsSimpleType: false,
                UseListForDataType: true,
                IsNullable: false,
                IsRequired: false,
                new List<ValidationAttribute>(),
                $"new List<{childModelName}>()"));
        }
        else
        {
            foreach (var apiSchema in apiSchemaModel.Properties)
            {
                var openApiParameter = apiSchema.Value;

                var useListForDataType = openApiParameter.IsTypeArray();

                var dataType = useListForDataType
                    ? openApiParameter.Items.GetDataType()
                    : openApiParameter.GetDataType();

                var isSimpleType = useListForDataType
                    ? openApiParameter.Items.IsSimpleDataType() || openApiParameter.Items.IsSchemaEnumOrPropertyEnum() || openApiParameter.Items.IsFormatTypeBinary()
                    : openApiParameter.IsSimpleDataType() || openApiParameter.IsSchemaEnumOrPropertyEnum() || openApiParameter.IsFormatTypeBinary();

                string? dataTypeForList = null;
                string description;
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
                else
                {
                    description = openApiParameter.GetSummaryDescription();
                }

                if (dataTypeForList is null &&
                    openApiParameter.Default is null &&
                    useListForDataType &&
                    !GetRequired(apiSchemaModel.Required, apiSchema.Key, hasAnyPropertiesAsArrayWithFormatTypeBinary))
                {
                    dataTypeForList = dataType;
                }

                parameters.Add(new ContentGeneratorServerHandlerModelParametersProperty(
                    apiSchema.Key,
                    apiSchema.Key.EnsureFirstCharacterToUpper(),
                    description,
                    dataType,
                    isSimpleType,
                    useListForDataType,
                    openApiParameter.Nullable,
                    GetRequired(apiSchemaModel.Required, apiSchema.Key, hasAnyPropertiesAsArrayWithFormatTypeBinary),
                    GetAdditionalValidationAttributes(openApiParameter),
                    GetDefaultValue(openApiParameter.Default, dataTypeForList)));
            }
        }

        return new ContentGeneratorServerHandlerModelParameters(
            @namespace,
            modelName,
            apiSchemaModel.GetSummaryDescription(),
            parameters);
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
        if (initializer is null &&
            !string.IsNullOrEmpty(dataTypeForList))
        {
            return $"new List<{dataTypeForList}>()";
        }

        if (initializer is null)
        {
            return null;
        }

        return initializer switch
        {
            OpenApiInteger apiInteger => apiInteger.Value.ToString(),
            OpenApiString apiString => string.IsNullOrEmpty(apiString.Value) ? "string.Empty" : $"\"{apiString.Value}\"",
            OpenApiBoolean apiBoolean when apiBoolean.Value => "true",
            OpenApiBoolean apiBoolean when !apiBoolean.Value => "false",
            _ => throw new NotImplementedException("Property initializer: " + initializer.GetType()),
        };
    }

    private static IList<ValidationAttribute> GetAdditionalValidationAttributes(
        OpenApiSchema openApiSchema)
    {
        var validationAttributeExtractor = new ValidationAttributeExtractor();
        return validationAttributeExtractor.Extract(openApiSchema);
    }
}