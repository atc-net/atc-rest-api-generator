// ReSharper disable ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
// ReSharper disable InvertIf
// ReSharper disable SuggestBaseTypeForParameter
// ReSharper disable ReturnTypeCanBeEnumerable.Local
// ReSharper disable ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
namespace Atc.Rest.ApiGenerator.Generators;

public class ServerDomainGenerator
{
    private readonly ILogger logger;
    private readonly INugetPackageReferenceProvider nugetPackageReferenceProvider;
    private readonly DomainProjectOptions projectOptions;

    private readonly IServerDomainGenerator serverDomainGeneratorMvc;
    private readonly IServerDomainGenerator serverDomainGeneratorMinimalApi;
    private readonly IServerDomainTestGenerator? serverDomainTestGeneratorMvc;

    public ServerDomainGenerator(
        ILoggerFactory loggerFactory,
        INugetPackageReferenceProvider nugetPackageReferenceProvider,
        DomainProjectOptions projectOptions)
    {
        ArgumentNullException.ThrowIfNull(loggerFactory);

        logger = loggerFactory.CreateLogger<ServerDomainGenerator>();
        this.nugetPackageReferenceProvider = nugetPackageReferenceProvider ?? throw new ArgumentNullException(nameof(nugetPackageReferenceProvider));
        this.projectOptions = projectOptions ?? throw new ArgumentNullException(nameof(projectOptions));

        serverDomainGeneratorMvc = new Framework.Mvc.ProjectGenerator.ServerDomainGenerator(
            loggerFactory,
            projectOptions.ApiGeneratorVersion,
            projectOptions.ProjectName,
            projectOptions.PathForSrcGenerate,
            projectOptions.Document);

        serverDomainGeneratorMinimalApi = new Framework.Minimal.ProjectGenerator.ServerDomainGenerator(
            loggerFactory,
            projectOptions.ApiGeneratorVersion,
            projectOptions.ProjectName,
            projectOptions.PathForSrcGenerate,
            projectOptions.Document);

        if (projectOptions.PathForTestGenerate is not null)
        {
            serverDomainTestGeneratorMvc = new Framework.Mvc.ProjectGenerator.ServerDomainTestGenerator(
                loggerFactory,
                projectOptions.ApiGeneratorVersion,
                projectOptions.ProjectName,
                projectOptions.PathForTestGenerate,
                projectOptions.Document);
        }
    }

    public bool Generate()
    {
        logger.LogInformation($"{ContentWriterConstants.AreaGenerateCode} Working on server domain generation ({projectOptions.ProjectName})");

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

        ScaffoldSrc();

        if (projectOptions.ApiOptions.Generator.AspNetOutputType == AspNetOutputType.Mvc)
        {
            serverDomainGeneratorMvc.GenerateAssemblyMarker();

            serverDomainGeneratorMvc.MaintainGlobalUsings(
                projectOptions.ProjectName.Replace(".Domain", ".Api.Generated", StringComparison.Ordinal),
                projectOptions.ApiGroupNames,
                projectOptions.RemoveNamespaceGroupSeparatorInGlobalUsings);
        }
        else
        {
            serverDomainGeneratorMinimalApi.GenerateAssemblyMarker();

            serverDomainGeneratorMinimalApi.GenerateServiceCollectionExtensions();

            serverDomainGeneratorMinimalApi.MaintainGlobalUsings(
                projectOptions.ProjectName.Replace(".Domain", ".Api.Generated", StringComparison.Ordinal),
                projectOptions.ApiGroupNames,
                projectOptions.RemoveNamespaceGroupSeparatorInGlobalUsings);
        }

        if (projectOptions.ApiOptions.Generator.AspNetOutputType == AspNetOutputType.Mvc)
        {
            GenerateSrcMvcHandlers(projectOptions.Document);
        }
        else
        {
            GenerateSrcMinimalApiHandlers(projectOptions.Document);
        }

        if (projectOptions.PathForTestGenerate is not null)
        {
            logger.LogInformation($"{ContentWriterConstants.AreaGenerateTest} Working on server domain unit-test generation ({projectOptions.ProjectName}.Tests)");

            ScaffoldTest();

            GenerateTestHandlers(projectOptions.Document);

            serverDomainTestGeneratorMvc?.MaintainGlobalUsings(
                projectOptions.UsingCodingRules,
                projectOptions.RemoveNamespaceGroupSeparatorInGlobalUsings);
        }

        return true;
    }

    private void GenerateSrcMvcHandlers(
        OpenApiDocument document)
    {
        ArgumentNullException.ThrowIfNull(projectOptions);

        foreach (var urlPath in document.Paths)
        {
            var apiGroupName = urlPath.GetApiGroupName();

            foreach (var openApiOperation in urlPath.Value.Operations)
            {
                GenerateSrcMvcHandler(
                    apiGroupName,
                    urlPath.Value,
                    openApiOperation.Value);
            }
        }
    }

    private void GenerateSrcMinimalApiHandlers(
        OpenApiDocument document)
    {
        ArgumentNullException.ThrowIfNull(projectOptions);

        foreach (var urlPath in document.Paths)
        {
            var apiGroupName = urlPath.GetApiGroupName();

            foreach (var openApiOperation in urlPath.Value.Operations)
            {
                GenerateSrcMinimalApiHandler(apiGroupName, urlPath.Value, openApiOperation.Value);
            }
        }
    }

