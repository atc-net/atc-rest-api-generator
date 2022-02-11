namespace Atc.Rest.ApiGenerator.Models;

public class DomainProjectOptions : BaseProjectOptions
{
    public DomainProjectOptions(
        DirectoryInfo projectSourceGeneratePath,
        DirectoryInfo? projectTestGeneratePath,
        OpenApiDocument openApiDocument,
        FileInfo openApiDocumentFile,
        string projectPrefixName,
        ApiOptions.ApiOptions apiOptions,
        bool useCodingRules,
        DirectoryInfo apiProjectSrcPath)
        : base(
            projectSourceGeneratePath,
            projectTestGeneratePath,
            openApiDocument,
            openApiDocumentFile,
            projectPrefixName,
            "Domain",
            apiOptions,
            useCodingRules)
    {
        ApiProjectSrcPath = apiProjectSrcPath ?? throw new ArgumentNullException(nameof(apiProjectSrcPath));
        PathForSrcHandlers = new DirectoryInfo(Path.Combine(PathForSrcGenerate.FullName, NameConstants.Handlers));
        if (PathForTestGenerate is not null)
        {
            PathForTestHandlers = new DirectoryInfo(Path.Combine(PathForTestGenerate.FullName, NameConstants.Handlers));
        }
    }

    public DirectoryInfo ApiProjectSrcPath { get; private set; }

    public FileInfo? ApiProjectSrcCsProj { get; private set; }

    public DirectoryInfo? PathForSrcHandlers { get; }

    public DirectoryInfo? PathForTestHandlers { get; }

    public List<LogKeyValueItem> SetPropertiesAfterValidationsOfProjectReferencesPathAndFiles()
    {
        var logItems = new List<LogKeyValueItem>();
        if (ApiProjectSrcPath.Exists)
        {
            var files = Directory.GetFiles(ApiProjectSrcPath.FullName, "ApiRegistration.cs", SearchOption.AllDirectories);
            if (files.Length == 1)
            {
                ApiProjectSrcPath = new FileInfo(files[0]).Directory!;
                files = Directory.GetFiles(ApiProjectSrcPath.FullName, "*.csproj", SearchOption.AllDirectories);
                if (files.Length == 1)
                {
                    ApiProjectSrcCsProj = new FileInfo(files[0]);
                }
            }
        }

        if (ApiProjectSrcCsProj is null ||
            !ApiProjectSrcCsProj.Exists)
        {
            logItems.Add(LogItemHelper.Create(LogCategoryType.Error, ValidationRuleNameConstants.ProjectHostGenerated04, "Can't find API .csproj file"));
        }

        return logItems;
    }
}