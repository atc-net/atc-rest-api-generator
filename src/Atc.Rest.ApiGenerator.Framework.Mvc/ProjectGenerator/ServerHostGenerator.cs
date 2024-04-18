namespace Atc.Rest.ApiGenerator.Framework.Mvc.ProjectGenerator;

public class ServerHostGenerator : IServerHostGenerator
{
    private readonly ILogger<ServerHostGenerator> logger;
    private readonly Version apiGeneratorVersion;
    private readonly string projectName;
    private readonly DirectoryInfo projectPath;
    private readonly OpenApiDocument openApiDocument;

    public ServerHostGenerator(
        ILoggerFactory loggerFactory,
        Version apiGeneratorVersion,
        string projectName,
        DirectoryInfo projectPath,
        OpenApiDocument openApiDocument)
    {
        ArgumentNullException.ThrowIfNull(loggerFactory);
        ArgumentNullException.ThrowIfNull(apiGeneratorVersion);
        ArgumentNullException.ThrowIfNull(projectName);
        ArgumentNullException.ThrowIfNull(projectPath);
        ArgumentNullException.ThrowIfNull(openApiDocument);

        logger = loggerFactory.CreateLogger<ServerHostGenerator>();
        this.apiGeneratorVersion = apiGeneratorVersion;
        this.projectName = projectName;
        this.projectPath = projectPath;
        this.openApiDocument = openApiDocument;
    }

    public void MaintainGlobalUsings(
        string domainProjectName,
        IList<string> apiGroupNames,
        bool removeNamespaceGroupSeparatorInGlobalUsings)
    {
        ArgumentNullException.ThrowIfNull(domainProjectName);
        ArgumentNullException.ThrowIfNull(apiGroupNames);

        var requiredUsings = new List<string>
        {
            "System.CodeDom.Compiler",
            "System.Reflection",
            domainProjectName,
            $"{projectName}.Generated",
        };

        //if (projectOptions.UseRestExtended)
        {
            requiredUsings.Add("Asp.Versioning.ApiExplorer");
            requiredUsings.Add("Atc.Rest.Extended.Options");
            requiredUsings.Add("Microsoft.Extensions.Options");
            requiredUsings.Add("Microsoft.OpenApi.Models");
            requiredUsings.Add("Swashbuckle.AspNetCore.SwaggerGen");
        }

        GlobalUsingsHelper.CreateOrUpdate(
            logger,
            ContentWriterArea.Src,
            projectPath,
            requiredUsings,
            removeNamespaceGroupSeparatorInGlobalUsings);
    }
}