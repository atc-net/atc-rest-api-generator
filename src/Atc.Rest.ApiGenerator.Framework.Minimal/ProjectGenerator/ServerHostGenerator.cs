// ReSharper disable ArrangeObjectCreationWhenTypeNotEvident
namespace Atc.Rest.ApiGenerator.Framework.Minimal.ProjectGenerator;

public class ServerHostGenerator : IServerHostGenerator
{
    private readonly ILogger<ServerHostGenerator> logger;
    private readonly INugetPackageReferenceProvider nugetPackageReferenceProvider;
    private readonly string apiProjectName;
    private readonly string domainProjectName;
    private readonly OpenApiDocument openApiDocument;
    private readonly GeneratorSettings settings;

    public ServerHostGenerator(
        ILoggerFactory loggerFactory,
        INugetPackageReferenceProvider nugetPackageReferenceProvider,
        string apiProjectName,
        string domainProjectName,
        OpenApiDocument openApiDocument,
        GeneratorSettings generatorSettings)
    {
        ArgumentNullException.ThrowIfNull(loggerFactory);
        ArgumentNullException.ThrowIfNull(nugetPackageReferenceProvider);
        ArgumentNullException.ThrowIfNull(apiProjectName);
        ArgumentNullException.ThrowIfNull(domainProjectName);
        ArgumentNullException.ThrowIfNull(openApiDocument);
        ArgumentNullException.ThrowIfNull(openApiDocument);

        logger = loggerFactory.CreateLogger<ServerHostGenerator>();
        this.nugetPackageReferenceProvider = nugetPackageReferenceProvider;
        this.apiProjectName = apiProjectName;
        this.domainProjectName = domainProjectName;
        this.openApiDocument = openApiDocument;
        settings = generatorSettings;
    }

    /// <summary>
    /// NOTE: This property is not used in MinimalApi
    /// </summary>
    public bool UseRestExtended { get; set; }

