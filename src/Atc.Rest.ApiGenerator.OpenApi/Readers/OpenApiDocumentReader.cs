namespace Atc.Rest.ApiGenerator.OpenApi.Readers;

public class OpenApiDocumentReader : IOpenApiDocumentReader
{
    public OpenApiDocumentContainer Read(
        FileInfo openApiSpecificationFile)
    {
        var text = File.ReadAllText(openApiSpecificationFile.FullName);

        try
        {
            var openApiStreamReader = new OpenApiStreamReader();
            var document = openApiStreamReader.Read(File.OpenRead(openApiSpecificationFile.FullName), out var diagnostic);

            return new OpenApiDocumentContainer(
                openApiSpecificationFile,
                text,
                document,
                diagnostic);
        }
        catch (Exception ex)
        {
            return new OpenApiDocumentContainer(
                openApiSpecificationFile,
                text,
                ex);
        }
    }
}