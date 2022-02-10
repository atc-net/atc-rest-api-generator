// ReSharper disable ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
// ReSharper disable ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
namespace Atc.Rest.ApiGenerator.Generators
{
    public class ClientCSharpApiGenerator
    {
        private readonly ClientCSharpApiProjectOptions projectOptions;
        private readonly ApiProjectOptions apiProjectOptions;

        public ClientCSharpApiGenerator(
            ClientCSharpApiProjectOptions projectOptions)
        {
            this.projectOptions = projectOptions ?? throw new ArgumentNullException(nameof(projectOptions));

            this.apiProjectOptions = new ApiProjectOptions(
                projectOptions.PathForSrcGenerate,
                projectTestGeneratePath: null,
                projectOptions.Document,
                projectOptions.DocumentFile,
                projectOptions.ProjectName,
                projectSuffixName: null,
                projectOptions.ApiOptions,
                projectOptions.ForClient,
                projectOptions.ClientFolderName);

            this.ExcludeEndpointGeneration = projectOptions.ExcludeEndpointGeneration;
        }

        public bool ExcludeEndpointGeneration { get; }

        public List<LogKeyValueItem> Generate()
        {
            var logItems = new List<LogKeyValueItem>();

            logItems.AddRange(ScaffoldSrc());

            var operationSchemaMappings = OpenApiOperationSchemaMapHelper.CollectMappings(projectOptions.Document);
            logItems.AddRange(GenerateContracts(operationSchemaMappings));
            if (!this.ExcludeEndpointGeneration)
            {
                logItems.AddRange(GenerateEndpoints(operationSchemaMappings));
            }

            logItems.AddRange(PerformCleanup(logItems));
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
                logItems.Add(LogItemFactory.CreateDebug("FileSkip", "#", "No updates for csproj"));
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
                    packageReferences: NugetPackageReferenceHelper.CreateForClientApiProject(),
                    projectReferences: null,
                    includeApiSpecification: false));
            }

            return logItems;
        }

        private List<LogKeyValueItem> GenerateContracts(
            List<ApiOperationSchemaMap> operationSchemaMappings)
        {
            ArgumentNullException.ThrowIfNull(operationSchemaMappings);

            var sgContractModels = new List<SyntaxGeneratorContractModel>();
            var sgContractParameters = new List<SyntaxGeneratorContractParameter>();
            foreach (var basePathSegmentName in projectOptions.BasePathSegmentNames)
            {
                var generatorModels = new SyntaxGeneratorContractModels(apiProjectOptions, operationSchemaMappings, basePathSegmentName);
                var generatedModels = generatorModels.GenerateSyntaxTrees();
                sgContractModels.AddRange(generatedModels);

                var generatorParameters = new SyntaxGeneratorContractParameters(apiProjectOptions, basePathSegmentName);
                var generatedParameters = generatorParameters.GenerateSyntaxTrees();
                sgContractParameters.AddRange(generatedParameters);
            }

            ApiGeneratorHelper.CollectMissingContractModelFromOperationSchemaMappings(
                apiProjectOptions,
                operationSchemaMappings,
                sgContractModels);

            var logItems = new List<LogKeyValueItem>();
            foreach (var sg in sgContractModels)
            {
                sg.IsForClient = true;
                sg.UseOwnFolder = false;
                logItems.Add(sg.ToFile());
            }

            foreach (var sg in sgContractParameters)
            {
                sg.IsForClient = true;
                sg.UseOwnFolder = false;
                logItems.Add(sg.ToFile());
            }

            return logItems;
        }

        private List<LogKeyValueItem> GenerateEndpoints(
            List<ApiOperationSchemaMap> operationSchemaMappings)
        {
            ArgumentNullException.ThrowIfNull(operationSchemaMappings);

            var sgEndpointResults = new List<SyntaxGeneratorClientEndpointResult>();
            var sgEndpointInterfaces = new List<SyntaxGeneratorClientEndpointInterface>();
            var sgEndpoints = new List<SyntaxGeneratorClientEndpoint>();
            foreach (var basePathSegmentName in projectOptions.BasePathSegmentNames)
            {
                var generatorEndpointResults = new SyntaxGeneratorClientEndpointResults(apiProjectOptions, operationSchemaMappings, basePathSegmentName);
                var generatedEndpointResults = generatorEndpointResults.GenerateSyntaxTrees();
                sgEndpointResults.AddRange(generatedEndpointResults);

                var generatorEndpointInterfaces = new SyntaxGeneratorClientEndpointInterfaces(apiProjectOptions, operationSchemaMappings, basePathSegmentName);
                var generatedEndpointInterfaces = generatorEndpointInterfaces.GenerateSyntaxTrees();
                sgEndpointInterfaces.AddRange(generatedEndpointInterfaces);

                var generatorEndpoints = new SyntaxGeneratorClientEndpoints(apiProjectOptions, operationSchemaMappings, basePathSegmentName);
                var generatedEndpoints = generatorEndpoints.GenerateSyntaxTrees();
                sgEndpoints.AddRange(generatedEndpoints);
            }

            var logItems = new List<LogKeyValueItem>();

            foreach (var sg in sgEndpointResults)
            {
                logItems.Add(sg.ToFile());
            }

            foreach (var sg in sgEndpointInterfaces)
            {
                logItems.Add(sg.ToFile());
            }

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

            // TODO: Fix Cleanup
            ////var apiProjectOptions = new ApiProjectOptions(
            ////    projectOptions.PathForSrcGenerate,
            ////    projectTestGeneratePath: null,
            ////    projectOptions.Document,
            ////    projectOptions.DocumentFile,
            ////    projectOptions.ProjectName,
            ////    projectSuffixName: null,
            ////    projectOptions.ApiOptions,
            ////    projectOptions.ClientFolderName);

            ////if (Directory.Exists(projectOptions.PathForContracts.FullName))
            ////{
            ////    var files = Directory.GetFiles(projectOptions.PathForContracts.FullName, "*.*", SearchOption.AllDirectories);
            ////    foreach (string file in files)
            ////    {
            ////        if (orgLogItems.FirstOrDefault(x => x.Description == file) is not null)
            ////        {
            ////            continue;
            ////        }

            ////        File.Delete(file);
            ////        logItems.Add(LogItemFactory.CreateDebug("FileDelete", "#", file));
            ////    }
            ////}

            ////if (Directory.Exists(projectOptions.PathForEndpoints.FullName))
            ////{
            ////    var files = Directory.GetFiles(projectOptions.PathForEndpoints.FullName, "*.*", SearchOption.AllDirectories);
            ////    foreach (string file in files)
            ////    {
            ////        if (orgLogItems.FirstOrDefault(x => x.Description == file) is not null)
            ////        {
            ////            continue;
            ////        }

            ////        File.Delete(file);
            ////        logItems.Add(LogItemFactory.CreateDebug("FileDelete", "#", file));
            ////    }
            ////}

            return logItems;
        }
    }
}