namespace Atc.Rest.ApiGenerator.Framework.Factories.Parameters.ServerClient;

public static class AttributeParametersFactory
{
    public static AttributeParameters Create(
        string name)
        => new(name, Content: null);

    public static AttributeParameters Create(
        string name,
        string content)
        => new(name, content);

    public static AttributeParameters CreateGeneratedCode(
        Version apiGeneratorVersion)
        => new(
            "GeneratedCode",
            $"\"{ContentWriterConstants.ApiGeneratorName}\", \"{apiGeneratorVersion}\"");
}