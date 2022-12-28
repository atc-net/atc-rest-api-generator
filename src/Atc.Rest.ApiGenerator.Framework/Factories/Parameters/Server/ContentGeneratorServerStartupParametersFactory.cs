namespace Atc.Rest.ApiGenerator.Framework.Factories.Parameters.Server;

public static class ContentGeneratorServerStartupParametersFactory
{
    public static ContentGeneratorServerStartupParameters Create(
        string @namespace)
        => new(@namespace);
}