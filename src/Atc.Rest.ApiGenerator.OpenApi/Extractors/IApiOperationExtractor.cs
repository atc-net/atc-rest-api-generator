namespace Atc.Rest.ApiGenerator.OpenApi.Extractors;

public interface IApiOperationExtractor
{
    IList<ApiOperation> Extract(
        OpenApiDocument apiDocument);
}