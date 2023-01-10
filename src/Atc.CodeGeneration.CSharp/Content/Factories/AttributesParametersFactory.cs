namespace Atc.CodeGeneration.CSharp.Content.Factories;

public static class AttributesParametersFactory
{
    public static IList<AttributeParameters> Create(
        string name)
        => new List<AttributeParameters>
        {
            new(name, Content: null),
        };

    public static IList<AttributeParameters> Create(
        string name,
        string content)
        => new List<AttributeParameters>
        {
            new(name, content),
        };
}