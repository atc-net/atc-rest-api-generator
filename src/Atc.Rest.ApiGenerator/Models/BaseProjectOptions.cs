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
        ApiOptions apiOptions,
        bool usingCodingRules,
        bool forClient = false,
        string? clientFolderName = null)
    {
        ArgumentNullException.ThrowIfNull(projectSrcGeneratePath);

        ProjectName = projectPrefixName ?? throw new ArgumentNullException(nameof(projectPrefixName));
        PathForSrcGenerate = projectSrcGeneratePath.Name.StartsWith(ProjectName, StringComparison.OrdinalIgnoreCase)
            ? projectSrcGeneratePath.Parent!
            : projectSrcGeneratePath;

        if (projectTestGeneratePath is not null)
        {
            PathForTestGenerate = projectTestGeneratePath.Name.StartsWith(ProjectName, StringComparison.OrdinalIgnoreCase)
                ? projectTestGeneratePath.Parent!
                : projectTestGeneratePath;
        }

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

        RouteBase = openApiDocument.GetServerUrlBasePath();
        ProjectName = string.IsNullOrEmpty(projectSuffixName)
            ? projectPrefixName
                .Replace(' ', '.')
                .Replace('-', '.')
                .Trim()
            : projectPrefixName
                .Replace(' ', '.')
                .Replace('-', '.')
                .Trim() + $".{projectSuffixName}";

        ProjectPrefixName = ProjectName.Contains('.', StringComparison.Ordinal)
            ? ProjectName.Substring(0, ProjectName.IndexOf('.', StringComparison.Ordinal))
            : ProjectName;

        PathForSrcGenerate = new DirectoryInfo(Path.Combine(PathForSrcGenerate.FullName, ProjectName));
        ProjectSrcCsProj = new FileInfo(Path.Combine(PathForSrcGenerate.FullName, $"{ProjectName}.csproj"));
        if (PathForTestGenerate is not null)
        {
            PathForTestGenerate = new DirectoryInfo(Path.Combine(PathForTestGenerate.FullName, $"{ProjectName}.Tests"));
            ProjectTestCsProj = new FileInfo(Path.Combine(PathForTestGenerate.FullName, $"{ProjectName}.Tests.csproj"));
        }

        UsingCodingRules = usingCodingRules;
        IsForClient = forClient;
        ClientFolderName = clientFolderName;

        ApiGroupNames = openApiDocument.GetApiGroupNames();
    }

    public bool UsingCodingRules { get; }

    public bool UseNullableReferenceTypes { get; } = true;

    public string ApiGeneratorName { get; }

    public Version ApiGeneratorVersion { get; }

    public string ApiGeneratorNameAndVersion => $"{ApiGeneratorName} {ApiGeneratorVersion}";

    public ApiOptions ApiOptions { get; }

    public DirectoryInfo PathForSrcGenerate { get; }

    public DirectoryInfo? PathForTestGenerate { get; }

    public FileInfo ProjectSrcCsProj { get; }

    public string ProjectSrcCsProjDisplayLocation
        => ProjectSrcCsProj.FullName.Replace(PathForSrcGenerate.FullName, "src: ", StringComparison.Ordinal);

    public FileInfo? ProjectTestCsProj { get; }

    public string ProjectTestCsProjDisplayLocation
        => ProjectTestCsProj is null || PathForTestGenerate is null
            ? ProjectTestCsProj is null
                ? string.Empty
                : ProjectTestCsProj.FullName
            : ProjectTestCsProj.FullName.Replace(PathForTestGenerate.FullName, "test: ", StringComparison.Ordinal);

    public OpenApiDocument Document { get; }

    public FileInfo DocumentFile { get; }

    public string ProjectPrefixName { get; }

    public string ProjectName { get; }

    public string RouteBase { get; }

    public bool IsForClient { get; }

    public string? ClientFolderName { get; }

    public IList<string> ApiGroupNames { get; }
}