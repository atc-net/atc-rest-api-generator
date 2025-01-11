namespace Atc.Rest.ApiGenerator.Framework.Factories;

public static class NamespaceFactory
{
    public static string Create(
        params string[] values)
    {
        if (values is null)
        {
            return string.Empty;
        }

        var fullNamespace = string
            .Join(' ', values)
            .Replace(" . ", " ", StringComparison.Ordinal);

        return fullNamespace.EnsureNamespaceFormat();
    }
}