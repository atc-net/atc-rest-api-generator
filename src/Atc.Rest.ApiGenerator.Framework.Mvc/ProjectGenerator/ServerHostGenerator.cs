// ReSharper disable ArrangeObjectCreationWhenTypeNotEvident
namespace Atc.Rest.ApiGenerator.Framework.Mvc.ProjectGenerator;

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
                            new("Include", "Atc.Rest.Extended"),
                            new("Version", "2.0.472"),
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

    public void ScaffoldPropertiesLaunchSettingsFile()
        => ResourcesHelper.ScaffoldPropertiesLaunchSettingsFile(
            projectName,
            projectPath,
            useExtended: true);

    public void ScaffoldJsonSerializerOptionsExtensions()
        => throw new NotSupportedException($"{nameof(ScaffoldJsonSerializerOptionsExtensions)} is not supported for MVC");

    public void ScaffoldServiceCollectionExtensions()
        => throw new NotSupportedException($"{nameof(ScaffoldServiceCollectionExtensions)} is not supported for MVC");

    public void ScaffoldWebApplicationBuilderExtensions()
        => throw new NotSupportedException($"{nameof(ScaffoldWebApplicationBuilderExtensions)} is not supported for MVC");

    public void ScaffoldWebApplicationExtensions(
        SwaggerThemeMode swaggerThemeMode)
        => throw new NotSupportedException($"{nameof(ScaffoldWebApplicationExtensions)} is not supported for MVC");

    public void ScaffoldConfigureSwaggerOptions()
        => throw new NotSupportedException($"{nameof(ScaffoldConfigureSwaggerOptions)} is not supported for MVC");

    public void ScaffoldProgramFile(
        SwaggerThemeMode swaggerThemeMode)
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
    {
        var contentGenerator = new ContentGenerators.Server.ContentGeneratorServerStartup(
            new ContentGeneratorBaseParameters(Namespace: projectName));

        var content = contentGenerator.Generate();

        var contentWriter = new ContentWriter(logger);
        contentWriter.Write(
            projectPath,
            projectPath.CombineFileInfo("Startup.cs"),
            ContentWriterArea.Src,
            content,
            overrideIfExist: false);
    }

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
        bool removeNamespaceGroupSeparatorInGlobalUsings)
    {
        var requiredUsings = new List<string>
        {
            "System.CodeDom.Compiler",
            "System.Reflection",
            domainProjectName,
            $"{projectName}.Generated",
            $"{projectName}.Options",
        };

        if (UseRestExtended)
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

    public void MaintainWwwResources()
        => ResourcesHelper.MaintainWwwResources(projectPath);
}