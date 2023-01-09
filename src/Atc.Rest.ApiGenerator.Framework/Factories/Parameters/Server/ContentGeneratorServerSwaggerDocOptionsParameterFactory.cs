namespace Atc.Rest.ApiGenerator.Framework.Factories.Parameters.Server;

public static class ContentGeneratorServerSwaggerDocOptionsParameterFactory
{
    public static ContentGeneratorServerSwaggerDocOptionsParameters Create(
        string @namespace,
        SwaggerDocOptionsParameters swaggerDocOptions)
        => new(
            @namespace,
            swaggerDocOptions);
}