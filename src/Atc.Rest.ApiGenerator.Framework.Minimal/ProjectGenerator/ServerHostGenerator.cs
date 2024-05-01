// ReSharper disable ArrangeObjectCreationWhenTypeNotEvident
namespace Atc.Rest.ApiGenerator.Framework.Minimal.ProjectGenerator;

public class ServerHostGenerator : IServerHostGenerator
{
    private readonly ILogger<ServerHostGenerator> logger;
    private readonly Version apiGeneratorVersion;
    private readonly string projectName;
    private readonly string apiProjectName;
    private readonly string domainProjectName;
    private readonly DirectoryInfo projectPath;
    private readonly OpenApiDocument openApiDocument;

    public ServerHostGenerator(
        ILoggerFactory loggerFactory,
        Version apiGeneratorVersion,
        string projectName,
        string apiProjectName,
        string domainProjectName,
        DirectoryInfo projectPath,
        OpenApiDocument openApiDocument)
    {
        ArgumentNullException.ThrowIfNull(loggerFactory);
        ArgumentNullException.ThrowIfNull(apiGeneratorVersion);
        ArgumentNullException.ThrowIfNull(projectName);
        ArgumentNullException.ThrowIfNull(apiProjectName);
        ArgumentNullException.ThrowIfNull(domainProjectName);
        ArgumentNullException.ThrowIfNull(projectPath);
        ArgumentNullException.ThrowIfNull(openApiDocument);

        logger = loggerFactory.CreateLogger<ServerHostGenerator>();
        this.apiGeneratorVersion = apiGeneratorVersion;
        this.projectName = projectName;
        this.apiProjectName = apiProjectName;
        this.domainProjectName = domainProjectName;
        this.projectPath = projectPath;
        this.openApiDocument = openApiDocument;
    }

    /// <summary>
    /// NOTE: This property is not used in MinimalApi
    /// </summary>
    public bool UseRestExtended { get; set; }

    public void ScaffoldProjectFile()
    {
        var projectFileParameters = new ProjectFileParameters(
            "Microsoft.NET.Sdk.Web",
            [
                new List<PropertyGroupParameter>
                {
                    new("TargetFramework", Attributes: null, "net8.0"),
                    new("IsPackable", Attributes: null, "false"),
                },
                new List<PropertyGroupParameter>
                {
                    new("GenerateDocumentationFile", Attributes: null, "true"),
                },
                new List<PropertyGroupParameter>
                {
                    new("DocumentationFile", Attributes: null, @$"bin\Debug\net8.0\{projectName}.xml"),
                    new("NoWarn", Attributes: null, "1573;1591;1701;1702;1712;8618;"),
                },
            ],
            [
                new List<ItemGroupParameter>
                {
                    new(
                        "PackageReference",
                        [
                            new("Include", "Asp.Versioning.Http"),
                            new("Version", "8.1.0"),
                        ],
                        Value: null),
                    new(
                        "PackageReference",
                        [
                            new("Include", "Atc"),
                            new("Version", "2.0.472"),
                        ],
                        Value: null),
                    new(
                        "PackageReference",
                        [
                            new("Include", "Atc.Rest.MinimalApi"),
                            new("Version", "1.0.65"),
                        ],
                        Value: null),
                    new(
                        "PackageReference",
                        [
                            new("Include", "Microsoft.NETCore.Platforms"),
                            new("Version", "7.0.4"),
                        ],
                        Value: null),
                    new(
                        "PackageReference",
                        [
                            new("Include", "Swashbuckle.AspNetCore"),
                            new("Version", "6.5.0"),
                        ],
                        Value: null),
                },
                new List<ItemGroupParameter>
                {
                    new(
                        "ProjectReference",
                        [
                            new("Include", @$"..\{apiProjectName}\{apiProjectName}.csproj"),
                        ],
                        Value: null),
                    new(
                        "ProjectReference",
                        [
                            new("Include", @$"..\{domainProjectName}\{domainProjectName}.csproj"),
                        ],
                        Value: null),
                },
            ]);

        var contentGenerator = new GenerateContentForProjectFile(
            projectFileParameters);

        var content = contentGenerator.Generate();

        var contentWriter = new ContentWriter(logger);
        contentWriter.Write(
            projectPath,
            projectPath.CombineFileInfo($"{projectName}.csproj"),
            ContentWriterArea.Src,
            content,
            overrideIfExist: false);
    }

