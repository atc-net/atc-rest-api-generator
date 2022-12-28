namespace Atc.Rest.ApiGenerator.OpenApi.Extensions;

public static class OpenApiOperationExtensions
{
    public static CodeDocumentationTags ExtractDocumentationTagsForInterface(
        this OpenApiOperation apiOperation)
        => apiOperation.ExtractDocumentationTags("Domain Interface for RequestHandler.");

    public static CodeDocumentationTags ExtractDocumentationTagsForHandler(
        this OpenApiOperation apiOperation)
        => apiOperation.ExtractDocumentationTags("Handler for operation request.");

    public static CodeDocumentationTags ExtractDocumentationTagsForParameters(
        this OpenApiOperation apiOperation)
        => apiOperation.ExtractDocumentationTags("Parameters for operation request.");

    public static CodeDocumentationTags ExtractDocumentationTagsForResult(
        this OpenApiOperation apiOperation)
        => apiOperation.ExtractDocumentationTags("Results for operation request.");

    public static CodeDocumentationTags ExtractDocumentationTags(
        this OpenApiOperation apiOperation,
        string? firstSummaryLine = null)
    {
        var summary = ContentGeneratorConstants.UndefinedDescription;

        if (!string.IsNullOrEmpty(apiOperation.Summary))
        {
            summary = apiOperation.Summary;
        }
        else if (!string.IsNullOrEmpty(apiOperation.Description))
        {
            summary = apiOperation.Description;
        }

        var sbSummary = new StringBuilder();

        if (!string.IsNullOrEmpty(firstSummaryLine))
        {
            sbSummary.AppendLine(firstSummaryLine);
        }

        sbSummary.AppendLine($"Description: {summary.EnsureEndsWithDot()}");
        sbSummary.AppendLine($"Operation: {apiOperation.GetOperationName()}.");

        return new CodeDocumentationTags(sbSummary.ToString());
    }
}