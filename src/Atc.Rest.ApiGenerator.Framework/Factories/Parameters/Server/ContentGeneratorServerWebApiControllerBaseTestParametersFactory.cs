namespace Atc.Rest.ApiGenerator.Framework.Factories.Parameters.Server;

public static class ContentGeneratorServerWebApiControllerBaseTestParametersFactory
{
    public static ContentGeneratorServerWebApiControllerBaseTestParameters Create(
        string @namespace)
        => new(@namespace);
}