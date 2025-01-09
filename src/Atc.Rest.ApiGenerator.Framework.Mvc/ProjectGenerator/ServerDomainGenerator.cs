// ReSharper disable ArrangeObjectCreationWhenTypeNotEvident
namespace Atc.Rest.ApiGenerator.Framework.Mvc.ProjectGenerator;

public class ServerDomainGenerator : IServerDomainGenerator
{
    private readonly ILogger<ServerDomainGenerator> logger;
    private readonly string apiProjectName;
    private readonly OpenApiDocument openApiDocument;
    private readonly string codeGeneratorContentHeader;
    private readonly AttributeParameters codeGeneratorAttribute;
    private readonly GeneratorSettings settings;

    public ServerDomainGenerator(
        ILoggerFactory loggerFactory,
        string apiProjectName,
        OpenApiDocument openApiDocument,
        GeneratorSettings generatorSettings)
    {
        ArgumentNullException.ThrowIfNull(loggerFactory);
        ArgumentNullException.ThrowIfNull(apiProjectName);
        ArgumentNullException.ThrowIfNull(openApiDocument);
        ArgumentNullException.ThrowIfNull(generatorSettings);

        logger = loggerFactory.CreateLogger<ServerDomainGenerator>();
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
        await Task.CompletedTask;

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
                        "FrameworkReference",
                        [
                            new("Include", "Microsoft.AspNetCore.App"),
                        ],
                        Value: null),
                ],
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

            foreach (var openApiOperation in urlPath.Value.Operations)
            {
                var fullNamespace = NamespaceFactory.CreateFull(settings.ProjectName, settings.HandlersLocation, apiGroupName);

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
                    FileInfoFactory.Create(settings.ProjectPath, settings.HandlersLocation, apiGroupName, $"{classParameters.TypeName}.cs"),
                    ContentWriterArea.Src,
                    content,
                    overrideIfExist: false);
            }
        }
    }

    public void GenerateAssemblyMarker()
    {
        var classParameters = ClassParametersFactory.Create(
            codeGeneratorContentHeader,
            settings.ProjectName,
            codeGeneratorAttribute,
            "DomainRegistration");

        var contentGenerator = new GenerateContentForClass(
            new CodeDocumentationTagsGenerator(),
            classParameters);

        var content = contentGenerator.Generate();

        var contentWriter = new ContentWriter(logger);
        contentWriter.Write(
            settings.ProjectPath,
            settings.ProjectPath.CombineFileInfo("DomainRegistration.cs"),
            ContentWriterArea.Src,
            content);
    }

    public void GenerateServiceCollectionEndpointHandlerExtensions()
        => throw new NotSupportedException($"{nameof(GenerateServiceCollectionEndpointHandlerExtensions)} is not supported for MVC");

    public void MaintainGlobalUsings(
        bool removeNamespaceGroupSeparatorInGlobalUsings)
    {
        var requiredUsings = new List<string>
        {
            "System.CodeDom.Compiler",
        };

        var apiGroupNames = openApiDocument.GetApiGroupNames();

        requiredUsings.AddRange(apiGroupNames.Select(x => NamespaceFactory.CreateFull(apiProjectName, settings.ContractsLocation, x)));

        GlobalUsingsHelper.CreateOrUpdate(
            logger,
            ContentWriterArea.Src,
            settings.ProjectPath,
            requiredUsings,
            removeNamespaceGroupSeparatorInGlobalUsings);
    }
}