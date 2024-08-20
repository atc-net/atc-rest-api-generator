// ReSharper disable ArrangeObjectCreationWhenTypeNotEvident
namespace Atc.Rest.ApiGenerator.Framework.Mvc.ProjectGenerator;

public class ServerHostGenerator : IServerHostGenerator
{
    private readonly ILogger<ServerHostGenerator> logger;
    private readonly INugetPackageReferenceProvider nugetPackageReferenceProvider;
    private readonly Version apiGeneratorVersion;
    private readonly string projectName;
    private readonly string apiProjectName;
    private readonly string domainProjectName;
    private readonly DirectoryInfo projectPath;
    private readonly OpenApiDocument openApiDocument;

    public ServerHostGenerator(
        ILoggerFactory loggerFactory,
        INugetPackageReferenceProvider nugetPackageReferenceProvider,
        Version apiGeneratorVersion,
        string projectName,
        string apiProjectName,
        string domainProjectName,
        DirectoryInfo projectPath,
        OpenApiDocument openApiDocument)
    {
        ArgumentNullException.ThrowIfNull(loggerFactory);
        ArgumentNullException.ThrowIfNull(nugetPackageReferenceProvider);
        ArgumentNullException.ThrowIfNull(apiGeneratorVersion);
        ArgumentNullException.ThrowIfNull(projectName);
        ArgumentNullException.ThrowIfNull(apiProjectName);
        ArgumentNullException.ThrowIfNull(domainProjectName);
        ArgumentNullException.ThrowIfNull(projectPath);
        ArgumentNullException.ThrowIfNull(openApiDocument);

        logger = loggerFactory.CreateLogger<ServerHostGenerator>();
        this.nugetPackageReferenceProvider = nugetPackageReferenceProvider;
        this.apiGeneratorVersion = apiGeneratorVersion;
        this.projectName = projectName;
        this.apiProjectName = apiProjectName;
        this.domainProjectName = domainProjectName;
        this.projectPath = projectPath;
        this.openApiDocument = openApiDocument;
    }

    public bool UseRestExtended { get; set; }

    public async Task ScaffoldProjectFile()
    {
        var packageReferences = await nugetPackageReferenceProvider.GetPackageReferencesForHostProjectForMvc(UseRestExtended);

        var itemGroupPackageReferences = packageReferences
            .Select(packageReference => new ItemGroupParameter(
                "PackageReference",
                [
                    new("Include", packageReference.PackageId),
                    new("Version", packageReference.PackageVersion),
                ],
                Value: null))
            .ToList();

        var projectFileParameters = new ProjectFileParameters(
            "Microsoft.NET.Sdk.Web",
            [
                [
                    new("TargetFramework", Attributes: null, "net8.0"),
                    new("IsPackable", Attributes: null, "false"),
                ],
                [
                    new("GenerateDocumentationFile", Attributes: null, "true"),
                ],
                [
                    new("DocumentationFile", Attributes: null, @$"bin\Debug\net8.0\{projectName}.xml"),
                    new("NoWarn", Attributes: null, "1573;1591;1701;1702;1712;8618;"),
                ],
            ],
            [
                itemGroupPackageReferences,
                [
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
                ],

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
    {
        var fullNamespace = $"{projectName}";

        var contentGeneratorServerSwaggerDocOptionsParameters = ContentGeneratorServerSwaggerDocOptionsParameterFactory
            .Create(
                fullNamespace,
                openApiDocument.ToSwaggerDocOptionsParameters());

        var contentGenerator = new ContentGeneratorServeConfigureSwaggerOptions(
            contentGeneratorServerSwaggerDocOptionsParameters);

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
        var contentGenerator = new ContentGenerators.ContentGeneratorServerProgram(
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
        var contentGenerator = new ContentGenerators.ContentGeneratorServerStartup(
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
        var contentGenerator = new ContentGeneratorServerWebConfig();

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

        var contentGenerator = new ContentGeneratorServerConfigureSwaggerDocOptions(
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
            "System.Text",
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