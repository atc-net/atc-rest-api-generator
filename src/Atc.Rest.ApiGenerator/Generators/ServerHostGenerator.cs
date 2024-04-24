// ReSharper disable ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
// ReSharper disable SuggestBaseTypeForParameter
// ReSharper disable ReturnTypeCanBeEnumerable.Local
// ReSharper disable StringLiteralTypo
namespace Atc.Rest.ApiGenerator.Generators;

public class ServerHostGenerator
{
    private readonly ILogger logger;
    private readonly IApiOperationExtractor apiOperationExtractor;
    private readonly INugetPackageReferenceProvider nugetPackageReferenceProvider;
    private readonly HostProjectOptions projectOptions;

    private readonly IServerHostGenerator serverHostGeneratorMvc;
    private readonly IServerHostGenerator serverHostGeneratorMinimalApi;
    private readonly IServerHostTestGenerator? serverHostTestGeneratorMvc;

    private readonly string codeGeneratorContentHeader;
    private readonly AttributeParameters codeGeneratorAttribute;

    public ServerHostGenerator(
        ILoggerFactory loggerFactory,
        IApiOperationExtractor apiOperationExtractor,
        INugetPackageReferenceProvider nugetPackageReferenceProvider,
        HostProjectOptions projectOptions)
    {
        ArgumentNullException.ThrowIfNull(loggerFactory);

        logger = loggerFactory.CreateLogger<ServerDomainGenerator>();
        this.apiOperationExtractor = apiOperationExtractor ?? throw new ArgumentNullException(nameof(apiOperationExtractor));
        this.nugetPackageReferenceProvider = nugetPackageReferenceProvider ?? throw new ArgumentNullException(nameof(nugetPackageReferenceProvider));
        this.projectOptions = projectOptions ?? throw new ArgumentNullException(nameof(projectOptions));

        serverHostGeneratorMvc = new Framework.Mvc.ProjectGenerator.ServerHostGenerator(
            loggerFactory,
            projectOptions.ApiGeneratorVersion,
            projectOptions.ProjectName,
            projectOptions.PathForSrcGenerate,
            projectOptions.Document)
        {
            UseRestExtended = projectOptions.UseRestExtended,
        };

        serverHostGeneratorMinimalApi = new Framework.Minimal.ProjectGenerator.ServerHostGenerator(
            loggerFactory,
            projectOptions.ApiGeneratorVersion,
            projectOptions.ProjectName,
            projectOptions.PathForSrcGenerate,
            projectOptions.Document);

        if (projectOptions.PathForTestGenerate is not null)
        {
            serverHostTestGeneratorMvc = new Framework.Mvc.ProjectGenerator.ServerHostTestGenerator(
                loggerFactory,
                projectOptions.ApiGeneratorVersion,
                projectOptions.ProjectName,
                projectOptions.PathForTestGenerate,
                projectOptions.Document);
        }

        // TODO: Optimize codeGeneratorContentHeader & codeGeneratorAttribute
        var codeHeaderGenerator = new GeneratedCodeHeaderGenerator(
        new GeneratedCodeGeneratorParameters(
            projectOptions.ApiGeneratorVersion));
        codeGeneratorContentHeader = codeHeaderGenerator.Generate();

        codeGeneratorAttribute = new AttributeParameters(
            "GeneratedCode",
            $"\"{ContentWriterConstants.ApiGeneratorName}\", \"{projectOptions.ApiGeneratorVersion}\"");
    }

