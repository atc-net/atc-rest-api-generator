namespace Atc.Rest.ApiGenerator.Framework.Factories;

public static class KeyValueItemFactory
{
    public static ICollection<KeyValueItem> CreateTemplateCollectionWithApiGroupName(
        string value)
        => new List<KeyValueItem> { new(ContentGeneratorConstants.TemplateKeyForApiGroupName, value) };
}