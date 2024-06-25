// ReSharper disable ArrangeObjectCreationWhenTypeNotEvident
// ReSharper disable ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
// ReSharper disable ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
namespace Atc.Rest.ApiGenerator.Client.CSharp.ProjectGenerator;

public class ClientCSharpApiGenerator : IClientCSharpApiGenerator
{
    private readonly ILogger<ClientCSharpApiGenerator> logger;
    private readonly INugetPackageReferenceProvider nugetPackageReferenceProvider;
    private readonly Version apiGeneratorVersion;
    private readonly string projectName;
    private readonly DirectoryInfo projectPath;
    private readonly OpenApiDocument openApiDocument;
    private readonly IList<ApiOperation> operationSchemaMappings;
    private readonly string codeGeneratorContentHeader;
    private readonly AttributeParameters codeGeneratorAttribute;
    private readonly bool useProblemDetailsAsDefaultResponseBody;

    public ClientCSharpApiGenerator(
        ILoggerFactory loggerFactory,
        INugetPackageReferenceProvider nugetPackageReferenceProvider,
        Version apiGeneratorVersion,
        string projectName,
        DirectoryInfo projectPath,
        OpenApiDocument openApiDocument,
        IList<ApiOperation> operationSchemaMappings,
        bool useProblemDetailsAsDefaultResponseBody)
    {
        ArgumentNullException.ThrowIfNull(loggerFactory);
        ArgumentNullException.ThrowIfNull(nugetPackageReferenceProvider);
        ArgumentNullException.ThrowIfNull(apiGeneratorVersion);
        ArgumentNullException.ThrowIfNull(projectName);
        ArgumentNullException.ThrowIfNull(projectPath);
        ArgumentNullException.ThrowIfNull(openApiDocument);
        ArgumentNullException.ThrowIfNull(operationSchemaMappings);

        logger = loggerFactory.CreateLogger<ClientCSharpApiGenerator>();
        this.nugetPackageReferenceProvider = nugetPackageReferenceProvider;
        this.apiGeneratorVersion = apiGeneratorVersion;
        this.projectName = projectName;
        this.projectPath = projectPath;
        this.openApiDocument = openApiDocument;
        this.operationSchemaMappings = operationSchemaMappings;
        this.useProblemDetailsAsDefaultResponseBody = useProblemDetailsAsDefaultResponseBody;

        codeGeneratorContentHeader = GeneratedCodeHeaderGeneratorFactory
            .Create(apiGeneratorVersion)
            .Generate();
        codeGeneratorAttribute = AttributeParametersFactory
            .CreateGeneratedCode(apiGeneratorVersion);
    }

    public string? ClientFolderName { get; set; }

