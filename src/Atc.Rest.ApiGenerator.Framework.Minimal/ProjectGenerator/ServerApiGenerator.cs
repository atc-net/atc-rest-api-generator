// ReSharper disable ArrangeObjectCreationWhenTypeNotEvident
namespace Atc.Rest.ApiGenerator.Framework.Minimal.ProjectGenerator;

public class ServerApiGenerator : IServerApiGenerator
{
    private readonly ILogger<ServerApiGenerator> logger;
    private readonly INugetPackageReferenceProvider nugetPackageReferenceProvider;
    private readonly Version apiGeneratorVersion;
    private readonly string projectName;
    private readonly DirectoryInfo projectPath;
    private readonly OpenApiDocument openApiDocument;
    private readonly IList<ApiOperation> operationSchemaMappings;
    private readonly string routeBase;
    private readonly string codeGeneratorContentHeader;
    private readonly AttributeParameters codeGeneratorAttribute;
    private readonly bool useProblemDetailsAsDefaultResponseBody;

    public ServerApiGenerator(
        ILoggerFactory loggerFactory,
        INugetPackageReferenceProvider nugetPackageReferenceProvider,
        Version apiGeneratorVersion,
        string projectName,
        DirectoryInfo projectPath,
        OpenApiDocument openApiDocument,
        IList<ApiOperation> operationSchemaMappings,
        string routeBase,
        bool useProblemDetailsAsDefaultResponseBody)
    {
        ArgumentNullException.ThrowIfNull(loggerFactory);
        ArgumentNullException.ThrowIfNull(nugetPackageReferenceProvider);
        ArgumentNullException.ThrowIfNull(apiGeneratorVersion);
        ArgumentNullException.ThrowIfNull(projectName);
        ArgumentNullException.ThrowIfNull(projectPath);
        ArgumentNullException.ThrowIfNull(openApiDocument);
        ArgumentNullException.ThrowIfNull(routeBase);

        logger = loggerFactory.CreateLogger<ServerApiGenerator>();
        this.nugetPackageReferenceProvider = nugetPackageReferenceProvider;
        this.apiGeneratorVersion = apiGeneratorVersion;
        this.projectName = projectName;
        this.projectPath = projectPath;
        this.openApiDocument = openApiDocument;
        this.operationSchemaMappings = operationSchemaMappings;
        this.routeBase = routeBase;
        this.useProblemDetailsAsDefaultResponseBody = useProblemDetailsAsDefaultResponseBody;

        codeGeneratorContentHeader = GeneratedCodeHeaderGeneratorFactory
            .Create(apiGeneratorVersion)
            .Generate();
        codeGeneratorAttribute = new AttributeParameters(
            "GeneratedCode",
            $"\"{ContentWriterConstants.ApiGeneratorName}\", \"{apiGeneratorVersion}\"");
    }

    public async Task ScaffoldProjectFile()
    {
        var packageReferences = await nugetPackageReferenceProvider.GetPackageReferencesForApiProjectForMinimalApi();

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

    public void GenerateAssemblyMarker()
    {
        ArgumentNullException.ThrowIfNull(apiGeneratorVersion);

        var suppressMessageAvoidEmptyInterfaceAttribute = new AttributeParameters(
            "SuppressMessage",
            "\"Design\", \"CA1040:Avoid empty interfaces\", Justification = \"OK.\"");

        var interfaceParameters = InterfaceParametersFactory.Create(
            codeGeneratorContentHeader,
            projectName,
            [suppressMessageAvoidEmptyInterfaceAttribute, codeGeneratorAttribute],
            "IApiContractAssemblyMarker");

        var contentGenerator = new GenerateContentForInterface(
            new CodeDocumentationTagsGenerator(),
            interfaceParameters);

        var content = contentGenerator.Generate();

        var contentWriter = new ContentWriter(logger);
        contentWriter.Write(
            projectPath,
            projectPath.CombineFileInfo("IApiContractAssemblyMarker.cs"),
            ContentWriterArea.Src,
            content);
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

            var fullNamespace = $"{projectName}.{ContentGeneratorConstants.Contracts}.{apiGroupName}";

            foreach (var openApiOperation in openApiPath.Value.Operations)
            {
                if (!openApiPath.Value.HasParameters() &&
                    !openApiOperation.Value.HasParametersOrRequestBody())
                {
                    continue;
                }

                var parameterParameters = ContentGeneratorServerParameterParametersFactory.CreateForRecord(
                    fullNamespace,
                    openApiOperation.Value,
                    openApiPath.Value.Parameters);

                var contentGenerator = new ContentGenerators.Server.ContentGeneratorServerParameter(
                    new GeneratedCodeHeaderGenerator(new GeneratedCodeGeneratorParameters(apiGeneratorVersion)),
                    new GeneratedCodeAttributeGenerator(new GeneratedCodeGeneratorParameters(apiGeneratorVersion)),
                    new CodeDocumentationTagsGenerator(),
                    parameterParameters);

                var content = contentGenerator.Generate();

                var contentWriter = new ContentWriter(logger);
                contentWriter.Write(
                    projectPath,
                    projectPath.CombineFileInfo(ContentGeneratorConstants.Contracts, apiGroupName, ContentGeneratorConstants.Parameters, $"{parameterParameters.ParameterName}.cs"),
                    ContentWriterArea.Src,
                    content);
            }
        }
    }