    public bool Generate()
    {
        logger.LogInformation($"{ContentWriterConstants.AreaGenerateCode} Working on server host generation ({projectOptions.ProjectName})");

        if (projectOptions.ApiOptions.Generator.AspNetOutputType == AspNetOutputType.Mvc)
        {
            if (!projectOptions.SetPropertiesAfterValidationsOfProjectReferencesPathAndFilesForMvc(logger))
            {
                return false;
            }
        }
        else
        {
            if (!projectOptions.SetPropertiesAfterValidationsOfProjectReferencesPathAndFilesForMinimalApi(logger))
            {
                return false;
            }
        }

        if (projectOptions.ApiOptions.Generator.AspNetOutputType == AspNetOutputType.Mvc)
        {
            serverHostGeneratorMvc.ScaffoldProgramFile(
                projectOptions.ApiOptions.Generator.SwaggerThemeMode);
            serverHostGeneratorMvc.ScaffoldStartupFile();
            serverHostGeneratorMvc.ScaffoldWebConfig();
            serverHostGeneratorMvc.ScaffoldConfigureSwaggerDocOptions();

            serverHostGeneratorMvc.MaintainGlobalUsings(
                projectOptions.ProjectName.Replace(".Api", ".Domain", StringComparison.Ordinal),
                projectOptions.ApiGroupNames,
                projectOptions.RemoveNamespaceGroupSeparatorInGlobalUsings);
        }
        else
        {
            serverHostGeneratorMinimalApi.ScaffoldProgramFile(
                projectOptions.ApiOptions.Generator.SwaggerThemeMode);
            serverHostGeneratorMinimalApi.ScaffoldWebConfig();
            serverHostGeneratorMinimalApi.ScaffoldConfigureSwaggerDocOptions();
            serverHostGeneratorMinimalApi.ScaffoldServiceCollectionExtensions();
            serverHostGeneratorMinimalApi.ScaffoldServiceWebApplicationExtensions(
                projectOptions.ApiOptions.Generator.SwaggerThemeMode);
            serverHostGeneratorMinimalApi.ScaffoldConfigureSwaggerOptions();

            serverHostGeneratorMinimalApi.MaintainGlobalUsings(
                projectOptions.ProjectName.Replace(".Api", ".Domain", StringComparison.Ordinal),
                projectOptions.ApiGroupNames,
                projectOptions.RemoveNamespaceGroupSeparatorInGlobalUsings);
            serverHostGeneratorMinimalApi.MaintainWwwResources();
        }

        ScaffoldSrc();

        if (projectOptions.PathForTestGenerate is not null)
        {
            var operationSchemaMappings = apiOperationExtractor.Extract(projectOptions.Document);

            logger.LogInformation($"{ContentWriterConstants.AreaGenerateTest} Working on server host unit-test generation ({projectOptions.ProjectName}.Tests)");
            ScaffoldTest();

            if (projectOptions.ApiOptions.Generator.AspNetOutputType == AspNetOutputType.Mvc)
            {
                GenerateTestEndpoints(projectOptions.Document);
            }

            serverHostTestGeneratorMvc?.MaintainGlobalUsings(
                projectOptions.ApiGroupNames,
                projectOptions.UsingCodingRules,
                projectOptions.RemoveNamespaceGroupSeparatorInGlobalUsings,
                operationSchemaMappings);
        }

        return true;
    }

    private void ScaffoldSrc()
    {
        if (!Directory.Exists(projectOptions.PathForSrcGenerate.FullName))
        {
            Directory.CreateDirectory(projectOptions.PathForSrcGenerate.FullName);
        }

        if (projectOptions.PathForSrcGenerate.Exists &&
            projectOptions.ProjectSrcCsProj.Exists)
        {
            var hasUpdates = SolutionAndProjectHelper.EnsureLatestPackageReferencesVersionInProjFile(
                logger,
                projectOptions.ProjectSrcCsProj,
                projectOptions.ProjectSrcCsProjDisplayLocation,
                ProjectType.ServerHost,
                isTestProject: false);
            if (!hasUpdates)
            {
                logger.LogDebug($"{EmojisConstants.FileNotUpdated}   No updates for csproj");
            }
        }
        else
        {
            var projectReferences = new List<FileInfo>();
            if (projectOptions.ApiProjectSrcCsProj is not null)
            {
                projectReferences.Add(projectOptions.ApiProjectSrcCsProj);
            }

            if (projectOptions.DomainProjectSrcCsProj is not null)
            {
                projectReferences.Add(projectOptions.DomainProjectSrcCsProj);
            }

            IList<(string PackageId, string PackageVersion, string? SubElements)>? packageReferencesBaseLineForHostProject = null;
            TaskHelper.RunSync(async () =>
            {
                if (projectOptions.ApiOptions.Generator.AspNetOutputType == AspNetOutputType.Mvc)
                {
                    packageReferencesBaseLineForHostProject = await nugetPackageReferenceProvider.GetPackageReferencesBaseLineForHostProjectForMvc(projectOptions.UseRestExtended);
                }
                else
                {
                    packageReferencesBaseLineForHostProject = await nugetPackageReferenceProvider.GetPackageReferencesBaseLineForHostProjectForMinimalApi();
                }
            });

            SolutionAndProjectHelper.ScaffoldProjFile(
                logger,
                projectOptions.ProjectSrcCsProj,
                projectOptions.ProjectSrcCsProjDisplayLocation,
                ProjectType.ServerHost,
                createAsWeb: true,
                createAsTestProject: false,
                projectOptions.ProjectName,
                "net8.0",
                frameworkReferences: null,
                packageReferencesBaseLineForHostProject,
                projectReferences,
                includeApiSpecification: false,
                usingCodingRules: projectOptions.UsingCodingRules);

            ScaffoldPropertiesLaunchSettingsFile(
                projectOptions.ProjectName,
                projectOptions.PathForSrcGenerate,
                projectOptions.UseRestExtended);
        }
    }