    public void ScaffoldJsonSerializerOptionsExtensions()
    {
        var contentGenerator = new ContentGenerators.Server.ContentGeneratorServerJsonSerializerOptionsExtensions(
                               new ContentGeneratorBaseParameters(Namespace: projectName));

        var content = contentGenerator.Generate();

        var contentWriter = new ContentWriter(logger);
        contentWriter.Write(
            projectPath,
            projectPath.CombineFileInfo("Extensions", "JsonSerializerOptionsExtensions.cs"),
            ContentWriterArea.Src,
            content,
            overrideIfExist: false);
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

    public void ScaffoldWebApplicationBuilderExtensions()
    {
        var contentGenerator = new ContentGenerators.Server.ContentGeneratorServerWebApplicationBuilderExtensions(
            new ContentGeneratorBaseParameters(Namespace: projectName));

        var content = contentGenerator.Generate();

        var contentWriter = new ContentWriter(logger);
        contentWriter.Write(
            projectPath,
            projectPath.CombineFileInfo("Extensions", "WebApplicationBuilderExtensions.cs"),
            ContentWriterArea.Src,
            content,
            overrideIfExist: false); // TODO: !!
    }

    public void ScaffoldWebApplicationExtensions(
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

    public void ScaffoldProgramFile(
        SwaggerThemeMode swaggerThemeMode)
    {
        var contentGenerator = new ContentGenerators.Server.ContentGeneratorServerProgram(
            new ContentGeneratorBaseParameters(Namespace: projectName))
        {
            SwaggerThemeMode = swaggerThemeMode,
        };

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

    public void GenerateConfigureSwaggerDocOptions()
    {
        var fullNamespace = $"{projectName}";

        var contentGeneratorServerSwaggerDocOptionsParameters = ContentGeneratorServerSwaggerDocOptionsParameterFactory
            .Create(
                fullNamespace,
                openApiDocument.ToSwaggerDocOptionsParameters());

        var contentGenerator = new Core.ContentGenerators.Server.ContentGeneratorServerSwaggerDocOptions(
            new GeneratedCodeHeaderGenerator(new GeneratedCodeGeneratorParameters(apiGeneratorVersion)),
            new GeneratedCodeAttributeGenerator(new GeneratedCodeGeneratorParameters(apiGeneratorVersion)),
            contentGeneratorServerSwaggerDocOptionsParameters);

        var content = contentGenerator.Generate();

        var contentWriter = new ContentWriter(logger);
        contentWriter.Write(
            projectPath,
            projectPath.CombineFileInfo("Options", "ConfigureSwaggerDocOptions.cs"),
            ContentWriterArea.Src,
            content);
    }

    public void MaintainGlobalUsings(
        IList<string> apiGroupNames,
        bool removeNamespaceGroupSeparatorInGlobalUsings)
    {
        ArgumentNullException.ThrowIfNull(domainProjectName);
        ArgumentNullException.ThrowIfNull(apiGroupNames);

        var requiredUsings = new List<string>
        {
            "System.CodeDom.Compiler",
            "System.Diagnostics.CodeAnalysis",
            "System.Text",
            "Asp.Versioning",
            "Asp.Versioning.ApiExplorer",
            "Atc.Rest.MinimalApi.Extensions",
            "Atc.Rest.MinimalApi.Filters.Endpoints",
            "Atc.Rest.MinimalApi.Filters.Swagger",
            "Atc.Rest.MinimalApi.Versioning",
            "Atc.Serialization",
            "FluentValidation",
            "Microsoft.AspNetCore.HttpLogging",
            "Microsoft.Extensions.Logging.ApplicationInsights",
            "Microsoft.Extensions.Options",
            "Microsoft.OpenApi.Models",
            "Swashbuckle.AspNetCore.SwaggerGen",
            $"{projectName}.Extensions",
            $"{projectName}.Generated",
            $"{projectName}.Options",
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