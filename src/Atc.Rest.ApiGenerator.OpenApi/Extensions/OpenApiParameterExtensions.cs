namespace Atc.Rest.ApiGenerator.OpenApi.Extensions;

public static class OpenApiParameterExtensions
{
    public static bool IsSchemaEnumAndUsesJsonString(
        this OpenApiParameter apiParameter)
    {
        if (apiParameter.Schema.IsSchemaEnum())
        {
            foreach (var apiAny in apiParameter.Schema.Enum)
            {
                if (apiAny is not OpenApiString openApiString)
                {
                    continue;
                }

                if ((!apiParameter.Schema.Type.Equals("string", StringComparison.Ordinal) && openApiString.Value.IsFirstCharacterLowerCase()) ||
                    openApiString.Value.Contains('-', StringComparison.Ordinal))
                {
                    return true;
                }
            }
        }

        return false;
    }

    public static bool ContainsEnumInSchemaOrProperties(
        this OpenApiParameter apiParameter)
        => apiParameter.Schema.IsSchemaEnumOrPropertyEnum() ||
           apiParameter.Schema.AllOf.Any(allOfSchema => allOfSchema.IsSchemaEnumOrPropertyEnum()) ||
           apiParameter.Schema.OneOf.Any(oneOfSchema => oneOfSchema.IsSchemaEnumOrPropertyEnum()) ||
           apiParameter.Schema.AnyOf.Any(anyOfSchema => anyOfSchema.IsSchemaEnumOrPropertyEnum());

    public static CodeDocumentationTags ExtractDocumentationTags(
        this OpenApiParameter apiParameter)
    {
        return new CodeDocumentationTags(
            summary: apiParameter.ExtractDocumentationTagSummary(),
            parameters: null,
            remark: apiParameter.ExtractDocumentationTagRemark(),
            code: null,
            example: null,
            exceptions: null,
            @return: null);
    }

    public static string ExtractDocumentationTagSummary(
        this OpenApiParameter apiParameter)
    {
        var summery = ContentGeneratorConstants.UndefinedDescription;

        if (!string.IsNullOrEmpty(apiParameter.Description))
        {
            summery = apiParameter.Description;
        }

        return summery.EnsureFormatForDocumentationTag();
    }

    public static string? ExtractDocumentationTagRemark(
        this OpenApiParameter apiParameter)
        => apiParameter.Schema.ExtractDocumentationTagRemark();
}