    public string HttpClientName { get; set; } = "DefaultHttpClient";

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
                    new("DocumentationFile", Attributes: null, @$"bin\Debug\net8.0\{projectName}.xml"),
                    new("NoWarn", Attributes: null, "1573;1591;1701;1702;1712;8618;"),
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
            projectPath,
            projectPath.CombineFileInfo($"{projectName}.csproj"),
            ContentWriterArea.Src,
            content,
            overrideIfExist: false);
    }

    public void GenerateModels()
    {
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

            var fullNamespace = string.IsNullOrEmpty(ClientFolderName)
                ? $"{projectName}.{ContentGeneratorConstants.Contracts}.{apiGroupName}"
                : $"{projectName}.{ClientFolderName}.{ContentGeneratorConstants.Contracts}.{apiGroupName}";

            foreach (var openApiOperation in openApiPath.Value.Operations)
            {
                if (openApiOperation.Value.Deprecated)
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
                    new GeneratedCodeHeaderGenerator(new GeneratedCodeGeneratorParameters(apiGeneratorVersion)),
                    new GeneratedCodeAttributeGenerator(new GeneratedCodeGeneratorParameters(apiGeneratorVersion)),
                    new CodeDocumentationTagsGenerator(),
                    parameterParameters);

                var content = contentGenerator.Generate();

                var contentWriter = new ContentWriter(logger);
                contentWriter.Write(
                    projectPath,
                    projectPath.CombineFileInfo(ContentGeneratorConstants.Contracts, apiGroupName, ContentGeneratorConstants.RequestParameters, $"{parameterParameters.ParameterName}.cs"),
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

            var fullNamespace = string.IsNullOrEmpty(ClientFolderName)
                ? $"{projectName}.{ContentGeneratorConstants.Endpoints}.{apiGroupName}.{ContentGeneratorConstants.Interfaces}"
                : $"{projectName}.{ClientFolderName}.{ContentGeneratorConstants.Endpoints}.{apiGroupName}.{ContentGeneratorConstants.Interfaces}";

            foreach (var openApiOperation in openApiPath.Value.Operations)
            {
                if (openApiOperation.Value.Deprecated)
                {
                    continue;
                }

                var interfaceParameters = ContentGeneratorClientEndpointInterfaceParametersFactory.Create(
                    codeGeneratorContentHeader,
                    fullNamespace,
                    codeGeneratorAttribute,
                    openApiPath.Value,
                    openApiOperation.Value,
                    HttpClientName);

                var contentGenerator = new GenerateContentForInterface(
                    new CodeDocumentationTagsGenerator(),
                    interfaceParameters);

                var content = contentGenerator.Generate();

                var contentWriter = new ContentWriter(logger);
                contentWriter.Write(
                    projectPath,
                    projectPath.CombineFileInfo(ContentGeneratorConstants.Endpoints, apiGroupName, ContentGeneratorConstants.Interfaces, $"{interfaceParameters.TypeName}.cs"),
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

            var fullNamespace = string.IsNullOrEmpty(ClientFolderName)
                ? $"{projectName}.{ContentGeneratorConstants.Endpoints}.{apiGroupName}"
                : $"{projectName}.{ClientFolderName}.{ContentGeneratorConstants.Endpoints}.{apiGroupName}";

            foreach (var openApiOperation in openApiPath.Value.Operations)
            {
                if (openApiOperation.Value.Deprecated)
                {
                    continue;
                }

                var endpointParameters = ContentGeneratorClientEndpointParametersFactory.Create(
                    projectName,
                    apiGroupName,
                    fullNamespace,
                    openApiPath.Value,
                    openApiOperation.Key,
                    openApiOperation.Value,
                    HttpClientName,
                    $"{openApiDocument.GetServerUrlBasePath()}{openApiPath.Key}");

                var contentGenerator = new ContentGeneratorClientEndpoint(
                    new GeneratedCodeHeaderGenerator(new GeneratedCodeGeneratorParameters(apiGeneratorVersion)),
                    new GeneratedCodeAttributeGenerator(new GeneratedCodeGeneratorParameters(apiGeneratorVersion)),
                    new CodeDocumentationTagsGenerator(),
                    endpointParameters,
                    useProblemDetailsAsDefaultResponseBody);

                var content = contentGenerator.Generate();

                var contentWriter = new ContentWriter(logger);
                contentWriter.Write(
                    projectPath,
                    projectPath.CombineFileInfo(ContentGeneratorConstants.Endpoints, apiGroupName, $"{endpointParameters.EndpointName}.cs"),
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

            var fullNamespace = string.IsNullOrEmpty(ClientFolderName)
                ? $"{projectName}.{ContentGeneratorConstants.Endpoints}.{apiGroupName}.{ContentGeneratorConstants.Interfaces}"
                : $"{projectName}.{ClientFolderName}.{ContentGeneratorConstants.Endpoints}.{apiGroupName}.{ContentGeneratorConstants.Interfaces}";

            foreach (var openApiOperation in openApiPath.Value.Operations)
            {
                if (openApiOperation.Value.Deprecated)
                {
                    continue;
                }

                var endpointResultInterfaceParameters = ContentGeneratorClientEndpointResultInterfaceParametersFactory.Create(
                    projectName,
                    apiGroupName,
                    fullNamespace,
                    openApiPath.Value,
                    openApiOperation.Value);

                var contentGenerator = new ContentGeneratorClientEndpointResultInterface(
                    new GeneratedCodeHeaderGenerator(new GeneratedCodeGeneratorParameters(apiGeneratorVersion)),
                    new GeneratedCodeAttributeGenerator(new GeneratedCodeGeneratorParameters(apiGeneratorVersion)),
                    new CodeDocumentationTagsGenerator(),
                    endpointResultInterfaceParameters,
                    useProblemDetailsAsDefaultResponseBody);

                var content = contentGenerator.Generate();

                var contentWriter = new ContentWriter(logger);
                contentWriter.Write(
                    projectPath,
                    projectPath.CombineFileInfo(ContentGeneratorConstants.Endpoints, apiGroupName, ContentGeneratorConstants.Interfaces, $"{endpointResultInterfaceParameters.InterfaceName}.cs"),
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

            var fullNamespace = string.IsNullOrEmpty(ClientFolderName)
                ? $"{projectName}.{ContentGeneratorConstants.Endpoints}.{apiGroupName}"
                : $"{projectName}.{ClientFolderName}.{ContentGeneratorConstants.Endpoints}.{apiGroupName}";

            foreach (var openApiOperation in openApiPath.Value.Operations)
            {
                if (openApiOperation.Value.Deprecated)
                {
                    continue;
                }

                var endpointResultParameters = ContentGeneratorClientEndpointResultParametersFactory.Create(
                    projectName,
                    apiGroupName,
                    fullNamespace,
                    openApiPath.Value,
                    openApiOperation.Value);

                var contentGenerator = new ContentGeneratorClientEndpointResult(
                    new GeneratedCodeHeaderGenerator(new GeneratedCodeGeneratorParameters(apiGeneratorVersion)),
                    new GeneratedCodeAttributeGenerator(new GeneratedCodeGeneratorParameters(apiGeneratorVersion)),
                    new CodeDocumentationTagsGenerator(),
                    endpointResultParameters,
                    useProblemDetailsAsDefaultResponseBody);

                var content = contentGenerator.Generate();

                var contentWriter = new ContentWriter(logger);
                contentWriter.Write(
                    projectPath,
                    projectPath.CombineFileInfo(ContentGeneratorConstants.Endpoints, apiGroupName, $"{endpointResultParameters.EndpointResultName}.cs"),
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

        if (openApiDocument.IsUsingRequiredForSystemLinq())
        {
            requiredUsings.Add("System.Linq");
        }

        if (openApiDocument.IsUsingRequiredForSystemCollectionGeneric())
        {
            requiredUsings.Add("System.Collections.Generic");
        }

        if (openApiDocument.IsUsingRequiredForSystemTextJsonSerializationAndSystemRuntimeSerialization())
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

        if (operationSchemaMappings.Any(apiOperation => apiOperation.Model.IsEnum ||
                                                        apiOperation.Model.IsShared))
        {
            requiredUsings.Add($"{projectName}.{ContentGeneratorConstants.Contracts}");
        }

        var apiGroupNames = openApiDocument.GetApiGroupNames();
        foreach (var openApiPath in openApiDocument.Paths)
        {
            var apiGroupName = openApiPath.GetApiGroupName();
            foreach (var openApiOperation in openApiPath.Value.Operations)
            {
                if (openApiOperation.Value.Deprecated)
                {
                    continue;
                }

                if (!openApiPath.Value.HasParameters() &&
                    !openApiOperation.Value.HasParametersOrRequestBody())
                {
                    continue;
                }

                var requiredUsing = $"{projectName}.{ContentGeneratorConstants.Contracts}.{apiGroupName}";
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

                var requiredUsing = $"{projectName}.{ContentGeneratorConstants.Contracts}.{apiGroupName}";
                if (!requiredUsings.Contains(requiredUsing, StringComparer.CurrentCulture))
                {
                    requiredUsings.Add(requiredUsing);
                }
            }
        }

        requiredUsings.AddRange(apiGroupNames.Select(x => $"{projectName}.{ContentGeneratorConstants.Endpoints}.{x}.{ContentGeneratorConstants.Interfaces}"));

        GlobalUsingsHelper.CreateOrUpdate(
            logger,
            ContentWriterArea.Src,
            projectPath,
            requiredUsings,
            removeNamespaceGroupSeparatorInGlobalUsings);
    }

    private void GenerateEnumerationType(
        string enumerationName,
        OpenApiSchema openApiSchemaEnumeration)
    {
        var fullNamespace = $"{projectName}.{ContentGeneratorConstants.Contracts}";

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
            projectPath,
            projectPath.CombineFileInfo(ContentGeneratorConstants.Contracts, ContentGeneratorConstants.SpecialFolderEnumerationTypes, $"{enumerationName}.cs"),
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
            ? $"{projectName}.{ContentGeneratorConstants.Contracts}"
            : $"{projectName}.{ContentGeneratorConstants.Contracts}.{apiGroupName}";

        var parameters = ContentGeneratorServerClientModelParametersFactory.CreateForClass(
            codeGeneratorContentHeader,
            fullNamespace,
            codeGeneratorAttribute,
            modelName,
            apiSchemaModel);

        var contentGeneratorClass = new GenerateContentForClass(
            new CodeDocumentationTagsGenerator(),
            parameters);

        var content = contentGeneratorClass.Generate();

        var contentWriter = new ContentWriter(logger);
        contentWriter.Write(
            projectPath,
            isSharedContract
                ? projectPath.CombineFileInfo(ContentGeneratorConstants.Contracts, ContentGeneratorConstants.SpecialFolderSharedModels, $"{modelName}.cs")
                : projectPath.CombineFileInfo(ContentGeneratorConstants.Contracts, apiGroupName, $"{modelName}.cs"),
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