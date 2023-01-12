namespace Atc.CodeGeneration.CSharp.Content.Factories;

public static class EnumValueParametersFactory
{
    public static EnumValueParameters Create(
        string name)
        => new(
            DocumentationTags: null,
            DescriptionAttribute: null,
            Name: name,
            Value: null);

    public static EnumValueParameters Create(
        string name,
        int? value)
        => new(
            DocumentationTags: null,
            DescriptionAttribute: null,
            Name: name,
            Value: value);
}