// ReSharper disable ArrangeObjectCreationWhenTypeNotEvident
// ReSharper disable ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
// ReSharper disable ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
namespace Atc.Rest.ApiGenerator.Client.CSharp.ProjectGenerator;

public class ClientCSharpApiGenerator : IClientCSharpApiGenerator
{
    private readonly ILogger<ClientCSharpApiGenerator> logger;
    private readonly INugetPackageReferenceProvider nugetPackageReferenceProvider;
    private readonly OpenApiDocument openApiDocument;
    private readonly IList<ApiOperation> operationSchemaMappings;
    private readonly string codeGeneratorContentHeader;
    private readonly AttributeParameters codeGeneratorAttribute;
    private readonly GeneratorSettings settings;
    private readonly CustomErrorResponseModel? customErrorResponseModel;

    public ClientCSharpApiGenerator(
        ILoggerFactory loggerFactory,
        INugetPackageReferenceProvider nugetPackageReferenceProvider,
        OpenApiDocument openApiDocument,
        IList<ApiOperation> operationSchemaMappings,
        GeneratorSettings generatorSettings,
        CustomErrorResponseModel? customErrorResponseModel)
    {
        ArgumentNullException.ThrowIfNull(loggerFactory);
        ArgumentNullException.ThrowIfNull(nugetPackageReferenceProvider);
        ArgumentNullException.ThrowIfNull(openApiDocument);
        ArgumentNullException.ThrowIfNull(operationSchemaMappings);
        ArgumentNullException.ThrowIfNull(generatorSettings);

        logger = loggerFactory.CreateLogger<ClientCSharpApiGenerator>();
        this.nugetPackageReferenceProvider = nugetPackageReferenceProvider;
        this.customErrorResponseModel = customErrorResponseModel;
        this.openApiDocument = openApiDocument;
        this.operationSchemaMappings = operationSchemaMappings;
        settings = generatorSettings;

        codeGeneratorContentHeader = GeneratedCodeHeaderGeneratorFactory
            .Create(settings.Version)
            .Generate();
        codeGeneratorAttribute = AttributeParametersFactory
            .CreateGeneratedCode(settings.Version);
    }

    public string HttpClientName { get; set; } = ContentGeneratorConstants.DefaultHttpClient;

    public async Task ScaffoldProjectFile()
    {
        var packageReferences = await nugetPackageReferenceProvider.GetPackageReferencesForApiClientProject();

        var itemGroupPackageReferences = packageReferences
            .Select(packageReference => new ItemGroupParameter(
                "PackageReference",
                [
                    new("Include", packageReference.PackageId),
                    new("Version", packageReference.PackageVersion),
                ],
                Value: null))
            .ToList();

        var projectFileParameters = new ProjectFileParameters(
            "Microsoft.NET.Sdk",
            [
                [
                    new("TargetFramework", Attributes: null, "net8.0"),
                    new("Nullable", Attributes: null, "enable"),
                    new("IsPackable", Attributes: null, "false"),
                ],
                [
                    new("GenerateDocumentationFile", Attributes: null, "true"),
                ],
                [
                    new("DocumentationFile", Attributes: null, @$"bin\Debug\net8.0\{settings.ProjectName}.xml"),
                    new("NoWarn", Attributes: null, "$(NoWarn);1573;1591;1701;1702;1712;8618;"),
                ],
            ],
            [
                itemGroupPackageReferences,
            ]);

        var contentGenerator = new GenerateContentForProjectFile(
            projectFileParameters);

        var content = contentGenerator.Generate();

        var contentWriter = new ContentWriter(logger);
        contentWriter.Write(
            settings.ProjectPath,
            FileInfoFactory.Create(settings.ProjectPath, $"{settings.ProjectName}.csproj"),
            ContentWriterArea.Src,
            content,
            overrideIfExist: false);
    }

