namespace Atc.Rest.ApiGenerator.OpenApi.Readers;

public class OpenApiDocumentReader : IOpenApiDocumentReader
{
    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "OK.")]
    public OpenApiDocumentContainer Read(
        FileInfo openApiSpecificationFile)
    {
        var text = File.ReadAllText(openApiSpecificationFile.FullName);

        try
        {
            var openApiStreamReader = new OpenApiStreamReader();
            using var fileStream = File.OpenRead(openApiSpecificationFile.FullName);
            var document = openApiStreamReader.Read(fileStream, out var diagnostic);

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