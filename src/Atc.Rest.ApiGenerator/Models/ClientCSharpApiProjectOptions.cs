namespace Atc.Rest.ApiGenerator.Models;

public class ClientCSharpApiProjectOptions
{
    public ClientCSharpApiProjectOptions(
        DirectoryInfo projectSrcGeneratePath,
        string? clientFolderName,
        OpenApiDocument openApiDocument,
        FileInfo openApiDocumentFile,
        string projectPrefixName,
        string projectSuffixName,
        string? httpClientName,
        bool excludeEndpointGeneration,
        ApiOptions apiOptions,
        bool usingCodingRules)
    {
        ArgumentNullException.ThrowIfNull(projectSrcGeneratePath);

        ProjectName = projectPrefixName ?? throw new ArgumentNullException(nameof(projectPrefixName));
        PathForSrcGenerate = projectSrcGeneratePath.Name.StartsWith(ProjectName, StringComparison.OrdinalIgnoreCase)
            ? projectSrcGeneratePath.Parent!
            : projectSrcGeneratePath;

        ForClient = true;
        ClientFolderName = clientFolderName;
        Document = openApiDocument ?? throw new ArgumentNullException(nameof(openApiDocument));
        DocumentFile = openApiDocumentFile ?? throw new ArgumentNullException(nameof(openApiDocumentFile));

        ApiGeneratorName = "ApiGenerator";

        // TODO: Cleanup this temp. re-write hack
        Version? apiGeneratorVersion = default;
        TaskHelper.RunSync(async () =>
        {
            apiGeneratorVersion = await new NugetPackageReferenceProvider(
                    new AtcApiNugetClient(NullLogger<AtcApiNugetClient>.Instance))
                .GetAtcApiGeneratorVersion();
        });

        ApiGeneratorVersion = apiGeneratorVersion!;

        ApiOptions = apiOptions;
        ProjectName = projectPrefixName
            .Replace(" ", ".", StringComparison.Ordinal)
            .Replace("-", ".", StringComparison.Ordinal)
            .Trim() + $".{projectSuffixName}";

        HttpClientName = httpClientName ?? $"{projectPrefixName}-ApiClient";

        PathForSrcGenerate = new DirectoryInfo(Path.Combine(PathForSrcGenerate.FullName, ProjectName));
        ProjectSrcCsProj = new FileInfo(Path.Combine(PathForSrcGenerate.FullName, $"{ProjectName}.csproj"));

        ApiGroupNames = openApiDocument.GetApiGroupNames();

        UsingCodingRules = usingCodingRules;
        ExcludeEndpointGeneration = excludeEndpointGeneration;
    }

    public bool UsingCodingRules { get; }

    public bool UseNullableReferenceTypes { get; } = true;

    public string ApiGeneratorName { get; }

    public Version ApiGeneratorVersion { get; }

    public string ApiGeneratorNameAndVersion => $"{ApiGeneratorName} {ApiGeneratorVersion}";

    public ApiOptions ApiOptions { get; }

    public DirectoryInfo PathForSrcGenerate { get; }

    public FileInfo ProjectSrcCsProj { get; }

    public string ProjectSrcCsProjDisplayLocation
        => ProjectSrcCsProj.FullName.Replace(PathForSrcGenerate.FullName, "src: ", StringComparison.Ordinal);

    public bool ForClient { get; }

    public string? ClientFolderName { get; }

    public OpenApiDocument Document { get; }

    public FileInfo DocumentFile { get; }

    public string ProjectName { get; }

    public string HttpClientName { get; set; }

    public IList<string> ApiGroupNames { get; }

    public bool ExcludeEndpointGeneration { get; }

    public override string ToString()
        => $"{nameof(ApiGeneratorName)}: {ApiGeneratorName}, {nameof(ApiGeneratorVersion)}: {ApiGeneratorVersion}, {nameof(ApiGeneratorNameAndVersion)}: {ApiGeneratorNameAndVersion}, {nameof(ApiOptions)}: {ApiOptions}, {nameof(PathForSrcGenerate)}: {PathForSrcGenerate}, {nameof(ProjectSrcCsProj)}: {ProjectSrcCsProj}, {nameof(ForClient)}: {ForClient}, {nameof(ClientFolderName)}: {ClientFolderName}, {nameof(Document)}: {Document}, {nameof(DocumentFile)}: {DocumentFile}, {nameof(ProjectName)}: {ProjectName}, {nameof(HttpClientName)}: {HttpClientName}, {nameof(ApiGroupNames)}: {ApiGroupNames}, {nameof(ExcludeEndpointGeneration)}: {ExcludeEndpointGeneration}";
}