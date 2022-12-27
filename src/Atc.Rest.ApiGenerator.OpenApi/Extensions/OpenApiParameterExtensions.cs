namespace Atc.Rest.ApiGenerator.OpenApi.Extensions;

public static class OpenApiParameterExtensions
{
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

        return summery.EnsureEndsWithDot();
    }

    public static string? ExtractDocumentationTagRemark(
        this OpenApiParameter apiParameter)
        => apiParameter.Schema.ExtractDocumentationTagRemark();
}