namespace Atc.Rest.ApiGenerator.OpenApi.Interfaces;

public interface IApiOperationSchemaMapExtractor
{
    IList<ApiOperationSchemaMap> Extract(
        OpenApiDocument apiDocument);
}