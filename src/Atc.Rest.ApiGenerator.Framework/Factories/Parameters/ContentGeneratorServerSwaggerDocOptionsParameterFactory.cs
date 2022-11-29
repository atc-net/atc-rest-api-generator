namespace Atc.Rest.ApiGenerator.Framework.Factories.Parameters;

public static class ContentGeneratorServerSwaggerDocOptionsParameterFactory
{
    public static ContentGeneratorServerSwaggerDocOptionsParameters Create(
        string @namespace,
        SwaggerDocOptionsParameters swaggerDocOptions)
    {
        return new ContentGeneratorServerSwaggerDocOptionsParameters(
            @namespace,
            swaggerDocOptions);
    }
}