    public void GenerateResults()
    {
        foreach (var openApiPath in openApiDocument.Paths)
        {
            var apiGroupName = openApiPath.GetApiGroupName();

            var fullNamespace = $"{projectName}.{ContentGeneratorConstants.Contracts}.{apiGroupName}";

            foreach (var openApiOperation in openApiPath.Value.Operations)
            {
                var resultParameters = ContentGeneratorServerResultParametersFactory.Create(
                    fullNamespace,
                    openApiOperation.Value);

                var contentGenerator = new ContentGenerators.Server.ContentGeneratorServerResult(
                    new GeneratedCodeHeaderGenerator(new GeneratedCodeGeneratorParameters(apiGeneratorVersion)),
                    new GeneratedCodeAttributeGenerator(new GeneratedCodeGeneratorParameters(apiGeneratorVersion)),
                    new CodeDocumentationTagsGenerator(),
                    resultParameters,
                    useProblemDetailsAsDefaultResponseBody);

                var content = contentGenerator.Generate();

                var contentWriter = new ContentWriter(logger);
                contentWriter.Write(
                    projectPath,
                    projectPath.CombineFileInfo(ContentGeneratorConstants.Contracts, apiGroupName, ContentGeneratorConstants.Results, $"{resultParameters.ResultName}.cs"),
                    ContentWriterArea.Src,
                    content);
            }
        }
    }

    public void GenerateInterfaces()
    {
        foreach (var openApiPath in openApiDocument.Paths)
        {
            var apiGroupName = openApiPath.GetApiGroupName();

            var fullNamespace = $"{projectName}.{ContentGeneratorConstants.Contracts}.{apiGroupName}";

            foreach (var openApiOperation in openApiPath.Value.Operations)
            {
                var interfaceParameters = ContentGeneratorServerHandlerInterfaceParametersFactory.Create(
                    codeGeneratorContentHeader,
                    fullNamespace,
                    codeGeneratorAttribute,
                    openApiPath.Value,
                    openApiOperation.Value);

                var contentGeneratorInterface = new GenerateContentForInterface(
                    new CodeDocumentationTagsGenerator(),
                    interfaceParameters);

                var content = contentGeneratorInterface.Generate();

                var contentWriter = new ContentWriter(logger);
                contentWriter.Write(
                    projectPath,
                    projectPath.CombineFileInfo(ContentGeneratorConstants.Contracts, apiGroupName, ContentGeneratorConstants.Interfaces, $"{interfaceParameters.InterfaceTypeName}.cs"),
                    ContentWriterArea.Src,
                    content);
            }
        }
    }

