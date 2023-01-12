namespace Atc.CodeGeneration.CSharp.Content.Factories;

public static class AttributeParametersFactory
{
    public static AttributeParameters Create(
        string name)
        => new(name, Content: null);

    public static AttributeParameters Create(
        string name,
        string content)
        => new(name, content);
}