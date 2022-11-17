namespace Atc.Rest.ApiGenerator.OpenApi.Readers;

public interface IOpenApiDocumentReader
{
    OpenApiDocumentContainer Read(
        FileInfo openApiSpecificationFile);
}