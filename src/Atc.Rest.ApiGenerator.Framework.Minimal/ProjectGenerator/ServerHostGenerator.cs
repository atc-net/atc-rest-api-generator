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

    /// <summary>
    /// NOTE: This property is not used in MinimalApi
    /// </summary>
    public bool UseRestExtended { get; set; }

    public void ScaffoldProgramFile()
    {
        var contentGenerator = new ContentGenerators.Server.ContentGeneratorServerProgram(
            new ContentGeneratorBaseParameters(Namespace: projectName));

        var content = contentGenerator.Generate();

        var contentWriter = new ContentWriter(logger);
        contentWriter.Write(
            projectPath,
            projectPath.CombineFileInfo("Program.cs"),
            ContentWriterArea.Src,
            content,
            overrideIfExist: false);
    }

    public void ScaffoldStartupFile()
        => throw new NotSupportedException($"{nameof(ScaffoldStartupFile)} is not supported for MinimalApi");

    public void ScaffoldWebConfig()
    {
        var contentGenerator = new Core.ContentGenerators.Server.ContentGeneratorServerWebConfig();

        var content = contentGenerator.Generate();

        var contentWriter = new ContentWriter(logger);
        contentWriter.Write(
            projectPath,
            projectPath.CombineFileInfo("web.config"),
            ContentWriterArea.Src,
            content,
            overrideIfExist: false);
    }

    public void ScaffoldConfigureSwaggerDocOptions()
    {
        var fullNamespace = $"{projectName}";

        var contentGeneratorServerSwaggerDocOptionsParameters = ContentGeneratorServerSwaggerDocOptionsParameterFactory
            .Create(
                fullNamespace,
                openApiDocument.ToSwaggerDocOptionsParameters());

        var contentGenerator = new ContentGenerators.Server.ContentGeneratorServerSwaggerDocOptions(
            new GeneratedCodeHeaderGenerator(new GeneratedCodeGeneratorParameters(apiGeneratorVersion)),
            new GeneratedCodeAttributeGenerator(new GeneratedCodeGeneratorParameters(apiGeneratorVersion)),
            contentGeneratorServerSwaggerDocOptionsParameters);

        var content = contentGenerator.Generate();

        var contentWriter = new ContentWriter(logger);
        contentWriter.Write(
            projectPath,
            projectPath.CombineFileInfo("ConfigureSwaggerDocOptions.cs"),
            ContentWriterArea.Src,
            content);
    }

    public void ScaffoldServiceCollectionExtensions()
    {
        var contentGenerator = new ContentGenerators.Server.ContentGeneratorServerServiceCollectionExtensions(
            new ContentGeneratorBaseParameters(Namespace: projectName));

        var content = contentGenerator.Generate();

        var contentWriter = new ContentWriter(logger);
        contentWriter.Write(
            projectPath,
            projectPath.CombineFileInfo("Extensions", "ServiceCollectionExtensions.cs"),
            ContentWriterArea.Src,
            content,
            overrideIfExist: false);
    }

    public void ScaffoldServiceWebApplicationExtensions(
        SwaggerThemeMode swaggerThemeMode)
    {
        var contentGenerator = new ContentGenerators.Server.ContentGeneratorServeWebApplicationExtensions(
            new ContentGeneratorBaseParameters(Namespace: projectName))
        {
            SwaggerThemeMode = swaggerThemeMode,
        };

        var content = contentGenerator.Generate();

        var contentWriter = new ContentWriter(logger);
        contentWriter.Write(
            projectPath,
            projectPath.CombineFileInfo("Extensions", "WebApplicationExtensions.cs"),
            ContentWriterArea.Src,
            content,
            overrideIfExist: false);
    }

    public void ScaffoldConfigureSwaggerOptions()
    {
        var contentGenerator = new ContentGenerators.Server.ContentGeneratorServeConfigureSwaggerOptions(
            new ContentGeneratorBaseParameters(Namespace: projectName));

        var content = contentGenerator.Generate();

        var contentWriter = new ContentWriter(logger);
        contentWriter.Write(
            projectPath,
            projectPath.CombineFileInfo("Options", "ConfigureSwaggerOptions.cs"),
            ContentWriterArea.Src,
            content,
            overrideIfExist: false);
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
            "System.Text",
            "Asp.Versioning",
            "Asp.Versioning.ApiExplorer",
            "Atc.Rest.MinimalApi.Extensions",
            "Atc.Rest.MinimalApi.Filters.Endpoints",
            "Atc.Rest.MinimalApi.Filters.Swagger",
            "Atc.Rest.MinimalApi.Middleware",
            "Atc.Rest.MinimalApi.Versioning",
            "FluentValidation",
            "Microsoft.Extensions.Options",
            "Microsoft.OpenApi.Models",
            "Swashbuckle.AspNetCore.SwaggerGen",
            $"{projectName}.Generated",
            $"{projectName}".Replace(".Api", ".Domain", StringComparison.Ordinal),
        };

        GlobalUsingsHelper.CreateOrUpdate(
            logger,
            ContentWriterArea.Src,
            projectPath,
            requiredUsings,
            removeNamespaceGroupSeparatorInGlobalUsings);
    }

    public void MaintainWwwResources()
        => ResourcesHelper.MaintainWwwResources(projectPath);
}