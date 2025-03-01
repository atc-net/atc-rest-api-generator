namespace Atc.Rest.ApiGenerator.Framework.Minimal.ProjectGenerator;

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
        var packageReferences = await nugetPackageReferenceProvider.GetPackageReferencesForTestHostProjectForMinimalApi();

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
                    new("TargetFramework", Attributes: null, "net9.0"),
                ],
                [
                    new("DocumentationFile", Attributes: null, @$"bin\Debug\net9.0\{settings.ProjectName}.xml"),
                    new("NoWarn", Attributes: null, "$(NoWarn);1573;1591;1701;1702;1712;8618;NU1603;NU1608;"),
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
        // TODO: Implement if needed
        ////var contentGenerator = new ContentGenerators.Server.ContentGeneratorServerAppSettingsIntegrationTest();

        ////var content = contentGenerator.Generate();

        ////var contentWriter = new ContentWriter(logger);
        ////contentWriter.Write(
        ////    projectPath,
        ////    projectPath.CombineFileInfo("appsettings.integrationtest.json"),
        ////    ContentWriterArea.Test,
        ////    content,
        ////    overrideIfExist: false);
    }

    public void GenerateWebApiStartupFactoryFile()
    {
        // TODO: Implement if needed
        ////var contentGeneratorServerWebApiStartupFactoryParameters = ContentGeneratorServerWebApiStartupFactoryParametersFactory.Create(
        ////    projectName);

        ////var contentGenerator = new ContentGenerators.Server.ContentGeneratorServerWebApiStartupFactory(
        ////    new GeneratedCodeHeaderGenerator(new GeneratedCodeGeneratorParameters(apiGeneratorVersion)),
        ////    new GeneratedCodeAttributeGenerator(new GeneratedCodeGeneratorParameters(apiGeneratorVersion)),
        ////    new CodeDocumentationTagsGenerator(),
        ////    contentGeneratorServerWebApiStartupFactoryParameters);

        ////var content = contentGenerator.Generate();

        ////var contentWriter = new ContentWriter(logger);
        ////contentWriter.Write(
        ////    projectPath,
        ////    projectPath.CombineFileInfo("WebApiStartupFactory.cs"),
        ////    ContentWriterArea.Test,
        ////    content,
        ////    overrideIfExist: false);
    }

    public void GenerateWebApiControllerBaseTestFile()
    {
        // TODO: Implement if needed
        ////var contentGenerator = new ContentGenerators.Server.ContentGeneratorServerWebApiControllerBaseTest(
        ////    new GeneratedCodeHeaderGenerator(new GeneratedCodeGeneratorParameters(apiGeneratorVersion)),
        ////    new GeneratedCodeAttributeGenerator(new GeneratedCodeGeneratorParameters(apiGeneratorVersion)),
        ////    new ContentGeneratorBaseParameters(projectName));

        ////var content = contentGenerator.Generate();

        ////var contentWriter = new ContentWriter(logger);
        ////contentWriter.Write(
        ////    projectPath,
        ////    projectPath.CombineFileInfo("WebApiControllerBaseTest.cs"),
        ////    ContentWriterArea.Test,
        ////    content,
        ////    overrideIfExist: false);
    }

    public void GenerateEndpointHandlerStubs()
    {
        foreach (var openApiPath in openApiDocument.Paths)
        {
            var apiGroupName = openApiPath.GetApiGroupName();

            var endpointsLocation = LocationFactory.CreateWithApiGroupName(apiGroupName, settings.EndpointsLocation);
            var endpointsNamespace = LocationFactory.CreateWithApiGroupName(apiGroupName, settings.EndpointsNamespace);
            var contractsNamespace = LocationFactory.CreateWithApiGroupName(apiGroupName, settings.ContractsNamespace);

            var fullNamespace = NamespaceFactory.Create(settings.ProjectName, endpointsNamespace);
            var fullContractNamespace = NamespaceFactory.Create(settings.ProjectName.Replace("Tests", "Generated", StringComparison.Ordinal), contractsNamespace);

            foreach (var openApiOperation in openApiPath.Value.Operations)
            {
                if (openApiOperation.Value.Deprecated)
                {
                    continue;
                }

                var classParameters = ContentGeneratorServerTestEndpointHandlerStubParametersFactory.Create(
                    codeGeneratorContentHeader,
                    fullNamespace,
                    codeGeneratorAttribute,
                    openApiPath.Value,
                    openApiOperation.Value,
                    fullContractNamespace,
                    AspNetOutputType.MinimalApi);

                var contentGenerator = new GenerateContentForClass(
                    new CodeDocumentationTagsGenerator(),
                    classParameters);

                var content = contentGenerator.Generate();

                var contentWriter = new ContentWriter(logger);
                contentWriter.Write(
                    settings.ProjectPath,
                    FileInfoFactory.Create(settings.ProjectPath, endpointsLocation, $"{classParameters.TypeName}.cs"),
                    ContentWriterArea.Test,
                    content);
            }
        }
    }

    public void GenerateEndpointTests()
    {
        // TODO: Implement then WebApiControllerBaseTest and WebApiStartupFactory is implemented
        ////foreach (var openApiPath in openApiDocument.Paths)
        ////{
        ////    if (openApiOperation.Value.Deprecated)
        ////    {
        ////        continue;
        ////    }
        ////
        ////    var apiGroupName = openApiPath.GetApiGroupName();

        ////    foreach (var openApiOperation in openApiPath.Value.Operations)
        ////    {
        ////        var fullNamespace = $"{projectName}.{ContentGeneratorConstants.Endpoints}.{apiGroupName}";

        ////        var classParameters = ContentGeneratorServerTestEndpointTestsParametersFactory.Create(
        ////            fullNamespace,
        ////            openApiOperation.Value);

        ////        var contentGenerator = new GenerateContentForClass(
        ////            new CodeDocumentationTagsGenerator(),
        ////            classParameters);

        ////        var content = contentGenerator.Generate();

        ////        var contentWriter = new ContentWriter(logger);
        ////        contentWriter.Write(
        ////            projectPath,
        ////            projectPath.CombineFileInfo(ContentGeneratorConstants.Endpoints, apiGroupName, $"{classParameters.TypeName}.cs"),
        ////            ContentWriterArea.Test,
        ////            content);
        ////    }
        ////}
    }

    public void MaintainGlobalUsings(
        bool usingCodingRules,
        bool removeNamespaceGroupSeparatorInGlobalUsings)
    {
        var requiredUsings = new List<string>
            {
                "System.CodeDom.Compiler",
                "AutoFixture",
            };

        //// TODO: Maybe some is needed?
        ////if (false)
        ////{
        ////    requiredUsings.Add("System.Reflection");
        ////    requiredUsings.Add("System.Text");
        ////    requiredUsings.Add("System.Text.Json");
        ////    requiredUsings.Add("System.Text.Json.Serialization");
        ////    requiredUsings.Add("Atc.Rest.Options");
        ////    requiredUsings.Add("Atc.XUnit");
        ////    requiredUsings.Add("Microsoft.AspNetCore.Hosting");
        ////    requiredUsings.Add("Microsoft.AspNetCore.Http");
        ////    requiredUsings.Add("Microsoft.Extensions.Configuration");
        ////    requiredUsings.Add("Microsoft.Extensions.DependencyInjection");
        ////    requiredUsings.Add("Xunit");
        ////}

        if (openApiDocument.IsUsingRequiredForSystemText(settings.IncludeDeprecatedOperations))
        {
            requiredUsings.Add("System.Text");
        }

        if (openApiDocument.IsUsingRequiredForAtcRestResults())
        {
            requiredUsings.Add("Atc.Rest.Results");
        }

        if (operationSchemaMappings.Any(apiOperation => apiOperation.Model.IsShared))
        {
            requiredUsings.Add(NamespaceFactory.Create(apiProjectName, NamespaceFactory.CreateWithoutTemplateForApiGroupName(settings.ContractsNamespace)));
        }

        var apiGroupNames = openApiDocument.GetApiGroupNames();

        foreach (var apiGroupName in apiGroupNames)
        {
            if (apiGroupName.Equals("Tasks", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            requiredUsings.Add(NamespaceFactory.Create(apiProjectName, NamespaceFactory.CreateWithApiGroupName(apiGroupName, settings.ContractsNamespace)));
        }

        GlobalUsingsHelper.CreateOrUpdate(
            logger,
            ContentWriterArea.Test,
            settings.ProjectPath,
            requiredUsings,
            removeNamespaceGroupSeparatorInGlobalUsings);
    }
}