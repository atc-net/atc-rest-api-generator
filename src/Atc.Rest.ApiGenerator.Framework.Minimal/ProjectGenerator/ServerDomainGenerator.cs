// ReSharper disable ArrangeObjectCreationWhenTypeNotEvident
// ReSharper disable ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
namespace Atc.Rest.ApiGenerator.Framework.Minimal.ProjectGenerator;

public class ServerDomainGenerator : IServerDomainGenerator
{
    private readonly ILogger<ServerDomainGenerator> logger;
    private readonly INugetPackageReferenceProvider nugetPackageReferenceProvider;
    private readonly string apiProjectName;
    private readonly OpenApiDocument openApiDocument;
    private readonly string codeGeneratorContentHeader;
    private readonly AttributeParameters codeGeneratorAttribute;
    private readonly GeneratorSettings settings;

    public ServerDomainGenerator(
        ILoggerFactory loggerFactory,
        INugetPackageReferenceProvider nugetPackageReferenceProvider,
        string apiProjectName,
        OpenApiDocument openApiDocument,
        GeneratorSettings generatorSettings)
    {
        ArgumentNullException.ThrowIfNull(loggerFactory);
        ArgumentNullException.ThrowIfNull(nugetPackageReferenceProvider);
        ArgumentNullException.ThrowIfNull(apiProjectName);
        ArgumentNullException.ThrowIfNull(openApiDocument);
        ArgumentNullException.ThrowIfNull(generatorSettings);

        logger = loggerFactory.CreateLogger<ServerDomainGenerator>();
        this.nugetPackageReferenceProvider = nugetPackageReferenceProvider;
        this.apiProjectName = apiProjectName;
        this.openApiDocument = openApiDocument;
        settings = generatorSettings;

        codeGeneratorContentHeader = GeneratedCodeHeaderGeneratorFactory
            .Create(settings.Version)
            .Generate();
        codeGeneratorAttribute = AttributeParametersFactory
            .CreateGeneratedCode(settings.Version);
    }

