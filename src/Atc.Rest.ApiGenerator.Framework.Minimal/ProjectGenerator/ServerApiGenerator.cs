// ReSharper disable ArrangeObjectCreationWhenTypeNotEvident
namespace Atc.Rest.ApiGenerator.Framework.Minimal.ProjectGenerator;

public class ServerApiGenerator : IServerApiGenerator
{
    private readonly ILogger<ServerApiGenerator> logger;
    private readonly INugetPackageReferenceProvider nugetPackageReferenceProvider;
    private readonly OpenApiDocument openApiDocument;
    private readonly IList<ApiOperation> operationSchemaMappings;
    private readonly string routeBase;
    private readonly string codeGeneratorContentHeader;
    private readonly AttributeParameters codeGeneratorAttribute;
    private readonly GeneratorSettings settings;

    public ServerApiGenerator(
        ILoggerFactory loggerFactory,
        INugetPackageReferenceProvider nugetPackageReferenceProvider,
        OpenApiDocument openApiDocument,
        IList<ApiOperation> operationSchemaMappings,
        string routeBase,
        GeneratorSettings generatorSettings)
    {
        ArgumentNullException.ThrowIfNull(loggerFactory);
        ArgumentNullException.ThrowIfNull(nugetPackageReferenceProvider);
        ArgumentNullException.ThrowIfNull(openApiDocument);
        ArgumentNullException.ThrowIfNull(routeBase);
        ArgumentNullException.ThrowIfNull(generatorSettings);

        logger = loggerFactory.CreateLogger<ServerApiGenerator>();
        this.nugetPackageReferenceProvider = nugetPackageReferenceProvider;
        this.openApiDocument = openApiDocument;
        this.operationSchemaMappings = operationSchemaMappings;
        this.routeBase = routeBase;
        settings = generatorSettings;

        codeGeneratorContentHeader = GeneratedCodeHeaderGeneratorFactory
            .Create(settings.Version)
            .Generate();
        codeGeneratorAttribute = new AttributeParameters(
            "GeneratedCode",
            $"\"{ContentWriterConstants.ApiGeneratorName}\", \"{settings.Version}\"");
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

    public void GenerateAssemblyMarker()
    {
        ArgumentNullException.ThrowIfNull(settings.Version);

        var suppressMessageAvoidEmptyInterfaceAttribute = new AttributeParameters(
            "SuppressMessage",
            "\"Design\", \"CA1040:Avoid empty interfaces\", Justification = \"OK.\"");

        var interfaceParameters = InterfaceParametersFactory.Create(
            codeGeneratorContentHeader,
            settings.ProjectName,
            [suppressMessageAvoidEmptyInterfaceAttribute, codeGeneratorAttribute],
            "IApiContractAssemblyMarker");

        var contentGenerator = new GenerateContentForInterface(
            new CodeDocumentationTagsGenerator(),
            interfaceParameters);

        var content = contentGenerator.Generate();

        var contentWriter = new ContentWriter(logger);
        contentWriter.Write(
            settings.ProjectPath,
            FileInfoFactory.Create(settings.ProjectPath, "IApiContractAssemblyMarker.cs"),
            ContentWriterArea.Src,
            content);
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

            var contractsNamespace = NamespaceFactory.CreateWithApiGroupName(apiGroupName, settings.ContractsNamespace);

            var fullNamespace = NamespaceFactory.Create(settings.ProjectName, contractsNamespace);

            var contractsLocation = LocationFactory.CreateWithApiGroupName(apiGroupName, settings.ContractsLocation);

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

                var parameterParameters = ContentGeneratorServerParameterParametersFactory.CreateForRecord(
                    fullNamespace,
                    openApiOperation.Value,
                    openApiPath.Value.Parameters);

                var contentGenerator = new ContentGenerators.ContentGeneratorServerParameter(
                    new GeneratedCodeHeaderGenerator(new GeneratedCodeGeneratorParameters(settings.Version)),
                    new GeneratedCodeAttributeGenerator(new GeneratedCodeGeneratorParameters(settings.Version)),
                    new CodeDocumentationTagsGenerator(),
                    parameterParameters);

                var content = contentGenerator.Generate();

                var contentWriter = new ContentWriter(logger);
                contentWriter.Write(
                    settings.ProjectPath,
                    FileInfoFactory.Create(settings.ProjectPath, contractsLocation, ContentGeneratorConstants.Parameters, $"{parameterParameters.ParameterName}.cs"),
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

            var contractsNamespace = NamespaceFactory.CreateWithApiGroupName(apiGroupName, settings.ContractsNamespace);

            var fullNamespace = NamespaceFactory.Create(settings.ProjectName, contractsNamespace);

            var contractsLocation = LocationFactory.CreateWithApiGroupName(apiGroupName, settings.ContractsLocation);

            foreach (var openApiOperation in openApiPath.Value.Operations)
            {
                if (openApiOperation.Value.Deprecated && !settings.IncludeDeprecatedOperations)
                {
                    continue;
                }

                var resultParameters = ContentGeneratorServerResultParametersFactory.Create(
                    fullNamespace,
                    openApiOperation.Value);

                var contentGenerator = new ContentGenerators.ContentGeneratorServerResult(
                    new GeneratedCodeHeaderGenerator(new GeneratedCodeGeneratorParameters(settings.Version)),
                    new GeneratedCodeAttributeGenerator(new GeneratedCodeGeneratorParameters(settings.Version)),
                    new CodeDocumentationTagsGenerator(),
                    resultParameters,
                    settings.UseProblemDetailsAsDefaultResponseBody);

                var content = contentGenerator.Generate();

                var contentWriter = new ContentWriter(logger);
                contentWriter.Write(
                    settings.ProjectPath,
                    FileInfoFactory.Create(settings.ProjectPath, contractsLocation, ContentGeneratorConstants.Results, $"{resultParameters.ResultName}.cs"),
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

            var contractsNamespace = NamespaceFactory.CreateWithApiGroupName(apiGroupName, settings.ContractsNamespace);

            var fullNamespace = NamespaceFactory.Create(settings.ProjectName, contractsNamespace);

            var contractsLocation = LocationFactory.CreateWithApiGroupName(apiGroupName, settings.ContractsLocation);

            foreach (var openApiOperation in openApiPath.Value.Operations)
            {
                if (openApiOperation.Value.Deprecated && !settings.IncludeDeprecatedOperations)
                {
                    continue;
                }

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
                    settings.ProjectPath,
                    FileInfoFactory.Create(settings.ProjectPath, contractsLocation, ContentGeneratorConstants.Interfaces, $"{interfaceParameters.InterfaceTypeName}.cs"),
                    ContentWriterArea.Src,
                    content);
            }
        }
    }

    public void GenerateEndpoints()
    {
        foreach (var apiGroupName in openApiDocument.GetApiGroupNames())
        {
            var endpointsNamespace = NamespaceFactory.CreateWithoutTemplateForApiGroupName(settings.EndpointsNamespace);

            var fullNamespace = NamespaceFactory.Create(settings.ProjectName, endpointsNamespace);

            var endpointsLocation = LocationFactory.CreateWithoutTemplateForApiGroupName(settings.EndpointsLocation);

            var endpointParameters = ContentGeneratorServerEndpointParametersFactory.Create(
                operationSchemaMappings,
                settings.ProjectName,
                fullNamespace,
                apiGroupName,
                GetRouteByApiGroupName(apiGroupName),
                ContentGeneratorConstants.EndpointDefinition,
                openApiDocument,
                settings.UsePartialClassForEndpoints);

            var contentGenerator = new ContentGenerators.ContentGeneratorServerEndpoints(
                new GeneratedCodeHeaderGenerator(new GeneratedCodeGeneratorParameters(settings.Version)),
                new GeneratedCodeAttributeGenerator(new GeneratedCodeGeneratorParameters(settings.Version)),
                new CodeDocumentationTagsGenerator(),
                endpointParameters,
                settings.UseProblemDetailsAsDefaultResponseBody);

            var content = contentGenerator.Generate();

            var contentWriter = new ContentWriter(logger);
            contentWriter.Write(
                settings.ProjectPath,
                FileInfoFactory.Create(settings.ProjectPath, endpointsLocation, $"{endpointParameters.EndpointName}.cs"),
                ContentWriterArea.Src,
                content);
        }
    }

    public void MaintainApiSpecification(
        FileInfo apiSpecificationFile)
        => ResourcesHelper.CopyApiSpecification(
            apiSpecificationFile,
            openApiDocument,
            settings.ProjectPath);

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

        if (openApiDocument.IsUsingRequiredForSystemTextJsonSerializationAndSystemRuntimeSerialization(settings.IncludeDeprecatedOperations))
        {
            requiredUsings.Add("System.Runtime.Serialization");
            requiredUsings.Add("System.Text.Json.Serialization");
        }

        if (openApiDocument.IsUsingRequiredForAtcRestResults())
        {
            requiredUsings.Add("Atc.Rest.Results");
        }

        if (openApiDocument.IsUsingRequiredForMicrosoftAspNetCoreAuthorization(settings.IncludeDeprecatedOperations))
        {
            requiredUsings.Add("Microsoft.AspNetCore.Authorization");
        }

        if (openApiDocument.IsUsingRequiredForAtcRestMinimalApiFiltersEndpoints())
        {
            requiredUsings.Add("Atc.Rest.MinimalApi.Filters.Endpoints");
        }

        if (operationSchemaMappings.Any(apiOperation => apiOperation.Model.IsShared))
        {
            var requiredUsing = NamespaceFactory.Create(
                settings.ProjectName,
                LocationFactory.CreateWithoutTemplateForApiGroupName(settings.ContractsNamespace));

            if (!requiredUsings.Contains(requiredUsing, StringComparer.CurrentCulture))
            {
                requiredUsings.Add(requiredUsing);
            }
        }

        var apiGroupNames = openApiDocument.GetApiGroupNames();

        requiredUsings.AddRange(apiGroupNames.Select(x => NamespaceFactory.Create(settings.ProjectName, NamespaceFactory.CreateWithApiGroupName(x, settings.ContractsNamespace))));

        GlobalUsingsHelper.CreateOrUpdate(
            logger,
            ContentWriterArea.Src,
            settings.ProjectPath,
            requiredUsings,
            removeNamespaceGroupSeparatorInGlobalUsings);
    }

    private string GetRouteByApiGroupName(
        string apiGroupName)
    {
        var (key, _) = openApiDocument.Paths.FirstOrDefault(x => x.IsPathStartingSegmentName(apiGroupName));
        if (key is null)
        {
            (key, _) = openApiDocument.Paths.FirstOrDefault(x => x.IsPathStartingApiGroupName(apiGroupName));
            if (key is null)
            {
                throw new NotSupportedException($"{nameof(apiGroupName)} was not found in any route.");
            }
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
        var contractsNamespace = NamespaceFactory.CreateWithoutTemplateForApiGroupName(settings.ContractsNamespace);

        var fullNamespace = NamespaceFactory.Create(settings.ProjectName, contractsNamespace);

        var contractsLocation = LocationFactory.CreateWithoutTemplateForApiGroupName(settings.ContractsLocation);

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
            FileInfoFactory.Create(settings.ProjectPath, contractsLocation, ContentGeneratorConstants.SpecialFolderEnumerationTypes, $"{enumerationName}.cs"),
            ContentWriterArea.Src,
            content);
    }

    private void GenerateModel(
        string modelName,
        OpenApiSchema apiSchemaModel,
        string apiGroupName,
        bool isSharedContract)
    {
        var contractsNamespace = isSharedContract
            ? NamespaceFactory.CreateWithoutTemplateForApiGroupName(settings.ContractsNamespace)
            : NamespaceFactory.CreateWithApiGroupName(apiGroupName, settings.ContractsNamespace);

        var fullNamespace = NamespaceFactory.Create(settings.ProjectName, contractsNamespace);

        var contractsLocation = isSharedContract
            ? LocationFactory.CreateWithoutTemplateForApiGroupName(settings.ContractsLocation)
            : LocationFactory.CreateWithApiGroupName(apiGroupName, settings.ContractsLocation);

        var parameters = ContentGeneratorServerClientModelParametersFactory.CreateForRecord(
            codeGeneratorContentHeader,
            fullNamespace,
            codeGeneratorAttribute,
            modelName,
            apiSchemaModel,
            settings.UsePartialClassForContracts,
            settings.IncludeDeprecatedOperations);

        var contentGeneratorRecord = new GenerateContentForRecords(
            new CodeDocumentationTagsGenerator(),
            parameters);

        var content = contentGeneratorRecord.Generate();

        var contentWriter = new ContentWriter(logger);
        contentWriter.Write(
            settings.ProjectPath,
            isSharedContract
                ? FileInfoFactory.Create(settings.ProjectPath, contractsLocation, ContentGeneratorConstants.SpecialFolderSharedModels, $"{modelName}.cs")
                : FileInfoFactory.Create(settings.ProjectPath, contractsLocation, ContentGeneratorConstants.Models, $"{modelName}.cs"),
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