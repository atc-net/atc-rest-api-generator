namespace Atc.Rest.ApiGenerator.Models;

public class ClientCSharpApiProjectOptions
{
    public ClientCSharpApiProjectOptions(
        DirectoryInfo projectSrcGeneratePath,
        OpenApiDocument openApiDocument,
        FileInfo openApiDocumentFile,
        ApiOptions apiOptions,
        bool usingCodingRules)
    {
        ArgumentNullException.ThrowIfNull(projectSrcGeneratePath);
        ArgumentNullException.ThrowIfNull(apiOptions);

        ProjectName = $"{apiOptions.Generator.ProjectName}.{apiOptions.Generator.ProjectSuffixName}";

        PathForSrcGenerate = projectSrcGeneratePath.Name.StartsWith(ProjectName, StringComparison.OrdinalIgnoreCase)
            ? projectSrcGeneratePath.Parent!
            : projectSrcGeneratePath;

        ForClient = true;
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

        PathForSrcGenerate = new DirectoryInfo(Path.Combine(PathForSrcGenerate.FullName, ProjectName));
        ProjectSrcCsProj = new FileInfo(Path.Combine(PathForSrcGenerate.FullName, $"{ProjectName}.csproj"));

        UsingCodingRules = usingCodingRules;
    }

    public bool UsingCodingRules { get; }

    public string ApiGeneratorName { get; }

    public Version ApiGeneratorVersion { get; }

    public string ApiGeneratorNameAndVersion => $"{ApiGeneratorName} {ApiGeneratorVersion}";

    public ApiOptions ApiOptions { get; }

    public DirectoryInfo PathForSrcGenerate { get; }

    public FileInfo ProjectSrcCsProj { get; }

    public string ProjectSrcCsProjDisplayLocation
        => ProjectSrcCsProj.FullName.Replace(PathForSrcGenerate.FullName, "src: ", StringComparison.Ordinal);

    public bool ForClient { get; }

    public OpenApiDocument Document { get; }

    public FileInfo DocumentFile { get; }

    public string ProjectName { get; }

    public override string ToString()
        => $"{nameof(ApiGeneratorName)}: {ApiGeneratorName}, {nameof(ApiGeneratorVersion)}: {ApiGeneratorVersion}, {nameof(ApiGeneratorNameAndVersion)}: {ApiGeneratorNameAndVersion}, {nameof(ApiOptions)}: {ApiOptions}, {nameof(PathForSrcGenerate)}: {PathForSrcGenerate}, {nameof(ProjectSrcCsProj)}: {ProjectSrcCsProj}, {nameof(ForClient)}: {ForClient}, {nameof(Document)}: {Document}, {nameof(DocumentFile)}: {DocumentFile}, {nameof(ProjectName)}: {ProjectName}";
}