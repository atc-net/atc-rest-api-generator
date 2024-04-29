namespace Atc.Rest.ApiGenerator.Framework.Minimal.ProjectGenerator;

public class ServerApiGenerator : IServerApiGenerator
{
    private readonly ILogger<ServerApiGenerator> logger;
    private readonly Version apiGeneratorVersion;
    private readonly string projectName;
    private readonly DirectoryInfo projectPath;
    private readonly OpenApiDocument openApiDocument;

    public ServerApiGenerator(
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

        logger = loggerFactory.CreateLogger<ServerApiGenerator>();
        this.apiGeneratorVersion = apiGeneratorVersion;
        this.projectName = projectName;
        this.projectPath = projectPath;
        this.openApiDocument = openApiDocument;
    }

    public void GenerateAssemblyMarker()
    {
        ArgumentNullException.ThrowIfNull(apiGeneratorVersion);

        var codeHeaderGenerator = new GeneratedCodeHeaderGenerator(
            new GeneratedCodeGeneratorParameters(
                apiGeneratorVersion));
        var codeGeneratorContentHeader = codeHeaderGenerator.Generate();

        var codeGeneratorAttribute = AttributeParametersFactory.Create(
            "GeneratedCode",
            $"\"{ContentWriterConstants.ApiGeneratorName}\", \"{apiGeneratorVersion}\"");

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

    public void MaintainGlobalUsings(
        IList<string> apiGroupNames,
        bool removeNamespaceGroupSeparatorInGlobalUsings,
        IList<ApiOperation> operationSchemaMappings,
        bool useProblemDetailsAsDefaultBody)
    {
        ArgumentNullException.ThrowIfNull(apiGroupNames);
        ArgumentNullException.ThrowIfNull(operationSchemaMappings);

        var requiredUsings = new List<string>
        {
            "System.CodeDom.Compiler",
            "System.ComponentModel.DataAnnotations",
            "System.Diagnostics.CodeAnalysis",
            "Atc.Rest.Results",
            "Atc.Rest.MinimalApi.Abstractions",
            "Microsoft.AspNetCore.Builder",
            "Microsoft.AspNetCore.Http",
            "Microsoft.AspNetCore.Http.HttpResults",
            "Microsoft.AspNetCore.Mvc",
        };

        if (openApiDocument.IsUsingRequiredForSystemNet(useProblemDetailsAsDefaultBody))
        {
            requiredUsings.Add("System.Net");
        }

        if (operationSchemaMappings.Any(apiOperation => apiOperation.Model.IsShared))
        {
            requiredUsings.Add($"{projectName}.{ContentGeneratorConstants.Contracts}");
        }

        requiredUsings.AddRange(apiGroupNames.Select(x => $"{projectName}.{ContentGeneratorConstants.Contracts}.{x}"));

        GlobalUsingsHelper.CreateOrUpdate(
            logger,
            ContentWriterArea.Src,
            projectPath,
            requiredUsings,
            removeNamespaceGroupSeparatorInGlobalUsings);
    }
}