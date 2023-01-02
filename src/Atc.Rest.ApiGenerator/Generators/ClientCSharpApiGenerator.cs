// ReSharper disable ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
// ReSharper disable ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
namespace Atc.Rest.ApiGenerator.Generators;

public class ClientCSharpApiGenerator
{
    private readonly ILogger logger;
    private readonly IApiOperationExtractor apiOperationExtractor;
    private readonly ClientCSharpApiProjectOptions projectOptions;
    private readonly ApiProjectOptions apiProjectOptions;

    public ClientCSharpApiGenerator(
        ILogger logger,
        IApiOperationExtractor apiOperationExtractor,
        ClientCSharpApiProjectOptions projectOptions)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.apiOperationExtractor = apiOperationExtractor ?? throw new ArgumentNullException(nameof(apiOperationExtractor));
        this.projectOptions = projectOptions ?? throw new ArgumentNullException(nameof(projectOptions));

        apiProjectOptions = new ApiProjectOptions(
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
    }

    public bool Generate()
    {
        logger.LogInformation($"{AppEmojisConstants.AreaGenerateCode} Working on client api generation ({projectOptions.ProjectName})");

        ScaffoldSrc();

        var operationSchemaMappings = apiOperationExtractor.Extract(projectOptions.Document);

        GenerateContracts(operationSchemaMappings);
        if (!projectOptions.ExcludeEndpointGeneration)
        {
            GenerateClientEndpoints(operationSchemaMappings);
        }

        GenerateSrcGlobalUsings();

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
            var hasUpdates = SolutionAndProjectHelper.EnsureLatestPackageReferencesVersionInProjFile(
                logger,
                projectOptions.ProjectSrcCsProj,
                projectOptions.ProjectSrcCsProjDisplayLocation,
                ProjectType.ClientApi,
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
                ProjectType.ClientApi,
                createAsWeb: false,
                createAsTestProject: false,
                projectName: projectOptions.ProjectName,
                "net6.0",
                frameworkReferences: null,
                packageReferences: NugetPackageReferenceHelper.CreateForClientApiProject(),
                projectReferences: null,
                includeApiSpecification: false,
                usingCodingRules: projectOptions.UsingCodingRules);
        }
    }

    private void GenerateContracts(
        IList<ApiOperation> operationSchemaMappings)
    {
        ArgumentNullException.ThrowIfNull(operationSchemaMappings);

        foreach (var basePathSegmentName in projectOptions.BasePathSegmentNames)
        {
            var apiGroupName = basePathSegmentName.EnsureFirstCharacterToUpper();

            GenerateModels(projectOptions.Document, apiGroupName, operationSchemaMappings);
            GenerateParameters(projectOptions.Document, apiGroupName);
        }
    }

    private void GenerateModels(
        OpenApiDocument document,
        string apiGroupName,
        IEnumerable<ApiOperation> operationSchemaMappings)
    {
        var apiOperations = operationSchemaMappings
            .Where(x => x.LocatedArea is ApiSchemaMapLocatedAreaType.Response or ApiSchemaMapLocatedAreaType.RequestBody &&
                        x.SegmentName.Equals(apiGroupName, StringComparison.OrdinalIgnoreCase))
            .ToList();

        var apiOperationModels = GetDistinctApiOperationModels(apiOperations);

        foreach (var apiOperationModel in apiOperationModels)
        {
            var apiSchema = document.Components.Schemas.First(x => x.Key.Equals(apiOperationModel.Name, StringComparison.OrdinalIgnoreCase));

            var modelName = apiSchema.Key.EnsureFirstCharacterToUpper();

            if (apiOperationModel.IsEnum)
            {
                GenerateEnumerationType(modelName, apiSchema.Value.GetEnumSchema().Item2);
            }
            else
            {
                GenerateModel(modelName, apiSchema.Value, apiGroupName, apiOperationModel.IsShared);
            }
        }
    }

    private void GenerateEnumerationType(
        string enumerationName,
        OpenApiSchema openApiSchemaEnumeration)
    {
        var fullNamespace = $"{projectOptions.ProjectName}.{ContentGeneratorConstants.Contracts}";

        // Generate
        var enumerationParameters = ContentGeneratorServerClientEnumerationParametersFactory.Create(
            fullNamespace,
            enumerationName,
            openApiSchemaEnumeration.Enum);

        var contentGeneratorEnum = new ContentGeneratorServerClientEnumeration(
            new GeneratedCodeHeaderGenerator(new GeneratedCodeGeneratorParameters(projectOptions.ApiGeneratorVersion)),
            new GeneratedCodeAttributeGenerator(new GeneratedCodeGeneratorParameters(projectOptions.ApiGeneratorVersion)),
            new CodeDocumentationTagsGenerator(),
            enumerationParameters);

        var enumContent = contentGeneratorEnum.Generate();

        // Write
        var file = new FileInfo(Helpers.DirectoryInfoHelper.GetCsFileNameForContractEnumTypes(
            apiProjectOptions.PathForContracts,
            enumerationName));

        var contentWriter = new ContentWriter(logger);
        contentWriter.Write(
            projectOptions.PathForSrcGenerate,
            file,
            ContentWriterArea.Src,
            enumContent);
    }

    private void GenerateModel(
        string modelName,
        OpenApiSchema apiSchemaModel,
        string apiGroupName,
        bool isSharedContract)
    {
        var fullNamespace = $"{projectOptions.ProjectName}.{ContentGeneratorConstants.Contracts}";

        // Generate
        var modelParameters = ContentGeneratorServerClientModelParametersFactory.Create(
            fullNamespace,
            modelName,
            apiSchemaModel);

        var contentGeneratorModel = new ContentGeneratorServerClientModel(
            new GeneratedCodeHeaderGenerator(new GeneratedCodeGeneratorParameters(projectOptions.ApiGeneratorVersion)),
            new GeneratedCodeAttributeGenerator(new GeneratedCodeGeneratorParameters(projectOptions.ApiGeneratorVersion)),
            new CodeDocumentationTagsGenerator(),
            modelParameters);

        var modelContent = contentGeneratorModel.Generate();

        // Write
        FileInfo file;
        if (modelName.EndsWith(NameConstants.Request, StringComparison.Ordinal))
        {
            file = new FileInfo(
                Helpers.DirectoryInfoHelper.GetCsFileNameForContract(
                    apiProjectOptions.PathForContracts,
                    apiGroupName,
                    NameConstants.ClientRequestParameters,
                    modelName));
        }
        else
        {
            if (isSharedContract)
            {
                file = new FileInfo(
                    Helpers.DirectoryInfoHelper.GetCsFileNameForContractShared(
                        apiProjectOptions.PathForContractsShared,
                        modelName));
            }
            else
            {
                file = new FileInfo(
                    Helpers.DirectoryInfoHelper.GetCsFileNameForContract(
                        apiProjectOptions.PathForContracts,
                        apiGroupName,
                        NameConstants.ContractModels,
                        modelName));
            }
        }

        var contentWriter = new ContentWriter(logger);
        contentWriter.Write(
            projectOptions.PathForSrcGenerate,
            file,
            ContentWriterArea.Src,
            modelContent);
    }

    private void GenerateParameters(
        OpenApiDocument document,
        string apiGroupName)
    {
        foreach (var urlPath in document.Paths)
        {
            if (!urlPath.IsPathStartingSegmentName(apiGroupName))
            {
                continue;
            }

            foreach (var apiOperation in urlPath.Value.Operations)
            {
                if (!apiOperation.Value.HasParametersOrRequestBody() &&
                    !urlPath.Value.HasParameters())
                {
                    continue;
                }

                GenerateParameter(apiGroupName, urlPath.Value, apiOperation.Value);
            }
        }
    }

    private void GenerateParameter(
        string apiGroupName,
        OpenApiPathItem apiPath,
        OpenApiOperation apiOperation)
    {
        var operationName = apiOperation.GetOperationName();
        var parameterName = $"{operationName}Parameters";

        var fullNamespace = string.IsNullOrEmpty(projectOptions.ClientFolderName)
            ? $"{projectOptions.ProjectName}.{ContentGeneratorConstants.Contracts}"
            : $"{projectOptions.ProjectName}.{projectOptions.ClientFolderName}.{ContentGeneratorConstants.Contracts}";

        // Generate
        var parameterParameters = ContentGeneratorClientParameterParametersFactory.Create(
            fullNamespace,
            apiOperation,
            apiPath.Parameters);

        var contentGeneratorParameter = new ContentGeneratorClientParameter(
            new GeneratedCodeHeaderGenerator(new GeneratedCodeGeneratorParameters(projectOptions.ApiGeneratorVersion)),
            new GeneratedCodeAttributeGenerator(new GeneratedCodeGeneratorParameters(projectOptions.ApiGeneratorVersion)),
            new CodeDocumentationTagsGenerator(),
            parameterParameters);

        var parameterContent = contentGeneratorParameter.Generate();

        // Write
        var file = new FileInfo(
            Helpers.DirectoryInfoHelper.GetCsFileNameForContract(
                apiProjectOptions.PathForContracts,
                apiGroupName,
                NameConstants.ClientRequestParameters,
                parameterName));

        var contentWriter = new ContentWriter(logger);
        contentWriter.Write(
            projectOptions.PathForSrcGenerate,
            file,
            ContentWriterArea.Src,
            parameterContent);
    }

    private void GenerateClientEndpoints(
        IList<ApiOperation> operationSchemaMappings)
    {
        ArgumentNullException.ThrowIfNull(operationSchemaMappings);

        var sgEndpointResults = new List<SyntaxGeneratorClientEndpointResult>();

        foreach (var basePathSegmentName in projectOptions.BasePathSegmentNames)
        {
            var apiGroupName = basePathSegmentName.EnsureFirstCharacterToUpper();

            var generatorEndpointResults = new SyntaxGeneratorClientEndpointResults(logger, apiProjectOptions, operationSchemaMappings, basePathSegmentName);
            var generatedEndpointResults = generatorEndpointResults.GenerateSyntaxTrees();
            sgEndpointResults.AddRange(generatedEndpointResults);

            GenerateInterfaces(projectOptions.Document, apiGroupName);
            GenerateEndpoints(projectOptions.Document, apiGroupName);
        }

        foreach (var sg in sgEndpointResults)
        {
            sg.ToFile();
        }
    }

    private static List<ApiOperationModel> GetDistinctApiOperationModels(
        List<ApiOperation> apiOperations)
    {
        var result = new List<ApiOperationModel>();

        foreach (var apiOperation in apiOperations)
        {
            var apiOperationModel = result.FirstOrDefault(x => x.Name.Equals(apiOperation.Model.Name, StringComparison.Ordinal));
            if (apiOperationModel is null)
            {
                result.Add(apiOperation.Model);
            }
        }

        return result;
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
            "System.Net",
            "System.Net.Http",
            "System.Threading",
            "System.Threading.Tasks",
            "Atc.Rest.Client",
            "Atc.Rest.Client.Builder",
            "Microsoft.AspNetCore.Mvc",
            "Microsoft.AspNetCore.Http",
            $"{projectOptions.ProjectName}.Contracts",
        };

        GlobalUsingsHelper.CreateOrUpdate(
           logger,
           ContentWriterArea.Src,
           projectOptions.PathForSrcGenerate,
           requiredUsings);
    }

    private void GenerateInterfaces(
        OpenApiDocument document,
        string apiGroupName)
    {
        var fullNamespace = string.IsNullOrEmpty(projectOptions.ClientFolderName)
                ? $"{projectOptions.ProjectName}.{ContentGeneratorConstants.Endpoints}"
                : $"{projectOptions.ProjectName}.{projectOptions.ClientFolderName}.{ContentGeneratorConstants.Endpoints}";

        foreach (var openApiPath in document.Paths)
        {
            if (!openApiPath.IsPathStartingSegmentName(apiGroupName))
            {
                continue;
            }

            foreach (var openApiOperation in openApiPath.Value.Operations)
            {
                // Generate
                var interfaceParameters = ContentGeneratorClientEndpointInterfaceParametersFactory.Create(
                    fullNamespace,
                    openApiPath.Value,
                    openApiOperation.Value);

                var contentGeneratorInterface = new ContentGeneratorClientEndpointInterface(
                    new GeneratedCodeHeaderGenerator(new GeneratedCodeGeneratorParameters(projectOptions.ApiGeneratorVersion)),
                    new GeneratedCodeAttributeGenerator(new GeneratedCodeGeneratorParameters(projectOptions.ApiGeneratorVersion)),
                    new CodeDocumentationTagsGenerator(),
                    interfaceParameters);

                var interfaceContent = contentGeneratorInterface.Generate();

                // Write
                var file = new FileInfo(
                    Helpers.DirectoryInfoHelper.GetCsFileNameForContract(
                        apiProjectOptions.PathForEndpoints,
                        apiGroupName,
                        ContentGeneratorConstants.Interfaces,
                        interfaceParameters.InterfaceName));

                var contentWriter = new ContentWriter(logger);
                contentWriter.Write(
                    projectOptions.PathForSrcGenerate,
                    file,
                    ContentWriterArea.Src,
                    interfaceContent);
            }
        }
    }

    private void GenerateEndpoints(
        OpenApiDocument document,
        string apiGroupName)
    {
        var fullNamespace = string.IsNullOrEmpty(projectOptions.ClientFolderName)
            ? $"{projectOptions.ProjectName}.{ContentGeneratorConstants.Endpoints}"
            : $"{projectOptions.ProjectName}.{projectOptions.ClientFolderName}.{ContentGeneratorConstants.Endpoints}";

        foreach (var openApiPath in document.Paths)
        {
            if (!openApiPath.IsPathStartingSegmentName(apiGroupName))
            {
                continue;
            }

            foreach (var openApiOperation in openApiPath.Value.Operations)
            {
                // Generate
                var endpointParameters = ContentGeneratorClientEndpointParametersFactory.Create(
                    fullNamespace,
                    openApiPath.Value,
                    openApiOperation.Key,
                    openApiOperation.Value,
                    $"{apiProjectOptions.ProjectPrefixName}-ApiClient",
                    $"{apiProjectOptions.RouteBase}{openApiPath.Key}");

                var contentGeneratorEndpoint = new ContentGeneratorClientEndpoint(
                    new GeneratedCodeHeaderGenerator(new GeneratedCodeGeneratorParameters(projectOptions.ApiGeneratorVersion)),
                    new GeneratedCodeAttributeGenerator(new GeneratedCodeGeneratorParameters(projectOptions.ApiGeneratorVersion)),
                    new CodeDocumentationTagsGenerator(),
                    endpointParameters);

                var endpointContent = contentGeneratorEndpoint.Generate();

                // Write
                var file = new FileInfo(
                    Helpers.DirectoryInfoHelper.GetCsFileNameForContract(
                        apiProjectOptions.PathForEndpoints,
                        apiGroupName,
                        endpointParameters.EndpointName));

                var contentWriter = new ContentWriter(logger);
                contentWriter.Write(
                    projectOptions.PathForSrcGenerate,
                    file,
                    ContentWriterArea.Src,
                    endpointContent);
            }
        }
    }
}