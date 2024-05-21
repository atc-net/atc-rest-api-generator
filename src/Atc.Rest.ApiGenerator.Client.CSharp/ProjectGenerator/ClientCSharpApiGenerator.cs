// ReSharper disable ArrangeObjectCreationWhenTypeNotEvident
namespace Atc.Rest.ApiGenerator.Client.CSharp.ProjectGenerator;

public class ClientCSharpApiGenerator : IClientCSharpApiGenerator
{
    private readonly ILogger<ClientCSharpApiGenerator> logger;
    private readonly Version apiGeneratorVersion;
    private readonly string projectName;
    private readonly DirectoryInfo projectPath;
    private readonly OpenApiDocument openApiDocument;
    private readonly IList<ApiOperation> operationSchemaMappings;
    private readonly string codeGeneratorContentHeader;
    private readonly AttributeParameters codeGeneratorAttribute;

    public ClientCSharpApiGenerator(
        ILoggerFactory loggerFactory,
        Version apiGeneratorVersion,
        string projectName,
        DirectoryInfo projectPath,
        OpenApiDocument openApiDocument,
        IList<ApiOperation> operationSchemaMappings)
    {
        ArgumentNullException.ThrowIfNull(loggerFactory);
        ArgumentNullException.ThrowIfNull(apiGeneratorVersion);
        ArgumentNullException.ThrowIfNull(projectName);
        ArgumentNullException.ThrowIfNull(projectPath);
        ArgumentNullException.ThrowIfNull(openApiDocument);
        ArgumentNullException.ThrowIfNull(operationSchemaMappings);

        logger = loggerFactory.CreateLogger<ClientCSharpApiGenerator>();
        this.apiGeneratorVersion = apiGeneratorVersion;
        this.projectName = projectName;
        this.projectPath = projectPath;
        this.openApiDocument = openApiDocument;
        this.operationSchemaMappings = operationSchemaMappings;

        codeGeneratorContentHeader = GeneratedCodeHeaderGeneratorFactory
            .Create(apiGeneratorVersion)
            .Generate();
        codeGeneratorAttribute = AttributeParametersFactory
            .CreateGeneratedCode(apiGeneratorVersion);
    }

    public string? ClientFolderName { get; set; }

    public string HttpClientName { get; set; } = "DefaultHttpClient";

    public bool UseProblemDetailsAsDefaultBody { get; set; }

