namespace Atc.Rest.ApiGenerator.OpenApi.Extractors;

public interface IValidationAttributeExtractor
{
    IList<ValidationAttribute> Extract(
        OpenApiSchema openApiSchema);
}