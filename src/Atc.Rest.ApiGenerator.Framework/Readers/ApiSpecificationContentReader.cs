namespace Atc.Rest.ApiGenerator.Framework.Readers;

public class ApiSpecificationContentReader : IApiSpecificationContentReader
{
    [SuppressMessage("Microsoft.Reliability", "CA2000:DisposeObjectsBeforeLosingScope", Justification = "OK.")]
    public OpenApiDocumentContainer CombineAndGetApiDocumentContainer(
        ILogger logger,
        string specificationPath)
    {
        ArgumentNullException.ThrowIfNull(specificationPath);

        FileInfo? apiDocFile;
        if (specificationPath.EndsWith(".yaml", StringComparison.Ordinal))
        {
            apiDocFile = specificationPath.StartsWith("http", StringComparison.CurrentCultureIgnoreCase)
                ? HttpClientHelper.DownloadToTempFile(
                    logger,
                    specificationPath)
                : new FileInfo(specificationPath);

            if (apiDocFile is null ||
                !apiDocFile.Exists)
            {
                throw new IOException("Api yaml file don't exist.");
            }
        }
        else if (specificationPath.EndsWith(".json", StringComparison.Ordinal))
        {
            apiDocFile = specificationPath.StartsWith("http", StringComparison.CurrentCultureIgnoreCase)
                ? HttpClientHelper.DownloadToTempFile(logger, specificationPath)
                : new FileInfo(specificationPath);

            if (apiDocFile is null ||
                !apiDocFile.Exists)
            {
                throw new IOException("Api json file don't exist.");
            }
        }
        else
        {
            // Find all yaml files, except files starting with '.' as example '.spectral.yaml'
            var docFiles = Directory
                .GetFiles(specificationPath, "*.yaml", SearchOption.AllDirectories)
                .Where(x => !x.Contains("\\.", StringComparison.Ordinal))
                .ToArray();

            if (docFiles.Length == 0)
            {
                // No yaml, then try all json files
                docFiles = Directory
                    .GetFiles(specificationPath, "*.json", SearchOption.AllDirectories)
                    .Where(x => !x.Contains("\\.", StringComparison.Ordinal))
                    .ToArray();
            }

            apiDocFile = docFiles.Length switch
            {
                0 => throw new IOException("Api specification file don't exist in folder."),
                1 => new FileInfo(docFiles.First()),
                _ => CreateCombineApiDocumentFile(specificationPath),
            };
        }

        if (apiDocFile is not { Exists: true })
        {
            throw new IOException("Api specification file don't exist");
        }

        var openApiDocumentReader = new OpenApiDocumentReader();
        return openApiDocumentReader.Read(apiDocFile);
    }

    [SuppressMessage("Minor Code Smell", "S1481:Unused local variables should be removed", Justification = "OK for now.")]
    private static FileInfo? CreateCombineApiDocumentFile(
        string specificationPath)
    {
        var openApiDocs = GetAllApiDocuments(new DirectoryInfo(specificationPath));

        // TODO: Combine all yaml files into 1
        throw new NotImplementedException();
    }

    [SuppressMessage("Microsoft.Reliability", "CA2000:DisposeObjectsBeforeLosingScope", Justification = "OK.")]
    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "OK.")]
    private static List<OpenApiDocumentContainer> GetAllApiDocuments(
        DirectoryInfo specificationPath)
    {
        ArgumentNullException.ThrowIfNull(specificationPath);

        var result = new List<OpenApiDocumentContainer>();
        var docFiles = Directory.GetFiles(specificationPath.FullName, "*.yaml", SearchOption.AllDirectories);
        if (docFiles.Length == 0)
        {
            docFiles = Directory.GetFiles(specificationPath.FullName, "*.json", SearchOption.AllDirectories);
        }

        foreach (var docFile in docFiles)
        {
            var text = File.ReadAllText(docFile);
            try
            {
                var openApiStreamReader = new OpenApiStreamReader();
                var document = openApiStreamReader.Read(File.OpenRead(docFile), out var diagnostic);
                result.Add(
                    new OpenApiDocumentContainer(
                        new FileInfo(docFile),
                        text,
                        document,
                        diagnostic));
            }
            catch (Exception ex)
            {
                result.Add(
                    new OpenApiDocumentContainer(
                        new FileInfo(docFile),
                        text,
                        ex));
            }
        }

        return result;
    }
}