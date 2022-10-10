namespace Atc.Rest.ApiGenerator.OpenApi.Interfaces;

public interface IApiOperationExtractor
{
    IList<ApiOperation> Extract(
        OpenApiDocument apiDocument);
}