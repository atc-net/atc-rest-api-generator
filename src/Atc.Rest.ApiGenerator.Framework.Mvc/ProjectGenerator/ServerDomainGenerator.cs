// ReSharper disable ArrangeObjectCreationWhenTypeNotEvident
namespace Atc.Rest.ApiGenerator.Framework.Mvc.ProjectGenerator;

public class ServerDomainGenerator : IServerDomainGenerator
{
    private readonly ILogger<ServerDomainGenerator> logger;
    private readonly string projectName;
    private readonly string apiProjectName;
    private readonly Version apiGeneratorVersion;
    private readonly DirectoryInfo projectPath;
    private readonly OpenApiDocument openApiDocument;

    public ServerDomainGenerator(
        ILoggerFactory loggerFactory,
        Version apiGeneratorVersion,
        string projectName,
        string apiProjectName,
        DirectoryInfo projectPath,
        OpenApiDocument openApiDocument)
    {
        ArgumentNullException.ThrowIfNull(loggerFactory);
        ArgumentNullException.ThrowIfNull(apiGeneratorVersion);
        ArgumentNullException.ThrowIfNull(projectName);
        ArgumentNullException.ThrowIfNull(apiProjectName);
        ArgumentNullException.ThrowIfNull(projectPath);
        ArgumentNullException.ThrowIfNull(openApiDocument);

        logger = loggerFactory.CreateLogger<ServerDomainGenerator>();
        this.apiGeneratorVersion = apiGeneratorVersion;
        this.projectName = projectName;
        this.apiProjectName = apiProjectName;
        this.projectPath = projectPath;
        this.openApiDocument = openApiDocument;
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
        var codeHeaderGenerator = new GeneratedCodeHeaderGenerator(
            new GeneratedCodeGeneratorParameters(
                apiGeneratorVersion));
        var codeGeneratorContentHeader = codeHeaderGenerator.Generate();

        var codeGeneratorAttribute = AttributeParametersFactory.Create(
            "GeneratedCode",
            $"\"{ContentWriterConstants.ApiGeneratorName}\", \"{apiGeneratorVersion}\"");

        var classParameters = ClassParametersFactory.Create(
            codeGeneratorContentHeader,
            projectName,
            codeGeneratorAttribute,
            "DomainRegistration");

        var contentGenerator = new GenerateContentForClass(
            new CodeDocumentationTagsGenerator(),
            classParameters);

        var content = contentGenerator.Generate();

        var contentWriter = new ContentWriter(logger);
        contentWriter.Write(
            projectPath,
            projectPath.CombineFileInfo("DomainRegistration.cs"),
            ContentWriterArea.Src,
            content);
    }

    public void GenerateServiceCollectionExtensions()
        => throw new NotSupportedException($"{nameof(GenerateServiceCollectionExtensions)} is not supported for MVC");

    public void GenerateHandlers()
    {
        foreach (var urlPath in openApiDocument.Paths)
        {
            var apiGroupName = urlPath.GetApiGroupName();

            foreach (var openApiOperation in urlPath.Value.Operations)
            {
                var fullNamespace = $"{projectName}.{ContentGeneratorConstants.Handlers}.{apiGroupName}";

                var classParameters = Factories.Parameters.Server.ContentGeneratorServerHandlerParametersFactory.Create(
                    fullNamespace,
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
        IList<string> apiGroupNames,
        bool removeNamespaceGroupSeparatorInGlobalUsings,
        IList<ApiOperation> operationSchemaMappings)
    {
        ArgumentNullException.ThrowIfNull(apiProjectName);
        ArgumentNullException.ThrowIfNull(apiGroupNames);

        var requiredUsings = new List<string>
        {
            "System.CodeDom.Compiler",
        };

        requiredUsings.AddRange(apiGroupNames.Select(x => $"{apiProjectName}.{ContentGeneratorConstants.Contracts}.{x}"));

        GlobalUsingsHelper.CreateOrUpdate(
            logger,
            ContentWriterArea.Src,
            projectPath,
            requiredUsings,
            removeNamespaceGroupSeparatorInGlobalUsings);
    }
}