namespace Atc.Rest.ApiGenerator.Framework.Extensions;

public static class StringExtensions
{
    public static string EnsureNamespaceFormat(
        this string value)
        => string.IsNullOrEmpty(value)
            ? value
            : value
                .Trim()
                .Replace('\\', ' ')
                .Replace('-', ' ')
                .Replace('.', ' ')
                .PascalCase()
                .Replace(' ', '.');

    public static string EnsureNamespaceFormatPart(
        this string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return value;
        }

        if (value.Contains('-', StringComparison.Ordinal))
        {
            return value
                .Trim()
                .PascalCase(removeSeparators: true);
        }

        return value
            .Replace('\\', ' ')
            .Replace('-', ' ')
            .Trim();
    }
}