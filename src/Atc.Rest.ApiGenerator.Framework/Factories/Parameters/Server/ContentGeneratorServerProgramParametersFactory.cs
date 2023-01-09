namespace Atc.Rest.ApiGenerator.Framework.Factories.Parameters.Server;

public static class ContentGeneratorServerProgramParametersFactory
{
    public static ContentGeneratorServerProgramParameters Create(
        string @namespace)
        => new(@namespace);
}