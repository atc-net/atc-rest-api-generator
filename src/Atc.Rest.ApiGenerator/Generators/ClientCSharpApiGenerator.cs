using Atc.Console.Spectre;

// ReSharper disable ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
// ReSharper disable ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
namespace Atc.Rest.ApiGenerator.Generators;

public class ClientCSharpApiGenerator
{
    private readonly ILogger logger;
    private readonly ClientCSharpApiProjectOptions projectOptions;
    private readonly ApiProjectOptions apiProjectOptions;

    public ClientCSharpApiGenerator(
        ILogger logger,
        ClientCSharpApiProjectOptions projectOptions)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.projectOptions = projectOptions ?? throw new ArgumentNullException(nameof(projectOptions));

        this.apiProjectOptions = new ApiProjectOptions(
            projectOptions.PathForSrcGenerate,
            projectTestGeneratePath: null,
            projectOptions.Document,
            projectOptions.DocumentFile,
            projectOptions.ProjectName,
            projectSuffixName: null,
            projectOptions.ApiOptions,
            projectOptions.UsingCodingRules,
            projectOptions.ForClient,
            projectOptions.ClientFolderName);

        this.ExcludeEndpointGeneration = projectOptions.ExcludeEndpointGeneration;
    }

    public bool ExcludeEndpointGeneration { get; }

    public bool Generate()
    {
        ScaffoldSrc();

        var operationSchemaMappings = OpenApiOperationSchemaMapHelper.CollectMappings(projectOptions.Document);
        GenerateContracts(operationSchemaMappings);
        if (!this.ExcludeEndpointGeneration)
        {
            GenerateEndpoints(operationSchemaMappings);
        }

        PerformCleanup();
        return true;
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
            logger.LogDebug($"{EmojisConstants.FileNotUpdated}   No updates for csproj");
        }
        else
        {
            SolutionAndProjectHelper.ScaffoldProjFile(
                logger,
                projectOptions.ProjectSrcCsProj,
                projectOptions.ProjectSrcCsProjDisplayLocation,
                createAsWeb: false,
                createAsTestProject: false,
                projectName: projectOptions.ProjectName,
                "netstandard2.1",
                frameworkReferences: null,
                packageReferences: NugetPackageReferenceHelper.CreateForClientApiProject(),
                projectReferences: null,
                includeApiSpecification: false,
                usingCodingRules: projectOptions.UsingCodingRules);
        }
    }

    private void GenerateContracts(
        List<ApiOperationSchemaMap> operationSchemaMappings)
    {
        ArgumentNullException.ThrowIfNull(operationSchemaMappings);

        var sgContractModels = new List<SyntaxGeneratorContractModel>();
        var sgContractParameters = new List<SyntaxGeneratorContractParameter>();
        foreach (var basePathSegmentName in projectOptions.BasePathSegmentNames)
        {
            var generatorModels = new SyntaxGeneratorContractModels(logger, apiProjectOptions, operationSchemaMappings, basePathSegmentName);
            var generatedModels = generatorModels.GenerateSyntaxTrees();
            sgContractModels.AddRange(generatedModels);

            var generatorParameters = new SyntaxGeneratorContractParameters(logger, apiProjectOptions, basePathSegmentName);
            var generatedParameters = generatorParameters.GenerateSyntaxTrees();
            sgContractParameters.AddRange(generatedParameters);
        }

        ApiGeneratorHelper.CollectMissingContractModelFromOperationSchemaMappings(
            logger,
            apiProjectOptions,
            operationSchemaMappings,
            sgContractModels);

        foreach (var sg in sgContractModels)
        {
            sg.IsForClient = true;
            sg.UseOwnFolder = false;
            sg.ToFile();
        }

        foreach (var sg in sgContractParameters)
        {
            sg.IsForClient = true;
            sg.UseOwnFolder = false;
            sg.ToFile();
        }
    }

    private void GenerateEndpoints(
        List<ApiOperationSchemaMap> operationSchemaMappings)
    {
        ArgumentNullException.ThrowIfNull(operationSchemaMappings);

        var sgEndpointResults = new List<SyntaxGeneratorClientEndpointResult>();
        var sgEndpointInterfaces = new List<SyntaxGeneratorClientEndpointInterface>();
        var sgEndpoints = new List<SyntaxGeneratorClientEndpoint>();
        foreach (var basePathSegmentName in projectOptions.BasePathSegmentNames)
        {
            var generatorEndpointResults = new SyntaxGeneratorClientEndpointResults(logger, apiProjectOptions, operationSchemaMappings, basePathSegmentName);
            var generatedEndpointResults = generatorEndpointResults.GenerateSyntaxTrees();
            sgEndpointResults.AddRange(generatedEndpointResults);

            var generatorEndpointInterfaces = new SyntaxGeneratorClientEndpointInterfaces(logger, apiProjectOptions, operationSchemaMappings, basePathSegmentName);
            var generatedEndpointInterfaces = generatorEndpointInterfaces.GenerateSyntaxTrees();
            sgEndpointInterfaces.AddRange(generatedEndpointInterfaces);

            var generatorEndpoints = new SyntaxGeneratorClientEndpoints(logger, apiProjectOptions, operationSchemaMappings, basePathSegmentName);
            var generatedEndpoints = generatorEndpoints.GenerateSyntaxTrees();
            sgEndpoints.AddRange(generatedEndpoints);
        }

        foreach (var sg in sgEndpointResults)
        {
            sg.ToFile();
        }

        foreach (var sg in sgEndpointInterfaces)
        {
            sg.ToFile();
        }

        foreach (var sg in sgEndpoints)
        {
            sg.ToFile();
        }
    }

    private static void PerformCleanup()
    {
        // TODO: Implement
    }
}