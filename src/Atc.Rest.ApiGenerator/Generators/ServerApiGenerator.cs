using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Atc.CodeAnalysis.CSharp.SyntaxFactories;
using Atc.Data.Models;
using Atc.Rest.ApiGenerator.Helpers;
using Atc.Rest.ApiGenerator.Models;
using Atc.Rest.ApiGenerator.ProjectSyntaxFactories;
using Atc.Rest.ApiGenerator.SyntaxGenerators.Api;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.OpenApi.Writers;

// ReSharper disable ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
// ReSharper disable SuggestBaseTypeForParameter
// ReSharper disable ReturnTypeCanBeEnumerable.Local
// ReSharper disable UseObjectOrCollectionInitializer
namespace Atc.Rest.ApiGenerator.Generators
{
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

            logItems.Add(ValidateVersioning(projectOptions));
            if (logItems.Any(x => x.LogCategory == LogCategoryType.Error))
            {
                return logItems;
            }

            logItems.AddRange(ScaffoldSrc(projectOptions));
            if (projectOptions.PathForTestGenerate != null)
            {
                logItems.AddRange(ScaffoldTest(projectOptions));
            }

            CopyApiSpecification(projectOptions);

            var operationSchemaMappings = OpenApiOperationSchemaMapHelper.CollectMappings(projectOptions.Document);
            logItems.AddRange(GenerateContracts(projectOptions, operationSchemaMappings));
            logItems.AddRange(GenerateEndpoints(projectOptions, operationSchemaMappings));
            logItems.AddRange(PerformCleanup(projectOptions, logItems));
            return logItems;
        }

        private static LogKeyValueItem ValidateVersioning(ApiProjectOptions apiProjectOptions)
        {
            if (apiProjectOptions == null)
            {
                throw new ArgumentNullException(nameof(apiProjectOptions));
            }

            if (!Directory.Exists(apiProjectOptions.PathForSrcGenerate.FullName))
            {
                return LogItemHelper.Create(LogCategoryType.Information, ValidationRuleNameConstants.ProjectApiGenerated01, "Old project don't exist.");
            }

            var apiGeneratedFile = Path.Combine(apiProjectOptions.PathForSrcGenerate.FullName, "ApiRegistration.cs");
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

        private static List<LogKeyValueItem> ScaffoldSrc(ApiProjectOptions apiProjectOptions)
        {
            if (apiProjectOptions == null)
            {
                throw new ArgumentNullException(nameof(apiProjectOptions));
            }

            if (!Directory.Exists(apiProjectOptions.PathForSrcGenerate.FullName))
            {
                Directory.CreateDirectory(apiProjectOptions.PathForSrcGenerate.FullName);
            }

            var logItems = new List<LogKeyValueItem>();

            if (apiProjectOptions.PathForSrcGenerate.Exists && apiProjectOptions.ProjectSrcCsProj.Exists)
            {
                var element = XElement.Load(apiProjectOptions.ProjectSrcCsProj.FullName);
                var originalNullableValue = SolutionAndProjectHelper.GetBoolFromNullableString(SolutionAndProjectHelper.GetNullableValueFromProject(element));

                bool hasUpdates = false;
                if (apiProjectOptions.ApiOptions.Generator.UseNullableReferenceTypes != originalNullableValue)
                {
                    var newNullableValue = SolutionAndProjectHelper.GetNullableStringFromBool(apiProjectOptions.ApiOptions.Generator.UseNullableReferenceTypes);
                    SolutionAndProjectHelper.SetNullableValueForProject(element, newNullableValue);
                    element.Save(apiProjectOptions.ProjectSrcCsProj.FullName);
                    logItems.Add(new LogKeyValueItem(LogCategoryType.Debug, "FileUpdate", "#", $"Update API csproj - Nullable value={newNullableValue}"));
                    hasUpdates = true;
                }

                if (!hasUpdates)
                {
                    logItems.Add(new LogKeyValueItem(LogCategoryType.Debug, "FileSkip", "#", "No updates for API csproj"));
                }
            }
            else
            {
                logItems.Add(SolutionAndProjectHelper.ScaffoldProjFile(
                    apiProjectOptions.ProjectSrcCsProj,
                    false,
                    false,
                    apiProjectOptions.ProjectName,
                    apiProjectOptions.ApiOptions.Generator.UseNullableReferenceTypes,
                    new List<string> { "Microsoft.AspNetCore.App" },
                    NugetPackageReferenceHelper.CreateForApiProject(),
                    null,
                    true));
            }

            ScaffoldBasicFileApiGenerated(apiProjectOptions);
            DeleteLegacyScaffoldBasicFileResultFactory(apiProjectOptions);
            DeleteLegacyScaffoldBasicFilePagination(apiProjectOptions);

            return logItems;
        }

        private static List<LogKeyValueItem> ScaffoldTest(ApiProjectOptions apiProjectOptions)
        {
            if (apiProjectOptions == null)
            {
                throw new ArgumentNullException(nameof(apiProjectOptions));
            }

            var logItems = new List<LogKeyValueItem>();

            if (apiProjectOptions.PathForTestGenerate == null || apiProjectOptions.ProjectTestCsProj == null)
            {
                return logItems;
            }

            if (apiProjectOptions.PathForTestGenerate.Exists && apiProjectOptions.ProjectTestCsProj.Exists)
            {
                logItems.Add(new LogKeyValueItem(LogCategoryType.Debug, "FileSkip", "#", "No updates for API test csproj"));
            }
            else
            {
                if (!Directory.Exists(apiProjectOptions.PathForTestGenerate.FullName))
                {
                    Directory.CreateDirectory(apiProjectOptions.PathForTestGenerate.FullName);
                }

                var projectReferences = new List<FileInfo>
                {
                    apiProjectOptions.ProjectSrcCsProj,
                };

                logItems.Add(SolutionAndProjectHelper.ScaffoldProjFile(
                    apiProjectOptions.ProjectTestCsProj,
                    false,
                    true,
                    $"{apiProjectOptions.ProjectName}.Tests",
                    apiProjectOptions.ApiOptions.Generator.UseNullableReferenceTypes,
                    null,
                    NugetPackageReferenceHelper.CreateForTestProject(false),
                    projectReferences,
                    true));
            }

            return logItems;
        }

        private static void CopyApiSpecification(ApiProjectOptions apiProjectOptions)
        {
            if (apiProjectOptions == null)
            {
                throw new ArgumentNullException(nameof(apiProjectOptions));
            }

            var resourceFolder = new DirectoryInfo(Path.Combine(apiProjectOptions.PathForSrcGenerate.FullName, "Resources"));
            if (!resourceFolder.Exists)
            {
                Directory.CreateDirectory(resourceFolder.FullName);
            }

            var resourceFile = new FileInfo(Path.Combine(resourceFolder.FullName, "ApiSpecification.yaml"));
            if (File.Exists(resourceFile.FullName))
            {
                File.Delete(resourceFile.FullName);
            }

            if (apiProjectOptions.DocumentFile.Extension.Equals(".json", StringComparison.OrdinalIgnoreCase))
            {
                using var writeFile = new StreamWriter(resourceFile.FullName);
                apiProjectOptions.Document.SerializeAsV3(new OpenApiYamlWriter(writeFile));
            }
            else
            {
                File.Copy(apiProjectOptions.DocumentFile.FullName, resourceFile.FullName);
            }
        }

        private static List<LogKeyValueItem> GenerateContracts(ApiProjectOptions apiProjectOptions, List<ApiOperationSchemaMap> operationSchemaMappings)
        {
            if (apiProjectOptions == null)
            {
                throw new ArgumentNullException(nameof(apiProjectOptions));
            }

            if (operationSchemaMappings == null)
            {
                throw new ArgumentNullException(nameof(operationSchemaMappings));
            }

            var sgContractModels = new List<SyntaxGeneratorContractModel>();
            var sgContractParameters = new List<SyntaxGeneratorContractParameter>();
            var sgContractResults = new List<SyntaxGeneratorContractResult>();
            var sgContractInterfaces = new List<SyntaxGeneratorContractInterface>();
            foreach (var basePathSegmentName in apiProjectOptions.BasePathSegmentNames)
            {
                var generatorModels = new SyntaxGeneratorContractModels(apiProjectOptions, operationSchemaMappings, basePathSegmentName);
                var generatedModels = generatorModels.GenerateSyntaxTrees();
                sgContractModels.AddRange(generatedModels);

                var generatorParameters = new SyntaxGeneratorContractParameters(apiProjectOptions, basePathSegmentName);
                var generatedParameters = generatorParameters.GenerateSyntaxTrees();
                sgContractParameters.AddRange(generatedParameters);

                var generatorResults = new SyntaxGeneratorContractResults(apiProjectOptions, basePathSegmentName);
                var generatedResults = generatorResults.GenerateSyntaxTrees();
                sgContractResults.AddRange(generatedResults);

                var generatorInterfaces = new SyntaxGeneratorContractInterfaces(apiProjectOptions, basePathSegmentName);
                var generatedInterfaces = generatorInterfaces.GenerateSyntaxTrees();
                sgContractInterfaces.AddRange(generatedInterfaces);
            }

            var missingOperationSchemaMappings = new List<ApiOperationSchemaMap>();
            foreach (var map in operationSchemaMappings)
            {
                if (sgContractModels.FirstOrDefault(x => x.ApiSchemaKey.Equals(map.SchemaKey, StringComparison.OrdinalIgnoreCase)) == null)
                {
                    missingOperationSchemaMappings.Add(map);
                }
            }

            foreach (var map in missingOperationSchemaMappings)
            {
                if (missingOperationSchemaMappings.Count(x => x.SchemaKey.Equals(map.SchemaKey, StringComparison.OrdinalIgnoreCase)) > 1)
                {
                    throw new NotImplementedException($"SchemaKey: {map.SchemaKey} is not generated and exist multiple times - location-calculation is missing.");
                }

                var generatorModel = new SyntaxGeneratorContractModel(
                    apiProjectOptions,
                    map.SchemaKey,
                    apiProjectOptions.Document.Components.Schemas.First(x => x.Key.Equals(map.SchemaKey, StringComparison.OrdinalIgnoreCase)).Value,
                    map.SegmentName);

                generatorModel.GenerateCode();
                sgContractModels.Add(generatorModel);
            }

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

        private static List<LogKeyValueItem> GenerateEndpoints(ApiProjectOptions apiProjectOptions, List<ApiOperationSchemaMap> operationSchemaMappings)
        {
            if (apiProjectOptions == null)
            {
                throw new ArgumentNullException(nameof(apiProjectOptions));
            }

            if (operationSchemaMappings == null)
            {
                throw new ArgumentNullException(nameof(operationSchemaMappings));
            }

            var sgEndpoints = new List<SyntaxGeneratorEndpointControllers>();
            foreach (var segmentName in apiProjectOptions.BasePathSegmentNames)
            {
                var generator = new SyntaxGeneratorEndpointControllers(apiProjectOptions, operationSchemaMappings, segmentName);
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

        private static List<LogKeyValueItem> PerformCleanup(ApiProjectOptions apiProjectOptions, List<LogKeyValueItem> orgLogItems)
        {
            if (apiProjectOptions == null)
            {
                throw new ArgumentNullException(nameof(apiProjectOptions));
            }

            if (orgLogItems == null)
            {
                throw new ArgumentNullException(nameof(orgLogItems));
            }

            var logItems = new List<LogKeyValueItem>();

            if (Directory.Exists(apiProjectOptions.PathForContracts.FullName))
            {
                var files = Directory.GetFiles(apiProjectOptions.PathForContracts.FullName, "*.*", SearchOption.AllDirectories);
                foreach (string file in files)
                {
                    if (orgLogItems.FirstOrDefault(x => x.Description == file) != null)
                    {
                        continue;
                    }

                    File.Delete(file);
                    logItems.Add(new LogKeyValueItem(LogCategoryType.Debug, "FileDelete", "#", file));
                }
            }

            if (Directory.Exists(apiProjectOptions.PathForEndpoints.FullName))
            {
                var files = Directory.GetFiles(apiProjectOptions.PathForEndpoints.FullName, "*.*", SearchOption.AllDirectories);
                foreach (string file in files)
                {
                    if (orgLogItems.FirstOrDefault(x => x.Description == file) != null)
                    {
                        continue;
                    }

                    File.Delete(file);
                    logItems.Add(new LogKeyValueItem(LogCategoryType.Debug, "FileDelete", "#", file));
                }
            }

            return logItems;
        }

        private static void ScaffoldBasicFileApiGenerated(ApiProjectOptions apiProjectOptions)
        {
            // Create compilationUnit
            var compilationUnit = SyntaxFactory.CompilationUnit();

            // Create a namespace
            var @namespace = SyntaxProjectFactory.CreateNamespace(apiProjectOptions);

            // Create class
            var classDeclaration = SyntaxClassDeclarationFactory.Create("ApiRegistration")
                .AddGeneratedCodeAttribute(apiProjectOptions.ToolName, apiProjectOptions.ToolVersion.ToString());

            // Add class to namespace
            @namespace = @namespace.AddMembers(classDeclaration);

            // Add using statement to compilationUnit
            compilationUnit = compilationUnit.AddUsingStatements(new[] { "System.CodeDom.Compiler" });

            // Add namespace to compilationUnit
            compilationUnit = compilationUnit.AddMembers(@namespace);

            var codeAsString = compilationUnit
                .NormalizeWhitespace()
                .ToFullString();

            var file = new FileInfo(Path.Combine(apiProjectOptions.PathForSrcGenerate.FullName, "ApiRegistration.cs"));
            TextFileHelper.Save(file, codeAsString);
        }

        private static void DeleteLegacyScaffoldBasicFileResultFactory(ApiProjectOptions apiProjectOptions)
        {
            var file = new FileInfo(Path.Combine(apiProjectOptions.PathForSrcGenerate.FullName, "ResultFactory.cs"));
            if (file.Exists)
            {
                File.Delete(file.FullName);
            }
        }

        private static void DeleteLegacyScaffoldBasicFilePagination(ApiProjectOptions apiProjectOptions)
        {
            var (key, _) = apiProjectOptions.Document.Components.Schemas.FirstOrDefault(x => x.Key.Equals(Microsoft.OpenApi.Models.NameConstants.Pagination, StringComparison.OrdinalIgnoreCase));
            if (key == null)
            {
                return;
            }

            var file = new FileInfo(Path.Combine(apiProjectOptions.PathForSrcGenerate.FullName, $"{Microsoft.OpenApi.Models.NameConstants.Pagination}.cs"));
            if (file.Exists)
            {
                File.Delete(file.FullName);
            }
        }
    }
}