    public void ScaffoldProjectFile()
    {
        var projectFileParameters = new ProjectFileParameters(
            "Microsoft.NET.Sdk",
            [
                new List<PropertyGroupParameter>
                {
                    new("TargetFramework", Attributes: null, "net8.0"),
                    new("Nullable", Attributes: null, "enable"),
                    new("IsPackable", Attributes: null, "false"),
                },
                new List<PropertyGroupParameter>
                {
                    new("GenerateDocumentationFile", Attributes: null, "true"),
                },
                new List<PropertyGroupParameter>
                {
                    new("DocumentationFile", Attributes: null, @$"bin\Debug\net8.0\{projectName}.xml"),
                    new("NoWarn", Attributes: null, "1573;1591;1701;1702;1712;8618;"),
                },
            ],
            [
                new List<ItemGroupParameter>
                {
                    new(
                        "PackageReference",
                        [
                            new("Include", "Atc"),
                            new("Version", "2.0.472"),
                        ],
                        Value: null),
                    new(
                        "PackageReference",
                        [
                            new("Include", "Atc.Rest"),
                            new("Version", "2.0.472"),
                        ],
                        Value: null),
                    new(
                        "PackageReference",
                        [
                            new("Include", "Atc.Rest.Client"),
                            new("Version", "1.0.36"),
                        ],
                        Value: null),
                },
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
                .Where(x => x.LocatedArea is ApiSchemaMapLocatedAreaType.Response or ApiSchemaMapLocatedAreaType.RequestBody &&
                            x.ApiGroupName.Equals(apiGroupName, StringComparison.OrdinalIgnoreCase))
                .ToList();

            var apiOperationModels = GetDistinctApiOperationModels(apiOperations);

            foreach (var apiOperationModel in apiOperationModels)
            {
                var apiSchema = openApiDocument.Components.Schemas.First(x => x.Key.Equals(apiOperationModel.Name, StringComparison.OrdinalIgnoreCase));

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

    public void GenerateParameters()
    {
        foreach (var openApiPath in openApiDocument.Paths)
        {
            var apiGroupName = openApiPath.GetApiGroupName();

            var fullNamespace = string.IsNullOrEmpty(ClientFolderName)
                ? $"{projectName}.{ContentGeneratorConstants.Contracts}.{apiGroupName}.{ContentGeneratorConstants.RequestParameters}"
                : $"{projectName}.{ClientFolderName}.{ContentGeneratorConstants.Contracts}.{apiGroupName}.{ContentGeneratorConstants.RequestParameters}";

            foreach (var apiOperation in openApiPath.Value.Operations)
            {
                if (!apiOperation.Value.HasParametersOrRequestBody() &&
                    !openApiPath.Value.HasParameters())
                {
                    continue;
                }

                var parameterParameters = ContentGeneratorClientParameterParametersFactory.Create(
                    fullNamespace,
                    apiOperation.Value,
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
                var endpointParameters = ContentGeneratorClientEndpointParametersFactory.Create(
                    UseProblemDetailsAsDefaultBody,
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
                    endpointParameters);

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
                var endpointResultInterfaceParameters = ContentGeneratorClientEndpointResultInterfaceParametersFactory.Create(
                    UseProblemDetailsAsDefaultBody,
                    projectName,
                    apiGroupName,
                    fullNamespace,
                    openApiPath.Value,
                    openApiOperation.Value);

                var contentGenerator = new ContentGeneratorClientEndpointResultInterface(
                    new GeneratedCodeHeaderGenerator(new GeneratedCodeGeneratorParameters(apiGeneratorVersion)),
                    new GeneratedCodeAttributeGenerator(new GeneratedCodeGeneratorParameters(apiGeneratorVersion)),
                    new CodeDocumentationTagsGenerator(),
                    endpointResultInterfaceParameters);

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
                var endpointResultParameters = ContentGeneratorClientEndpointResultParametersFactory.Create(
                    UseProblemDetailsAsDefaultBody,
                    projectName,
                    apiGroupName,
                    fullNamespace,
                    UseProblemDetailsAsDefaultBody,
                    openApiPath.Value,
                    openApiOperation.Value);

                var contentGenerator = new ContentGeneratorClientEndpointResult(
                    new GeneratedCodeHeaderGenerator(new GeneratedCodeGeneratorParameters(apiGeneratorVersion)),
                    new GeneratedCodeAttributeGenerator(new GeneratedCodeGeneratorParameters(apiGeneratorVersion)),
                    new CodeDocumentationTagsGenerator(),
                    endpointResultParameters);

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
            "System.Net",
            "System.Net.Http",
            "System.Threading",
            "System.Threading.Tasks",
            "Atc.Rest.Client",
            "Atc.Rest.Client.Builder",
            "Microsoft.AspNetCore.Mvc",
        };

        if (true) // TODO: Add check for using example Guid
        {
            requiredUsings.Add("System");
        }

        if (true) // TODO: Add check for using example ".Any()"
        {
            requiredUsings.Add("System.Linq");
        }

        if (true) // TODO: Add check for using example List
        {
            requiredUsings.Add("System.Collections.Generic");
        }

        if (operationSchemaMappings.FirstOrDefault(x => x.Model.UsesIFormFile) is not null)
        {
            requiredUsings.Add("Microsoft.AspNetCore.Http");
        }

        if (operationSchemaMappings.Any(apiOperation => apiOperation.Model.IsShared))
        {
            requiredUsings.Add($"{projectName}.{ContentGeneratorConstants.Contracts}");
        }

        var apiGroupNames = openApiDocument.GetApiGroupNames();
        foreach (var openApiPath in openApiDocument.Paths)
        {
            var apiGroupName = openApiPath.GetApiGroupName();
            foreach (var openApiOperation in openApiPath.Value.Operations)
            {
                if (!openApiPath.Value.HasParameters() &&
                    !openApiOperation.Value.HasParametersOrRequestBody())
                {
                    continue;
                }

                requiredUsings.Add($"{projectName}.{ContentGeneratorConstants.Contracts}.{apiGroupName}.{ContentGeneratorConstants.RequestParameters}");
            }
        }

        foreach (var apiGroupName in apiGroupNames)
        {
            var apiOperations = operationSchemaMappings
                .Where(x => x.LocatedArea is ApiSchemaMapLocatedAreaType.Response or ApiSchemaMapLocatedAreaType.RequestBody &&
                            x.ApiGroupName.Equals(apiGroupName, StringComparison.OrdinalIgnoreCase))
                .ToList();

            var apiOperationModels = GetDistinctApiOperationModels(apiOperations);

            foreach (var apiOperationModel in apiOperationModels)
            {
                if (apiOperationModel.IsEnum ||
                    apiOperationModel.IsShared)
                {
                    continue;
                }

                requiredUsings.Add($"{projectName}.{ContentGeneratorConstants.Contracts}.{apiGroupName}");
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