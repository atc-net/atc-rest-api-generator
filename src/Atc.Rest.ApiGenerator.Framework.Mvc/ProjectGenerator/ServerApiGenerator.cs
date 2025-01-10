// ReSharper disable ArrangeObjectCreationWhenTypeNotEvident
namespace Atc.Rest.ApiGenerator.Framework.Mvc.ProjectGenerator;

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
        ArgumentNullException.ThrowIfNull(operationSchemaMappings);
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
        codeGeneratorAttribute = AttributeParametersFactory
            .CreateGeneratedCode(settings.Version);
    }

    public async Task ScaffoldProjectFile()
    {
        var packageReferences = await nugetPackageReferenceProvider.GetPackageReferencesForApiProjectForMvc();

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
                [
                    new(
                        "None",
                        [
                            new("Remove", @"Resources\ApiSpecification.yaml"),
                        ],
                        Value: null),
                    new(
                        "EmbeddedResource",
                        [
                            new("Include", @"Resources\ApiSpecification.yaml"),
                        ],
                        Value: null),
                ],
                [
                    new(
                        "FrameworkReference",
                        [
                            new("Include", "Microsoft.AspNetCore.App"),
                        ],
                        Value: null),
                ],
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
        var classParameters = ClassParametersFactory.Create(
            codeGeneratorContentHeader,
            settings.ProjectName,
            codeGeneratorAttribute,
            "ApiRegistration");

        var contentGenerator = new GenerateContentForClass(
            new CodeDocumentationTagsGenerator(),
            classParameters);

        var content = contentGenerator.Generate();

        var contentWriter = new ContentWriter(logger);
        contentWriter.Write(
            settings.ProjectPath,
            FileInfoFactory.Create(settings.ProjectPath, "ApiRegistration.cs"),
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

            var contractsLocation = LocationFactory.CreateWithApiGroupName(apiGroupName, settings.ContractsLocation);

            var fullNamespace = NamespaceFactory.Create(settings.ProjectName, contractsLocation);

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

                var parameterParameters = ContentGeneratorServerParameterParametersFactory.CreateForClass(
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

            var contractsLocation = LocationFactory.CreateWithApiGroupName(apiGroupName, settings.ContractsLocation);

            var fullNamespace = NamespaceFactory.Create(settings.ProjectName, contractsLocation);

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

            var contractsLocation = LocationFactory.CreateWithApiGroupName(apiGroupName, settings.ContractsLocation);

            var fullNamespace = NamespaceFactory.Create(settings.ProjectName, contractsLocation);

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
            var endpointsLocation = LocationFactory.CreateWithoutTemplateForApiGroupName(settings.EndpointsLocation);

            var fullNamespace = NamespaceFactory.Create(settings.ProjectName, endpointsLocation);

            var controllerParameters = ContentGeneratorServerEndpointParametersFactory.Create(
                operationSchemaMappings,
                settings.ProjectName,
                fullNamespace,
                apiGroupName,
                GetRouteByApiGroupName(apiGroupName),
                ContentGeneratorConstants.Controller,
                openApiDocument,
                settings.UsePartialClassForEndpoints);

            var contentGenerator = new ContentGenerators.ContentGeneratorServerController(
                new GeneratedCodeHeaderGenerator(new GeneratedCodeGeneratorParameters(settings.Version)),
                new GeneratedCodeAttributeGenerator(new GeneratedCodeGeneratorParameters(settings.Version)),
                new CodeDocumentationTagsGenerator(),
                controllerParameters,
                settings.UseProblemDetailsAsDefaultResponseBody);

            var content = contentGenerator.Generate();

            var contentWriter = new ContentWriter(logger);
            contentWriter.Write(
                settings.ProjectPath,
                FileInfoFactory.Create(settings.ProjectPath, endpointsLocation, $"{controllerParameters.EndpointName}.cs"),
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
            "Microsoft.AspNetCore.Http",
            "Microsoft.AspNetCore.Mvc",
            "Atc.Rest.Results",
        };

        // TODO: Check for any use ??
        requiredUsings.Add("System.Net");

        if (openApiDocument.IsUsingRequiredForSystemTextJsonSerializationAndSystemRuntimeSerialization(settings.IncludeDeprecatedOperations))
        {
            requiredUsings.Add("System.Runtime.Serialization");
            requiredUsings.Add("System.Text.Json.Serialization");
        }

        if (openApiDocument.IsUsingRequiredForAsyncEnumerable(settings.IncludeDeprecatedOperations))
        {
            requiredUsings.Add("Atc.Factories");
        }

        if (openApiDocument.IsUsingRequiredForMicrosoftAspNetCoreAuthorization(settings.IncludeDeprecatedOperations))
        {
            requiredUsings.Add("Microsoft.AspNetCore.Authorization");
        }

        if (operationSchemaMappings.Any(apiOperation => apiOperation.Model.IsShared))
        {
            var requiredUsing = NamespaceFactory.Create(
                settings.ProjectName,
                LocationFactory.CreateWithoutTemplateForApiGroupName(settings.ContractsLocation));

            if (!requiredUsings.Contains(requiredUsing, StringComparer.CurrentCulture))
            {
                requiredUsings.Add(requiredUsing);
            }
        }

        var apiGroupNames = openApiDocument.GetApiGroupNames();

        requiredUsings.AddRange(apiGroupNames.Select(x => LocationFactory.Create(settings.ProjectName, LocationFactory.CreateWithApiGroupName(x, settings.ContractsLocation))));

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
        var contractsLocation = LocationFactory.CreateWithoutTemplateForApiGroupName(settings.ContractsLocation);

        var fullNamespace = NamespaceFactory.Create(settings.ProjectName, contractsLocation);

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
        var contractsLocation = isSharedContract
            ? LocationFactory.CreateWithoutTemplateForApiGroupName(settings.ContractsLocation)
            : LocationFactory.CreateWithApiGroupName(apiGroupName, settings.ContractsLocation);

        var fullNamespace = NamespaceFactory.Create(settings.ProjectName, contractsLocation);

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