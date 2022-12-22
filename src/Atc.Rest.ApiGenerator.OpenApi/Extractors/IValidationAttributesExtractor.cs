namespace Atc.Rest.ApiGenerator.OpenApi.Extractors;

public interface IValidationAttributesExtractor
{
    IList<ValidationAttribute> Extract(
        OpenApiSchema openApiSchema);
}