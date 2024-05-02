// ReSharper disable ArrangeObjectCreationWhenTypeNotEvident
namespace Atc.Rest.ApiGenerator.Framework.Mvc.ProjectGenerator;

public class ServerDomainTestGenerator : IServerDomainTestGenerator
{
    private readonly ILogger<ServerDomainTestGenerator> logger;
    private readonly string projectName;
    private readonly string apiProjectName;
    private readonly string domainProjectName;
    private readonly DirectoryInfo projectPath;
    private readonly OpenApiDocument openApiDocument;

    public ServerDomainTestGenerator(
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

        logger = loggerFactory.CreateLogger<ServerDomainTestGenerator>();
        this.projectName = projectName;
        this.apiProjectName = apiProjectName;
        this.domainProjectName = domainProjectName;
        this.projectPath = projectPath;
        this.openApiDocument = openApiDocument;
    }

    public void ScaffoldProjectFile()
    {
        var projectFileParameters = new ProjectFileParameters(
            "Microsoft.NET.Sdk",
            [
                new List<PropertyGroupParameter>
                {
                    new("TargetFramework", Attributes: null, "net8.0"),
                },
            ],
            [
                new List<ItemGroupParameter>
                {
                    new(
                        "PackageReference",
                        [
                            new("Include", "Atc.XUnit"),
                            new("Version", "2.0.472"),
                        ],
                        Value: null),
                    new(
                        "PackageReference",
                        [
                            new("Include", "AutoFixture"),
                            new("Version", "4.18.1"),
                        ],
                        Value: null),
                    new(
                        "PackageReference",
                        [
                            new("Include", "AutoFixture.AutoNSubstitute"),
                            new("Version", "4.18.1"),
                        ],
                        Value: null),
                    new(
                        "PackageReference",
                        [
                            new("Include", "AutoFixture.Xunit2"),
                            new("Version", "4.18.1"),
                        ],
                        Value: null),
                    new(
                        "PackageReference",
                        [
                            new("Include", "FluentAssertions"),
                            new("Version", "6.12.0"),
                        ],
                        Value: null),
                    new(
                        "PackageReference",
                        [
                            new("Include", "Microsoft.NET.Test.Sdk"),
                            new("Version", "17.9.0"),
                        ],
                        Value: null),
                    new(
                        "PackageReference",
                        [
                            new("Include", "NSubstitute"),
                            new("Version", "5.1.0"),
                        ],
                        Value: null),
                    new(
                        "PackageReference",
                        [
                            new("Include", "xunit"),
                            new("Version", "2.8.0"),
                        ],
                        Value: null),
                    new(
                        "PackageReference",
                        [
                            new("Include", "xunit.runner.visualstudio"),
                            new("Version", "2.8.0"),
                        ],
                        Value: "<PrivateAssets>all</PrivateAssets><IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>"),
                },
                new List<ItemGroupParameter>
                {
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

    public void GenerateHandlers()
    {
        foreach (var urlPath in openApiDocument.Paths)
        {
            var apiGroupName = urlPath.GetApiGroupName();

            foreach (var openApiOperation in urlPath.Value.Operations)
            {
                var fullNamespace = $"{projectName}.{ContentGeneratorConstants.Handlers}.{apiGroupName}";

                var classParameters = ContentGeneratorServerHandlerParametersFactory.CreateForCustomTest(
                    fullNamespace,
                    openApiOperation.Value);

                var contentGenerator = new GenerateContentForClass(
                    new CodeDocumentationTagsGenerator(),
                    classParameters);

                var content = contentGenerator.Generate();

                var contentWriter = new ContentWriter(logger);
                contentWriter.Write(
                    projectPath,
                    projectPath.CombineFileInfo(
                        ContentGeneratorConstants.Handlers,
                        apiGroupName,
                        $"{classParameters.TypeName}.cs"),
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
        var requiredUsings = new List<string>();

        if (!usingCodingRules)
        {
            requiredUsings.Add("Xunit");
        }

        GlobalUsingsHelper.CreateOrUpdate(
            logger,
            ContentWriterArea.Test,
            projectPath,
            requiredUsings,
            removeNamespaceGroupSeparatorInGlobalUsings);
    }
}