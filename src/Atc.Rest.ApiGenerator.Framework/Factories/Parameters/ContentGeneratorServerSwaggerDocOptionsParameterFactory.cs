namespace Atc.Rest.ApiGenerator.Framework.Factories.Parameters;

public static class ContentGeneratorServerSwaggerDocOptionsParameterFactory
{
    public static ContentGeneratorServerSwaggerDocOptionsParameters Create(
        Version apiGeneratorVersion,
        string @namespace,
        SwaggerDocOptionsParameters swaggerDocOptions)
    {
        return new ContentGeneratorServerSwaggerDocOptionsParameters(
            apiGeneratorVersion,
            @namespace,
            swaggerDocOptions);
    }
}