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
        ApiGeneratorVersion = GenerateHelper.GetAtcApiGeneratorVersion();
        ApiOptions = apiOptions;
        ProjectName = projectPrefixName
            .Replace(" ", ".", StringComparison.Ordinal)
            .Replace("-", ".", StringComparison.Ordinal)
            .Trim() + $".{projectSuffixName}";
        PathForSrcGenerate = new DirectoryInfo(Path.Combine(PathForSrcGenerate.FullName, ProjectName));
        ProjectSrcCsProj = new FileInfo(Path.Combine(PathForSrcGenerate.FullName, $"{ProjectName}.csproj"));

        BasePathSegmentNames = OpenApiDocumentHelper.GetBasePathSegmentNames(openApiDocument);

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

    public List<string> BasePathSegmentNames { get; }

    public bool ExcludeEndpointGeneration { get; }

    public override string ToString()
        => $"{nameof(ApiGeneratorName)}: {ApiGeneratorName}, {nameof(ApiGeneratorVersion)}: {ApiGeneratorVersion}, {nameof(ApiGeneratorNameAndVersion)}: {ApiGeneratorNameAndVersion}, {nameof(ApiOptions)}: {ApiOptions}, {nameof(PathForSrcGenerate)}: {PathForSrcGenerate}, {nameof(ProjectSrcCsProj)}: {ProjectSrcCsProj}, {nameof(ForClient)}: {ForClient}, {nameof(ClientFolderName)}: {ClientFolderName}, {nameof(Document)}: {Document}, {nameof(DocumentFile)}: {DocumentFile}, {nameof(ProjectName)}: {ProjectName}, {nameof(BasePathSegmentNames)}: {BasePathSegmentNames}, {nameof(ExcludeEndpointGeneration)}: {ExcludeEndpointGeneration}";
}