    public void GenerateModels()
    {
        GenerateCustomErrorResponseModel();

        foreach (var apiGroupName in openApiDocument.GetApiGroupNames())
        {
            var apiOperations = operationSchemaMappings
                .Where(x => x.ApiGroupName.Equals(apiGroupName, StringComparison.OrdinalIgnoreCase))
                .ToList();

            var apiOperationModels = GetDistinctApiOperationModels(apiOperations);

            foreach (var apiOperationModel in apiOperationModels)
            {
                var apiSchema = openApiDocument.Components.Schemas.First(x => x.GetFormattedKey().Equals(apiOperationModel.Name, StringComparison.OrdinalIgnoreCase));

                var modelName = apiSchema.GetFormattedKey();

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

    public void GenerateParameters()
    {
        foreach (var openApiPath in openApiDocument.Paths)
        {
            var apiGroupName = openApiPath.GetApiGroupName();

            var fullNamespace = NamespaceFactory.CreateFull(settings.ProjectName, settings.ContractsLocation, apiGroupName);

            foreach (var openApiOperation in openApiPath.Value.Operations)
            {
                if (openApiOperation.Value.Deprecated && !settings.IncludeDeprecatedOperations)
                {
                    continue;
                }

                if (!openApiOperation.Value.HasParametersOrRequestBody() &&
                    !openApiPath.Value.HasParameters())
                {
                    continue;
                }

                var parameterParameters = ContentGeneratorClientParameterParametersFactory.Create(
                    fullNamespace,
                    openApiOperation.Value,
                    openApiPath.Value.Parameters);

                var contentGenerator = new ContentGeneratorClientParameter(
                    new GeneratedCodeHeaderGenerator(new GeneratedCodeGeneratorParameters(settings.Version)),
                    new GeneratedCodeAttributeGenerator(new GeneratedCodeGeneratorParameters(settings.Version)),
                    new CodeDocumentationTagsGenerator(),
                    parameterParameters);

                var content = contentGenerator.Generate();

                var contentWriter = new ContentWriter(logger);
                contentWriter.Write(
                    settings.ProjectPath,
                    FileInfoFactory.Create(settings.ProjectPath, settings.ContractsLocation, apiGroupName, ContentGeneratorConstants.RequestParameters, $"{parameterParameters.ParameterName}.cs"),
                    ContentWriterArea.Src,
                    content);
            }
        }
    }

    public void GenerateEndpointInterfaces()
    {
        foreach (var openApiPath in openApiDocument.Paths)
        {
            var apiGroupName = openApiPath.GetApiGroupName();

            var fullNamespace = NamespaceFactory.CreateFull(settings.ProjectName, settings.EndpointsLocation, apiGroupName, ContentGeneratorConstants.Interfaces);

            foreach (var openApiOperation in openApiPath.Value.Operations)
            {
                if (openApiOperation.Value.Deprecated && !settings.IncludeDeprecatedOperations)
                {
                    continue;
                }

                var interfaceParameters = ContentGeneratorClientEndpointInterfaceParametersFactory.Create(
                    codeGeneratorContentHeader,
                    fullNamespace,
                    codeGeneratorAttribute,
                    openApiPath.Value,
                    openApiOperation.Value,
                    HttpClientName,
                    settings.UsePartialClassForEndpoints);

                var contentGenerator = new GenerateContentForInterface(
                    new CodeDocumentationTagsGenerator(),
                    interfaceParameters);

                var content = contentGenerator.Generate();

                var contentWriter = new ContentWriter(logger);
                contentWriter.Write(
                    settings.ProjectPath,
                    FileInfoFactory.Create(settings.ProjectPath, settings.EndpointsLocation, apiGroupName, ContentGeneratorConstants.Interfaces, $"{interfaceParameters.TypeName}.cs"),
                    ContentWriterArea.Src,
                    content);
            }
        }
    }

    public void GenerateEndpoints()
    {
        foreach (var openApiPath in openApiDocument.Paths)
        {
            var apiGroupName = openApiPath.GetApiGroupName();

            var fullNamespace = NamespaceFactory.CreateFull(settings.ProjectName, settings.EndpointsLocation, apiGroupName);

            foreach (var openApiOperation in openApiPath.Value.Operations)
            {
                if (openApiOperation.Value.Deprecated && !settings.IncludeDeprecatedOperations)
                {
                    continue;
                }

                var endpointParameters = ContentGeneratorClientEndpointParametersFactory.Create(
                    settings.ProjectName,
                    apiGroupName,
                    fullNamespace,
                    settings.ContractsLocation,
                    openApiPath.Value,
                    openApiOperation.Key,
                    openApiOperation.Value,
                    HttpClientName,
                    $"{openApiDocument.GetServerUrlBasePath()}{openApiPath.Key}",
                    settings.UsePartialClassForEndpoints);

                var contentGenerator = new ContentGeneratorClientEndpoint(
                    new GeneratedCodeHeaderGenerator(new GeneratedCodeGeneratorParameters(settings.Version)),
                    new GeneratedCodeAttributeGenerator(new GeneratedCodeGeneratorParameters(settings.Version)),
                    new CodeDocumentationTagsGenerator(),
                    endpointParameters,
                    settings.UseProblemDetailsAsDefaultResponseBody,
                    customErrorResponseModel?.Name);

                var content = contentGenerator.Generate();

                var contentWriter = new ContentWriter(logger);
                contentWriter.Write(
                    settings.ProjectPath,
                    FileInfoFactory.Create(settings.ProjectPath, settings.EndpointsLocation, apiGroupName, $"{endpointParameters.EndpointName}.cs"),
                    ContentWriterArea.Src,
                    content);
            }
        }
    }

    public void GenerateEndpointResultInterfaces()
    {
        foreach (var openApiPath in openApiDocument.Paths)
        {
            var apiGroupName = openApiPath.GetApiGroupName();

            var fullNamespace = NamespaceFactory.CreateFull(settings.ProjectName, settings.EndpointsLocation, apiGroupName, ContentGeneratorConstants.Interfaces);

            foreach (var openApiOperation in openApiPath.Value.Operations)
            {
                if (openApiOperation.Value.Deprecated && !settings.IncludeDeprecatedOperations)
                {
                    continue;
                }

                var endpointResultInterfaceParameters = ContentGeneratorClientEndpointResultInterfaceParametersFactory.Create(
                    settings.ProjectName,
                    apiGroupName,
                    fullNamespace,
                    settings.ContractsLocation,
                    openApiPath.Value,
                    openApiOperation.Value,
                    settings.UsePartialClassForContracts);

                var contentGenerator = new ContentGeneratorClientEndpointResultInterface(
                    new GeneratedCodeHeaderGenerator(new GeneratedCodeGeneratorParameters(settings.Version)),
                    new GeneratedCodeAttributeGenerator(new GeneratedCodeGeneratorParameters(settings.Version)),
                    new CodeDocumentationTagsGenerator(),
                    endpointResultInterfaceParameters,
                    settings.UseProblemDetailsAsDefaultResponseBody);

                var content = contentGenerator.Generate();

                var contentWriter = new ContentWriter(logger);
                contentWriter.Write(
                    settings.ProjectPath,
                    FileInfoFactory.Create(settings.ProjectPath, settings.EndpointsLocation, apiGroupName, ContentGeneratorConstants.Interfaces, $"{endpointResultInterfaceParameters.InterfaceName}.cs"),
                    ContentWriterArea.Src,
                    content);
            }
        }
    }

    public void GenerateEndpointResults()
    {
        foreach (var openApiPath in openApiDocument.Paths)
        {
            var apiGroupName = openApiPath.GetApiGroupName();

            var fullNamespace = NamespaceFactory.CreateFull(settings.ProjectName, settings.EndpointsLocation, apiGroupName);

            foreach (var openApiOperation in openApiPath.Value.Operations)
            {
                if (openApiOperation.Value.Deprecated && !settings.IncludeDeprecatedOperations)
                {
                    continue;
                }

                var endpointResultParameters = ContentGeneratorClientEndpointResultParametersFactory.Create(
                    settings.ProjectName,
                    apiGroupName,
                    fullNamespace,
                    settings.ContractsLocation,
                    openApiPath.Value,
                    openApiOperation.Value,
                    settings.UsePartialClassForContracts);

                var contentGenerator = new ContentGeneratorClientEndpointResult(
                    new GeneratedCodeHeaderGenerator(new GeneratedCodeGeneratorParameters(settings.Version)),
                    new GeneratedCodeAttributeGenerator(new GeneratedCodeGeneratorParameters(settings.Version)),
                    new CodeDocumentationTagsGenerator(),
                    endpointResultParameters,
                    settings.UseProblemDetailsAsDefaultResponseBody);

                var content = contentGenerator.Generate();

                var contentWriter = new ContentWriter(logger);
                contentWriter.Write(
                    settings.ProjectPath,
                    FileInfoFactory.Create(settings.ProjectPath, settings.EndpointsLocation, apiGroupName, $"{endpointResultParameters.EndpointResultName}.cs"),
                    ContentWriterArea.Src,
                    content);
            }
        }
    }

    public void MaintainGlobalUsings(
        bool removeNamespaceGroupSeparatorInGlobalUsings)
    {
        var requiredUsings = new List<string>
        {
            "System.CodeDom.Compiler",
            "System.ComponentModel.DataAnnotations",
            "System",
            "System.Net",
            "System.Net.Http",
            "System.Threading",
            "System.Threading.Tasks",
            "Atc.Rest.Client",
            "Atc.Rest.Client.Builder",
            "Microsoft.AspNetCore.Mvc",
        };

        if (openApiDocument.IsUsingRequiredForSystemLinq(settings.IncludeDeprecatedOperations))
        {
            requiredUsings.Add("System.Linq");
        }

        if (openApiDocument.IsUsingRequiredForSystemCollectionGeneric(settings.IncludeDeprecatedOperations))
        {
            requiredUsings.Add("System.Collections.Generic");
        }

        if (openApiDocument.IsUsingRequiredForSystemTextJsonSerializationAndSystemRuntimeSerialization(settings.IncludeDeprecatedOperations))
        {
            requiredUsings.Add("System.Runtime.Serialization");
            requiredUsings.Add("System.Text.Json.Serialization");
        }

        if (operationSchemaMappings.FirstOrDefault(x => x.Model.UsesIFormFile) is not null)
        {
            requiredUsings.Add("Microsoft.AspNetCore.Http");
        }

        if (openApiDocument.IsUsingRequiredForAtcRestResults())
        {
            requiredUsings.Add("Atc.Rest.Results");
        }

        if (operationSchemaMappings.Any(apiOperation => apiOperation.Model.IsShared))
        {
            requiredUsings.Add(NamespaceFactory.CreateFull(settings.ProjectName, settings.ContractsLocation));
        }

        var apiGroupNames = openApiDocument.GetApiGroupNames();
        foreach (var openApiPath in openApiDocument.Paths)
        {
            var apiGroupName = openApiPath.GetApiGroupName();
            foreach (var openApiOperation in openApiPath.Value.Operations)
            {
                if (openApiOperation.Value.Deprecated && !settings.IncludeDeprecatedOperations)
                {
                    continue;
                }

                if (!openApiPath.Value.HasParameters() &&
                    !openApiOperation.Value.HasParametersOrRequestBody())
                {
                    continue;
                }

                var requiredUsing = NamespaceFactory.CreateFull(settings.ProjectName, settings.ContractsLocation, apiGroupName);
                if (!requiredUsings.Contains(requiredUsing, StringComparer.CurrentCulture))
                {
                    requiredUsings.Add(requiredUsing);
                }
            }
        }

        foreach (var apiGroupName in apiGroupNames)
        {
            var apiOperations = operationSchemaMappings
                .Where(x => x.ApiGroupName.Equals(apiGroupName, StringComparison.OrdinalIgnoreCase))
                .ToList();

            var apiOperationModels = GetDistinctApiOperationModels(apiOperations);

            foreach (var apiOperationModel in apiOperationModels)
            {
                if (apiOperationModel.IsEnum ||
                    apiOperationModel.IsShared)
                {
                    continue;
                }

                if (apiGroupName.IsWellKnownSystemTypeName())
                {
                    continue;
                }

                var requiredUsing = NamespaceFactory.CreateFull(settings.ProjectName, settings.ContractsLocation, apiGroupName);
                if (!requiredUsings.Contains(requiredUsing, StringComparer.CurrentCulture))
                {
                    requiredUsings.Add(requiredUsing);
                }
            }
        }

        requiredUsings.AddRange(apiGroupNames.Select(x => NamespaceFactory.CreateFull(settings.ProjectName, settings.EndpointsLocation, x, ContentGeneratorConstants.Interfaces)));

        GlobalUsingsHelper.CreateOrUpdate(
            logger,
            ContentWriterArea.Src,
            settings.ProjectPath,
            requiredUsings,
            removeNamespaceGroupSeparatorInGlobalUsings);
    }

    private void GenerateEnumerationType(
        string enumerationName,
        OpenApiSchema openApiSchemaEnumeration)
    {
        var fullNamespace = NamespaceFactory.CreateFull(settings.ProjectName, settings.ContractsLocation);

        var enumParameters = ContentGeneratorServerClientEnumParametersFactory.Create(
            codeGeneratorContentHeader,
            fullNamespace,
            codeGeneratorAttribute,
            enumerationName,
            openApiSchemaEnumeration.Enum);

        var contentGenerator = new GenerateContentForEnum(
            new CodeDocumentationTagsGenerator(),
            enumParameters);

        var content = contentGenerator.Generate();

        var contentWriter = new ContentWriter(logger);
        contentWriter.Write(
            settings.ProjectPath,
            FileInfoFactory.Create(settings.ProjectPath, settings.ContractsLocation, ContentGeneratorConstants.SpecialFolderEnumerationTypes, $"{enumerationName}.cs"),
            ContentWriterArea.Src,
            content);
    }

    private void GenerateModel(
        string modelName,
        OpenApiSchema apiSchemaModel,
        string apiGroupName,
        bool isSharedContract)
    {
        var fullNamespace = isSharedContract
            ? NamespaceFactory.CreateFull(settings.ProjectName, settings.ContractsLocation)
            : NamespaceFactory.CreateFull(settings.ProjectName, settings.ContractsLocation, apiGroupName);

        var parameters = ContentGeneratorServerClientModelParametersFactory.CreateForClass(
            codeGeneratorContentHeader,
            fullNamespace,
            codeGeneratorAttribute,
            modelName,
            apiSchemaModel,
            settings.UsePartialClassForContracts,
            settings.IncludeDeprecatedOperations);

        var contentGeneratorClass = new GenerateContentForClass(
            new CodeDocumentationTagsGenerator(),
            parameters);

        var content = contentGeneratorClass.Generate();

        var contentWriter = new ContentWriter(logger);
        contentWriter.Write(
            settings.ProjectPath,
            isSharedContract
                ? FileInfoFactory.Create(settings.ProjectPath, settings.ContractsLocation, ContentGeneratorConstants.SpecialFolderSharedModels, $"{modelName}.cs")
                : FileInfoFactory.Create(settings.ProjectPath, settings.ContractsLocation, apiGroupName, $"{modelName}.cs"),
            ContentWriterArea.Src,
            content);
    }

    private void GenerateCustomErrorResponseModel()
    {
        if (customErrorResponseModel is null ||
            string.IsNullOrEmpty(customErrorResponseModel.Name))
        {
            return;
        }

        var fullNamespace = NamespaceFactory.CreateFull(settings.ProjectName, settings.ContractsLocation);

        var parameters = ContentGeneratorServerClientModelParametersFactory.CreateForCustomErrorResponseModel(
            codeGeneratorContentHeader,
            fullNamespace,
            codeGeneratorAttribute,
            customErrorResponseModel,
            settings.UsePartialClassForContracts);

        var contentGeneratorClass = new GenerateContentForClass(
            new CodeDocumentationTagsGenerator(),
            parameters);

        var content = contentGeneratorClass.Generate();

        var contentWriter = new ContentWriter(logger);
        contentWriter.Write(
            settings.ProjectPath,
            FileInfoFactory.Create(settings.ProjectPath, settings.ContractsLocation, ContentGeneratorConstants.SpecialFolderSharedModels, $"{customErrorResponseModel.Name.EnsureFirstCharacterToUpper()}.cs"),
            ContentWriterArea.Src,
            content);
    }

    private static List<ApiOperationModel> GetDistinctApiOperationModels(
        List<ApiOperation> apiOperations)
    {
        var result = new List<ApiOperationModel>();

        foreach (var apiOperation in apiOperations)
        {
            var apiOperationModel = result.Find(x => x.Name.Equals(apiOperation.Model.Name, StringComparison.Ordinal));
            if (apiOperationModel is null)
            {
                result.Add(apiOperation.Model);
            }
        }

        return result;
    }
}