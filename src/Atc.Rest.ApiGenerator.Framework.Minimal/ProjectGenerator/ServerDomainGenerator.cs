// ReSharper disable ArrangeObjectCreationWhenTypeNotEvident
// ReSharper disable ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
namespace Atc.Rest.ApiGenerator.Framework.Minimal.ProjectGenerator;

public class ServerDomainGenerator : IServerDomainGenerator
{
    private readonly ILogger<ServerDomainGenerator> logger;
    private readonly string projectName;
    private readonly string apiProjectName;
    private readonly DirectoryInfo projectPath;
    private readonly OpenApiDocument openApiDocument;
    private readonly IList<ApiOperation> operationSchemaMappings;
    private readonly string codeGeneratorContentHeader;
    private readonly AttributeParameters codeGeneratorAttribute;

    public ServerDomainGenerator(
        ILoggerFactory loggerFactory,
        Version apiGeneratorVersion,
        string projectName,
        string apiProjectName,
        DirectoryInfo projectPath,
        OpenApiDocument openApiDocument,
        IList<ApiOperation> operationSchemaMappings)
    {
        ArgumentNullException.ThrowIfNull(loggerFactory);
        ArgumentNullException.ThrowIfNull(apiGeneratorVersion);
        ArgumentNullException.ThrowIfNull(projectName);
        ArgumentNullException.ThrowIfNull(apiProjectName);
        ArgumentNullException.ThrowIfNull(projectPath);
        ArgumentNullException.ThrowIfNull(openApiDocument);
        ArgumentNullException.ThrowIfNull(operationSchemaMappings);

        logger = loggerFactory.CreateLogger<ServerDomainGenerator>();
        this.projectName = projectName;
        this.apiProjectName = apiProjectName;
        this.projectPath = projectPath;
        this.openApiDocument = openApiDocument;
        this.operationSchemaMappings = operationSchemaMappings;

        codeGeneratorContentHeader = GeneratedCodeHeaderGeneratorFactory
            .Create(apiGeneratorVersion)
            .Generate();
        codeGeneratorAttribute = AttributeParametersFactory
            .CreateGeneratedCode(apiGeneratorVersion);
    }

