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
        bool excludeEndpointGeneration,
        ApiOptions.ApiOptions apiOptions)
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

        ToolName = "ApiGenerator";
        ToolVersion = GenerateHelper.GetAtcToolVersion();
        ApiOptions = apiOptions;
        ProjectName = projectPrefixName
            .Replace(" ", ".", StringComparison.Ordinal)
            .Replace("-", ".", StringComparison.Ordinal)
            .Trim() + $".{projectSuffixName}";
        PathForSrcGenerate = new DirectoryInfo(Path.Combine(PathForSrcGenerate.FullName, ProjectName));
        ProjectSrcCsProj = new FileInfo(Path.Combine(PathForSrcGenerate.FullName, $"{ProjectName}.csproj"));

        BasePathSegmentNames = OpenApiDocumentHelper.GetBasePathSegmentNames(openApiDocument);

        ExcludeEndpointGeneration = excludeEndpointGeneration;
    }

    public bool UseNullableReferenceTypes { get; } = true;

    public string ToolName { get; }

    public Version ToolVersion { get; }

    public string ToolNameAndVersion => $"{ToolName} {ToolVersion}";

    public ApiOptions.ApiOptions ApiOptions { get; }

    public DirectoryInfo PathForSrcGenerate { get; }

    public FileInfo ProjectSrcCsProj { get; }

    public bool ForClient { get; }

    public string? ClientFolderName { get; }

    public OpenApiDocument Document { get; }

    public FileInfo DocumentFile { get; }

    public string ProjectName { get; }

    public List<string> BasePathSegmentNames { get; }

    public bool ExcludeEndpointGeneration { get; }

    public override string ToString()
        => $"{nameof(ToolName)}: {ToolName}, {nameof(ToolVersion)}: {ToolVersion}, {nameof(ToolNameAndVersion)}: {ToolNameAndVersion}, {nameof(ApiOptions)}: {ApiOptions}, {nameof(PathForSrcGenerate)}: {PathForSrcGenerate}, {nameof(ProjectSrcCsProj)}: {ProjectSrcCsProj}, {nameof(ForClient)}: {ForClient}, {nameof(ClientFolderName)}: {ClientFolderName}, {nameof(Document)}: {Document}, {nameof(DocumentFile)}: {DocumentFile}, {nameof(ProjectName)}: {ProjectName}, {nameof(BasePathSegmentNames)}: {BasePathSegmentNames}, {nameof(ExcludeEndpointGeneration)}: {ExcludeEndpointGeneration}";
}