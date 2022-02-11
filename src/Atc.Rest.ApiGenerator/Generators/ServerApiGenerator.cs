// ReSharper disable ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
// ReSharper disable SuggestBaseTypeForParameter
// ReSharper disable ReplaceSubstringWithRangeIndexer
// ReSharper disable ReturnTypeCanBeEnumerable.Local
// ReSharper disable UseObjectOrCollectionInitializer
namespace Atc.Rest.ApiGenerator.Generators;

public class ServerApiGenerator
{
    private readonly ApiProjectOptions projectOptions;

    public ServerApiGenerator(ApiProjectOptions projectOptions)
    {
        this.projectOptions = projectOptions ?? throw new ArgumentNullException(nameof(projectOptions));
    }

    public List<LogKeyValueItem> Generate()
    {
        var logItems = new List<LogKeyValueItem>();

        logItems.Add(ValidateVersioning());
        if (logItems.Any(x => x.LogCategory == LogCategoryType.Error))
        {
            return logItems;
        }

        logItems.AddRange(ScaffoldSrc());

        CopyApiSpecification();

        var operationSchemaMappings = OpenApiOperationSchemaMapHelper.CollectMappings(projectOptions.Document);

        logItems.AddRange(GenerateContracts(operationSchemaMappings));
        logItems.AddRange(GenerateEndpoints(operationSchemaMappings));
        logItems.AddRange(PerformCleanup(logItems));
        return logItems;
    }

    private LogKeyValueItem ValidateVersioning()
    {
        if (!Directory.Exists(projectOptions.PathForSrcGenerate.FullName))
        {
            return LogItemHelper.Create(LogCategoryType.Information, ValidationRuleNameConstants.ProjectApiGenerated01, "Old project don't exist.");
        }

        var apiGeneratedFile = Path.Combine(projectOptions.PathForSrcGenerate.FullName, "ApiRegistration.cs");
        if (!File.Exists(apiGeneratedFile))
        {
            return LogItemHelper.Create(LogCategoryType.Information, ValidationRuleNameConstants.ProjectApiGenerated02, "Old ApiRegistration.cs in project don't exist.");
        }

        var lines = File.ReadLines(apiGeneratedFile).ToList();

        const string toolName = "ApiGenerator";
        var newVersion = GenerateHelper.GetAtcToolVersion();

        foreach (var line in lines)
        {
            var indexOfToolName = line.IndexOf(toolName!, StringComparison.Ordinal);
            if (indexOfToolName == -1)
            {
                continue;
            }

            var oldVersion = line.Substring(indexOfToolName + toolName!.Length);
            if (oldVersion.EndsWith('.'))
            {
                oldVersion = oldVersion.Substring(0, oldVersion.Length - 1);
            }

            if (!Version.TryParse(oldVersion, out var oldVersionResult))
            {
                return LogItemHelper.Create(LogCategoryType.Error, ValidationRuleNameConstants.ProjectApiGenerated03, "Existing project version is invalid.");
            }

            if (newVersion >= oldVersionResult)
            {
                return LogItemHelper.Create(LogCategoryType.Information, ValidationRuleNameConstants.ProjectApiGenerated04, "The generate project version is the same or newer.");
            }

            return LogItemHelper.Create(LogCategoryType.Error, ValidationRuleNameConstants.ProjectApiGenerated05, "Existing project version is never than this tool version.");
        }

        return LogItemHelper.Create(LogCategoryType.Error, ValidationRuleNameConstants.ProjectApiGenerated06, "Existing project did not contain a version.");
    }

    private List<LogKeyValueItem> ScaffoldSrc()
    {
        if (!Directory.Exists(projectOptions.PathForSrcGenerate.FullName))
        {
            Directory.CreateDirectory(projectOptions.PathForSrcGenerate.FullName);
        }

        var logItems = new List<LogKeyValueItem>();

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
                logItems.Add(LogItemFactory.CreateDebug("FileUpdate", "#", $"Update API csproj - Nullable value={newNullableValue}"));
                hasUpdates = true;
            }