    public async Task ScaffoldProjectFile()
    {
        var packageReferences = await nugetPackageReferenceProvider.GetPackageReferencesForDomainProjectForMinimalApi();

        var itemGroupPackageReferences = packageReferences!
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
                    new("TargetFramework", Attributes: null, "net9.0"),
                    new("IsPackable", Attributes: null, "false"),
                ],
                [
                    new("GenerateDocumentationFile", Attributes: null, "true"),
                ],
                [
                    new("DocumentationFile", Attributes: null, @$"bin\Debug\net9.0\{settings.ProjectName}.xml"),
                    new("NoWarn", Attributes: null, "$(NoWarn);1573;1591;1701;1702;1712;8618;"),
                ],
            ],
            [
                [
                    new(
                        "FrameworkReference",
                        [
                            new("Include", "Microsoft.AspNetCore.App"),
                        ],
                        Value: null),
                ],
                itemGroupPackageReferences,
                [
                    new(
                        "ProjectReference",
                        [
                            new("Include", @$"..\{apiProjectName}\{apiProjectName}.csproj"),
                        ],
                        Value: null),
                ],
            ]);

        var contentGenerator = new GenerateContentForProjectFile(
            projectFileParameters);

        var content = contentGenerator.Generate();

        var contentWriter = new ContentWriter(logger);
        contentWriter.Write(
            settings.ProjectPath,
            settings.ProjectPath.CombineFileInfo($"{settings.ProjectName}.csproj"),
            ContentWriterArea.Src,
            content,
            overrideIfExist: false);
    }

    public void ScaffoldHandlers()
    {
        foreach (var urlPath in openApiDocument.Paths)
        {
            var apiGroupName = urlPath.GetApiGroupName();

            var handlersLocation = LocationFactory.CreateWithApiGroupName(apiGroupName, settings.HandlersLocation);

            var fullNamespace = NamespaceFactory.Create(settings.ProjectName, handlersLocation);

            foreach (var openApiOperation in urlPath.Value.Operations)
            {
                if (openApiOperation.Value.Deprecated && !settings.IncludeDeprecatedOperations)
                {
                    continue;
                }

                var classParameters = ContentGeneratorServerHandlerParametersFactory.Create(
                    fullNamespace,
                    urlPath.Value,
                    openApiOperation.Value);

                var contentGenerator = new GenerateContentForClass(
                    new CodeDocumentationTagsGenerator(),
                    classParameters);

                var content = contentGenerator.Generate();

                var contentWriter = new ContentWriter(logger);
                contentWriter.Write(
                    settings.ProjectPath,
                    FileInfoFactory.Create(settings.ProjectPath, handlersLocation, $"{classParameters.TypeName}.cs"),
                    ContentWriterArea.Src,
                    content,
                    overrideIfExist: false);
            }
        }
    }

    public void GenerateAssemblyMarker()
    {
        var suppressMessageAvoidEmptyInterfaceAttribute = new AttributeParameters(
            "SuppressMessage",
            "\"Design\", \"CA1040:Avoid empty interfaces\", Justification = \"OK.\"");

        var interfaceParameters = InterfaceParametersFactory.Create(
            codeGeneratorContentHeader,
            settings.ProjectName,
            [suppressMessageAvoidEmptyInterfaceAttribute, codeGeneratorAttribute],
            "IDomainAssemblyMarker");

        var contentGenerator = new GenerateContentForInterface(
            new CodeDocumentationTagsGenerator(),
            interfaceParameters);

        var content = contentGenerator.Generate();

        var contentWriter = new ContentWriter(logger);
        contentWriter.Write(
            settings.ProjectPath,
            settings.ProjectPath.CombineFileInfo("IDomainAssemblyMarker.cs"),
            ContentWriterArea.Src,
            content);
    }

    public void GenerateServiceCollectionEndpointHandlerExtensions()
    {
        var sbServicesHandlerRegistrations = new StringBuilder();
        foreach (var urlPath in openApiDocument.Paths)
        {
            foreach (var openApiOperation in urlPath.Value.Operations)
            {
                var operationName = openApiOperation.Value.GetOperationName();

                sbServicesHandlerRegistrations.AppendLine($"services.AddTransient<I{operationName}{ContentGeneratorConstants.Handler}, {operationName}{ContentGeneratorConstants.Handler}>();");
            }
        }

        sbServicesHandlerRegistrations.AppendLine();
        sbServicesHandlerRegistrations.AppendLine("return services;");

        var methodConfigureDomainServices = new MethodParameters(
            DocumentationTags: null,
            Attributes: null,
            DeclarationModifiers.PublicStatic,
            ReturnGenericTypeName: null,
            ReturnTypeName: "IServiceCollection",
            Name: "ConfigureDomainHandlers",
            Parameters:
            [
                new(
                    Attributes: null,
                    GenericTypeName: null,
                    IsGenericListType: false,
                    TypeName: "this IServiceCollection",
                    IsNullableType: false,
                    IsReferenceType: false,
                    Name: "services",
                    DefaultValue: null),
                new(
                    Attributes: null,
                    GenericTypeName: null,
                    IsGenericListType: false,
                    TypeName: "IConfiguration",
                    IsNullableType: false,
                    IsReferenceType: false,
                    Name: "configuration",
                    DefaultValue: null),
            ],
            AlwaysBreakDownParameters: true,
            UseExpressionBody: false,
            Content: sbServicesHandlerRegistrations.ToString());

        var classParameters = new ClassParameters(
            codeGeneratorContentHeader,
            Namespace: $"{settings.ProjectName}.Extensions",
            DocumentationTags: null,
            Attributes: [codeGeneratorAttribute],
            DeclarationModifiers.PublicStaticClass,
            ClassTypeName: "ServiceCollectionEndpointHandlerExtensions",
            GenericTypeName: null,
            InheritedClassTypeName: null,
            InheritedGenericClassTypeName: null,
            InheritedInterfaceTypeName: null,
            Constructors: null,
            Properties: null,
            Methods:
            [
                methodConfigureDomainServices,
            ],
            GenerateToStringMethod: false);

        var contentGenerator = new GenerateContentForClass(
            new CodeDocumentationTagsGenerator(),
            classParameters);

        var content = contentGenerator.Generate();

        var contentWriter = new ContentWriter(logger);
        contentWriter.Write(
            settings.ProjectPath,
            settings.ProjectPath.CombineFileInfo("Extensions", "ServiceCollectionEndpointHandlerExtensions.cs"),
            ContentWriterArea.Src,
            content);
    }

    public void MaintainGlobalUsings(
        bool removeNamespaceGroupSeparatorInGlobalUsings)
    {
        var requiredUsings = new List<string>
        {
            "System.CodeDom.Compiler",
            "System.Diagnostics.CodeAnalysis",
            "Microsoft.Extensions.Configuration",
            "Microsoft.Extensions.DependencyInjection",
        };

        var apiGroupNames = openApiDocument.GetApiGroupNames();

        requiredUsings.AddRange(apiGroupNames.Select(x => NamespaceFactory.Create(apiProjectName, NamespaceFactory.CreateWithApiGroupName(x, settings.ContractsNamespace))));
        requiredUsings.AddRange(apiGroupNames.Select(x => NamespaceFactory.Create(settings.ProjectName, NamespaceFactory.CreateWithApiGroupName(x, settings.HandlersNamespace))));

        GlobalUsingsHelper.CreateOrUpdate(
            logger,
            ContentWriterArea.Src,
            settings.ProjectPath,
            requiredUsings,
            removeNamespaceGroupSeparatorInGlobalUsings);
    }
}