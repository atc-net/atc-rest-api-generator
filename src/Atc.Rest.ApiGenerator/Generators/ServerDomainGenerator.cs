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

    private readonly string codeGeneratorContentHeader;
    private readonly AttributeParameters codeGeneratorAttribute;

    public ServerDomainGenerator(
        ILogger logger,
        INugetPackageReferenceProvider nugetPackageReferenceProvider,
        DomainProjectOptions projectOptions)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.nugetPackageReferenceProvider = nugetPackageReferenceProvider ?? throw new ArgumentNullException(nameof(nugetPackageReferenceProvider));
        this.projectOptions = projectOptions ?? throw new ArgumentNullException(nameof(projectOptions));

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
        logger.LogInformation($"{ContentWriterConstants.AreaGenerateCode} Working on server domain generation ({projectOptions.ProjectName})");

        if (projectOptions.AspNetOutputType == AspNetOutputType.Mvc &&
            !projectOptions.SetPropertiesAfterValidationsOfProjectReferencesPathAndFiles(logger))
        {
            return false;
        }

        ScaffoldSrc();

        if (projectOptions.AspNetOutputType == AspNetOutputType.Mvc)
        {
            GenerateSrcMvcHandlers(projectOptions.Document);
        }
        else
        {
            GenerateSrcMinimalApiHandlers(projectOptions.Document);
        }

        if (projectOptions.AspNetOutputType == AspNetOutputType.Mvc)
        {
            GenerateSrcGlobalUsingsForMvc(projectOptions.RemoveNamespaceGroupSeparatorInGlobalUsings);
        }
        else
        {
            GenerateSrcGlobalUsingsForMinimalApi(projectOptions.RemoveNamespaceGroupSeparatorInGlobalUsings);
        }

        if (projectOptions.PathForTestGenerate is not null)
        {
            logger.LogInformation($"{ContentWriterConstants.AreaGenerateTest} Working on server domain unit-test generation ({projectOptions.ProjectName}.Tests)");

            ScaffoldTest();

            GenerateTestHandlers(projectOptions.Document);

            GenerateTestGlobalUsings(
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

    private void ScaffoldBasicFileDomainRegistration()
    {
        var classParameters = ClassParametersFactory.Create(
            codeGeneratorContentHeader,
            projectOptions.ProjectName,
            codeGeneratorAttribute,
            "DomainRegistration");

        var contentGeneratorClass = new GenerateContentForClass(
            new CodeDocumentationTagsGenerator(),
            classParameters);

        var classContent = contentGeneratorClass.Generate();

        var file = new FileInfo(
            Path.Combine(
                projectOptions.PathForSrcGenerate.FullName,
                "DomainRegistration.cs"));

        var contentWriter = new ContentWriter(logger);
        contentWriter.Write(
            projectOptions.PathForSrcGenerate,
            file,
            ContentWriterArea.Src,
            classContent);
    }

    private void GenerateSrcGlobalUsingsForMvc(
        bool removeNamespaceGroupSeparatorInGlobalUsings)
    {
        var requiredUsings = new List<string>
        {
            "System.CodeDom.Compiler",
        };

        var projectName = projectOptions.ProjectName.Replace(".Domain", ".Api.Generated", StringComparison.Ordinal);
        foreach (var apiGroupName in projectOptions.ApiGroupNames)
        {
            requiredUsings.Add($"{projectName}.Contracts.{apiGroupName}");
        }

        GlobalUsingsHelper.CreateOrUpdate(
            logger,
            ContentWriterArea.Src,
            projectOptions.PathForSrcGenerate,
            requiredUsings,
            removeNamespaceGroupSeparatorInGlobalUsings);
    }

    private void GenerateSrcGlobalUsingsForMinimalApi(
        bool removeNamespaceGroupSeparatorInGlobalUsings)
    {
        var requiredUsings = new List<string>
        {
            "System.CodeDom.Compiler",
            "Microsoft.AspNetCore.Http.HttpResults",
        };

        var projectName = projectOptions.ProjectName.Replace(".Domain", ".Api.Generated", StringComparison.Ordinal);
        foreach (var apiGroupName in projectOptions.ApiGroupNames)
        {
            requiredUsings.Add($"{projectName}.Contracts.{apiGroupName}");
        }

        GlobalUsingsHelper.CreateOrUpdate(
            logger,
            ContentWriterArea.Src,
            projectOptions.PathForSrcGenerate,
            requiredUsings,
            removeNamespaceGroupSeparatorInGlobalUsings);
    }

    private void GenerateTestGlobalUsings(
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
            projectOptions.PathForTestGenerate!,
            requiredUsings,
            removeNamespaceGroupSeparatorInGlobalUsings);
    }
}