            if (!hasUpdates)
            {
                logItems.Add(LogItemFactory.CreateDebug("FileSkip", "#", "No updates for API csproj"));
            }
        }
        else
        {
            logItems.Add(SolutionAndProjectHelper.ScaffoldProjFile(
                projectOptions.ProjectSrcCsProj,
                createAsWeb: false,
                createAsTestProject: false,
                projectOptions.ProjectName,
                "net6.0",
                projectOptions.UseNullableReferenceTypes,
                new List<string> { "Microsoft.AspNetCore.App" },
                NugetPackageReferenceHelper.CreateForApiProject(),
                projectReferences: null,
                includeApiSpecification: true));
        }

        ScaffoldBasicFileApiGenerated();
        DeleteLegacyScaffoldBasicFileResultFactory();
        DeleteLegacyScaffoldBasicFilePagination();

        return logItems;
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

    private List<LogKeyValueItem> GenerateContracts(
        List<ApiOperationSchemaMap> operationSchemaMappings)
    {
        ArgumentNullException.ThrowIfNull(operationSchemaMappings);

        var sgContractModels = new List<SyntaxGeneratorContractModel>();
        var sgContractParameters = new List<SyntaxGeneratorContractParameter>();
        var sgContractResults = new List<SyntaxGeneratorContractResult>();
        var sgContractInterfaces = new List<SyntaxGeneratorContractInterface>();
        foreach (var basePathSegmentName in projectOptions.BasePathSegmentNames)
        {
            var generatorModels = new SyntaxGeneratorContractModels(projectOptions, operationSchemaMappings, basePathSegmentName);
            var generatedModels = generatorModels.GenerateSyntaxTrees();
            sgContractModels.AddRange(generatedModels);

            var generatorParameters = new SyntaxGeneratorContractParameters(projectOptions, basePathSegmentName);
            var generatedParameters = generatorParameters.GenerateSyntaxTrees();
            sgContractParameters.AddRange(generatedParameters);

            var generatorResults = new SyntaxGeneratorContractResults(projectOptions, basePathSegmentName);
            var generatedResults = generatorResults.GenerateSyntaxTrees();
            sgContractResults.AddRange(generatedResults);

            var generatorInterfaces = new SyntaxGeneratorContractInterfaces(projectOptions, basePathSegmentName);
            var generatedInterfaces = generatorInterfaces.GenerateSyntaxTrees();
            sgContractInterfaces.AddRange(generatedInterfaces);
        }

        ApiGeneratorHelper.CollectMissingContractModelFromOperationSchemaMappings(
            projectOptions,
            operationSchemaMappings,
            sgContractModels);

        var logItems = new List<LogKeyValueItem>();
        foreach (var sg in sgContractModels)
        {
            logItems.Add(sg.ToFile());
        }

        foreach (var sg in sgContractParameters)
        {
            logItems.Add(sg.ToFile());
        }

        foreach (var sg in sgContractResults)
        {
            logItems.Add(sg.ToFile());
        }

        foreach (var sg in sgContractInterfaces)
        {
            logItems.Add(sg.ToFile());
        }

        return logItems;
    }

    private List<LogKeyValueItem> GenerateEndpoints(
        List<ApiOperationSchemaMap> operationSchemaMappings)
    {
        ArgumentNullException.ThrowIfNull(operationSchemaMappings);

        var sgEndpoints = new List<SyntaxGeneratorEndpointControllers>();
        foreach (var segmentName in projectOptions.BasePathSegmentNames)
        {
            var generator = new SyntaxGeneratorEndpointControllers(projectOptions, operationSchemaMappings, segmentName);
            generator.GenerateCode();
            sgEndpoints.Add(generator);
        }

        var logItems = new List<LogKeyValueItem>();
        foreach (var sg in sgEndpoints)
        {
            logItems.Add(sg.ToFile());
        }

        return logItems;
    }

    private List<LogKeyValueItem> PerformCleanup(
        List<LogKeyValueItem> orgLogItems)
    {
        ArgumentNullException.ThrowIfNull(orgLogItems);

        var logItems = new List<LogKeyValueItem>();

        if (Directory.Exists(projectOptions.PathForContracts.FullName))
        {
            var files = Directory.GetFiles(projectOptions.PathForContracts.FullName, "*.*", SearchOption.AllDirectories);
            foreach (string file in files)
            {
                if (orgLogItems.FirstOrDefault(x => x.Description == file) is not null)
                {
                    continue;
                }

                File.Delete(file);
                logItems.Add(LogItemFactory.CreateDebug("FileDelete", "#", file));
            }
        }

        if (Directory.Exists(projectOptions.PathForEndpoints.FullName))
        {
            var files = Directory.GetFiles(projectOptions.PathForEndpoints.FullName, "*.*", SearchOption.AllDirectories);
            foreach (string file in files)
            {
                if (orgLogItems.FirstOrDefault(x => x.Description == file) is not null)
                {
                    continue;
                }

                File.Delete(file);
                logItems.Add(LogItemFactory.CreateDebug("FileDelete", "#", file));
            }
        }

        return logItems;
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
        TextFileHelper.Save(file, codeAsString);
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