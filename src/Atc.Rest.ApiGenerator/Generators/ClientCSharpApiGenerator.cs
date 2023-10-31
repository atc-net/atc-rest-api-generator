// ReSharper disable ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
// ReSharper disable ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
namespace Atc.Rest.ApiGenerator.Generators;

public class ClientCSharpApiGenerator
{
    private readonly ILogger logger;
    private readonly IApiOperationExtractor apiOperationExtractor;
    private readonly INugetPackageReferenceProvider nugetPackageReferenceProvider;
    private readonly ClientCSharpApiProjectOptions projectOptions;
    private readonly ApiProjectOptions apiProjectOptions;

    private readonly string codeGeneratorContentHeader;
    private readonly AttributeParameters codeGeneratorAttribute;

    public ClientCSharpApiGenerator(
        ILogger logger,
        IApiOperationExtractor apiOperationExtractor,
        INugetPackageReferenceProvider nugetPackageReferenceProvider,
        ClientCSharpApiProjectOptions projectOptions)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.apiOperationExtractor = apiOperationExtractor ?? throw new ArgumentNullException(nameof(apiOperationExtractor));
        this.nugetPackageReferenceProvider = nugetPackageReferenceProvider ?? throw new ArgumentNullException(nameof(nugetPackageReferenceProvider));
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
            projectOptions.RemoveNamespaceGroupSeparatorInGlobalUsings,
            projectOptions.ForClient,
            projectOptions.ClientFolderName);

        // TODO: Optimize codeGeneratorContentHeader & codeGeneratorAttribute
        var codeHeaderGenerator = new GeneratedCodeHeaderGenerator(
            new GeneratedCodeGeneratorParameters(
                projectOptions.ApiGeneratorVersion));
        codeGeneratorContentHeader = codeHeaderGenerator.Generate();

        codeGeneratorAttribute = new AttributeParameters(
            "GeneratedCode",
            $"\"{ContentWriterConstants.ApiGeneratorName}\", \"{projectOptions.ApiGeneratorVersion}\"");
    }

    public bool Generate()
    {
        logger.LogInformation($"{ContentWriterConstants.AreaGenerateCode} Working on client api generation ({projectOptions.ProjectName})");

        ScaffoldSrc();

        var operationSchemaMappings = apiOperationExtractor.Extract(projectOptions.Document);

        GenerateModels(projectOptions.Document, operationSchemaMappings);

        GenerateParameters(projectOptions.Document);

        if (!projectOptions.ExcludeEndpointGeneration)
        {
            GenerateEndpointInterfaces(projectOptions.Document);

            GenerateEndpoints(projectOptions.Document);

            GenerateEndpointResultInterfaces(projectOptions.Document);

            GenerateEndpointResults(projectOptions.Document);
        }

        GenerateSrcGlobalUsings(
            projectOptions.RemoveNamespaceGroupSeparatorInGlobalUsings,
            operationSchemaMappings);

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
            IList<(string PackageId, string PackageVersion, string? SubElements)>? packageReferencesBaseLineForApiClientProject = null;
            TaskHelper.RunSync(async () =>
            {
                packageReferencesBaseLineForApiClientProject = await nugetPackageReferenceProvider.GetPackageReferencesBaseLineForApiClientProject();
            });

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
                packageReferences: packageReferencesBaseLineForApiClientProject,
                projectReferences: null,
                includeApiSpecification: false,
                usingCodingRules: projectOptions.UsingCodingRules);
        }
    }

    private void GenerateModels(
        OpenApiDocument document,
        IList<ApiOperation> operationSchemaMappings)
    {
        ArgumentNullException.ThrowIfNull(operationSchemaMappings);

        foreach (var apiGroupName in projectOptions.ApiGroupNames)
        {
            var apiOperations = operationSchemaMappings
                .Where(x => x.LocatedArea is ApiSchemaMapLocatedAreaType.Response or ApiSchemaMapLocatedAreaType.RequestBody &&
                            x.ApiGroupName.Equals(apiGroupName, StringComparison.OrdinalIgnoreCase))
                .ToList();

            var apiOperationModels = GetDistinctApiOperationModels(apiOperations);

            foreach (var apiOperationModel in apiOperationModels)
            {
                var apiSchema = document.Components.Schemas.First(x =>
                    x.Key.Equals(apiOperationModel.Name, StringComparison.OrdinalIgnoreCase));

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
    }

    private void GenerateEnumerationType(
        string enumerationName,
        OpenApiSchema openApiSchemaEnumeration)
    {
        var fullNamespace = $"{projectOptions.ProjectName}.{ContentGeneratorConstants.Contracts}";

        // Generate
        var enumParameters = ContentGeneratorServerClientEnumParametersFactory.Create(
            codeGeneratorContentHeader,
            fullNamespace,
            codeGeneratorAttribute,
            enumerationName,
            openApiSchemaEnumeration.Enum);

        var contentGeneratorEnum = new GenerateContentForEnum(
            new CodeDocumentationTagsGenerator(),
            enumParameters);

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
        var classParameters = ContentGeneratorServerClientModelParametersFactory.Create(
            codeGeneratorContentHeader,
            fullNamespace,
            codeGeneratorAttribute,
            modelName,
            apiSchemaModel);

        var contentGeneratorClass = new GenerateContentForClass(
            new CodeDocumentationTagsGenerator(),
            classParameters);

        var classContent = contentGeneratorClass.Generate();

        // Write
        FileInfo file;
        if (modelName.EndsWith(ContentGeneratorConstants.Request, StringComparison.Ordinal))
        {
            file = new FileInfo(
                Helpers.DirectoryInfoHelper.GetCsFileNameForContract(
                    apiProjectOptions.PathForContracts,
                    apiGroupName,
                    ContentGeneratorConstants.RequestParameters,
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
                        ContentGeneratorConstants.Models,
                        modelName));
            }
        }

        var contentWriter = new ContentWriter(logger);
        contentWriter.Write(
            projectOptions.PathForSrcGenerate,
            file,
            ContentWriterArea.Src,
            classContent);
    }

    private void GenerateParameters(
        OpenApiDocument document)
    {
        foreach (var openApiPath in document.Paths)
        {
            var apiGroupName = openApiPath.GetApiGroupName();

            foreach (var apiOperation in openApiPath.Value.Operations)
            {
                if (!apiOperation.Value.HasParametersOrRequestBody() &&
                    !openApiPath.Value.HasParameters())
                {
                    continue;
                }

                GenerateParameter(apiGroupName, openApiPath.Value, apiOperation.Value);
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
                ContentGeneratorConstants.RequestParameters,
                parameterName));

        var contentWriter = new ContentWriter(logger);
        contentWriter.Write(
            projectOptions.PathForSrcGenerate,
            file,
            ContentWriterArea.Src,
            parameterContent);
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

    private void GenerateSrcGlobalUsings(
        bool removeNamespaceGroupSeparatorInGlobalUsings,
        IList<ApiOperation> operationSchemaMappings)
    {
        var requiredUsings = new List<string>
        {
            "System.CodeDom.Compiler",
            "System.ComponentModel.DataAnnotations",
            "System.Net",
            "Atc.Rest.Client",
            "Atc.Rest.Client.Builder",
            "Microsoft.AspNetCore.Mvc",
            $"{projectOptions.ProjectName}.Contracts",
        };

        if (operationSchemaMappings.FirstOrDefault(x => x.Model.UsesIFormFile) is not null)
        {
            requiredUsings.Add("Microsoft.AspNetCore.Http");
        }

        GlobalUsingsHelper.CreateOrUpdate(
           logger,
           ContentWriterArea.Src,
           projectOptions.PathForSrcGenerate,
           requiredUsings,
           removeNamespaceGroupSeparatorInGlobalUsings);
    }

    private void GenerateEndpointInterfaces(
        OpenApiDocument document)
    {
        var fullNamespace = string.IsNullOrEmpty(projectOptions.ClientFolderName)
                ? $"{projectOptions.ProjectName}.{ContentGeneratorConstants.Endpoints}"
                : $"{projectOptions.ProjectName}.{projectOptions.ClientFolderName}.{ContentGeneratorConstants.Endpoints}";

        foreach (var openApiPath in document.Paths)
        {
            var apiGroupName = openApiPath.GetApiGroupName();

            foreach (var openApiOperation in openApiPath.Value.Operations)
            {
                // Generate
                var interfaceParameters = ContentGeneratorClientEndpointInterfaceParametersFactory.Create(
                    codeGeneratorContentHeader,
                    fullNamespace,
                    codeGeneratorAttribute,
                    openApiPath.Value,
                    openApiOperation.Value,
                    projectOptions.HttpClientName);

                var contentGeneratorInterface = new GenerateContentForInterface(
                    new CodeDocumentationTagsGenerator(),
                    interfaceParameters);

                var interfaceContent = contentGeneratorInterface.Generate();

                // Write
                var file = new FileInfo(
                    Helpers.DirectoryInfoHelper.GetCsFileNameForContract(
                        apiProjectOptions.PathForEndpoints,
                        apiGroupName,
                        ContentGeneratorConstants.Interfaces,
                        interfaceParameters.TypeName));

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
        OpenApiDocument document)
    {
        var fullNamespace = string.IsNullOrEmpty(projectOptions.ClientFolderName)
            ? $"{projectOptions.ProjectName}.{ContentGeneratorConstants.Endpoints}"
            : $"{projectOptions.ProjectName}.{projectOptions.ClientFolderName}.{ContentGeneratorConstants.Endpoints}";

        foreach (var openApiPath in document.Paths)
        {
            var apiGroupName = openApiPath.GetApiGroupName();

            foreach (var openApiOperation in openApiPath.Value.Operations)
            {
                // Generate
                var endpointParameters = ContentGeneratorClientEndpointParametersFactory.Create(
                    apiProjectOptions.ApiOptions.Generator.Response.UseProblemDetailsAsDefaultBody,
                    projectOptions.ProjectName,
                    apiGroupName,
                    fullNamespace,
                    openApiPath.Value,
                    openApiOperation.Key,
                    openApiOperation.Value,
                    projectOptions.HttpClientName,
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

    private void GenerateEndpointResultInterfaces(
        OpenApiDocument document)
    {
        var fullNamespace = string.IsNullOrEmpty(projectOptions.ClientFolderName)
            ? $"{projectOptions.ProjectName}.{ContentGeneratorConstants.Endpoints}"
            : $"{projectOptions.ProjectName}.{projectOptions.ClientFolderName}.{ContentGeneratorConstants.Endpoints}";

        foreach (var openApiPath in document.Paths)
        {
            var apiGroupName = openApiPath.GetApiGroupName();

            foreach (var openApiOperation in openApiPath.Value.Operations)
            {
                // Generate
                var endpointResultInterfaceParameters = ContentGeneratorClientEndpointResultInterfaceParametersFactory.Create(
                    apiProjectOptions.ApiOptions.Generator.Response.UseProblemDetailsAsDefaultBody,
                    apiGroupName,
                    projectOptions.ProjectName,
                    fullNamespace,
                    openApiPath.Value,
                    openApiOperation.Value);

                var contentGeneratorEndpointResult = new ContentGeneratorClientEndpointResultInterface(
                    new GeneratedCodeHeaderGenerator(new GeneratedCodeGeneratorParameters(projectOptions.ApiGeneratorVersion)),
                    new GeneratedCodeAttributeGenerator(new GeneratedCodeGeneratorParameters(projectOptions.ApiGeneratorVersion)),
                    new CodeDocumentationTagsGenerator(),
                    endpointResultInterfaceParameters);

                var endpointContent = contentGeneratorEndpointResult.Generate();

                // Write
                var file = new FileInfo(
                    Helpers.DirectoryInfoHelper.GetCsFileNameForContract(
                        apiProjectOptions.PathForEndpoints,
                        apiGroupName,
                        ContentGeneratorConstants.Interfaces,
                        endpointResultInterfaceParameters.InterfaceName));

                var contentWriter = new ContentWriter(logger);
                contentWriter.Write(
                    projectOptions.PathForSrcGenerate,
                    file,
                    ContentWriterArea.Src,
                    endpointContent);
            }
        }
    }

    private void GenerateEndpointResults(
        OpenApiDocument document)
    {
        var fullNamespace = string.IsNullOrEmpty(projectOptions.ClientFolderName)
            ? $"{projectOptions.ProjectName}.{ContentGeneratorConstants.Endpoints}"
            : $"{projectOptions.ProjectName}.{projectOptions.ClientFolderName}.{ContentGeneratorConstants.Endpoints}";

        foreach (var openApiPath in document.Paths)
        {
            var apiGroupName = openApiPath.GetApiGroupName();

            foreach (var openApiOperation in openApiPath.Value.Operations)
            {
                // Generate
                var endpointResultParameters = ContentGeneratorClientEndpointResultParametersFactory.Create(
                    apiProjectOptions.ApiOptions.Generator.Response.UseProblemDetailsAsDefaultBody,
                    projectOptions.ProjectName,
                    apiGroupName,
                    fullNamespace,
                    projectOptions.ApiOptions.Generator.Response.UseProblemDetailsAsDefaultBody,
                    openApiPath.Value,
                    openApiOperation.Value);

                var contentGeneratorEndpointResult = new ContentGeneratorClientEndpointResult(
                    new GeneratedCodeHeaderGenerator(new GeneratedCodeGeneratorParameters(projectOptions.ApiGeneratorVersion)),
                    new GeneratedCodeAttributeGenerator(new GeneratedCodeGeneratorParameters(projectOptions.ApiGeneratorVersion)),
                    new CodeDocumentationTagsGenerator(),
                    endpointResultParameters);

                var endpointContent = contentGeneratorEndpointResult.Generate();

                // Write
                var file = new FileInfo(
                    Helpers.DirectoryInfoHelper.GetCsFileNameForContract(
                        apiProjectOptions.PathForEndpoints,
                        apiGroupName,
                        endpointResultParameters.EndpointResultName));

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