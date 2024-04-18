namespace Atc.Rest.ApiGenerator.Framework.Minimal.ProjectGenerator;

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
            "Atc.Rest.Extended.Options",
            "Asp.Versioning.ApiExplorer",
            "Microsoft.Extensions.Options",
            "Microsoft.OpenApi.Models",
            "Swashbuckle.AspNetCore.SwaggerGen",
        };

        GlobalUsingsHelper.CreateOrUpdate(
            logger,
            ContentWriterArea.Src,
            projectPath,
            requiredUsings,
            removeNamespaceGroupSeparatorInGlobalUsings);
    }
}