namespace Atc.Rest.ApiGenerator.OpenApi.Extensions;

public static class OpenApiDocumentExtensions
{
    public static SwaggerDocOptionsParameters ToSwaggerDocOptionsParameters(
        this OpenApiDocument openApiDocument)
    {
        ArgumentNullException.ThrowIfNull(openApiDocument);

        return new SwaggerDocOptionsParameters(
            openApiDocument.Info?.Version,
            openApiDocument.Info?.Title,
            openApiDocument.Info?.Description,
            openApiDocument.Info?.Contact?.Name,
            openApiDocument.Info?.Contact?.Email,
            openApiDocument.Info?.Contact?.Url?.ToString(),
            openApiDocument.Info?.TermsOfService?.ToString(),
            openApiDocument.Info?.License?.Name,
            openApiDocument.Info?.License?.Url?.ToString());
    }
}