namespace Atc.Rest.ApiGenerator.Framework.Factories;

public static class NamespaceFactory
{
    public static string Create(
        params string[] values)
    {
        if (values is null || values.Length == 0)
        {
            return string.Empty;
        }

        var formattedValues = values
            .Select((value, index) =>
                index == 0
                    ? value.EnsureNamespaceFormatPart()
                    : value.EnsureNamespaceFormat())
            .Where(value => !string.IsNullOrEmpty(value));

        var fullNamespace = string.Join('.', formattedValues);

        return fullNamespace;
    }
}