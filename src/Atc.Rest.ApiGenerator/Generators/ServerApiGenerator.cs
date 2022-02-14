using Atc.Console.Spectre;

// ReSharper disable ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
// ReSharper disable SuggestBaseTypeForParameter
// ReSharper disable ReplaceSubstringWithRangeIndexer
// ReSharper disable ReturnTypeCanBeEnumerable.Local
// ReSharper disable UseObjectOrCollectionInitializer
namespace Atc.Rest.ApiGenerator.Generators;

public class ServerApiGenerator
{
    private readonly ILogger logger;
    private readonly ApiProjectOptions projectOptions;

    public ServerApiGenerator(
        ILogger logger,
        ApiProjectOptions projectOptions)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
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

        var operationSchemaMappings = OpenApiOperationSchemaMapHelper.CollectMappings(projectOptions.Document);

        GenerateContracts(operationSchemaMappings);
        GenerateEndpoints(operationSchemaMappings);
        PerformCleanup();

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

        const string toolName = "ApiGenerator";
        var newVersion = GenerateHelper.GetAtcToolVersion();

        foreach (var line in lines)
        {
            var indexOfToolName = line.IndexOf(toolName, StringComparison.Ordinal);
            if (indexOfToolName == -1)
            {
                continue;
            }

            var oldVersion = line.Substring(indexOfToolName + toolName.Length);
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
            var element = XElement.Load(projectOptions.ProjectSrcCsProj.FullName);
            var originalNullableValue = SolutionAndProjectHelper.GetBoolFromNullableString(SolutionAndProjectHelper.GetNullableValueFromProject(element));

            var hasUpdates = false;
            if (projectOptions.UseNullableReferenceTypes != originalNullableValue)
            {
                var newNullableValue = SolutionAndProjectHelper.GetNullableStringFromBool(projectOptions.UseNullableReferenceTypes);
                SolutionAndProjectHelper.SetNullableValueForProject(element, newNullableValue);
                element.Save(projectOptions.ProjectSrcCsProj.FullName);
                logger.LogDebug($"{EmojisConstants.FileUpdated}   Update API csproj - Nullable value={newNullableValue}");
                hasUpdates = true;
            }

            if (!hasUpdates)
            {
                logger.LogDebug($"{EmojisConstants.FileNotUpdated}   No updates for API csproj");
            }
        }
        else
        {
            SolutionAndProjectHelper.ScaffoldProjFile(
                logger,
                projectOptions.ProjectSrcCsProj,
                projectOptions.ProjectSrcCsProjDisplayLocation,
                createAsWeb: false,
                createAsTestProject: false,
                projectOptions.ProjectName,
                "net6.0",
                new List<string> { "Microsoft.AspNetCore.App" },
                NugetPackageReferenceHelper.CreateForApiProject(),
                projectReferences: null,
                includeApiSpecification: true,
                usingCodingRules: projectOptions.UsingCodingRules);
        }

        ScaffoldBasicFileApiGenerated();
        DeleteLegacyScaffoldBasicFileResultFactory();
        DeleteLegacyScaffoldBasicFilePagination();
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
        List<ApiOperationSchemaMap> operationSchemaMappings)
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
        List<ApiOperationSchemaMap> operationSchemaMappings)
    {
        ArgumentNullException.ThrowIfNull(operationSchemaMappings);

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

    private void PerformCleanup()
    {
        // TODO: Implement
    }

    private void ScaffoldBasicFileApiGenerated()
    {
        // Create compilationUnit
        var compilationUnit = SyntaxFactory.CompilationUnit();

        // Create a namespace
        var @namespace = SyntaxProjectFactory.CreateNamespace(projectOptions);

        // Create class
        var classDeclaration = SyntaxClassDeclarationFactory.Create("ApiRegistration")
            .AddGeneratedCodeAttribute(projectOptions.ToolName, projectOptions.ToolVersion.ToString());

        // Add class to namespace
        @namespace = @namespace.AddMembers(classDeclaration);

        // Add using statement to compilationUnit
        compilationUnit = compilationUnit.AddUsingStatements(new[] { "System.CodeDom.Compiler" });

        // Add namespace to compilationUnit
        compilationUnit = compilationUnit.AddMembers(@namespace);

        var codeAsString = compilationUnit
            .NormalizeWhitespace()
            .ToFullString()
            .EnsureEnvironmentNewLines();

        var file = new FileInfo(Path.Combine(projectOptions.PathForSrcGenerate.FullName, "ApiRegistration.cs"));
        var fileDisplayLocation = file.FullName.Replace(projectOptions.PathForSrcGenerate.FullName, "src: ", StringComparison.Ordinal);
        TextFileHelper.Save(logger, file, fileDisplayLocation, codeAsString);
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
}