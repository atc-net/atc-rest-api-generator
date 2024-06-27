namespace Atc.Rest.ApiGenerator.Models;

public class DomainProjectOptions : BaseProjectOptions
{
    public DomainProjectOptions(
        DirectoryInfo projectSourceGeneratePath,
        DirectoryInfo? projectTestGeneratePath,
        OpenApiDocument openApiDocument,
        FileInfo openApiDocumentFile,
        string projectPrefixName,
        ApiOptions apiOptions,
        bool useCodingRules,
        bool removeNamespaceGroupSeparatorInGlobalUsings,
        DirectoryInfo apiProjectSrcPath)
        : base(
            projectSourceGeneratePath,
            projectTestGeneratePath,
            openApiDocument,
            openApiDocumentFile,
            projectPrefixName,
            "Domain",
            apiOptions,
            useCodingRules,
            removeNamespaceGroupSeparatorInGlobalUsings)
    {
        ApiProjectSrcPath = apiProjectSrcPath ?? throw new ArgumentNullException(nameof(apiProjectSrcPath));
        PathForSrcHandlers = new DirectoryInfo(Path.Combine(PathForSrcGenerate.FullName, ContentGeneratorConstants.Handlers));
        if (PathForTestGenerate is not null)
        {
            PathForTestHandlers = new DirectoryInfo(Path.Combine(PathForTestGenerate.FullName, ContentGeneratorConstants.Handlers));
        }
    }

    public DirectoryInfo ApiProjectSrcPath { get; private set; }

    public FileInfo? ApiProjectSrcCsProj { get; private set; }

    public DirectoryInfo? PathForSrcHandlers { get; }

    public DirectoryInfo? PathForTestHandlers { get; }

    public bool SetPropertiesAfterValidationsOfProjectReferencesPathAndFilesForMvc(
        ILogger logger)
    {
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
            logger.LogError($"{EmojisConstants.Error} {ValidationRuleNameConstants.ProjectHostGenerated04} - Can't find API .csproj file");
            return false;
        }

        return true;
    }

    public bool SetPropertiesAfterValidationsOfProjectReferencesPathAndFilesForMinimalApi(
        ILogger logger)
    {
        if (ApiProjectSrcPath.Exists)
        {
            var files = Directory.GetFiles(ApiProjectSrcPath.FullName, "IApiContractAssemblyMarker.cs", SearchOption.AllDirectories);
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
            logger.LogError($"{EmojisConstants.Error} {ValidationRuleNameConstants.ProjectHostGenerated04} - Can't find API .csproj file");
            return false;
        }

        return true;
    }
}