    public async Task ScaffoldProjectFile()
    {
        var packageReferences = await nugetPackageReferenceProvider.GetPackageReferencesForHostProjectForMinimalApi();

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
                    new("TargetFramework", Attributes: null, "net9.0"),
                    new("IsPackable", Attributes: null, "false"),
                ],
                [
                    new("GenerateDocumentationFile", Attributes: null, "true"),
                ],
                [
                    new("DocumentationFile", Attributes: null, @$"bin\Debug\net9.0\{settings.ProjectName}.xml"),
                    new("NoWarn", Attributes: null, "$(NoWarn);1573;1591;1701;1702;1712;8618;"),
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
            settings.ProjectPath,
            settings.ProjectPath.CombineFileInfo($"{settings.ProjectName}.csproj"),
            ContentWriterArea.Src,
            content,
            overrideIfExist: false);
    }

    public void ScaffoldPropertiesLaunchSettingsFile()
        => ResourcesHelper.ScaffoldPropertiesLaunchSettingsFile(
            settings.ProjectName,
            settings.ProjectPath,
            useExtended: true);

    public void ScaffoldJsonSerializerOptionsExtensions()
    {
        var contentGenerator = new ContentGenerators.ContentGeneratorServerJsonSerializerOptionsExtensions(
                               new ContentGeneratorBaseParameters(Namespace: settings.ProjectName));

        var content = contentGenerator.Generate();

        var contentWriter = new ContentWriter(logger);
        contentWriter.Write(
            settings.ProjectPath,
            settings.ProjectPath.CombineFileInfo("Extensions", "JsonSerializerOptionsExtensions.cs"),
            ContentWriterArea.Src,
            content,
            overrideIfExist: false);
    }

    public void ScaffoldServiceCollectionExtensions()
    {
        var contentGenerator = new ContentGenerators.ContentGeneratorServerServiceCollectionExtensions(
            new ContentGeneratorBaseParameters(Namespace: settings.ProjectName));

        var content = contentGenerator.Generate();

        var contentWriter = new ContentWriter(logger);
        contentWriter.Write(
            settings.ProjectPath,
            settings.ProjectPath.CombineFileInfo("Extensions", "ServiceCollectionExtensions.cs"),
            ContentWriterArea.Src,
            content,
            overrideIfExist: false);
    }

    public void ScaffoldWebApplicationBuilderExtensions()
    {
        var contentGenerator = new ContentGenerators.ContentGeneratorServerWebApplicationBuilderExtensions(
            new ContentGeneratorBaseParameters(Namespace: settings.ProjectName));

        var content = contentGenerator.Generate();

        var contentWriter = new ContentWriter(logger);
        contentWriter.Write(
            settings.ProjectPath,
            settings.ProjectPath.CombineFileInfo("Extensions", "WebApplicationBuilderExtensions.cs"),
            ContentWriterArea.Src,
            content,
            overrideIfExist: false);
    }

    public void ScaffoldWebApplicationExtensions(
        SwaggerThemeMode swaggerThemeMode)
    {
        var contentGenerator = new ContentGenerators.ContentGeneratorServerWebApplicationExtensions(
            new ContentGeneratorBaseParameters(Namespace: settings.ProjectName))
        {
            SwaggerThemeMode = swaggerThemeMode,
        };

        var content = contentGenerator.Generate();

        var contentWriter = new ContentWriter(logger);
        contentWriter.Write(
            settings.ProjectPath,
            settings.ProjectPath.CombineFileInfo("Extensions", "WebApplicationExtensions.cs"),
            ContentWriterArea.Src,
            content,
            overrideIfExist: false);
    }

    public void ScaffoldProgramFile(
        SwaggerThemeMode swaggerThemeMode)
    {
        var contentGenerator = new ContentGenerators.ContentGeneratorServerProgram(
            new ContentGeneratorBaseParameters(Namespace: settings.ProjectName))
        {
            SwaggerThemeMode = swaggerThemeMode,
        };

        var content = contentGenerator.Generate();

        var contentWriter = new ContentWriter(logger);
        contentWriter.Write(
            settings.ProjectPath,
            settings.ProjectPath.CombineFileInfo("Program.cs"),
            ContentWriterArea.Src,
            content,
            overrideIfExist: false);
    }

    public void ScaffoldStartupFile()
        => throw new NotSupportedException($"{nameof(ScaffoldStartupFile)} is not supported for MinimalApi");

    public void ScaffoldWebConfig()
    {
        var contentGenerator = new ContentGeneratorServerWebConfig();

        var content = contentGenerator.Generate();

        var contentWriter = new ContentWriter(logger);
        contentWriter.Write(
            settings.ProjectPath,
            settings.ProjectPath.CombineFileInfo("web.config"),
            ContentWriterArea.Src,
            content,
            overrideIfExist: false);
    }

    public void GenerateConfigureSwaggerDocOptions()
    {
        var fullNamespace = $"{settings.ProjectName}";

        var contentGeneratorServerSwaggerDocOptionsParameters = ContentGeneratorServerSwaggerDocOptionsParameterFactory
            .Create(
                fullNamespace,
                openApiDocument.ToSwaggerDocOptionsParameters());

        var contentGenerator = new ContentGeneratorServerConfigureSwaggerDocOptions(
            new GeneratedCodeHeaderGenerator(new GeneratedCodeGeneratorParameters(settings.Version)),
            new GeneratedCodeAttributeGenerator(new GeneratedCodeGeneratorParameters(settings.Version)),
            contentGeneratorServerSwaggerDocOptionsParameters);

        var content = contentGenerator.Generate();

        var contentWriter = new ContentWriter(logger);
        contentWriter.Write(
            settings.ProjectPath,
            settings.ProjectPath.CombineFileInfo("Options", "ConfigureSwaggerDocOptions.cs"),
            ContentWriterArea.Src,
            content);
    }

    public void MaintainGlobalUsings(
        bool removeNamespaceGroupSeparatorInGlobalUsings)
    {
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
            $"{settings.ProjectName}.Extensions",
            $"{settings.ProjectName}.Generated",
            $"{settings.ProjectName}.Options",
            domainProjectName,
            $"{domainProjectName}.Extensions",
        };

        GlobalUsingsHelper.CreateOrUpdate(
            logger,
            ContentWriterArea.Src,
            settings.ProjectPath,
            requiredUsings,
            removeNamespaceGroupSeparatorInGlobalUsings);
    }

    public void MaintainWwwResources()
        => ResourcesHelper.MaintainWwwResources(settings.ProjectPath);
}