    private void GenerateSrcMvcHandler(
        string apiGroupName,
        OpenApiPathItem apiPath,
        OpenApiOperation apiOperation)
    {
        var fullNamespace = $"{projectOptions.ProjectName}.{ContentGeneratorConstants.Handlers}.{apiGroupName}";

        // Generate
        var classParameters = Framework.Mvc.Factories.Parameters.Server.ContentGeneratorServerHandlerParametersFactory.Create(
            fullNamespace,
            apiPath,
            apiOperation);

        var contentGeneratorClass = new GenerateContentForClass(
            new CodeDocumentationTagsGenerator(),
            classParameters);

        var classContent = contentGeneratorClass.Generate();

        // Write
        var file = new FileInfo(
            Helpers.DirectoryInfoHelper.GetCsFileNameForHandler(
                projectOptions.PathForSrcHandlers!,
                apiGroupName,
                classParameters.TypeName));

        var contentWriter = new ContentWriter(logger);
        contentWriter.Write(
            projectOptions.PathForSrcGenerate,
            file,
            ContentWriterArea.Src,
            classContent,
            overrideIfExist: false);
    }

    private void GenerateSrcMinimalApiHandler(
        string apiGroupName,
        OpenApiPathItem apiPath,
        OpenApiOperation apiOperation)
    {
        var fullNamespace = $"{projectOptions.ProjectName}.{ContentGeneratorConstants.Handlers}.{apiGroupName}";

        // Generate
        var classParameters = Framework.Minimal.Factories.Parameters.Server.ContentGeneratorServerHandlerParametersFactory.Create(
            fullNamespace,
            apiPath,
            apiOperation);

        var contentGeneratorClass = new GenerateContentForClass(
            new CodeDocumentationTagsGenerator(),
            classParameters);

        var classContent = contentGeneratorClass.Generate();

        // Write
        var file = new FileInfo(
            Helpers.DirectoryInfoHelper.GetCsFileNameForHandler(
                projectOptions.PathForSrcHandlers!,
                apiGroupName,
                classParameters.TypeName));

        var contentWriter = new ContentWriter(logger);
        contentWriter.Write(
            projectOptions.PathForSrcGenerate,
            file,
            ContentWriterArea.Src,
            classContent,
            overrideIfExist: false);
    }

    private void GenerateTestHandlers(
        OpenApiDocument document)
    {
        ArgumentNullException.ThrowIfNull(projectOptions);

        foreach (var urlPath in document.Paths)
        {
            var apiGroupName = urlPath.GetApiGroupName();

            foreach (var openApiOperation in urlPath.Value.Operations)
            {
                GenerateTestHandler(apiGroupName, openApiOperation.Value);
            }
        }
    }

    private void GenerateTestHandler(
        string apiGroupName,
        OpenApiOperation apiOperation)
    {
        var fullNamespace = $"{projectOptions.ProjectName}.{ContentGeneratorConstants.Tests}.{ContentGeneratorConstants.Handlers}.{apiGroupName}";

        // Generate
        var classParameters = Framework.Mvc.Factories.Parameters.Server.ContentGeneratorServerHandlerParametersFactory.CreateForCustomTest(
            fullNamespace,
            apiOperation);

        var contentGeneratorClass = new GenerateContentForClass(
            new CodeDocumentationTagsGenerator(),
            classParameters);

        var classContent = contentGeneratorClass.Generate();

        // Write
        var file = new FileInfo(
            Helpers.DirectoryInfoHelper.GetCsFileNameForHandler(
                projectOptions.PathForTestHandlers!,
                apiGroupName,
                classParameters.TypeName));

        var contentWriter = new ContentWriter(logger);
        contentWriter.Write(
            projectOptions.PathForTestGenerate!,
            file,
            ContentWriterArea.Test,
            classContent,
            overrideIfExist: false);
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
                ProjectType.ServerDomain,
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

            IList<(string PackageId, string PackageVersion, string? SubElements)>? packageReferencesBaseLineForDomainProject = null;
            if (projectOptions.ApiOptions.Generator.AspNetOutputType == AspNetOutputType.MinimalApi)
            {
                TaskHelper.RunSync(async () =>
                {
                    packageReferencesBaseLineForDomainProject = await nugetPackageReferenceProvider.GetPackageReferencesBaseLineForDomainProjectForMinimalApi();
                });
            }

            SolutionAndProjectHelper.ScaffoldProjFile(
                logger,
                projectOptions.ProjectSrcCsProj,
                projectOptions.ProjectSrcCsProjDisplayLocation,
                ProjectType.ServerDomain,
                createAsWeb: false,
                createAsTestProject: false,
                projectOptions.ProjectName,
                "net8.0",
                new List<string> { "Microsoft.AspNetCore.App" },
                packageReferences: packageReferencesBaseLineForDomainProject,
                projectReferences,
                includeApiSpecification: false,
                usingCodingRules: projectOptions.UsingCodingRules);
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
                ProjectType.ServerDomain,
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
                projectReferences.Add(projectOptions.ApiProjectSrcCsProj);
                projectReferences.Add(projectOptions.ProjectSrcCsProj);
            }

            IList<(string PackageId, string PackageVersion, string? SubElements)>? packageReferencesBaseLineForTestProject = null;
            TaskHelper.RunSync(async () =>
            {
                packageReferencesBaseLineForTestProject = await nugetPackageReferenceProvider.GetPackageReferencesBaseLineForTestProject(useMvc: false);
            });

            SolutionAndProjectHelper.ScaffoldProjFile(
                logger,
                projectOptions.ProjectTestCsProj,
                projectOptions.ProjectTestCsProjDisplayLocation,
                ProjectType.ServerDomain,
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
    }
}