// ReSharper disable ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
// ReSharper disable InvertIf
// ReSharper disable SuggestBaseTypeForParameter
// ReSharper disable ReturnTypeCanBeEnumerable.Local
// ReSharper disable ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
namespace Atc.Rest.ApiGenerator.Generators;

public class ServerDomainGenerator
{
    private readonly ILogger logger;
    private readonly DomainProjectOptions projectOptions;

    public ServerDomainGenerator(
        ILogger logger,
        DomainProjectOptions projectOptions)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.projectOptions = projectOptions ?? throw new ArgumentNullException(nameof(projectOptions));
    }

    public bool Generate()
    {
        logger.LogInformation($"{AppEmojisConstants.AreaGenerateCode} Working on server domain generation ({projectOptions.ProjectName})");

        if (!projectOptions.SetPropertiesAfterValidationsOfProjectReferencesPathAndFiles(logger))
        {
            return false;
        }

        ScaffoldSrc();

        GenerateSrcHandlers(projectOptions.Document);

        GenerateSrcGlobalUsings();

        if (projectOptions.PathForTestGenerate is not null)
        {
            logger.LogInformation($"{AppEmojisConstants.AreaGenerateTest} Working on server domain unit-test generation ({projectOptions.ProjectName}.Tests)");

            ScaffoldTest();

            GenerateTestHandlers(projectOptions.Document);

            GenerateTestGlobalUsings();
        }

        return true;
    }

    private void GenerateSrcHandlers(
        OpenApiDocument document)
    {
        ArgumentNullException.ThrowIfNull(projectOptions);

        foreach (var basePathSegmentName in projectOptions.BasePathSegmentNames)
        {
            var apiGroupName = basePathSegmentName.EnsureFirstCharacterToUpper();

            foreach (var urlPath in document.Paths)
            {
                if (!urlPath.IsPathStartingSegmentName(apiGroupName))
                {
                    continue;
                }

                foreach (var openApiOperation in urlPath.Value.Operations)
                {
                    GenerateSrcHandler(apiGroupName, urlPath.Value, openApiOperation.Value);
                }
            }
        }
    }

    private void GenerateSrcHandler(
        string apiGroupName,
        OpenApiPathItem apiPath,
        OpenApiOperation apiOperation)
    {
        var fullNamespace = $"{projectOptions.ProjectName}.{ContentGeneratorConstants.Handlers}.{apiGroupName}";

        // Generate
        var handlerParameters = ContentGeneratorServerHandlerParametersFactory.Create(
            fullNamespace,
            apiPath,
            apiOperation);

        var contentGeneratorHandler = new ContentGeneratorServerHandler(
            new CodeDocumentationTagsGenerator(),
            handlerParameters);

        var handlerContent = contentGeneratorHandler.Generate();

        // Write
        var file = new FileInfo(
            Helpers.DirectoryInfoHelper.GetCsFileNameForHandler(
                projectOptions.PathForSrcHandlers!,
                apiGroupName,
                handlerParameters.HandlerName));

        var contentWriter = new ContentWriter(logger);
        contentWriter.Write(
            projectOptions.PathForSrcGenerate,
            file,
            ContentWriterArea.Src,
            handlerContent,
            overrideIfExist: false);
    }

    private void GenerateTestHandlers(
        OpenApiDocument document)
    {
        ArgumentNullException.ThrowIfNull(projectOptions);

        foreach (var basePathSegmentName in projectOptions.BasePathSegmentNames)
        {
            var apiGroupName = basePathSegmentName.EnsureFirstCharacterToUpper();

            foreach (var urlPath in document.Paths)
            {
                if (!urlPath.IsPathStartingSegmentName(apiGroupName))
                {
                    continue;
                }

                foreach (var openApiOperation in urlPath.Value.Operations)
                {
                    GenerateTestHandler(apiGroupName, urlPath.Value, openApiOperation.Value);
                }
            }
        }
    }

    private void GenerateTestHandler(
        string apiGroupName,
        OpenApiPathItem apiPath,
        OpenApiOperation apiOperation)
    {
        // TODO: Rewrite:

        var operationName = apiOperation.GetOperationName();
        var handlerName = $"{operationName}{ContentGeneratorConstants.Handler}";

        var hasParametersOrRequestBody = apiPath.HasParameters() ||
                                         apiOperation.HasParametersOrRequestBody();

        GenerateServerDomainXunitTestHelper.GenerateGeneratedTests(logger, projectOptions, apiGroupName, handlerName, hasParametersOrRequestBody);
        GenerateServerDomainXunitTestHelper.GenerateCustomTests(logger, projectOptions, apiGroupName, handlerName);
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

            SolutionAndProjectHelper.ScaffoldProjFile(
                logger,
                projectOptions.ProjectSrcCsProj,
                projectOptions.ProjectSrcCsProjDisplayLocation,
                ProjectType.ServerDomain,
                createAsWeb: false,
                createAsTestProject: false,
                projectOptions.ProjectName,
                "net6.0",
                new List<string> { "Microsoft.AspNetCore.App" },
                packageReferences: null,
                projectReferences,
                includeApiSpecification: false,
                usingCodingRules: projectOptions.UsingCodingRules);

            ScaffoldBasicFileDomainRegistration();
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

            SolutionAndProjectHelper.ScaffoldProjFile(
                logger,
                projectOptions.ProjectTestCsProj,
                projectOptions.ProjectTestCsProjDisplayLocation,
                ProjectType.ServerDomain,
                createAsWeb: false,
                createAsTestProject: true,
                $"{projectOptions.ProjectName}.Tests",
                "net6.0",
                frameworkReferences: null,
                NugetPackageReferenceHelper.CreateForTestProject(false),
                projectReferences,
                includeApiSpecification: true,
                usingCodingRules: projectOptions.UsingCodingRules);
        }
    }

    private void ScaffoldBasicFileDomainRegistration()
    {
        // Create compilationUnit
        var compilationUnit = SyntaxFactory.CompilationUnit();

        // Create a namespace
        var @namespace = SyntaxProjectFactory.CreateNamespace(projectOptions);

        // Create class
        var classDeclaration = SyntaxClassDeclarationFactory.Create("DomainRegistration")
            .AddGeneratedCodeAttribute(projectOptions.ApiGeneratorName, projectOptions.ApiGeneratorVersion.ToString());

        // Add class to namespace
        @namespace = @namespace.AddMembers(classDeclaration);

        // Add namespace to compilationUnit
        compilationUnit = compilationUnit.AddMembers(@namespace);

        var codeAsString = compilationUnit
            .NormalizeWhitespace()
            .ToFullString()
            .EnsureEnvironmentNewLines()
            .EnsureFileScopedNamespace();

        var file = new FileInfo(Path.Combine(projectOptions.PathForSrcGenerate.FullName, "DomainRegistration.cs"));

        var contentWriter = new ContentWriter(logger);
        contentWriter.Write(
            projectOptions.PathForSrcGenerate,
            file,
            ContentWriterArea.Src,
            codeAsString);
    }

    private void GenerateSrcGlobalUsings()
    {
        var requiredUsings = new List<string>
        {
            "System",
            "System.CodeDom.Compiler",
            "System.Threading",
            "System.Threading.Tasks",
        };

        var projectName = projectOptions.ProjectName.Replace(".Domain", ".Api.Generated", StringComparison.Ordinal);
        foreach (var basePathSegmentName in projectOptions.BasePathSegmentNames)
        {
            requiredUsings.Add($"{projectName}.Contracts.{basePathSegmentName}");
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
            "System",
            "System.CodeDom.Compiler",
            "Xunit",
        };

        foreach (var basePathSegmentName in projectOptions.BasePathSegmentNames)
        {
            requiredUsings.Add($"{projectOptions.ProjectName}.Handlers.{basePathSegmentName}");
        }

        GlobalUsingsHelper.CreateOrUpdate(
            logger,
            ContentWriterArea.Test,
            projectOptions.PathForTestGenerate!,
            requiredUsings);
    }
}