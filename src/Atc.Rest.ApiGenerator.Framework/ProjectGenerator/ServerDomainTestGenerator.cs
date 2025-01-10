// ReSharper disable ArrangeObjectCreationWhenTypeNotEvident
namespace Atc.Rest.ApiGenerator.Framework.ProjectGenerator;

public class ServerDomainTestGenerator : IServerDomainTestGenerator
{
    private readonly ILogger<ServerDomainTestGenerator> logger;
    private readonly INugetPackageReferenceProvider nugetPackageReferenceProvider;
    private readonly string apiProjectName;
    private readonly string domainProjectName;
    private readonly OpenApiDocument openApiDocument;
    private readonly GeneratorSettings settings;

    public ServerDomainTestGenerator(
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
        ArgumentNullException.ThrowIfNull(generatorSettings);

        logger = loggerFactory.CreateLogger<ServerDomainTestGenerator>();
        this.nugetPackageReferenceProvider = nugetPackageReferenceProvider;
        this.apiProjectName = apiProjectName;
        this.domainProjectName = domainProjectName;
        this.openApiDocument = openApiDocument;
        settings = generatorSettings;
    }

    public async Task ScaffoldProjectFile()
    {
        var packageReferences = await nugetPackageReferenceProvider.GetPackageReferencesForTestDomainProjectForMvc();

        var itemGroupPackageReferences = packageReferences
            .Select(packageReference => new ItemGroupParameter(
                "PackageReference",
                [
                    new("Include", packageReference.PackageId),
                    new("Version", packageReference.PackageVersion),
                ],
                Value: packageReference.SubElements))
            .ToImmutableList();

        var projectFileParameters = new ProjectFileParameters(
            "Microsoft.NET.Sdk",
            [
                [
                    new("TargetFramework", Attributes: null, "net8.0"),
                ],
            ],
            [
                itemGroupPackageReferences,
                [
                    new(
                        "ProjectReference",
                        [
                            new("Include", @$"..\..\src\{apiProjectName}\{apiProjectName}.csproj"),
                        ],
                        Value: null),
                    new(
                        "ProjectReference",
                        [
                            new("Include", @$"..\..\src\{domainProjectName}\{domainProjectName}.csproj"),
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

    public void ScaffoldHandlers()
    {
        foreach (var urlPath in openApiDocument.Paths)
        {
            var apiGroupName = urlPath.GetApiGroupName();

            var handlersLocation = LocationFactory.CreateWithApiGroupName(apiGroupName, settings.HandlersLocation);

            var fullNamespace = NamespaceFactory.Create(settings.ProjectName, handlersLocation);

            foreach (var openApiOperation in urlPath.Value.Operations)
            {
                var classParameters = ContentGeneratorServerHandlerParametersTestFactory.CreateForCustomTest(
                    fullNamespace,
                    openApiOperation.Value);

                var contentGenerator = new GenerateContentForClass(
                    new CodeDocumentationTagsGenerator(),
                    classParameters);

                var content = contentGenerator.Generate();

                var contentWriter = new ContentWriter(logger);
                contentWriter.Write(
                    settings.ProjectPath,
                    FileInfoFactory.Create(settings.ProjectPath, handlersLocation, $"{classParameters.TypeName}.cs"),
                    ContentWriterArea.Test,
                    content,
                    overrideIfExist: false);
            }
        }
    }

    public void MaintainGlobalUsings(
        bool usingCodingRules,
        bool removeNamespaceGroupSeparatorInGlobalUsings)
    {
        var requiredUsings = new List<string>
        {
            "Xunit",
        };

        GlobalUsingsHelper.CreateOrUpdate(
            logger,
            ContentWriterArea.Test,
            settings.ProjectPath,
            requiredUsings,
            removeNamespaceGroupSeparatorInGlobalUsings);
    }
}