    private void ScaffoldPropertiesLaunchSettingsFile(
        string projectName,
        DirectoryInfo pathForSrcGenerate,
        bool useExtended)
    {
        var propertiesPath = new DirectoryInfo(Path.Combine(pathForSrcGenerate.FullName, "Properties"));
        propertiesPath.Create();

        var resourceName = "Atc.Rest.ApiGenerator.Resources.launchSettings.json";
        if (useExtended)
        {
            resourceName = "Atc.Rest.ApiGenerator.Resources.launchSettingsExtended.json";
        }

        var resourceStream = typeof(ServerHostGenerator).Assembly.GetManifestResourceStream(resourceName);
        var json = resourceStream!.ToStringData();
        json = json.Replace("\"[[PROJECTNAME]]\":", $"\"{projectName}\":", StringComparison.Ordinal);

        var file = propertiesPath.CombineFileInfo("launchSettings.json");

        if (file.Exists)
        {
            logger.LogTrace($"{EmojisConstants.FileNotUpdated}   {file.FullName} nothing to update");
        }
        else
        {
            var contentWriter = new ContentWriter(logger);
            contentWriter.Write(
                projectOptions.PathForSrcGenerate,
                file,
                ContentWriterArea.Src,
                json);
        }
    }

    private void ScaffoldTest()
    {
        if (projectOptions.PathForTestGenerate is null ||
            projectOptions.ProjectTestCsProj is null)
        {
            return;
        }

        if (projectOptions.PathForTestGenerate.Exists &&
            projectOptions.ProjectTestCsProj.Exists)
        {
            var hasUpdates = SolutionAndProjectHelper.EnsureLatestPackageReferencesVersionInProjFile(
                logger,
                projectOptions.ProjectTestCsProj,
                projectOptions.ProjectTestCsProjDisplayLocation,
                ProjectType.ServerHost,
                isTestProject: true);
            if (!hasUpdates)
            {
                logger.LogDebug($"{EmojisConstants.FileNotUpdated}   No updates for csproj");
            }
        }
        else
        {
            if (!Directory.Exists(projectOptions.PathForTestGenerate.FullName))
            {
                Directory.CreateDirectory(projectOptions.PathForTestGenerate.FullName);
            }

            var projectReferences = new List<FileInfo>();
            if (projectOptions.ApiProjectSrcCsProj is not null)
            {
                projectReferences.Add(projectOptions.ProjectSrcCsProj);
                projectReferences.Add(projectOptions.ApiProjectSrcCsProj);
            }

            if (projectOptions.DomainProjectSrcCsProj is not null)
            {
                projectReferences.Add(projectOptions.DomainProjectSrcCsProj);
            }

            IList<(string PackageId, string PackageVersion, string? SubElements)>? packageReferencesBaseLineForTestProject = null;
            TaskHelper.RunSync(async () =>
            {
                packageReferencesBaseLineForTestProject = await nugetPackageReferenceProvider.GetPackageReferencesBaseLineForTestProject(useMvc: true);
            });

            SolutionAndProjectHelper.ScaffoldProjFile(
                logger,
                projectOptions.ProjectTestCsProj,
                projectOptions.ProjectTestCsProjDisplayLocation,
                ProjectType.ServerHost,
                createAsWeb: false,
                createAsTestProject: true,
                $"{projectOptions.ProjectName}.Tests",
                "net8.0",
                frameworkReferences: null,
                packageReferencesBaseLineForTestProject,
                projectReferences,
                includeApiSpecification: true,
                usingCodingRules: projectOptions.UsingCodingRules);
        }

        GenerateTestWebApiStartupFactory();
        GenerateTestWebApiControllerBaseTest();
        ScaffoldAppSettingsIntegrationTest();
    }

    private void GenerateTestEndpoints(
        OpenApiDocument document)
    {
        foreach (var openApiPath in document.Paths)
        {
            var apiGroupName = openApiPath.GetApiGroupName();

            foreach (var openApiOperation in openApiPath.Value.Operations)
            {
                GenerateTestEndpointHandlerStub(apiGroupName, openApiPath.Value, openApiOperation.Value);
                GenerateTestEndpointTests(apiGroupName, openApiOperation.Value);
            }
        }
    }

