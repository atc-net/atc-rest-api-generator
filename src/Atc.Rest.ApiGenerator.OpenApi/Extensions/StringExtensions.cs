namespace Atc.Rest.ApiGenerator.OpenApi.Extensions;

public static class StringExtensions
{
    public static bool IsNamedAsItemsOrResult(
        this string value)
        => value.Equals("items", StringComparison.Ordinal) ||
           value.Equals("results", StringComparison.Ordinal);
}