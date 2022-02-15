// ReSharper disable ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
// ReSharper disable InvertIf
// ReSharper disable LoopCanBeConvertedToQuery
// ReSharper disable ReturnTypeCanBeEnumerable.Local
namespace Atc.Rest.ApiGenerator.Helpers;

public static class OpenApiDocumentHelper
{
    [SuppressMessage("Microsoft.Reliability", "CA2000:DisposeObjectsBeforeLosingScope", Justification = "OK.")]
    public static Tuple<OpenApiDocument, OpenApiDiagnostic, FileInfo> CombineAndGetApiDocument(
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
                _ => CreateCombineApiDocumentFile(specificationPath)
            };
        }

        if (apiDocFile is not { Exists: true })
        {
            throw new IOException("Api specification file don't exist");
        }

        var openApiStreamReader = new OpenApiStreamReader();
        var openApiDocument = openApiStreamReader.Read(File.OpenRead(apiDocFile.FullName), out var diagnostic);
        return new Tuple<OpenApiDocument, OpenApiDiagnostic, FileInfo>(openApiDocument, diagnostic, new FileInfo(apiDocFile.FullName));
    }

    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:Parameter should not span multiple lines", Justification = "OK.")]
    public static bool Validate(
        ILogger logger,
        Tuple<OpenApiDocument, OpenApiDiagnostic, FileInfo> apiDocument,
        ApiOptionsValidation validationOptions)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(apiDocument);
        ArgumentNullException.ThrowIfNull(validationOptions);

        logger.LogInformation($"{AppEmojisConstants.AreaValidation} Working on validation");

        if (apiDocument.Item2.Errors.Count > 0)
        {
            var validationErrors = apiDocument.Item2.Errors
                .Where(e => !e.Message.EndsWith(
                    "#/components/schemas",
                    StringComparison.Ordinal))
                .Select(e => LogItemHelper.Create(
                    LogCategoryType.Error,
                    ValidationRuleNameConstants.OpenApiCore,
                    string.IsNullOrEmpty(e.Pointer)
                        ? $"{e.Message}"
                        : $"{e.Message} <#> {e.Pointer}"))
                .ToList();

            logger.LogKeyValueItems(validationErrors);
            return false;
        }

        if (apiDocument.Item2.SpecificationVersion == OpenApiSpecVersion.OpenApi2_0)
        {
            logger.LogError("OpenApi 2.x is not supported.");
            return false;
        }

        return OpenApiDocumentValidationHelper.ValidateDocument(logger, apiDocument.Item1, validationOptions);
    }

    public static List<string> GetBasePathSegmentNames(
        OpenApiDocument openApiDocument)
    {
        ArgumentNullException.ThrowIfNull(openApiDocument);

        var names = new List<string>();
        if (openApiDocument.Paths?.Keys is null)
        {
            return names.ToList();
        }

        foreach (var name in openApiDocument.Paths.Keys
                     .Select(x => x.Split('/', StringSplitOptions.RemoveEmptyEntries))
                     .Where(sa => sa.Length != 0)
                     .Select(sa => sa[0].ToLower(CultureInfo.CurrentCulture)).Where(name => !names.Contains(name, StringComparer.Ordinal)))
        {
            names.Add(name);
        }

        return names
            .Select(x => x.PascalCase(true))
            .OrderBy(x => x)
            .ToList()!;
    }

    [SuppressMessage("Design", "CA1055:URI-like return values should not be strings", Justification = "OK.")]
    public static string GetServerUrlBasePath(
        OpenApiDocument openApiDocument)
    {
        ArgumentNullException.ThrowIfNull(openApiDocument);

        var serverUrl = openApiDocument.Servers?.FirstOrDefault()?.Url;
        if (string.IsNullOrWhiteSpace(serverUrl))
        {
            return "/api/v1";
        }

        serverUrl = serverUrl.Replace("//", "/", StringComparison.Ordinal);
        serverUrl = serverUrl.Replace("http:/", "http://", StringComparison.OrdinalIgnoreCase);
        serverUrl = serverUrl.Replace("https:/", "https://", StringComparison.OrdinalIgnoreCase);
        if (!serverUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase) &&
            !serverUrl.StartsWith("/", StringComparison.Ordinal))
        {
            serverUrl = $"/{serverUrl}";
        }

        if (serverUrl.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
            serverUrl.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
        {
            var temp = serverUrl
                .Replace("http://", string.Empty, StringComparison.OrdinalIgnoreCase)
                .Replace("https://", string.Empty, StringComparison.OrdinalIgnoreCase);

            return temp.Substring(temp.IndexOf('/', StringComparison.Ordinal));
        }

        return serverUrl;
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

    [SuppressMessage("Minor Code Smell", "S1481:Unused local variables should be removed", Justification = "OK for now.")]
    private static FileInfo? CreateCombineApiDocumentFile(
        string specificationPath)
    {
        var openApiDocs = GetAllApiDocuments(new DirectoryInfo(specificationPath));

        // TODO: Combine all yaml files into 1
        throw new NotImplementedException();
    }
}