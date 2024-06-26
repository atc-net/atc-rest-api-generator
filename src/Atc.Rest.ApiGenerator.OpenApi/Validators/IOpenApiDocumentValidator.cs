namespace Atc.Rest.ApiGenerator.OpenApi.Validators;

public interface IOpenApiDocumentValidator
{
    bool IsValid(
        ApiOptionsValidation apiOptionsValidation,
        bool includeDeprecated,
        OpenApiDocumentContainer apiDocumentContainer);
}