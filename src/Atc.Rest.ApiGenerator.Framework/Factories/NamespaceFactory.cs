namespace Atc.Rest.ApiGenerator.Framework.Factories;

public static class NamespaceFactory
{
    public static string CreateFull(
        string projectName,
        params string[] subNamespaces)
    {
        var fullNamespace = string.Join(' ', new[] { projectName }.Concat(subNamespaces));

        return Create(fullNamespace);
    }

    public static string Create(
        params string[] subNamespaces)
    {
        var fullNamespace = string
            .Join(' ', subNamespaces)
            .Replace(" . ", " ", StringComparison.Ordinal);

        return fullNamespace.EnsureNamespaceFormat();
    }
}