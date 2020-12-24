using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Atc.CodeAnalysis.CSharp.SyntaxFactories;
using Atc.Data.Models;
using Atc.Rest.ApiGenerator.Helpers;
using Atc.Rest.ApiGenerator.Helpers.XunitTest;
using Atc.Rest.ApiGenerator.Models;
using Atc.Rest.ApiGenerator.ProjectSyntaxFactories;
using Atc.Rest.ApiGenerator.SyntaxGenerators.Domain;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

// ReSharper disable InvertIf
// ReSharper disable SuggestBaseTypeForParameter
// ReSharper disable ReturnTypeCanBeEnumerable.Local
namespace Atc.Rest.ApiGenerator.Generators
{
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

            logItems.AddRange(ScaffoldSrc(projectOptions));
            if (projectOptions.PathForTestGenerate != null)
            {
                logItems.AddRange(ScaffoldTest(projectOptions));
            }

            logItems.AddRange(GenerateSrcHandlers(projectOptions, out List<SyntaxGeneratorHandler> sgHandlers));
            if (projectOptions.PathForTestGenerate != null)
            {
                logItems.AddRange(GenerateTestHandlers(projectOptions, sgHandlers));
            }

            return logItems;
        }

        private static List<LogKeyValueItem> ScaffoldSrc(DomainProjectOptions domainProjectOptions)
        {
            if (domainProjectOptions == null)
            {
                throw new ArgumentNullException(nameof(domainProjectOptions));
            }

            if (!Directory.Exists(domainProjectOptions.PathForSrcGenerate.FullName))
            {
                Directory.CreateDirectory(domainProjectOptions.PathForSrcGenerate.FullName);
            }

            var logItems = new List<LogKeyValueItem>();

            if (domainProjectOptions.PathForSrcGenerate.Exists && domainProjectOptions.ProjectSrcCsProj.Exists)
            {
                var element = XElement.Load(domainProjectOptions.ProjectSrcCsProj.FullName);
                var originalNullableValue = SolutionAndProjectHelper.GetBoolFromNullableString(SolutionAndProjectHelper.GetNullableValueFromProject(element));

                bool hasUpdates = false;
                if (domainProjectOptions.ApiOptions.Generator.UseNullableReferenceTypes != originalNullableValue)
                {
                    var newNullableValue = SolutionAndProjectHelper.GetNullableStringFromBool(domainProjectOptions.ApiOptions.Generator.UseNullableReferenceTypes);
                    SolutionAndProjectHelper.SetNullableValueForProject(element, newNullableValue);
                    element.Save(domainProjectOptions.ProjectSrcCsProj.FullName);
                    logItems.Add(new LogKeyValueItem(LogCategoryType.Debug, "FileUpdate", "#", $"Update domain csproj - Nullable value={newNullableValue}"));
                    hasUpdates = true;
                }

                if (!hasUpdates)
                {
                    logItems.Add(new LogKeyValueItem(LogCategoryType.Debug, "FileSkip", "#", "No updates for domain csproj"));
                }
            }
            else
            {
                var projectReferences = new List<FileInfo>();
                if (domainProjectOptions.ApiProjectSrcCsProj != null)
                {
                    projectReferences.Add(domainProjectOptions.ApiProjectSrcCsProj);
                }

                logItems.Add(SolutionAndProjectHelper.ScaffoldProjFile(
                    domainProjectOptions.ProjectSrcCsProj,
                    false,
                    false,
                    domainProjectOptions.ProjectName,
                    domainProjectOptions.ApiOptions.Generator.UseNullableReferenceTypes,
                    new List<string> { "Microsoft.AspNetCore.App" },
                    null,
                    projectReferences,
                    false));
            }

            ScaffoldBasicFileDomainRegistration(domainProjectOptions);

            return logItems;
        }

        private static List<LogKeyValueItem> ScaffoldTest(DomainProjectOptions domainProjectOptions)
        {
            if (domainProjectOptions == null)
            {
                throw new ArgumentNullException(nameof(domainProjectOptions));
            }

            var logItems = new List<LogKeyValueItem>();

            if (domainProjectOptions.PathForTestGenerate == null || domainProjectOptions.ProjectTestCsProj == null)
            {
                return logItems;
            }

            if (domainProjectOptions.PathForTestGenerate.Exists && domainProjectOptions.ProjectTestCsProj.Exists)
            {
                // Update
            }
            else
            {
                if (!Directory.Exists(domainProjectOptions.PathForTestGenerate.FullName))
                {
                    Directory.CreateDirectory(domainProjectOptions.PathForTestGenerate.FullName);
                }

                var projectReferences = new List<FileInfo>();
                if (domainProjectOptions.ApiProjectSrcCsProj != null)
                {
                    projectReferences.Add(domainProjectOptions.ApiProjectSrcCsProj);
                    projectReferences.Add(domainProjectOptions.ProjectSrcCsProj);
                }

                logItems.Add(SolutionAndProjectHelper.ScaffoldProjFile(
                    domainProjectOptions.ProjectTestCsProj,
                    false,
                    true,
                    $"{domainProjectOptions.ProjectName}.Tests",
                    domainProjectOptions.ApiOptions.Generator.UseNullableReferenceTypes,
                    null,
                    NugetPackageReferenceHelper.CreateForTestProject(false),
                    projectReferences,
                    true));
            }

            return logItems;
        }

        private static List<LogKeyValueItem> GenerateSrcHandlers(DomainProjectOptions domainProjectOptions, out List<SyntaxGeneratorHandler> sgHandlers)
        {
            if (domainProjectOptions == null)
            {
                throw new ArgumentNullException(nameof(domainProjectOptions));
            }

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

        private static List<LogKeyValueItem> GenerateTestHandlers(DomainProjectOptions domainProjectOptions, List<SyntaxGeneratorHandler> sgHandlers)
        {
            if (domainProjectOptions == null)
            {
                throw new ArgumentNullException(nameof(domainProjectOptions));
            }

            if (sgHandlers == null)
            {
                throw new ArgumentNullException(nameof(sgHandlers));
            }

            var logItems = new List<LogKeyValueItem>();
            if (domainProjectOptions.PathForTestHandlers != null)
            {
                foreach (var sgHandler in sgHandlers)
                {
                    logItems.Add(GenerateServerDomainXunitTestHelper.GenerateGeneratedTests(domainProjectOptions, sgHandler));
                    logItems.Add(GenerateServerDomainXunitTestHelper.GenerateCustomTests(domainProjectOptions, sgHandler));
                }
            }

            return logItems;
        }

        private static void ScaffoldBasicFileDomainRegistration(DomainProjectOptions domainProjectOptions)
        {
            // Create compilationUnit
            var compilationUnit = SyntaxFactory.CompilationUnit();

            // Create a namespace
            var @namespace = SyntaxProjectFactory.CreateNamespace(domainProjectOptions);

            // Create class
            var classDeclaration = SyntaxClassDeclarationFactory.Create("DomainRegistration")
                .AddGeneratedCodeAttribute(domainProjectOptions.ToolName, domainProjectOptions.ToolVersion.ToString());

            // Add class to namespace
            @namespace = @namespace.AddMembers(classDeclaration);

            // Add using statement to compilationUnit
            compilationUnit = compilationUnit.AddUsingStatements(new[] { "System.CodeDom.Compiler" });

            // Add namespace to compilationUnit
            compilationUnit = compilationUnit.AddMembers(@namespace);

            var codeAsString = compilationUnit
                .NormalizeWhitespace()
                .ToFullString();

            var file = new FileInfo(Path.Combine(domainProjectOptions.PathForSrcGenerate.FullName, "DomainRegistration.cs"));
            TextFileHelper.Save(file, codeAsString);
        }
    }
}