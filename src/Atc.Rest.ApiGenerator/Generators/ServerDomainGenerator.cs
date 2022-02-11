// ReSharper disable InvertIf
// ReSharper disable SuggestBaseTypeForParameter
// ReSharper disable ReturnTypeCanBeEnumerable.Local
namespace Atc.Rest.ApiGenerator.Generators;

public class ServerDomainGenerator
{
    private readonly DomainProjectOptions projectOptions;

    public ServerDomainGenerator(DomainProjectOptions projectOptions)
    {
        this.projectOptions = projectOptions ?? throw new ArgumentNullException(nameof(projectOptions));
    }

    public List<LogKeyValueItem> Generate()
    {
        var logItems = new List<LogKeyValueItem>();

        logItems.AddRange(projectOptions.SetPropertiesAfterValidationsOfProjectReferencesPathAndFiles());
        if (logItems.Any(x => x.LogCategory == LogCategoryType.Error))
        {
            return logItems;
        }

        logItems.AddRange(ScaffoldSrc());
        if (projectOptions.PathForTestGenerate is not null)
        {
            logItems.AddRange(ScaffoldTest());
        }

        logItems.AddRange(GenerateSrcHandlers(projectOptions, out var sgHandlers));
        if (projectOptions.PathForTestGenerate is not null)
        {
            logItems.AddRange(GenerateTestHandlers(projectOptions, sgHandlers));
        }

        return logItems;
    }

    private static List<LogKeyValueItem> GenerateSrcHandlers(
        DomainProjectOptions domainProjectOptions,
        out List<SyntaxGeneratorHandler> sgHandlers)
    {
        ArgumentNullException.ThrowIfNull(domainProjectOptions);

        var logItems = new List<LogKeyValueItem>();
        sgHandlers = new List<SyntaxGeneratorHandler>();
        foreach (var basePathSegmentName in domainProjectOptions.BasePathSegmentNames)
        {
            var generatorHandlers = new SyntaxGeneratorHandlers(domainProjectOptions, basePathSegmentName);
            var generatedHandlers = generatorHandlers.GenerateSyntaxTrees();
            sgHandlers.AddRange(generatedHandlers);
        }

        foreach (var sg in sgHandlers)
        {
            logItems.Add(sg.ToFile());
        }

        return logItems;
    }

    private static List<LogKeyValueItem> GenerateTestHandlers(
        DomainProjectOptions domainProjectOptions,
        List<SyntaxGeneratorHandler> sgHandlers)
    {
        ArgumentNullException.ThrowIfNull(domainProjectOptions);
        ArgumentNullException.ThrowIfNull(sgHandlers);

        var logItems = new List<LogKeyValueItem>();
        if (domainProjectOptions.PathForTestHandlers is not null)
        {
            foreach (var sgHandler in sgHandlers)
            {
                logItems.Add(GenerateServerDomainXunitTestHelper.GenerateGeneratedTests(domainProjectOptions, sgHandler));
                logItems.Add(GenerateServerDomainXunitTestHelper.GenerateCustomTests(domainProjectOptions, sgHandler));
            }
        }

        return logItems;
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

            bool hasUpdates = false;
            if (projectOptions.UseNullableReferenceTypes != originalNullableValue)
            {
                var newNullableValue = SolutionAndProjectHelper.GetNullableStringFromBool(projectOptions.UseNullableReferenceTypes);
                SolutionAndProjectHelper.SetNullableValueForProject(element, newNullableValue);
                element.Save(projectOptions.ProjectSrcCsProj.FullName);
                logItems.Add(LogItemFactory.CreateDebug("FileUpdate", "#", $"Update domain csproj - Nullable value={newNullableValue}"));
                hasUpdates = true;
            }

            if (!hasUpdates)
            {
                logItems.Add(LogItemFactory.CreateDebug("FileSkip", "#", "No updates for domain csproj"));
            }
        }
        else
        {
            var projectReferences = new List<FileInfo>();
            if (projectOptions.ApiProjectSrcCsProj is not null)
            {
                projectReferences.Add(projectOptions.ApiProjectSrcCsProj);
            }

            logItems.Add(SolutionAndProjectHelper.ScaffoldProjFile(
                projectOptions.ProjectSrcCsProj,
                false,
                false,
                projectOptions.ProjectName,
                "net6.0",
                projectOptions.UseNullableReferenceTypes,
                new List<string> { "Microsoft.AspNetCore.App" },
                null,
                projectReferences,
                false));
        }

        ScaffoldBasicFileDomainRegistration();

        return logItems;
    }

    private List<LogKeyValueItem> ScaffoldTest()
    {
        var logItems = new List<LogKeyValueItem>();

        if (projectOptions.PathForTestGenerate is null ||
            projectOptions.ProjectTestCsProj is null)
        {
            return logItems;
        }

        if (projectOptions.PathForTestGenerate.Exists &&
            projectOptions.ProjectTestCsProj.Exists)
        {
            // Update
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

            logItems.Add(SolutionAndProjectHelper.ScaffoldProjFile(
                projectOptions.ProjectTestCsProj,
                false,
                true,
                $"{projectOptions.ProjectName}.Tests",
                "net6.0",
                projectOptions.UseNullableReferenceTypes,
                null,
                NugetPackageReferenceHelper.CreateForTestProject(false),
                projectReferences,
                true));
        }

        return logItems;
    }

    private void ScaffoldBasicFileDomainRegistration()
    {
        // Create compilationUnit
        var compilationUnit = SyntaxFactory.CompilationUnit();

        // Create a namespace
        var @namespace = SyntaxProjectFactory.CreateNamespace(projectOptions);

        // Create class
        var classDeclaration = SyntaxClassDeclarationFactory.Create("DomainRegistration")
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

        var file = new FileInfo(Path.Combine(projectOptions.PathForSrcGenerate.FullName, "DomainRegistration.cs"));
        TextFileHelper.Save(file, codeAsString);
    }
}