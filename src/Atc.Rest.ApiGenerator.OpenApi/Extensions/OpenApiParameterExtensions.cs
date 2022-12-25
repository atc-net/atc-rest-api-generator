namespace Atc.Rest.ApiGenerator.OpenApi.Extensions;

public static class OpenApiParameterExtensions
{
    public static CodeDocumentationTags ExtractDocumentationTags(
        this OpenApiParameter apiParameter)
    {
        return new CodeDocumentationTags(
            Summary: apiParameter.ExtractDocumentationTagSummary(),
            Parameters: null,
            Remark: apiParameter.ExtractDocumentationTagRemark(),
            Example: null,
            Exception: null,
            Return: null);
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