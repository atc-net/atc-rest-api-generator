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
        if (!apiSchema.IsPaging())
        {
            return new Dictionary<string, OpenApiSchema>(StringComparer.Ordinal);
        }

        var apiSchemaPaging = NameConstants.Pagination.Equals(apiSchema.AllOf[0].Reference?.Id, StringComparison.OrdinalIgnoreCase)
            ? apiSchema.AllOf[0]
            : apiSchema.AllOf[1];

        return apiSchemaPaging.Properties;
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

    public static bool IsPaging(
        this OpenApiSchema apiSchema)
        => apiSchema.AllOf.Count == 2 &&
           (NameConstants.Pagination.Equals(apiSchema.AllOf[0].Reference?.Id, StringComparison.OrdinalIgnoreCase) ||
            NameConstants.Pagination.Equals(apiSchema.AllOf[1].Reference?.Id, StringComparison.OrdinalIgnoreCase));

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