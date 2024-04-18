// ReSharper disable ArrangeObjectCreationWhenTypeNotEvident
// ReSharper disable ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
namespace Atc.Rest.ApiGenerator.Framework.Minimal.ProjectGenerator;

public class ServerDomainGenerator : IServerDomainGenerator
{
    private readonly ILogger<ServerDomainGenerator> logger;
    private readonly Version apiGeneratorVersion;
    private readonly string projectName;
    private readonly DirectoryInfo projectPath;

    public ServerDomainGenerator(
        ILoggerFactory loggerFactory,
        Version apiGeneratorVersion,
        string projectName,
        DirectoryInfo projectPath)
    {
        ArgumentNullException.ThrowIfNull(loggerFactory);
        ArgumentNullException.ThrowIfNull(apiGeneratorVersion);
        ArgumentNullException.ThrowIfNull(projectName);
        ArgumentNullException.ThrowIfNull(projectPath);

        logger = loggerFactory.CreateLogger<ServerDomainGenerator>();
        this.apiGeneratorVersion = apiGeneratorVersion;
        this.projectName = projectName;
        this.projectPath = projectPath;
    }

    public void GenerateAssemblyMarker()
    {
        var codeHeaderGenerator = new GeneratedCodeHeaderGenerator(
            new GeneratedCodeGeneratorParameters(
                apiGeneratorVersion));
        var codeGeneratorContentHeader = codeHeaderGenerator.Generate();

        var codeGeneratorAttribute = new AttributeParameters(
            "GeneratedCode",
            $"\"{ContentWriterConstants.ApiGeneratorName}\", \"{apiGeneratorVersion}\"");

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

        var file = new FileInfo(
            Path.Combine(
                projectPath.FullName,
                "IDomainAssemblyMarker.cs"));

        var contentWriter = new ContentWriter(logger);
        contentWriter.Write(
            projectPath,
            file,
            ContentWriterArea.Src,
            content);
    }

    public void GenerateServiceCollectionExtensions(
        OpenApiDocument openApiDocument)
    {
        ArgumentNullException.ThrowIfNull(openApiDocument);

        var codeHeaderGenerator = new GeneratedCodeHeaderGenerator(
            new GeneratedCodeGeneratorParameters(
                apiGeneratorVersion));

        var codeGeneratorContentHeader = codeHeaderGenerator.Generate();

        var attributes = AttributesParametersFactory.Create(
            "GeneratedCode",
            $"\"{ContentWriterConstants.ApiGeneratorName}\", \"{apiGeneratorVersion}\"");

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
            Attributes: attributes,
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

        var file = new FileInfo(
            Path.Combine(
                Path.Combine(projectPath.FullName, "Extensions"),
                "ServiceCollectionExtensions.cs"));

        var contentWriter = new ContentWriter(logger);
        contentWriter.Write(
            projectPath,
            file,
            ContentWriterArea.Src,
            content);
    }

    public void MaintainGlobalUsings(
        string apiProjectName,
        IList<string> apiGroupNames,
        bool removeNamespaceGroupSeparatorInGlobalUsings)
    {
        ArgumentNullException.ThrowIfNull(projectName);
        ArgumentNullException.ThrowIfNull(apiGroupNames);

        var requiredUsings = new List<string>
        {
            "System.CodeDom.Compiler",
            "Atc.Rest.Results",
            "Microsoft.AspNetCore.Http.HttpResults",
        };

        requiredUsings.AddRange(apiGroupNames.Select(x => $"{apiProjectName}.Contracts.{x}"));

        GlobalUsingsHelper.CreateOrUpdate(
            logger,
            ContentWriterArea.Src,
            projectPath,
            requiredUsings,
            removeNamespaceGroupSeparatorInGlobalUsings);
    }
}