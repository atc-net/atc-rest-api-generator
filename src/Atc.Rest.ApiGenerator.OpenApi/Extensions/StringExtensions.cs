namespace Atc.Rest.ApiGenerator.OpenApi.Extensions;

public static class StringExtensions
{
    public static bool IsNamedAsItemsOrResult(
        this string value)
        => value.Equals("items", StringComparison.Ordinal) ||
           value.Equals("results", StringComparison.Ordinal);

    public static string EnsureFormatForDocumentationTag(
        this string value)
    {
        if (value.Contains("</br>", StringComparison.OrdinalIgnoreCase))
        {
            value = value
                .EnsureEndsWithDot()
                .Replace("</br>", "<br />", StringComparison.OrdinalIgnoreCase);
        }

        if (value.Contains('<', StringComparison.Ordinal) ||
            value.Contains('>', StringComparison.Ordinal))
        {
            value = value
                .Replace("<", "&lt;", StringComparison.OrdinalIgnoreCase)
                .Replace(">", "&gt;", StringComparison.OrdinalIgnoreCase);
        }

        while (value.Contains("  ", StringComparison.Ordinal))
        {
            value = value.Replace("  ", " ", StringComparison.Ordinal);
        }

        while (value.Contains("\n ", StringComparison.Ordinal))
        {
            value = value.Replace("\n ", "\n", StringComparison.Ordinal);
        }

        if (value.EndsWith("\n", StringComparison.Ordinal))
        {
            value = value[..^1];
        }

        return value.EnsureEndsWithDot();
    }
}