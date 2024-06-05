// ReSharper disable InvertIf
namespace Atc.Rest.ApiGenerator.OpenApi.Extensions;

public static class OpenApiSchemaExtensions
{
    public static string? GetDefaultValueAsString(
        this OpenApiSchema apiSchema)
        => apiSchema.Default.GetDefaultValueAsString();

    public static ParameterLocationType GetParameterLocationType(
        this OpenApiSchema apiSchema,
        ParameterLocationType defaultParameterLocationType = ParameterLocationType.Body)
    {
        var parameterLocationType = defaultParameterLocationType;
        if (apiSchema.IsTypeArray())
        {
            if (apiSchema.HasItemsWithFormatTypeBinary())
            {
                parameterLocationType = ParameterLocationType.Form;
            }
        }
        else
        {
            if (apiSchema.HasAnyPropertiesWithFormatTypeBinary() ||
                apiSchema.HasAnyPropertiesAsArrayWithFormatTypeBinary())
            {
                parameterLocationType = ParameterLocationType.Form;
            }
        }

        return parameterLocationType;
    }

    public static IDictionary<string, OpenApiSchema> GetPaginationParameters(
        this OpenApiSchema apiSchema)
    {
        if (!apiSchema.IsTypePagination())
        {
            return new Dictionary<string, OpenApiSchema>(StringComparer.Ordinal);
        }

        var apiSchemaPaging = NameConstants.Pagination.Equals(apiSchema.AllOf[0].Reference?.Id, StringComparison.OrdinalIgnoreCase)
            ? apiSchema.AllOf[0]
            : apiSchema.AllOf[1];

        return apiSchemaPaging.Properties;
    }

    public static OpenApiSchema? GetCustomPaginationSchema(
        this OpenApiSchema apiSchema)
    {
        if (!apiSchema.IsTypeCustomPagination())
        {
            return null;
        }

        return apiSchema.AllOf[0].Reference?.Id is not null
            ? apiSchema.AllOf[0]
            : apiSchema.AllOf[1];
    }

    public static OpenApiSchema? GetCustomPaginationItemsSchema(
        this OpenApiSchema apiSchema)
    {
        if (!apiSchema.IsTypeCustomPagination())
        {
            return null;
        }

        var apiSchemaForItems = apiSchema.AllOf[0].Reference?.Id is null
            ? apiSchema.AllOf[0]?.Properties?.FirstOrDefault()
            : apiSchema.AllOf[1]?.Properties?.FirstOrDefault();

        if (apiSchemaForItems is null ||
            string.IsNullOrEmpty(apiSchemaForItems.Value.Key) ||
            apiSchemaForItems.Value.Value is null)
        {
            return null;
        }

        return apiSchemaForItems.Value.Value;
    }

    public static string GetSimpleDataTypeFromCustomPagination(
        this OpenApiSchema schema)
    {
        ArgumentNullException.ThrowIfNull(schema);

        var customPaginationItemsSchema = schema.GetCustomPaginationItemsSchema();
        return customPaginationItemsSchema is null
            ? string.Empty
            : customPaginationItemsSchema.GetDataType();
    }

    public static string? GetCustomPaginationGenericTypeWithItemType(
        this OpenApiSchema schema,
        string projectName,
        string apiGroupName,
        bool isClient = false)
    {
        if (!schema.IsTypeCustomPagination())
        {
            return null;
        }

        var customPaginationSchema = schema.GetCustomPaginationSchema();
        var customPaginationItemsSchema = schema.GetCustomPaginationItemsSchema();
        if (customPaginationSchema is null ||
            customPaginationItemsSchema is null)
        {
            return null;
        }

        string? itemTypeName;
        if (customPaginationItemsSchema.IsSimpleDataType())
        {
            itemTypeName = customPaginationItemsSchema.GetDataType();
        }
        else
        {
            itemTypeName = OpenApiDocumentSchemaModelNameResolver.EnsureModelNameWithNamespaceIfNeeded(
                projectName,
                apiGroupName,
                customPaginationItemsSchema.GetModelName(),
                isShared: false,
                isClient);
        }

        var customPaginationTypeName = customPaginationSchema.GetModelName();
        return $"{customPaginationTypeName}<{itemTypeName}>";
    }

    public static string? GetCollectionDataType(
        this OpenApiSchema schema)
    {
        if (schema.IsArray())
        {
            return NameConstants.List;
        }

        if (schema.IsTypePagination() ||
            schema.IsTypeCustomPagination())
        {
            return NameConstants.Pagination;
        }

        return null;
    }

    public static bool IsModelOfTypeArray(
        this OpenApiSchema apiSchema,
        IDictionary<string, OpenApiSchema> modelSchemas)
    {
        var modelType = apiSchema.GetModelType();
        if (modelType is null &&
            apiSchema.Reference?.Id is not null)
        {
            var (key, value) = modelSchemas.FirstOrDefault(x => x.Key.Equals(apiSchema.Reference.Id, StringComparison.OrdinalIgnoreCase));
            if (key is not null)
            {
                return value.Type is not null &&
                       value.Type.EndsWith(OpenApiDataTypeConstants.Array, StringComparison.OrdinalIgnoreCase);
            }
        }

        return modelType is not null &&
               modelType.EndsWith(OpenApiDataTypeConstants.Array, StringComparison.OrdinalIgnoreCase);
    }

    public static bool IsArray(
        this OpenApiSchema apiSchema)
        => apiSchema.IsTypeArray() &&
           apiSchema.Items?.Reference?.Id is not null;

    public static bool IsTypeCustomPagination(
        this OpenApiSchema schema)
    {
        ArgumentNullException.ThrowIfNull(schema);

        if (schema.AllOf.Count == 2)
        {
            if (schema.AllOf[0].Reference?.Id is null &&
                schema.AllOf[1].Reference?.Id is not null)
            {
                return true;
            }

            if (schema.AllOf[0].Reference?.Id is not null &&
                schema.AllOf[1].Reference?.Id is null)
            {
                return true;
            }
        }

        return false;
    }

    public static CodeDocumentationTags ExtractDocumentationTags(
        this OpenApiSchema apiSchema,
        string? defaultSummary = null)
    {
        return new CodeDocumentationTags(
            summary: apiSchema.ExtractDocumentationTagSummary(defaultSummary),
            parameters: null,
            remark: apiSchema.ExtractDocumentationTagRemark(),
            code: null,
            example: null,
            exceptions: null,
            @return: null);
    }

    public static string ExtractDocumentationTagSummary(
        this OpenApiSchema apiSchema,
        string? defaultSummary = null)
    {
        var summery = ContentGeneratorConstants.UndefinedDescription;

        if (!string.IsNullOrEmpty(apiSchema.Description))
        {
            summery = apiSchema.Description;
        }
        else if (!string.IsNullOrEmpty(apiSchema.Title))
        {
            summery = apiSchema.Title;
        }
        else if (!string.IsNullOrEmpty(defaultSummary))
        {
            summery = defaultSummary;
        }

        return summery.EnsureEndsWithDot();
    }

    public static string? ExtractDocumentationTagRemark(
        this OpenApiSchema apiSchema)
    {
        if (apiSchema.Format is null)
        {
            return null;
        }

        return apiSchema.Format switch
        {
            OpenApiFormatTypeConstants.Byte => "This string should be base64-encoded.",
            OpenApiFormatTypeConstants.Email => "Email validation being enforced.",
            OpenApiFormatTypeConstants.Uri => "Url validation being enforced.",
            _ => null,
        };
    }
}