using System;
using System.Collections.Generic;
using System.IO;
using Atc.Data.Models;
using Atc.Rest.ApiGenerator.Helpers;
using Atc.Rest.ApiGenerator.Models;
using Atc.Rest.ApiGenerator.SyntaxGenerators.Api;

// ReSharper disable ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
// ReSharper disable ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
namespace Atc.Rest.ApiGenerator.Generators
{
    public class ClientCSharpApiGenerator
    {
        private readonly ClientCSharpApiProjectOptions projectOptions;

        public ClientCSharpApiGenerator(ClientCSharpApiProjectOptions projectOptions)
        {
            this.projectOptions = projectOptions ?? throw new ArgumentNullException(nameof(projectOptions));
        }

        public List<LogKeyValueItem> Generate()
        {
            var logItems = new List<LogKeyValueItem>();

            logItems.AddRange(ScaffoldSrc());

            var operationSchemaMappings = OpenApiOperationSchemaMapHelper.CollectMappings(projectOptions.Document);
            logItems.AddRange(GenerateContracts(operationSchemaMappings));

            return logItems;
        }

        private List<LogKeyValueItem> ScaffoldSrc()
        {
            if (!Directory.Exists(projectOptions.PathForSrcGenerate.FullName))
            {
                Directory.CreateDirectory(projectOptions.PathForSrcGenerate.FullName);
            }

            var logItems = new List<LogKeyValueItem>();

            if (projectOptions.PathForSrcGenerate.Exists && projectOptions.ProjectSrcCsProj.Exists)
            {
                logItems.Add(new LogKeyValueItem(LogCategoryType.Debug, "FileSkip", "#", "No updates for csproj"));
            }
            else
            {
                logItems.Add(SolutionAndProjectHelper.ScaffoldProjFile(
                    projectOptions.ProjectSrcCsProj,
                    createAsWeb: false,
                    createAsTestProject: false,
                    projectName: projectOptions.ProjectName,
                    "netstandard2.1",
                    useNullableReferenceTypes: projectOptions.ApiOptions.Generator.UseNullableReferenceTypes,
                    frameworkReferences: null,
                    packageReferences: null,
                    projectReferences: null,
                    includeApiSpecification: false));
            }

            return logItems;
        }

        private List<LogKeyValueItem> GenerateContracts(List<ApiOperationSchemaMap> operationSchemaMappings)
        {
            if (operationSchemaMappings == null)
            {
                throw new ArgumentNullException(nameof(operationSchemaMappings));
            }

            var apiProjectOptions = new ApiProjectOptions(
                projectOptions.PathForSrcGenerate,
                projectTestGeneratePath: null,
                projectOptions.Document,
                projectOptions.DocumentFile,
                projectOptions.ProjectName,
                projectSuffixName: null,
                projectOptions.ApiOptions,
                projectOptions.ClientFolderName);

            var sgContractModels = new List<SyntaxGeneratorContractModel>();
            foreach (var basePathSegmentName in projectOptions.BasePathSegmentNames)
            {
                var generatorModels = new SyntaxGeneratorContractModels(apiProjectOptions, operationSchemaMappings, basePathSegmentName);
                var generatedModels = generatorModels.GenerateSyntaxTrees();
                sgContractModels.AddRange(generatedModels);
            }

            ApiGeneratorHelper.CollectMissingContractModelFromOperationSchemaMappings(
                apiProjectOptions,
                operationSchemaMappings,
                sgContractModels);

            var logItems = new List<LogKeyValueItem>();
            foreach (var sg in sgContractModels)
            {
                sg.UseModelFolder = false;
                logItems.Add(sg.ToFile());
            }

            return logItems;
        }
    }
}