// ReSharper disable ArrangeObjectCreationWhenTypeNotEvident
namespace Atc.Rest.ApiGenerator.Framework.Mvc.ProjectGenerator;

public class ServerHostTestGenerator : IServerHostTestGenerator
{
    private readonly ILogger<ServerHostTestGenerator> logger;
    private readonly Version apiGeneratorVersion;
    private readonly string projectName;
    private readonly string hostProjectName;
    private readonly string apiProjectName;
    private readonly string domainProjectName;
    private readonly DirectoryInfo projectPath;
    private readonly OpenApiDocument openApiDocument;
    private readonly IList<ApiOperation> operationSchemaMappings;

    public ServerHostTestGenerator(
        ILoggerFactory loggerFactory,
        Version apiGeneratorVersion,
        string projectName,
        string hostProjectName,
        string apiProjectName,
        string domainProjectName,
        DirectoryInfo projectPath,
        OpenApiDocument openApiDocument,
        IList<ApiOperation> operationSchemaMappings)
    {
        ArgumentNullException.ThrowIfNull(loggerFactory);
        ArgumentNullException.ThrowIfNull(apiGeneratorVersion);
        ArgumentNullException.ThrowIfNull(projectName);
        ArgumentNullException.ThrowIfNull(hostProjectName);
        ArgumentNullException.ThrowIfNull(apiProjectName);
        ArgumentNullException.ThrowIfNull(domainProjectName);
        ArgumentNullException.ThrowIfNull(projectPath);
        ArgumentNullException.ThrowIfNull(openApiDocument);
        ArgumentNullException.ThrowIfNull(operationSchemaMappings);

        logger = loggerFactory.CreateLogger<ServerHostTestGenerator>();
        this.apiGeneratorVersion = apiGeneratorVersion;
        this.projectName = projectName;
        this.hostProjectName = hostProjectName;
        this.apiProjectName = apiProjectName;
        this.domainProjectName = domainProjectName;
        this.projectPath = projectPath;
        this.openApiDocument = openApiDocument;
        this.operationSchemaMappings = operationSchemaMappings;
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
                            new("Include", "Atc.Rest.FluentAssertions"),
                            new("Version", "2.0.472"),
                        ],
                        Value: null),
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
                            new("Include", "Microsoft.AspNetCore.Mvc.Testing"),
                            new("Version", "8.0.4"),
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

    public void ScaffoldAppSettingsIntegrationTestFile()
    {
        var contentGenerator = new ContentGenerators.Server.ContentGeneratorServerAppSettingsIntegrationTest();

        var content = contentGenerator.Generate();

        var contentWriter = new ContentWriter(logger);
        contentWriter.Write(
            projectPath,
            projectPath.CombineFileInfo("appsettings.integrationtest.json"),
            ContentWriterArea.Test,
            content,
            overrideIfExist: false);
    }

    public void GenerateWebApiStartupFactoryFile()
    {
        var contentGeneratorServerWebApiStartupFactoryParameters = ContentGeneratorServerWebApiStartupFactoryParametersFactory.Create(
            projectName);

        var contentGenerator = new ContentGenerators.Server.ContentGeneratorServerWebApiStartupFactory(
            new GeneratedCodeHeaderGenerator(new GeneratedCodeGeneratorParameters(apiGeneratorVersion)),
            new GeneratedCodeAttributeGenerator(new GeneratedCodeGeneratorParameters(apiGeneratorVersion)),
            new CodeDocumentationTagsGenerator(),
            contentGeneratorServerWebApiStartupFactoryParameters);

        var content = contentGenerator.Generate();

        var contentWriter = new ContentWriter(logger);
        contentWriter.Write(
            projectPath,
            projectPath.CombineFileInfo("WebApiStartupFactory.cs"),
            ContentWriterArea.Test,
            content,
            overrideIfExist: false);
    }

    public void GenerateWebApiControllerBaseTestFile()
    {
        var contentGenerator = new ContentGenerators.Server.ContentGeneratorServerWebApiControllerBaseTest(
            new GeneratedCodeHeaderGenerator(new GeneratedCodeGeneratorParameters(apiGeneratorVersion)),
            new GeneratedCodeAttributeGenerator(new GeneratedCodeGeneratorParameters(apiGeneratorVersion)),
            new ContentGeneratorBaseParameters(projectName));

        var content = contentGenerator.Generate();

        var contentWriter = new ContentWriter(logger);
        contentWriter.Write(
            projectPath,
            projectPath.CombineFileInfo("WebApiControllerBaseTest.cs"),
            ContentWriterArea.Test,
            content,
            overrideIfExist: false);
    }

    public void GenerateEndpointHandlerStubs()
    {
        var codeHeaderGenerator = new GeneratedCodeHeaderGenerator(
            new GeneratedCodeGeneratorParameters(
                apiGeneratorVersion));

        var codeGeneratorContentHeader = codeHeaderGenerator.Generate();

        var codeGeneratorAttribute = new AttributeParameters(
            "GeneratedCode",
            $"\"{ContentWriterConstants.ApiGeneratorName}\", \"{apiGeneratorVersion}\"");

        foreach (var openApiPath in openApiDocument.Paths)
        {
            var apiGroupName = openApiPath.GetApiGroupName();

            foreach (var openApiOperation in openApiPath.Value.Operations)
            {
                var fullNamespace = $"{projectName}.{ContentGeneratorConstants.Endpoints}.{apiGroupName}";

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
                    projectPath,
                    projectPath.CombineFileInfo(ContentGeneratorConstants.Endpoints, apiGroupName, $"{classParameters.TypeName}.cs"),
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
                var fullNamespace = $"{projectName}.{ContentGeneratorConstants.Endpoints}.{apiGroupName}";

                var classParameters = ContentGeneratorServerTestEndpointTestsParametersFactory.Create(
                    fullNamespace,
                    openApiOperation.Value);

                var contentGenerator = new GenerateContentForClass(
                    new CodeDocumentationTagsGenerator(),
                    classParameters);

                var content = contentGenerator.Generate();

                var contentWriter = new ContentWriter(logger);
                contentWriter.Write(
                    projectPath,
                    projectPath.CombineFileInfo(ContentGeneratorConstants.Endpoints, apiGroupName, $"{classParameters.TypeName}.cs"),
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
                "Microsoft.AspNetCore.Hosting",
                "Microsoft.AspNetCore.Http",
                "Microsoft.AspNetCore.TestHost",
                "Microsoft.AspNetCore.Mvc.Testing",
                "Microsoft.Extensions.Configuration",
                "Microsoft.Extensions.DependencyInjection",
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

        if (!usingCodingRules)
        {
            requiredUsings.Add("AutoFixture");
            requiredUsings.Add("Xunit");
        }

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
            projectPath,
            requiredUsings,
            removeNamespaceGroupSeparatorInGlobalUsings);
    }
}