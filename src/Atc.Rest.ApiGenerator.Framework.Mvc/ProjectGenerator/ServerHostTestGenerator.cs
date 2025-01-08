// ReSharper disable ArrangeObjectCreationWhenTypeNotEvident
// ReSharper disable StringLiteralTypo
namespace Atc.Rest.ApiGenerator.Framework.Mvc.ProjectGenerator;

public class ServerHostTestGenerator : IServerHostTestGenerator
{
    private readonly ILogger<ServerHostTestGenerator> logger;
    private readonly INugetPackageReferenceProvider nugetPackageReferenceProvider;
    private readonly string hostProjectName;
    private readonly string apiProjectName;
    private readonly string domainProjectName;
    private readonly OpenApiDocument openApiDocument;
    private readonly IList<ApiOperation> operationSchemaMappings;
    private readonly string codeGeneratorContentHeader;
    private readonly AttributeParameters codeGeneratorAttribute;
    private readonly GeneratorSettings settings;

    public ServerHostTestGenerator(
        ILoggerFactory loggerFactory,
        INugetPackageReferenceProvider nugetPackageReferenceProvider,
        string hostProjectName,
        string apiProjectName,
        string domainProjectName,
        OpenApiDocument openApiDocument,
        IList<ApiOperation> operationSchemaMappings,
        GeneratorSettings generatorSettings)
    {
        ArgumentNullException.ThrowIfNull(loggerFactory);
        ArgumentNullException.ThrowIfNull(nugetPackageReferenceProvider);
        ArgumentNullException.ThrowIfNull(hostProjectName);
        ArgumentNullException.ThrowIfNull(apiProjectName);
        ArgumentNullException.ThrowIfNull(domainProjectName);
        ArgumentNullException.ThrowIfNull(openApiDocument);
        ArgumentNullException.ThrowIfNull(operationSchemaMappings);
        ArgumentNullException.ThrowIfNull(generatorSettings);

        logger = loggerFactory.CreateLogger<ServerHostTestGenerator>();
        this.nugetPackageReferenceProvider = nugetPackageReferenceProvider;
        this.hostProjectName = hostProjectName;
        this.apiProjectName = apiProjectName;
        this.domainProjectName = domainProjectName;
        this.openApiDocument = openApiDocument;
        this.operationSchemaMappings = operationSchemaMappings;
        settings = generatorSettings;

        codeGeneratorContentHeader = GeneratedCodeHeaderGeneratorFactory
            .Create(settings.Version)
            .Generate();
        codeGeneratorAttribute = AttributeParametersFactory
            .CreateGeneratedCode(settings.Version);
    }

