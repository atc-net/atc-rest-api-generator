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

        if (value.Contains('\n', StringComparison.Ordinal))
        {
            var lines = value.ToLines();
            var sb = new StringBuilder();
            foreach (var line in lines)
            {
                var trimmedLine = line.Trim();
                if (trimmedLine.Length > 0)
                {
                    sb.AppendLine(trimmedLine);
                }
            }

            value = sb.ToString();
        }
        else
        {
            value = value.Trim();
        }

        if (value.EndsWith(Environment.NewLine, StringComparison.Ordinal))
        {
            value = value[..^Environment.NewLine.Length];
        }

        return value.EnsureEndsWithDot();
    }
}