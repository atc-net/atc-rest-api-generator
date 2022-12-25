namespace Atc.Rest.ApiGenerator.Framework.ContentGenerators;

public class CodeDocumentationTagsGenerator : ICodeDocumentationTagsGenerator
{
    public bool ShouldGenerateTags(
        CodeDocumentationTags codeDocumentationTags)
    {
        ArgumentNullException.ThrowIfNull(codeDocumentationTags);

        if (codeDocumentationTags.Parameters is not null ||
            codeDocumentationTags.Remark is not null ||
            codeDocumentationTags.Exception is not null ||
            codeDocumentationTags.Return is not null)
        {
            return true;
        }

        return !string.IsNullOrEmpty(codeDocumentationTags.Summary) &&
               !codeDocumentationTags.Summary.StartsWith(ContentGeneratorConstants.UndefinedDescription, StringComparison.Ordinal);
    }

    public string GenerateTags(
        ushort indentSpaces,
        CodeDocumentationTags codeDocumentationTags)
    {
        ArgumentNullException.ThrowIfNull(codeDocumentationTags);

        if (!ShouldGenerateTags(codeDocumentationTags))
        {
            return string.Empty;
        }

        var sb = new StringBuilder();
        sb.Append(GenerateSummary(indentSpaces, codeDocumentationTags.Summary));

        //// TODO: Append 'Parameters' if needed.

        if (!string.IsNullOrEmpty(codeDocumentationTags.Remark))
        {
            sb.Append(GenerateRemarks(indentSpaces, codeDocumentationTags.Remark));
        }

        //// TODO: Append 'Exception' if needed.

        if (!string.IsNullOrEmpty(codeDocumentationTags.Return))
        {
            sb.Append(GenerateReturns(indentSpaces, codeDocumentationTags.Return));
        }

        return sb.ToString();
    }

    public string GenerateEndpointClassSummary(
        ushort indentSpaces)
        => GenerateSummary(indentSpaces, "Endpoint definitions.");

    public string GenerateEndpointMethodSummary(
        ushort indentSpaces,
        CodeDocumentationTags codeDocumentationTags)
    {
        ArgumentNullException.ThrowIfNull(codeDocumentationTags);

        return GenerateSummary(
            indentSpaces,
            codeDocumentationTags.Summary);
    }

    public string GenerateResultMethodSummary(
        ushort indentSpaces,
        HttpStatusCode httpStatusCode)
        => GenerateSummary(indentSpaces, $"{(int)httpStatusCode} - {httpStatusCode.ToNormalizedString()} response.");

    public string GenerateHandlerMethodTags(
        ushort indentSpaces,
        string? parameterName)
    {
        var sb = new StringBuilder();
        sb.Append(GenerateSummary(indentSpaces, "Execute method."));
        if (!string.IsNullOrEmpty(parameterName))
        {
            sb.AppendLine(indentSpaces, "/// <param name=\"parameters\">The parameters.</param>");
        }

        sb.AppendLine(indentSpaces, "/// <param name=\"cancellationToken\">The cancellation token.</param>");
        return sb.ToString();
    }

    private static string GenerateSummary(
        ushort indentSpaces,
        string value)
        => GenerateTag(indentSpaces, "summary", value);

    private static string GenerateRemarks(
        ushort indentSpaces,
        string value)
        => GenerateTag(indentSpaces, "remarks", value);

    private static string GenerateReturns(
        ushort indentSpaces,
        string value)
        => GenerateTag(indentSpaces, "returns", value);

    private static string GenerateTag(
        ushort indentSpaces,
        string tag,
        string value)
    {
        var sb = new StringBuilder();
        sb.AppendLine(indentSpaces, $"/// <{tag}>");

        var lines = value
            .EnsureEnvironmentNewLines()
            .Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

        foreach (var line in lines)
        {
            sb.AppendLine(indentSpaces, $"/// {line}");
        }

        sb.AppendLine(indentSpaces, $"/// </{tag}>");
        return sb.ToString();
    }
}