    private void GenerateTestEndpointHandlerStub(
        string apiGroupName,
        OpenApiPathItem openApiPath,
        OpenApiOperation openApiOperation)
    {
        var fullNamespace = $"{projectOptions.ProjectName}.{ContentGeneratorConstants.Tests}.{ContentGeneratorConstants.Endpoints}.{apiGroupName}";

        // Generate
        var classParameters = ContentGeneratorServerTestEndpointHandlerStubParametersFactory.Create(
            codeGeneratorContentHeader,
            fullNamespace,
            codeGeneratorAttribute,
            openApiPath,
            openApiOperation);

        var contentGeneratorClass = new GenerateContentForClass(
            new CodeDocumentationTagsGenerator(),
            classParameters);

        var classContent = contentGeneratorClass.Generate();

        // Write
        var pathA = Path.Combine(projectOptions.PathForTestGenerate!.FullName, ContentGeneratorConstants.Endpoints);
        var pathB = Path.Combine(pathA, apiGroupName);
        var fileName = $"{classParameters.TypeName}.cs";
        var file = new FileInfo(Path.Combine(pathB, fileName));

        var contentWriter = new ContentWriter(logger);
        contentWriter.Write(
            projectOptions.PathForTestGenerate,
            file,
            ContentWriterArea.Test,
            classContent);
    }

    private void GenerateTestEndpointTests(
        string apiGroupName,
        OpenApiOperation openApiOperation)
    {
        var fullNamespace = $"{projectOptions.ProjectName}.{ContentGeneratorConstants.Tests}.{ContentGeneratorConstants.Endpoints}.{apiGroupName}";

        // Generate
        var classParameters = ContentGeneratorServerTestEndpointTestsParametersFactory.Create(
            fullNamespace,
            openApiOperation);

        var contentGeneratorClass = new GenerateContentForClass(
            new CodeDocumentationTagsGenerator(),
            classParameters);

        var classContent = contentGeneratorClass.Generate();

        // Write
        var pathA = Path.Combine(projectOptions.PathForTestGenerate!.FullName, ContentGeneratorConstants.Endpoints);
        var pathB = Path.Combine(pathA, apiGroupName);
        var fileName = $"{classParameters.TypeName}.cs";
        var file = new FileInfo(Path.Combine(pathB, fileName));

        var contentWriter = new ContentWriter(logger);
        contentWriter.Write(
            projectOptions.PathForTestGenerate,
            file,
            ContentWriterArea.Test,
            classContent,
            overrideIfExist: false);
    }

    private void GenerateTestWebApiStartupFactory()
    {
        var fullNamespace = $"{projectOptions.ProjectName}.Tests";

        var contentGeneratorServerWebApiStartupFactoryParameters = ContentGeneratorServerWebApiStartupFactoryParametersFactory.Create(
            fullNamespace);

        var contentGenerator = new Framework.Mvc.ContentGenerators.Server.ContentGeneratorServerWebApiStartupFactory(
            new GeneratedCodeHeaderGenerator(new GeneratedCodeGeneratorParameters(projectOptions.ApiGeneratorVersion)),
            new GeneratedCodeAttributeGenerator(new GeneratedCodeGeneratorParameters(projectOptions.ApiGeneratorVersion)),
            new CodeDocumentationTagsGenerator(),
            contentGeneratorServerWebApiStartupFactoryParameters);

        var content = contentGenerator.Generate();

        var file = projectOptions.PathForTestGenerate!.CombineFileInfo("WebApiStartupFactory.cs");

        var contentWriter = new ContentWriter(logger);
        contentWriter.Write(
            projectOptions.PathForTestGenerate!,
            file,
            ContentWriterArea.Test,
            content);
    }

    private void GenerateTestWebApiControllerBaseTest()
    {
        var fullNamespace = $"{projectOptions.ProjectName}.Tests";

        var contentGenerator = new Framework.Mvc.ContentGenerators.Server.ContentGeneratorServerWebApiControllerBaseTest(
            new GeneratedCodeHeaderGenerator(new GeneratedCodeGeneratorParameters(projectOptions.ApiGeneratorVersion)),
            new GeneratedCodeAttributeGenerator(new GeneratedCodeGeneratorParameters(projectOptions.ApiGeneratorVersion)),
            new ContentGeneratorBaseParameters(fullNamespace));

        var content = contentGenerator.Generate();

        var file = projectOptions.PathForTestGenerate!.CombineFileInfo("WebApiControllerBaseTest.cs");

        var contentWriter = new ContentWriter(logger);
        contentWriter.Write(
            projectOptions.PathForTestGenerate!,
            file,
            ContentWriterArea.Test,
            content);
    }

    private void ScaffoldAppSettingsIntegrationTest()
    {
        var contentGenerator = new Framework.Mvc.ContentGenerators.Server.ContentGeneratorServerAppSettingsIntegrationTest();

        var content = contentGenerator.Generate();

        var file = projectOptions.PathForTestGenerate!.CombineFileInfo("appsettings.integrationtest.json");

        var contentWriter = new ContentWriter(logger);
        contentWriter.Write(
            projectOptions.PathForTestGenerate!,
            file,
            ContentWriterArea.Test,
            content,
            overrideIfExist: false);
    }
}