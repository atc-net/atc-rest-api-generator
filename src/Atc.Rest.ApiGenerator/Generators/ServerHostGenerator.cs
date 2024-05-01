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

        var apiProjectName = projectOptions.ProjectName.Replace(".Api", ".Api.Generated", StringComparison.Ordinal);
        var domainProjectName = projectOptions.ProjectName.Replace(".Api", ".Domain", StringComparison.Ordinal);

        serverHostGeneratorMvc = new Framework.Mvc.ProjectGenerator.ServerHostGenerator(
            loggerFactory,
            projectOptions.ApiGeneratorVersion,
            projectOptions.ProjectName,
            apiProjectName,
            domainProjectName,
            projectOptions.PathForSrcGenerate,
            projectOptions.Document)
        {
            UseRestExtended = projectOptions.UseRestExtended,
        };

        serverHostGeneratorMinimalApi = new Framework.Minimal.ProjectGenerator.ServerHostGenerator(
            loggerFactory,
            projectOptions.ApiGeneratorVersion,
            projectOptions.ProjectName,
            apiProjectName,
            domainProjectName,
            projectOptions.PathForSrcGenerate,
            projectOptions.Document);

        if (projectOptions.PathForTestGenerate is not null)
        {
            serverHostTestGeneratorMvc = new Framework.Mvc.ProjectGenerator.ServerHostTestGenerator(
                loggerFactory,
                projectOptions.ApiGeneratorVersion,
                $"{projectOptions.ProjectName}.{ContentGeneratorConstants.Tests}",
                projectOptions.ProjectName,
                apiProjectName,
                domainProjectName,
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
            serverHostGeneratorMvc.ScaffoldProjectFile();
            serverHostGeneratorMvc.ScaffoldProgramFile(
                projectOptions.ApiOptions.Generator.SwaggerThemeMode);
            serverHostGeneratorMvc.ScaffoldStartupFile();
            serverHostGeneratorMvc.ScaffoldWebConfig();

            serverHostGeneratorMvc.GenerateConfigureSwaggerDocOptions();

            serverHostGeneratorMvc.MaintainGlobalUsings(
                projectOptions.ApiGroupNames,
                projectOptions.RemoveNamespaceGroupSeparatorInGlobalUsings);

            if (serverHostTestGeneratorMvc is not null &&
                projectOptions.PathForTestGenerate is not null)
            {
                var operationSchemaMappings = apiOperationExtractor.Extract(projectOptions.Document);

                logger.LogInformation($"{ContentWriterConstants.AreaGenerateTest} Working on server host unit-test generation ({projectOptions.ProjectName}.Tests)");

                serverHostTestGeneratorMvc.ScaffoldProjectFile();
                // TODO: serverHostTestGeneratorMvc.ScaffoldPropertiesLaunchSettingsFile();
                serverHostTestGeneratorMvc.ScaffoldAppSettingsIntegrationTestFile();

                serverHostTestGeneratorMvc.GenerateWebApiStartupFactoryFile();
                serverHostTestGeneratorMvc.GenerateWebApiControllerBaseTestFile();

                // TODO: Move logic to ServerHostTestGenerator
                GenerateTestEndpoints(projectOptions.Document);

                serverHostTestGeneratorMvc.MaintainGlobalUsings(
                    projectOptions.ApiGroupNames,
                    projectOptions.UsingCodingRules,
                    projectOptions.RemoveNamespaceGroupSeparatorInGlobalUsings,
                    operationSchemaMappings);
            }
        }
        else
        {
            serverHostGeneratorMinimalApi.ScaffoldProjectFile();
            serverHostGeneratorMinimalApi.ScaffoldProgramFile(
                projectOptions.ApiOptions.Generator.SwaggerThemeMode);
            serverHostGeneratorMinimalApi.ScaffoldWebConfig();

            serverHostGeneratorMinimalApi.ScaffoldJsonSerializerOptionsExtensions();
            serverHostGeneratorMinimalApi.ScaffoldServiceCollectionExtensions();
            serverHostGeneratorMinimalApi.ScaffoldWebApplicationBuilderExtensions();
            serverHostGeneratorMinimalApi.ScaffoldWebApplicationExtensions(
                projectOptions.ApiOptions.Generator.SwaggerThemeMode);
            serverHostGeneratorMinimalApi.ScaffoldConfigureSwaggerOptions();

            serverHostGeneratorMinimalApi.GenerateConfigureSwaggerDocOptions();

            serverHostGeneratorMinimalApi.MaintainGlobalUsings(
                projectOptions.ApiGroupNames,
                projectOptions.RemoveNamespaceGroupSeparatorInGlobalUsings);
            serverHostGeneratorMinimalApi.MaintainWwwResources();
        }

        return true;
    }

    // TODO: ??
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
}