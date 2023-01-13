// ReSharper disable ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
// ReSharper disable SuggestBaseTypeForParameter
// ReSharper disable ReturnTypeCanBeEnumerable.Local
namespace Atc.Rest.ApiGenerator.Generators;

public class ServerHostGenerator
{
    private readonly ILogger logger;
    private readonly IApiOperationExtractor apiOperationExtractor;
    private readonly HostProjectOptions projectOptions;

    public ServerHostGenerator(
        ILogger logger,
        IApiOperationExtractor apiOperationExtractor,
        HostProjectOptions projectOptions)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.apiOperationExtractor = apiOperationExtractor ?? throw new ArgumentNullException(nameof(apiOperationExtractor));
        this.projectOptions = projectOptions ?? throw new ArgumentNullException(nameof(projectOptions));
    }

    public bool Generate()
    {
        logger.LogInformation($"{AppEmojisConstants.AreaGenerateCode} Working on server host generation ({projectOptions.ProjectName})");

        if (!projectOptions.SetPropertiesAfterValidationsOfProjectReferencesPathAndFiles(logger))
        {
            return false;
        }

        ScaffoldSrc();
        GenerateSrcGlobalUsings();

        if (projectOptions.PathForTestGenerate is not null)
        {
            logger.LogInformation($"{AppEmojisConstants.AreaGenerateTest} Working on server host unit-test generation ({projectOptions.ProjectName}.Tests)");
            ScaffoldTest();
            GenerateTestEndpoints();
            GenerateTestGlobalUsings();
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

            SolutionAndProjectHelper.ScaffoldProjFile(
                logger,
                projectOptions.ProjectSrcCsProj,
                projectOptions.ProjectSrcCsProjDisplayLocation,
                ProjectType.ServerHost,
                createAsWeb: true,
                createAsTestProject: false,
                projectOptions.ProjectName,
                "net6.0",
                frameworkReferences: null,
                NugetPackageReferenceHelper.CreateForHostProject(projectOptions.UseRestExtended),
                projectReferences,
                includeApiSpecification: false,
                usingCodingRules: projectOptions.UsingCodingRules);

            ScaffoldPropertiesLaunchSettingsFile(
                projectOptions.PathForSrcGenerate,
                projectOptions.UseRestExtended);

            ScaffoldProgramFile();
            ScaffoldStartupFile();
            ScaffoldWebConfig();

            if (projectOptions.UseRestExtended)
            {
                ScaffoldConfigureSwaggerDocOptions();
            }
        }
    }

    private void ScaffoldPropertiesLaunchSettingsFile(
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

        var file = new FileInfo(Path.Combine(propertiesPath.FullName, "launchSettings.json"));

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

            SolutionAndProjectHelper.ScaffoldProjFile(
                logger,
                projectOptions.ProjectTestCsProj,
                projectOptions.ProjectTestCsProjDisplayLocation,
                ProjectType.ServerHost,
                createAsWeb: false,
                createAsTestProject: true,
                $"{projectOptions.ProjectName}.Tests",
                "net6.0",
                frameworkReferences: null,
                NugetPackageReferenceHelper.CreateForTestProject(useMvc: true),
                projectReferences,
                includeApiSpecification: true,
                usingCodingRules: projectOptions.UsingCodingRules);
        }

        GenerateTestWebApiStartupFactory();
        GenerateTestWebApiControllerBaseTest();
        ScaffoldAppSettingsIntegrationTest();
    }

    private void GenerateTestEndpoints()
    {
        var apiProjectOptions = new ApiProjectOptions(
            projectOptions.ApiProjectSrcPath,
            projectTestGeneratePath: null,
            projectOptions.Document,
            projectOptions.DocumentFile,
            projectOptions.ProjectName.Replace(".Api", string.Empty, StringComparison.Ordinal),
            "Api.Generated",
            projectOptions.ApiOptions,
            projectOptions.UsingCodingRules);

        var operationSchemaMappings = apiOperationExtractor.Extract(projectOptions.Document);

        foreach (var basePathSegmentName in projectOptions.BasePathSegmentNames)
        {
            var apiGroupName = basePathSegmentName.EnsureFirstCharacterToUpper();

            var generator = new SyntaxGeneratorEndpointControllers(apiProjectOptions, operationSchemaMappings, apiGroupName);
            generator.GenerateCode();

            var controllerParameters = ContentGeneratorServerControllerParametersFactory.Create(
                operationSchemaMappings,
                projectOptions.ProjectName,
                projectOptions.ApiOptions.Generator.Response.UseProblemDetailsAsDefaultBody,
                $"{projectOptions.ProjectName}.{ContentGeneratorConstants.Endpoints}",
                basePathSegmentName,
                GetRouteByArea(basePathSegmentName),
                projectOptions.Document);

            var metadataForMethods = generator.GetMetadataForMethods();
            for (var i = 0; i < metadataForMethods.Count; i++)
            {
                var endpointMethodMetadata = metadataForMethods[i];
                var contentGeneratorServerControllerMethodParameters = controllerParameters.MethodParameters[i];

                GenerateServerApiXunitTestEndpointHandlerStubHelper.Generate(
                    logger,
                    projectOptions,
                    endpointMethodMetadata,
                    apiGroupName,
                    contentGeneratorServerControllerMethodParameters);

                GenerateServerApiXunitTestEndpointTestHelper.Generate(
                    logger,
                    projectOptions,
                    endpointMethodMetadata);
            }
        }
    }

    private void ScaffoldProgramFile()
    {
        var fullNamespace = $"{projectOptions.ProjectName}";

        var contentGenerator = new ContentGeneratorServerProgram(
            new ContentGeneratorBaseParameters(fullNamespace));

        var content = contentGenerator.Generate();

        var file = new FileInfo(Path.Combine(projectOptions.PathForSrcGenerate.FullName, "Program.cs"));

        var contentWriter = new ContentWriter(logger);
        contentWriter.Write(
            projectOptions.PathForSrcGenerate,
            file,
            ContentWriterArea.Src,
            content,
            overrideIfExist: false);
    }

    private void ScaffoldStartupFile()
    {
        var fullNamespace = $"{projectOptions.ProjectName}";

        var contentGenerator = new ContentGeneratorServerStartup(
            new ContentGeneratorBaseParameters(fullNamespace));

        var content = contentGenerator.Generate();

        var file = new FileInfo(Path.Combine(projectOptions.PathForSrcGenerate.FullName, "Startup.cs"));

        var contentWriter = new ContentWriter(logger);
        contentWriter.Write(
            projectOptions.PathForSrcGenerate,
            file,
            ContentWriterArea.Src,
            content,
            overrideIfExist: false);
    }

    private void ScaffoldWebConfig()
    {
        var contentGenerator = new ContentGeneratorServerWebConfig();

        var content = contentGenerator.Generate();

        var file = new FileInfo(Path.Combine(projectOptions.PathForSrcGenerate.FullName, "web.config"));

        var contentWriter = new ContentWriter(logger);
        contentWriter.Write(
            projectOptions.PathForSrcGenerate,
            file,
            ContentWriterArea.Src,
            content,
            overrideIfExist: false);
    }

    private void ScaffoldConfigureSwaggerDocOptions()
    {
        var fullNamespace = $"{projectOptions.ProjectName}";

        var contentGeneratorServerSwaggerDocOptionsParameters = ContentGeneratorServerSwaggerDocOptionsParameterFactory
            .Create(
                fullNamespace,
                projectOptions.Document.ToSwaggerDocOptionsParameters());

        var contentGenerator = new ContentGeneratorServerSwaggerDocOptions(
            new GeneratedCodeHeaderGenerator(new GeneratedCodeGeneratorParameters(projectOptions.ApiGeneratorVersion)),
            new GeneratedCodeAttributeGenerator(new GeneratedCodeGeneratorParameters(projectOptions.ApiGeneratorVersion)),
            contentGeneratorServerSwaggerDocOptionsParameters);

        var content = contentGenerator.Generate();

        var file = new FileInfo(Path.Combine(projectOptions.PathForSrcGenerate.FullName, "ConfigureSwaggerDocOptions.cs"));

        var contentWriter = new ContentWriter(logger);
        contentWriter.Write(
            projectOptions.PathForSrcGenerate,
            file,
            ContentWriterArea.Src,
            content);
    }

    private void GenerateTestWebApiStartupFactory()
    {
        var fullNamespace = $"{projectOptions.ProjectName}.Tests";

        var contentGeneratorServerWebApiStartupFactoryParameters = ContentGeneratorServerWebApiStartupFactoryParametersFactory.Create(
            fullNamespace);

        var contentGenerator = new ContentGeneratorServerWebApiStartupFactory(
            new GeneratedCodeHeaderGenerator(new GeneratedCodeGeneratorParameters(projectOptions.ApiGeneratorVersion)),
            new GeneratedCodeAttributeGenerator(new GeneratedCodeGeneratorParameters(projectOptions.ApiGeneratorVersion)),
            new CodeDocumentationTagsGenerator(),
            contentGeneratorServerWebApiStartupFactoryParameters);

        var content = contentGenerator.Generate();

        var file = new FileInfo(Path.Combine(projectOptions.PathForTestGenerate!.FullName, "WebApiStartupFactory.cs"));

        var contentWriter = new ContentWriter(logger);
        contentWriter.Write(
            projectOptions.PathForTestGenerate,
            file,
            ContentWriterArea.Test,
            content);
    }

    private void GenerateTestWebApiControllerBaseTest()
    {
        var fullNamespace = $"{projectOptions.ProjectName}.Tests";

        var contentGenerator = new ContentGeneratorServerWebApiControllerBaseTest(
            new GeneratedCodeHeaderGenerator(new GeneratedCodeGeneratorParameters(projectOptions.ApiGeneratorVersion)),
            new GeneratedCodeAttributeGenerator(new GeneratedCodeGeneratorParameters(projectOptions.ApiGeneratorVersion)),
            new ContentGeneratorBaseParameters(fullNamespace));

        var content = contentGenerator.Generate();

        var file = new FileInfo(Path.Combine(projectOptions.PathForTestGenerate!.FullName, "WebApiControllerBaseTest.cs"));

        var contentWriter = new ContentWriter(logger);
        contentWriter.Write(
            projectOptions.PathForTestGenerate,
            file,
            ContentWriterArea.Test,
            content);
    }

    private void ScaffoldAppSettingsIntegrationTest()
    {
        var contentGenerator = new ContentGeneratorServerAppSettingsIntegrationTest();

        var content = contentGenerator.Generate();

        var file = new FileInfo(Path.Combine(projectOptions.PathForTestGenerate!.FullName, "appsettings.integrationtest.json"));

        var contentWriter = new ContentWriter(logger);
        contentWriter.Write(
            projectOptions.PathForTestGenerate,
            file,
            ContentWriterArea.Test,
            content,
            overrideIfExist: false);
    }

    private void GenerateSrcGlobalUsings()
    {
        var requiredUsings = new List<string>
        {
            "System",
            "System.CodeDom.Compiler",
            "System.Reflection",
            "System.IO",
            "Microsoft.AspNetCore.Builder",
            "Microsoft.AspNetCore.Hosting",
            "Microsoft.Extensions.Configuration",
            "Microsoft.Extensions.DependencyInjection",
            "Microsoft.Extensions.Hosting",
            projectOptions.ProjectName.Replace(".Api", ".Domain", StringComparison.Ordinal),
            $"{projectOptions.ProjectName}.Generated",
        };

        if (projectOptions.UseRestExtended)
        {
            requiredUsings.Add("Atc.Rest.Extended.Options");
            requiredUsings.Add("Microsoft.AspNetCore.Mvc.ApiExplorer");
            requiredUsings.Add("Microsoft.Extensions.Options");
            requiredUsings.Add("Microsoft.OpenApi.Models");
            requiredUsings.Add("Swashbuckle.AspNetCore.SwaggerGen");
        }

        GlobalUsingsHelper.CreateOrUpdate(
            logger,
            ContentWriterArea.Src,
            projectOptions.PathForSrcGenerate,
            requiredUsings);
    }

    private void GenerateTestGlobalUsings()
    {
        var requiredUsings = new List<string>
        {
            "System.CodeDom.Compiler",
            "System.Collections.Generic",
            "System.IO",
            "System.Net",
            "System.Net.Http",
            "System.Text",
            "System.Text.Json",
            "System.Text.Json.Serialization",
            "Microsoft.AspNetCore.Http",
            "Microsoft.Extensions.Configuration",
            "Xunit",
            "System.Reflection",
            "System.Threading",
            "System.Threading.Tasks",
            "System",
            "Atc.Rest.Options",
            "FluentAssertions",
            "Microsoft.AspNetCore.Hosting",
            "Microsoft.AspNetCore.Mvc.Testing",
            "Microsoft.AspNetCore.TestHost",
            "Microsoft.Extensions.Configuration",
            "Microsoft.Extensions.DependencyInjection",
            "Atc.Rest.Results",
            "Atc.XUnit",
            $"{projectOptions.ProjectName}.Generated",
            $"{projectOptions.ProjectName}.Generated.Contracts",
        };

        foreach (var basePathSegmentName in projectOptions.BasePathSegmentNames)
        {
            if (basePathSegmentName.Equals("Tasks", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            requiredUsings.Add($"{projectOptions.ProjectName}.Generated.Contracts.{basePathSegmentName}");
        }

        GlobalUsingsHelper.CreateOrUpdate(
            logger,
            ContentWriterArea.Test,
            projectOptions.PathForTestGenerate!,
            requiredUsings);
    }

    private string GetRouteByArea(
        string area)
    {
        var (key, _) = projectOptions.Document.Paths.FirstOrDefault(x => x.IsPathStartingSegmentName(area));
        if (key is null)
        {
            throw new NotSupportedException("Area was not found in any route.");
        }

        var routeSuffix = key
            .Split('/', StringSplitOptions.RemoveEmptyEntries)
            .FirstOrDefault();

        return $"{projectOptions.RouteBase}/{routeSuffix}";
    }
}