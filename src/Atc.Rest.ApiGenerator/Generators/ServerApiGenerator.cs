// ReSharper disable ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
// ReSharper disable SuggestBaseTypeForParameter
// ReSharper disable ReplaceSubstringWithRangeIndexer
// ReSharper disable ReturnTypeCanBeEnumerable.Local
// ReSharper disable UseObjectOrCollectionInitializer
namespace Atc.Rest.ApiGenerator.Generators;

public class ServerApiGenerator
{
    private readonly ILogger logger;
    private readonly IApiOperationExtractor apiOperationExtractor;
    private readonly ApiProjectOptions projectOptions;

    public ServerApiGenerator(
        ILogger logger,
        IApiOperationExtractor apiOperationExtractor,
        ApiProjectOptions projectOptions)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.apiOperationExtractor = apiOperationExtractor ?? throw new ArgumentNullException(nameof(apiOperationExtractor));
        this.projectOptions = projectOptions ?? throw new ArgumentNullException(nameof(projectOptions));
    }

    public bool Generate()
    {
        logger.LogInformation($"{AppEmojisConstants.AreaGenerateCode} Working on server api generation ({projectOptions.ProjectName})");

        var isVersionValid = ValidateVersioning();
        if (!isVersionValid)
        {
            return false;
        }

        ScaffoldSrc();

        CopyApiSpecification();

        var operationSchemaMappings = apiOperationExtractor.Extract(projectOptions.Document);

        GenerateContracts(operationSchemaMappings);
        GenerateEndpoints(operationSchemaMappings);
        GenerateSrcGlobalUsings();

        return true;
    }

    private bool ValidateVersioning()
    {
        if (!Directory.Exists(projectOptions.PathForSrcGenerate.FullName))
        {
            logger.LogInformation($"     {ValidationRuleNameConstants.ProjectApiGenerated01} - Old project does not exist");
            return true;
        }

        var apiGeneratedFile = Path.Combine(projectOptions.PathForSrcGenerate.FullName, "ApiRegistration.cs");
        if (!File.Exists(apiGeneratedFile))
        {
            logger.LogInformation($"     {ValidationRuleNameConstants.ProjectApiGenerated02} - Old ApiRegistration.cs in project does not exist.");
            return true;
        }

        var lines = File.ReadLines(apiGeneratedFile).ToList();

        var newVersion = GenerateHelper.GetAtcApiGeneratorVersion();

        foreach (var line in lines)
        {
            var indexOfApiGeneratorName = line.IndexOf(projectOptions.ApiGeneratorName, StringComparison.Ordinal);
            if (indexOfApiGeneratorName == -1)
            {
                continue;
            }

            var oldVersion = line.Substring(indexOfApiGeneratorName + projectOptions.ApiGeneratorName.Length);
            if (oldVersion.EndsWith('.'))
            {
                oldVersion = oldVersion.Substring(0, oldVersion.Length - 1);
            }

            if (!Version.TryParse(oldVersion, out var oldVersionResult))
            {
                logger.LogError($"     {ValidationRuleNameConstants.ProjectApiGenerated03} - Existing project version is invalid.");
                return false;
            }

            if (newVersion >= oldVersionResult)
            {
                logger.LogInformation($"     {ValidationRuleNameConstants.ProjectApiGenerated04} - The generate project version is the same or newer.");
                return true;
            }

            logger.LogError($"     {ValidationRuleNameConstants.ProjectApiGenerated05} - Existing project version is never than this tool version.");
            return false;
        }

        logger.LogError($"     {ValidationRuleNameConstants.ProjectApiGenerated06} - Existing project did not contain a version.");
        return false;
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
                ProjectType.ServerApi,
                isTestProject: false);
            if (!hasUpdates)
            {
                logger.LogDebug($"{EmojisConstants.FileNotUpdated}   No updates for csproj");
            }
        }
        else
        {
            SolutionAndProjectHelper.ScaffoldProjFile(
                logger,
                projectOptions.ProjectSrcCsProj,
                projectOptions.ProjectSrcCsProjDisplayLocation,
                ProjectType.ServerApi,
                createAsWeb: false,
                createAsTestProject: false,
                projectOptions.ProjectName,
                "net6.0",
                new List<string> { "Microsoft.AspNetCore.App" },
                NugetPackageReferenceHelper.CreateForApiProject(),
                projectReferences: null,
                includeApiSpecification: true,
                usingCodingRules: projectOptions.UsingCodingRules);

            ScaffoldBasicFileApiGenerated();
            DeleteLegacyScaffoldBasicFileResultFactory();
            DeleteLegacyScaffoldBasicFilePagination();
        }
    }

    private void CopyApiSpecification()
    {
        var resourceFolder = new DirectoryInfo(Path.Combine(projectOptions.PathForSrcGenerate.FullName, "Resources"));
        if (!resourceFolder.Exists)
        {
            Directory.CreateDirectory(resourceFolder.FullName);
        }

        var resourceFile = new FileInfo(Path.Combine(resourceFolder.FullName, "ApiSpecification.yaml"));
        if (File.Exists(resourceFile.FullName))
        {
            File.Delete(resourceFile.FullName);
        }

        if (projectOptions.DocumentFile.Extension.Equals(".json", StringComparison.OrdinalIgnoreCase))
        {
            using var writeFile = new StreamWriter(resourceFile.FullName);
            projectOptions.Document.SerializeAsV3(new OpenApiYamlWriter(writeFile));
        }
        else
        {
            File.Copy(projectOptions.DocumentFile.FullName, resourceFile.FullName);
        }
    }

    private void GenerateContracts(
        IList<ApiOperation> operationSchemaMappings)
    {
        ArgumentNullException.ThrowIfNull(operationSchemaMappings);

        var sgContractModels = new List<SyntaxGeneratorContractModel>();
        var sgContractParameters = new List<SyntaxGeneratorContractParameter>();
        var sgContractResults = new List<SyntaxGeneratorContractResult>();
        var sgContractInterfaces = new List<SyntaxGeneratorContractInterface>();
        foreach (var basePathSegmentName in projectOptions.BasePathSegmentNames)
        {
            var generatorModels = new SyntaxGeneratorContractModels(logger, projectOptions, operationSchemaMappings, basePathSegmentName);
            var generatedModels = generatorModels.GenerateSyntaxTrees();
            sgContractModels.AddRange(generatedModels);

            var generatorParameters = new SyntaxGeneratorContractParameters(logger, projectOptions, basePathSegmentName);
            var generatedParameters = generatorParameters.GenerateSyntaxTrees();
            sgContractParameters.AddRange(generatedParameters);

            var generatorResults = new SyntaxGeneratorContractResults(logger, projectOptions, basePathSegmentName);
            var generatedResults = generatorResults.GenerateSyntaxTrees();
            sgContractResults.AddRange(generatedResults);

            var generatorInterfaces = new SyntaxGeneratorContractInterfaces(logger, projectOptions, basePathSegmentName);
            var generatedInterfaces = generatorInterfaces.GenerateSyntaxTrees();
            sgContractInterfaces.AddRange(generatedInterfaces);
        }

        ApiGeneratorHelper.CollectMissingContractModelFromOperationSchemaMappings(
            logger,
            projectOptions,
            operationSchemaMappings,
            sgContractModels);

        foreach (var sg in sgContractModels)
        {
            sg.ToFile();
        }

        foreach (var sg in sgContractParameters)
        {
            sg.ToFile();
        }

        foreach (var sg in sgContractResults)
        {
            sg.ToFile();
        }

        foreach (var sg in sgContractInterfaces)
        {
            sg.ToFile();
        }
    }

    private void GenerateEndpoints(
        IList<ApiOperation> operationSchemaMappings)
    {
        ArgumentNullException.ThrowIfNull(operationSchemaMappings);

        if (projectOptions.IsForClient)
        {
            // TODO: This is the old approach only generating for ApiClient now - consolidate later with new ContentGenerator
            var sgEndpoints = new List<SyntaxGeneratorEndpointControllers>();
            foreach (var segmentName in projectOptions.BasePathSegmentNames)
            {
                var generator = new SyntaxGeneratorEndpointControllers(logger, projectOptions, operationSchemaMappings, segmentName);
                generator.GenerateCode();
                sgEndpoints.Add(generator);
            }

            foreach (var sg in sgEndpoints)
            {
                sg.ToFile();
            }
        }
        else
        {
            // New approach for server
            foreach (var area in projectOptions.BasePathSegmentNames)
            {
                var contentGeneratorServerControllerParameters = ContentGeneratorServerControllerParameterFactory
                    .Create(
                        operationSchemaMappings,
                        projectOptions.ProjectName,
                        projectOptions.ApiOptions.Generator.Response.UseProblemDetailsAsDefaultBody,
                        $"{projectOptions.ProjectName}.{ContentGeneratorConstants.Endpoints}",
                        area,
                        GetRouteByArea(area),
                        projectOptions.Document.GetPathsByBasePathSegmentName(area));

                var contentGenerator = new ContentGeneratorServerController(
                    new GeneratedCodeHeaderGenerator(new GeneratedCodeGeneratorParameters(projectOptions.ApiGeneratorVersion)),
                    new GeneratedCodeAttributeGenerator(new GeneratedCodeGeneratorParameters(projectOptions.ApiGeneratorVersion)),
                    contentGeneratorServerControllerParameters);

                var content = contentGenerator.Generate();

                // TODO: Move responsibility of generating the file object
                var controllerName = area.EnsureFirstCharacterToUpper() + ContentGeneratorConstants.Controller;
                var fileAsString = DirectoryInfoHelper.GetCsFileNameForEndpoints(projectOptions.PathForEndpoints, controllerName);
                var file = new FileInfo(fileAsString);

                var contentWriter = new ContentWriter(logger);
                contentWriter.Write(
                    projectOptions.PathForSrcGenerate,
                    file,
                    ContentWriterArea.Src,
                    content);
            }
        }
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
            .Split('/', StringSplitOptions.RemoveEmptyEntries)?
            .FirstOrDefault();

        return $"{projectOptions.RouteBase}/{routeSuffix}";
    }

    private void ScaffoldBasicFileApiGenerated()
    {
        // Create compilationUnit
        var compilationUnit = SyntaxFactory.CompilationUnit();

        // Create a namespace
        var @namespace = SyntaxProjectFactory.CreateNamespace(projectOptions);

        // Create class
        var classDeclaration = SyntaxClassDeclarationFactory.Create("ApiRegistration")
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

        var file = new FileInfo(Path.Combine(projectOptions.PathForSrcGenerate.FullName, "ApiRegistration.cs"));

        var contentWriter = new ContentWriter(logger);
        contentWriter.Write(
            projectOptions.PathForSrcGenerate,
            file,
            ContentWriterArea.Src,
            codeAsString);
    }

    private void DeleteLegacyScaffoldBasicFileResultFactory()
    {
        var file = new FileInfo(Path.Combine(projectOptions.PathForSrcGenerate.FullName, "ResultFactory.cs"));
        if (file.Exists)
        {
            File.Delete(file.FullName);
        }
    }

    private void DeleteLegacyScaffoldBasicFilePagination()
    {
        var (key, _) = projectOptions.Document.Components.Schemas.FirstOrDefault(x => x.Key.Equals(Microsoft.OpenApi.Models.NameConstants.Pagination, StringComparison.OrdinalIgnoreCase));
        if (key is null)
        {
            return;
        }

        var file = new FileInfo(Path.Combine(projectOptions.PathForSrcGenerate.FullName, $"{Microsoft.OpenApi.Models.NameConstants.Pagination}.cs"));
        if (file.Exists)
        {
            File.Delete(file.FullName);
        }
    }

    private void GenerateSrcGlobalUsings()
    {
        var requiredUsings = new List<string>
        {
            "System",
            "System.CodeDom.Compiler",
            "System.Collections.Generic",
            "System.ComponentModel.DataAnnotations",
            "System.Diagnostics.CodeAnalysis",
            "System.Linq",
            "System.Net",
            "System.Threading",
            "System.Threading.Tasks",
            "Microsoft.AspNetCore.Http",
            "Microsoft.AspNetCore.Mvc",
            "Atc.Rest.Results",
        };

        if (projectOptions.ApiOptions.Generator.UseAuthorization)
        {
            requiredUsings.Add("Microsoft.AspNetCore.Authorization");
        }

        requiredUsings.Add($"{projectOptions.ProjectName}.Contracts");
        foreach (var basePathSegmentName in projectOptions.BasePathSegmentNames)
        {
            requiredUsings.Add($"{projectOptions.ProjectName}.Contracts.{basePathSegmentName}");
        }

        GlobalUsingsHelper.CreateOrUpdate(
            logger,
            ContentWriterArea.Src,
            projectOptions.PathForSrcGenerate,
            requiredUsings);
    }
}