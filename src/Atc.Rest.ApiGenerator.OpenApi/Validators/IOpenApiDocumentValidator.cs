namespace Atc.Rest.ApiGenerator.OpenApi.Validators;

public interface IOpenApiDocumentValidator
{
    bool IsValid(
        ApiOptionsValidation apiOptionsValidation,
        OpenApiDocumentContainer apiDocumentContainer);
}