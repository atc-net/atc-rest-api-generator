namespace Atc.Rest.ApiGenerator.Contracts.ContentGeneratorsParameters.Server;

public record ContentGeneratorServerSwaggerDocOptionsParameters(
    string Namespace,
    SwaggerDocOptionsParameters SwaggerDocOptions);

public record SwaggerDocOptionsParameters(
    string? Version,
    string? Title,
    string? Description,
    string? ContactName,
    string? ContactEmail,
    string? ContactUrl,
    string? TermsOfService,
    string? LicenseName,
    string? LicenseUrl);