    public void ScaffoldProjectFile()
    {
        var projectFileParameters = new ProjectFileParameters(
            "Microsoft.NET.Sdk",
            [
                new List<PropertyGroupParameter>
                {
                    new("TargetFramework", Attributes: null, "net8.0"),
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
                        "FrameworkReference",
                        [
                            new("Include", "Microsoft.AspNetCore.App"),
                        ],
                        Value: null),
                },
                new List<ItemGroupParameter>
                {
                    new(
                        "PackageReference",
                        [
                            new("Include", "Atc.Azure.Options"),
                            new("Version", "3.0.31"),
                        ],
                        Value: null),
                    new(
                        "PackageReference",
                        [
                            new("Include", "Atc.Rest"),
                            new("Version", "2.0.472"),
                        ],
                        Value: null),
                },
                new List<ItemGroupParameter>
                {
                    new(
                        "ProjectReference",
                        [
                            new("Include", @$"..\{apiProjectName}\{apiProjectName}.csproj"),
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

    public void GenerateAssemblyMarker()
    {
        var suppressMessageAvoidEmptyInterfaceAttribute = new AttributeParameters(
            "SuppressMessage",
            "\"Design\", \"CA1040:Avoid empty interfaces\", Justification = \"OK.\"");

        var interfaceParameters = InterfaceParametersFactory.Create(
            codeGeneratorContentHeader,
            projectName,
            [suppressMessageAvoidEmptyInterfaceAttribute, codeGeneratorAttribute],
            "IDomainAssemblyMarker");

        var contentGenerator = new GenerateContentForInterface(
            new CodeDocumentationTagsGenerator(),
            interfaceParameters);

        var content = contentGenerator.Generate();

        var contentWriter = new ContentWriter(logger);
        contentWriter.Write(
            projectPath,
            projectPath.CombineFileInfo("IDomainAssemblyMarker.cs"),
            ContentWriterArea.Src,
            content);
    }

    public void GenerateServiceCollectionExtensions()
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

        var methodConfigureDomainServices = new MethodParameters(
            DocumentationTags: null,
            Attributes: null,
            AccessModifiers.PublicStatic,
            ReturnGenericTypeName: null,
            ReturnTypeName: "IServiceCollection",
            Name: "ConfigureDomainServices",
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
            Content: """
                     services.ConfigureOptions(configuration);
                     services.DefineHandlersAndServices();
                     return services;
                     """);

        var methodConfigureOptions = new MethodParameters(
            DocumentationTags: null,
            Attributes: null,
            AccessModifiers.PublicStatic,
            ReturnGenericTypeName: null,
            ReturnTypeName: "void",
            Name: "ConfigureOptions",
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
            Content: """
                     services.Configure<ServiceOptions>(options => configuration.GetRequiredSection(nameof(ServiceOptions)).Bind(options));
                     services.Configure<EnvironmentOptions>(options => configuration.GetRequiredSection(nameof(EnvironmentOptions)).Bind(options));
                     services.Configure<NamingOptions>(options => configuration.GetRequiredSection(nameof(NamingOptions)).Bind(options));
                     """);

        var methodDefineHandlersAndServices = new MethodParameters(
            DocumentationTags: null,
            Attributes: null,
            AccessModifiers.PublicStatic,
            ReturnGenericTypeName: null,
            ReturnTypeName: "void",
            Name: "DefineHandlersAndServices",
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
            ],
            AlwaysBreakDownParameters: true,
            UseExpressionBody: false,
            Content: sbServicesHandlerRegistrations.ToString());

        var classParameters = new ClassParameters(
            codeGeneratorContentHeader,
            Namespace: projectName,
            DocumentationTags: null,
            Attributes: [codeGeneratorAttribute],
            AccessModifiers.PublicStaticClass,
            ClassTypeName: "ServiceCollectionExtensions",
            GenericTypeName: null,
            InheritedClassTypeName: null,
            InheritedGenericClassTypeName: null,
            InheritedInterfaceTypeName: null,
            Constructors: null,
            Properties: null,
            Methods:
            [
                methodConfigureDomainServices,
                methodConfigureOptions,
                methodDefineHandlersAndServices,
            ],
            GenerateToStringMethod: false);

        var contentGenerator = new GenerateContentForClass(
            new CodeDocumentationTagsGenerator(),
            classParameters);

        var content = contentGenerator.Generate();

        var contentWriter = new ContentWriter(logger);
        contentWriter.Write(
            projectPath,
            projectPath.CombineFileInfo("Extensions", "ServiceCollectionExtensions.cs"),
            ContentWriterArea.Src,
            content);
    }

    public void GenerateHandlers()
    {
        foreach (var urlPath in openApiDocument.Paths)
        {
            var apiGroupName = urlPath.GetApiGroupName();

            foreach (var openApiOperation in urlPath.Value.Operations)
            {
                var fullNamespace = $"{projectName}.{ContentGeneratorConstants.Handlers}.{apiGroupName}";

                var classParameters = ContentGeneratorServerHandlerParametersFactory.Create(
                    fullNamespace,
                    $"Api.Generated.{ContentGeneratorConstants.Contracts}.{apiGroupName}",
                    urlPath.Value,
                    openApiOperation.Value);

                var contentGenerator = new GenerateContentForClass(
                    new CodeDocumentationTagsGenerator(),
                    classParameters);

                var content = contentGenerator.Generate();

                var contentWriter = new ContentWriter(logger);
                contentWriter.Write(
                    projectPath,
                    projectPath.CombineFileInfo(
                        ContentGeneratorConstants.Handlers,
                        apiGroupName,
                        $"{classParameters.TypeName}.cs"),
                    ContentWriterArea.Src,
                    content,
                    overrideIfExist: false);
            }
        }
    }

    public void MaintainGlobalUsings(
        bool removeNamespaceGroupSeparatorInGlobalUsings)
    {
        var requiredUsings = new List<string>
        {
            "System.CodeDom.Compiler",
            "System.Diagnostics.CodeAnalysis",
            "Atc.Azure.Options",
            "Atc.Azure.Options.Environment",
            "Microsoft.AspNetCore.Http.HttpResults",
            "Microsoft.Extensions.Configuration",
            "Microsoft.Extensions.DependencyInjection",
        };

        if (openApiDocument.IsUsingRequiredForAtcRestResults())
        {
            requiredUsings.Add("Atc.Rest.Results");
        }

        if (operationSchemaMappings.Any(apiOperation => apiOperation.Model.IsShared))
        {
            requiredUsings.Add($"{apiProjectName}.{ContentGeneratorConstants.Contracts}");
        }

        var apiGroupNames = openApiDocument.GetApiGroupNames();

        requiredUsings.AddRange(apiGroupNames.Select(x => $"{apiProjectName}.{ContentGeneratorConstants.Contracts}.{x}"));
        requiredUsings.AddRange(apiGroupNames.Select(x => $"{projectName}.{ContentGeneratorConstants.Handlers}.{x}"));

        GlobalUsingsHelper.CreateOrUpdate(
            logger,
            ContentWriterArea.Src,
            projectPath,
            requiredUsings,
            removeNamespaceGroupSeparatorInGlobalUsings);
    }
}