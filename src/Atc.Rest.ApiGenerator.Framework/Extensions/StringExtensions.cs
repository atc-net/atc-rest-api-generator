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
}