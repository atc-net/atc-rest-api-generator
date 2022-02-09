namespace Atc.Rest.ApiGenerator.Models;

public abstract class BaseProjectOptions
{
    protected BaseProjectOptions(
        DirectoryInfo projectSrcGeneratePath,
        DirectoryInfo? projectTestGeneratePath,
        OpenApiDocument openApiDocument,
        FileInfo openApiDocumentFile,
        string projectPrefixName,
        string? projectSuffixName,
        ApiOptions.ApiOptions apiOptions,
        bool forClient = false,
        string? clientFolderName = null)
    {
        ArgumentNullException.ThrowIfNull(projectSrcGeneratePath);

        ProjectName = projectPrefixName ?? throw new ArgumentNullException(nameof(projectPrefixName));
        PathForSrcGenerate = projectSrcGeneratePath.Name.StartsWith(ProjectName, StringComparison.OrdinalIgnoreCase)
            ? projectSrcGeneratePath.Parent!
            : projectSrcGeneratePath;

        if (projectTestGeneratePath != null)
        {
            PathForTestGenerate = projectTestGeneratePath.Name.StartsWith(ProjectName, StringComparison.OrdinalIgnoreCase)
                ? projectTestGeneratePath.Parent!
                : projectTestGeneratePath;
        }

        Document = openApiDocument ?? throw new ArgumentNullException(nameof(openApiDocument));
        DocumentFile = openApiDocumentFile ?? throw new ArgumentNullException(nameof(openApiDocumentFile));

        ToolName = "ApiGenerator";
        ToolVersion = GenerateHelper.GetAtcToolVersion();
        ApiOptions = apiOptions;

        RouteBase = OpenApiDocumentHelper.GetServerUrl(openApiDocument);
        ProjectName = string.IsNullOrEmpty(projectSuffixName)
            ? projectPrefixName
                .Replace(" ", ".", StringComparison.Ordinal)
                .Replace("-", ".", StringComparison.Ordinal)
                .Trim()
            : projectPrefixName
                .Replace(" ", ".", StringComparison.Ordinal)
                .Replace("-", ".", StringComparison.Ordinal)
                .Trim() + $".{projectSuffixName}";

        ProjectPrefixName = ProjectName.Contains('.', StringComparison.Ordinal)
            ? ProjectName.Substring(0, ProjectName.IndexOf('.', StringComparison.Ordinal))
            : ProjectName;

        PathForSrcGenerate = new DirectoryInfo(Path.Combine(PathForSrcGenerate.FullName, ProjectName));
        ProjectSrcCsProj = new FileInfo(Path.Combine(PathForSrcGenerate.FullName, $"{ProjectName}.csproj"));
        if (PathForTestGenerate != null)
        {
            PathForTestGenerate = new DirectoryInfo(Path.Combine(PathForTestGenerate.FullName, $"{ProjectName}.Tests"));
            ProjectTestCsProj = new FileInfo(Path.Combine(PathForTestGenerate.FullName, $"{ProjectName}.Tests.csproj"));
        }

        this.IsForClient = forClient;
        this.ClientFolderName = clientFolderName;

        BasePathSegmentNames = OpenApiDocumentHelper.GetBasePathSegmentNames(openApiDocument);
    }

    public string ToolName { get; }

    public Version ToolVersion { get; }

    public string ToolNameAndVersion => $"{ToolName} {ToolVersion}";

    public ApiOptions.ApiOptions ApiOptions { get; }

    public DirectoryInfo PathForSrcGenerate { get; }

    public DirectoryInfo? PathForTestGenerate { get; }

    public FileInfo ProjectSrcCsProj { get; }

    public FileInfo? ProjectTestCsProj { get; }

    public OpenApiDocument Document { get; }

    public FileInfo DocumentFile { get; }

    public string ProjectPrefixName { get; }

    public string ProjectName { get; }

    public string RouteBase { get; }

    public bool IsForClient { get; }

    public string? ClientFolderName { get; }

    public List<string> BasePathSegmentNames { get; }
}