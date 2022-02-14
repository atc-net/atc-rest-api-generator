using Atc.Console.Spectre;

namespace Atc.Rest.ApiGenerator.Models;

public class HostProjectOptions : BaseProjectOptions
{
    [SuppressMessage("Major Code Smell", "S107:Methods should not have too many parameters", Justification = "OK.")]
    public HostProjectOptions(
        DirectoryInfo projectSrcGeneratePath,
        DirectoryInfo? projectTestGeneratePath,
        OpenApiDocument openApiDocument,
        FileInfo openApiDocumentFile,
        string projectPrefixName,
        ApiOptions.ApiOptions apiOptions,
        bool usingCodingRules,
        DirectoryInfo apiProjectSrcPath,
        DirectoryInfo domainProjectSrcPath)
        : base(
            projectSrcGeneratePath,
            projectTestGeneratePath,
            openApiDocument,
            openApiDocumentFile,
            projectPrefixName,
            "Api",
            apiOptions,
            usingCodingRules)
    {
        ApiProjectSrcPath = apiProjectSrcPath ?? throw new ArgumentNullException(nameof(apiProjectSrcPath));
        DomainProjectSrcPath = domainProjectSrcPath ?? throw new ArgumentNullException(nameof(domainProjectSrcPath));
        UseRestExtended = apiOptions.Generator.UseRestExtended;
    }

    public DirectoryInfo ApiProjectSrcPath { get; private set; }

    public FileInfo? ApiProjectSrcCsProj { get; private set; }

    public DirectoryInfo DomainProjectSrcPath { get; private set; }

    public FileInfo? DomainProjectSrcCsProj { get; private set; }

    public bool UseRestExtended { get; set; }

    public bool SetPropertiesAfterValidationsOfProjectReferencesPathAndFiles(
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

        if (DomainProjectSrcPath.Exists)
        {
            var files = Directory.GetFiles(DomainProjectSrcPath.FullName, "DomainRegistration.cs", SearchOption.AllDirectories);
            if (files.Length == 1)
            {
                DomainProjectSrcPath = new FileInfo(files[0]).Directory!;
                files = Directory.GetFiles(DomainProjectSrcPath.FullName, "*.csproj", SearchOption.AllDirectories);
                if (files.Length == 1)
                {
                    DomainProjectSrcCsProj = new FileInfo(files[0]);
                }
            }
        }

        if (DomainProjectSrcCsProj is null ||
            !DomainProjectSrcCsProj.Exists)
        {
            logger.LogError($"{EmojisConstants.Error} {ValidationRuleNameConstants.ProjectHostGenerated05} - Can't find Domain .csproj file");
            return false;
        }

        return true;
    }
}