namespace Atc.Rest.ApiGenerator.Framework.Mvc.ProjectGenerator;

public class ServerDomainGenerator : IServerDomainGenerator
{
    private readonly ILogger<ServerDomainGenerator> logger;
    private readonly string projectName;
    private readonly Version apiGeneratorVersion;
    private readonly DirectoryInfo projectPath;
    private readonly OpenApiDocument openApiDocument;

    public ServerDomainGenerator(
        ILoggerFactory loggerFactory,
        Version apiGeneratorVersion,
        string projectName,
        DirectoryInfo projectPath,
        OpenApiDocument openApiDocument)
    {
        ArgumentNullException.ThrowIfNull(loggerFactory);
        ArgumentNullException.ThrowIfNull(apiGeneratorVersion);
        ArgumentNullException.ThrowIfNull(projectName);
        ArgumentNullException.ThrowIfNull(projectPath);
        ArgumentNullException.ThrowIfNull(openApiDocument);

        logger = loggerFactory.CreateLogger<ServerDomainGenerator>();
        this.apiGeneratorVersion = apiGeneratorVersion;
        this.projectName = projectName;
        this.projectPath = projectPath;
        this.openApiDocument = openApiDocument;
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

        var file = new FileInfo(Path.Combine(
            projectPath.FullName,
            "DomainRegistration.cs"));

        var contentWriter = new ContentWriter(logger);
        contentWriter.Write(
            projectPath,
            file,
            ContentWriterArea.Src,
            content);
    }

    public void GenerateServiceCollectionExtensions()
        => throw new NotSupportedException($"{nameof(GenerateServiceCollectionExtensions)} is not supported for MVC");

    public void MaintainGlobalUsings(
        string apiProjectName,
        IList<string> apiGroupNames,
        bool removeNamespaceGroupSeparatorInGlobalUsings)
    {
        ArgumentNullException.ThrowIfNull(apiProjectName);
        ArgumentNullException.ThrowIfNull(apiGroupNames);

        var requiredUsings = new List<string>
        {
            "System.CodeDom.Compiler",
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