    public void GenerateEndpoints()
    {
        foreach (var apiGroupName in openApiDocument.GetApiGroupNames())
        {
            var endpointParameters = ContentGeneratorServerEndpointParametersFactory.Create(
                operationSchemaMappings,
                projectName,
                $"{projectName}.{ContentGeneratorConstants.Endpoints}",
                apiGroupName,
                GetRouteByApiGroupName(apiGroupName),
                ContentGeneratorConstants.EndpointDefinition,
                openApiDocument);

            var contentGenerator = new ContentGenerators.Server.ContentGeneratorServerEndpoints(
                new GeneratedCodeHeaderGenerator(new GeneratedCodeGeneratorParameters(apiGeneratorVersion)),
                new GeneratedCodeAttributeGenerator(new GeneratedCodeGeneratorParameters(apiGeneratorVersion)),
                new CodeDocumentationTagsGenerator(),
                endpointParameters,
                useProblemDetailsAsDefaultResponseBody);

            var content = contentGenerator.Generate();

            var contentWriter = new ContentWriter(logger);
            contentWriter.Write(
                projectPath,
                projectPath.CombineFileInfo(ContentGeneratorConstants.Endpoints, $"{endpointParameters.EndpointName}.cs"),
                ContentWriterArea.Src,
                content);
        }
    }

    public void MaintainApiSpecification(
        FileInfo apiSpecificationFile)
        => ResourcesHelper.CopyApiSpecification(
            apiSpecificationFile,
            openApiDocument,
            projectPath);

    public void MaintainGlobalUsings(
        bool removeNamespaceGroupSeparatorInGlobalUsings)
    {
        var requiredUsings = new List<string>
        {
            "System.CodeDom.Compiler",
            "System.ComponentModel.DataAnnotations",
            "System.Diagnostics.CodeAnalysis",
            "Atc.Rest.MinimalApi.Abstractions",
            "Microsoft.AspNetCore.Builder",
            "Microsoft.AspNetCore.Http",
            "Microsoft.AspNetCore.Mvc",
        };

        if (openApiDocument.IsUsingRequiredForAtcRestResults())
        {
            requiredUsings.Add("Atc.Rest.Results");
        }

        //// TODO: Check for any use ??
        //requiredUsings.Add("System.Net");

        // TODO: Check for any use of operations parameters
        requiredUsings.Add("Atc.Rest.MinimalApi.Filters.Endpoints");

        var apiGroupNames = openApiDocument.GetApiGroupNames();

        // TODO: Check for any use ??
        requiredUsings.Add($"{projectName}.{ContentGeneratorConstants.Contracts}");

        requiredUsings.AddRange(apiGroupNames.Select(x => $"{projectName}.{ContentGeneratorConstants.Contracts}.{x}"));

        GlobalUsingsHelper.CreateOrUpdate(
            logger,
            ContentWriterArea.Src,
            projectPath,
            requiredUsings,
            removeNamespaceGroupSeparatorInGlobalUsings);
    }

    private string GetRouteByApiGroupName(
        string apiGroupName)
    {
        var (key, _) = openApiDocument.Paths.FirstOrDefault(x => x.IsPathStartingSegmentName(apiGroupName));
        if (key is null)
        {
            throw new NotSupportedException($"{nameof(apiGroupName)} was not found in any route.");
        }

        var routeSuffix = key
            .Split('/', StringSplitOptions.RemoveEmptyEntries)
            .FirstOrDefault();

        return $"{routeBase}/{routeSuffix}";
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

        var parameters = ContentGeneratorServerClientModelParametersFactory.CreateForRecord(
            codeGeneratorContentHeader,
            fullNamespace,
            codeGeneratorAttribute,
            modelName,
            apiSchemaModel);

        var contentGeneratorRecord = new GenerateContentForRecords(
            new CodeDocumentationTagsGenerator(),
            parameters);

        var content = contentGeneratorRecord.Generate();

        var contentWriter = new ContentWriter(logger);
        contentWriter.Write(
            projectPath,
            isSharedContract
                ? projectPath.CombineFileInfo(ContentGeneratorConstants.Contracts, ContentGeneratorConstants.SpecialFolderSharedModels, $"{modelName}.cs")
                : projectPath.CombineFileInfo(ContentGeneratorConstants.Contracts, apiGroupName, ContentGeneratorConstants.Models, $"{modelName}.cs"),
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