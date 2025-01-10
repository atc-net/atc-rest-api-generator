namespace Atc.Rest.ApiGenerator.Framework.Factories;

public static class LocationFactory
{
    public static string CreateWithApiGroupName(
        string apiGroupName,
        string value)
    {
        var templateNames = KeyValueItemFactory.CreateTemplateCollectionWithApiGroupName(apiGroupName);

        return Create(
            templateNames,
            value);
    }

    public static string CreateWithoutTemplateForApiGroupName(
        string value)
    {
        if (value is null)
        {
            return string.Empty;
        }

        if (value.Contains(ContentGeneratorConstants.TemplateForApiGroupName, StringComparison.Ordinal))
        {
            value = value
                .Replace(ContentGeneratorConstants.TemplateForApiGroupName, string.Empty, StringComparison.Ordinal)
                .TrimStart('.')
                .TrimEnd('.');
        }

        return value;
    }

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

    public static string Create(
        ICollection<KeyValueItem> templateItems,
        params string[] values)
    {
        if (values is null)
        {
            return string.Empty;
        }

        if (templateItems is not null && templateItems.Count > 0)
        {
            values = AppendMissingTemplateKeysToNamespaceParts(templateItems, values);
        }

        var fullNamespace = string
            .Join(' ', values)
            .Replace(" . ", " ", StringComparison.Ordinal);

        if (templateItems is not null && templateItems.Count > 0)
        {
            fullNamespace = ApplyTemplates(templateItems, fullNamespace);
        }

        return fullNamespace.EnsureNamespaceFormat();
    }

    private static string[] AppendMissingTemplateKeysToNamespaceParts(
        ICollection<KeyValueItem> templateItems,
        string[] namespaceParts)
    {
        var templateKeys = new List<string>();
        foreach (var namespacePart in namespaceParts)
        {
            templateKeys.AddRange(namespacePart.GetTemplateKeys());
        }

        return templateItems
            .Where(x => !templateKeys.Contains(x.Key, StringComparer.Ordinal))
            .Aggregate(
                namespaceParts,
                (current, templateItem) => current
                    .Append(templateItem.Value)
                    .ToArray());
    }

    private static string ApplyTemplates(
        ICollection<KeyValueItem> templateItems,
        string fullNamespace)
    {
        return templateItems.Aggregate(
            fullNamespace,
            (current, templateItem) => current.ReplaceTemplateKeyWithValue(
                templateItem.Key,
                templateItem.Value));
    }
}