    public async Task ScaffoldProjectFile()
    {
        var packageReferences = await nugetPackageReferenceProvider.GetPackageReferencesForTestHostProjectForMvc();

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
            "Microsoft.NET.Sdk",
            [
                [
                    new("TargetFramework", Attributes: null, "net8.0"),
                ],
                [
                    new("DocumentationFile", Attributes: null, @$"bin\Debug\net8.0\{settings.ProjectName}.xml"),
                    new("NoWarn", Attributes: null, "$(NoWarn);1573;1591;1701;1702;1712;8618;"),
                ],
            ],
            [
                itemGroupPackageReferences,
                [
                    new(
                        "ProjectReference",
                        [
                            new("Include", @$"..\..\src\{hostProjectName}\{hostProjectName}.csproj"),
                        ],
                        Value: null),
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

    public void ScaffoldAppSettingsIntegrationTestFile()
    {
        var contentGenerator = new ContentGenerators.ContentGeneratorServerAppSettingsIntegrationTest();

        var content = contentGenerator.Generate();

        var contentWriter = new ContentWriter(logger);
        contentWriter.Write(
            settings.ProjectPath,
            settings.ProjectPath.CombineFileInfo("appsettings.integrationtest.json"),
            ContentWriterArea.Test,
            content,
            overrideIfExist: false);
    }

    public void GenerateWebApiStartupFactoryFile()
    {
        var contentGeneratorServerWebApiStartupFactoryParameters = ContentGeneratorServerWebApiStartupFactoryParametersFactory.Create(
            settings.ProjectName);

        var contentGenerator = new ContentGenerators.ContentGeneratorServerWebApiStartupFactory(
            new GeneratedCodeHeaderGenerator(new GeneratedCodeGeneratorParameters(settings.Version)),
            new GeneratedCodeAttributeGenerator(new GeneratedCodeGeneratorParameters(settings.Version)),
            new CodeDocumentationTagsGenerator(),
            contentGeneratorServerWebApiStartupFactoryParameters);

        var content = contentGenerator.Generate();

        var contentWriter = new ContentWriter(logger);
        contentWriter.Write(
            settings.ProjectPath,
            settings.ProjectPath.CombineFileInfo("WebApiStartupFactory.cs"),
            ContentWriterArea.Test,
            content,
            overrideIfExist: false);
    }

    public void GenerateWebApiControllerBaseTestFile()
    {
        var contentGenerator = new ContentGenerators.ContentGeneratorServerWebApiControllerBaseTest(
            new GeneratedCodeHeaderGenerator(new GeneratedCodeGeneratorParameters(settings.Version)),
            new GeneratedCodeAttributeGenerator(new GeneratedCodeGeneratorParameters(settings.Version)),
            new ContentGeneratorBaseParameters(settings.ProjectName));

        var content = contentGenerator.Generate();

        var contentWriter = new ContentWriter(logger);
        contentWriter.Write(
            settings.ProjectPath,
            settings.ProjectPath.CombineFileInfo("WebApiControllerBaseTest.cs"),
            ContentWriterArea.Test,
            content,
            overrideIfExist: false);
    }

    public void GenerateEndpointHandlerStubs()
    {
        foreach (var openApiPath in openApiDocument.Paths)
        {
            var apiGroupName = openApiPath.GetApiGroupName();

            foreach (var openApiOperation in openApiPath.Value.Operations)
            {
                if (openApiOperation.Value.Deprecated && !settings.IncludeDeprecatedOperations)
                {
                    continue;
                }

                var fullNamespace = $"{settings.ProjectName}.{ContentGeneratorConstants.Endpoints}.{apiGroupName}";

                var classParameters = ContentGeneratorServerTestEndpointHandlerStubParametersFactory.Create(
                    codeGeneratorContentHeader,
                    fullNamespace,
                    codeGeneratorAttribute,
                    openApiPath.Value,
                    openApiOperation.Value);

                var contentGenerator = new GenerateContentForClass(
                    new CodeDocumentationTagsGenerator(),
                    classParameters);

                var content = contentGenerator.Generate();

                var contentWriter = new ContentWriter(logger);
                contentWriter.Write(
                    settings.ProjectPath,
                    settings.ProjectPath.CombineFileInfo(ContentGeneratorConstants.Endpoints, apiGroupName, $"{classParameters.TypeName}.cs"),
                    ContentWriterArea.Test,
                    content);
            }
        }
    }

    public void GenerateEndpointTests()
    {
        foreach (var openApiPath in openApiDocument.Paths)
        {
            var apiGroupName = openApiPath.GetApiGroupName();

            foreach (var openApiOperation in openApiPath.Value.Operations)
            {
                if (openApiOperation.Value.Deprecated && !settings.IncludeDeprecatedOperations)
                {
                    continue;
                }

                var fullNamespace = $"{settings.ProjectName}.{ContentGeneratorConstants.Endpoints}.{apiGroupName}";

                var classParameters = ContentGeneratorServerTestEndpointTestsParametersFactory.Create(
                    fullNamespace,
                    openApiOperation.Value);

                var contentGenerator = new GenerateContentForClass(
                    new CodeDocumentationTagsGenerator(),
                    classParameters);

                var content = contentGenerator.Generate();

                var contentWriter = new ContentWriter(logger);
                contentWriter.Write(
                    settings.ProjectPath,
                    settings.ProjectPath.CombineFileInfo(ContentGeneratorConstants.Endpoints, apiGroupName, $"{classParameters.TypeName}.cs"),
                    ContentWriterArea.Test,
                    content);
            }
        }
    }

    public void MaintainGlobalUsings(
        bool usingCodingRules,
        bool removeNamespaceGroupSeparatorInGlobalUsings)
    {
        var requiredUsings = new List<string>
            {
                "System.CodeDom.Compiler",
                "System.Text",
                "System.Text.Json",
                "System.Text.Json.Serialization",
                "System.Reflection",
                "Atc.XUnit",
                "Atc.Rest.Options",
                "AutoFixture",
                "Microsoft.AspNetCore.Hosting",
                "Microsoft.AspNetCore.Http",
                "Microsoft.AspNetCore.TestHost",
                "Microsoft.AspNetCore.Mvc.Testing",
                "Microsoft.Extensions.Configuration",
                "Microsoft.Extensions.DependencyInjection",
                "Xunit",
                apiProjectName,
            };

        if (openApiDocument.IsUsingRequiredForAtcRestResults())
        {
            requiredUsings.Add("Atc.Rest.Results");
        }

        if (operationSchemaMappings.Any(apiOperation => apiOperation.Model.IsShared))
        {
            requiredUsings.Add($"{apiProjectName}.Contracts");
        }

        requiredUsings.Add("AutoFixture");
        requiredUsings.Add("Xunit");

        var apiGroupNames = openApiDocument.GetApiGroupNames();

        foreach (var apiGroupName in apiGroupNames)
        {
            if (apiGroupName.Equals("Tasks", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            requiredUsings.Add($"{apiProjectName}.Contracts.{apiGroupName}");
        }

        GlobalUsingsHelper.CreateOrUpdate(
            logger,
            ContentWriterArea.Test,
            settings.ProjectPath,
            requiredUsings,
            removeNamespaceGroupSeparatorInGlobalUsings);
    }
}