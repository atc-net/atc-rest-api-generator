// ReSharper disable ArrangeObjectCreationWhenTypeNotEvident
namespace Atc.Rest.ApiGenerator.Framework.Mvc.ProjectGenerator;

public class ServerApiGenerator : IServerApiGenerator
{
    private readonly ILogger<ServerApiGenerator> logger;
    private readonly Version apiGeneratorVersion;
    private readonly string projectName;
    private readonly DirectoryInfo projectPath;
    private readonly OpenApiDocument openApiDocument;
    private readonly IList<ApiOperation> operationSchemaMappings;
    private readonly string routeBase;

    public ServerApiGenerator(
        ILoggerFactory loggerFactory,
        Version apiGeneratorVersion,
        string projectName,
        DirectoryInfo projectPath,
        OpenApiDocument openApiDocument,
        IList<ApiOperation> operationSchemaMappings,
        string routeBase)
    {
        ArgumentNullException.ThrowIfNull(loggerFactory);
        ArgumentNullException.ThrowIfNull(apiGeneratorVersion);
        ArgumentNullException.ThrowIfNull(projectName);
        ArgumentNullException.ThrowIfNull(projectPath);
        ArgumentNullException.ThrowIfNull(openApiDocument);
        ArgumentNullException.ThrowIfNull(operationSchemaMappings);
        ArgumentNullException.ThrowIfNull(routeBase);

        logger = loggerFactory.CreateLogger<ServerApiGenerator>();
        this.apiGeneratorVersion = apiGeneratorVersion;
        this.projectName = projectName;
        this.projectPath = projectPath;
        this.openApiDocument = openApiDocument;
        this.operationSchemaMappings = operationSchemaMappings;
        this.routeBase = routeBase;
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
                },
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
                            new("Include", "Asp.Versioning.Http"),
                            new("Version", "8.1.0"),
                        ],
                        Value: null),
                    new(
                        "PackageReference",
                        [
                            new("Include", "Atc"),
                            new("Version", "2.0.472"),
                        ],
                        Value: null),
                    new(
                        "PackageReference",
                        [
                            new("Include", "Atc.Rest"),
                            new("Version", "2.0.472"),
                        ],
                        Value: null),
                    new(
                        "PackageReference",
                        [
                            new("Include", "FluentValidation.AspNetCore"),
                            new("Version", "11.3.0"),
                        ],
                        Value: null),
                    new(
                        "PackageReference",
                        [
                            new("Include", "Microsoft.AspNetCore.OpenApi"),
                            new("Version", "8.0.4"),
                        ],
                        Value: null),
                    new(
                        "PackageReference",
                        [
                            new("Include", "Microsoft.NETCore.Platforms"),
                            new("Version", "7.0.4"),
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
            "ApiRegistration");

        var contentGenerator = new GenerateContentForClass(
            new CodeDocumentationTagsGenerator(),
            classParameters);

        var content = contentGenerator.Generate();

        var contentWriter = new ContentWriter(logger);
        contentWriter.Write(
            projectPath,
            projectPath.CombineFileInfo("ApiRegistration.cs"),
            ContentWriterArea.Src,
            content);
    }

    public void GenerateModels()
    {
        // TODO: Implement this.
    }

    public void GenerateParameters()
    {
        // TODO: Implement this.
    }

    public void GenerateResults()
    {
        // TODO: Implement this.
    }

    public void GenerateInterfaces()
    {
        // TODO: Implement this.
    }

    public void GenerateEndpoints(
        bool useProblemDetailsAsDefaultBody)
    {
        foreach (var apiGroupName in openApiDocument.GetApiGroupNames())
        {
            var controllerParameters = ContentGeneratorServerControllerParametersFactory.Create(
                operationSchemaMappings,
                projectName,
                useProblemDetailsAsDefaultBody,
                $"{projectName}.{ContentGeneratorConstants.Endpoints}",
                apiGroupName,
                GetRouteByApiGroupName(apiGroupName),
                openApiDocument);

            var contentGenerator = new ContentGenerators.Server.ContentGeneratorServerController(
                new GeneratedCodeHeaderGenerator(new GeneratedCodeGeneratorParameters(apiGeneratorVersion)),
                new GeneratedCodeAttributeGenerator(new GeneratedCodeGeneratorParameters(apiGeneratorVersion)),
                new CodeDocumentationTagsGenerator(),
                controllerParameters);

            var content = contentGenerator.Generate();

            var contentWriter = new ContentWriter(logger);
            contentWriter.Write(
                projectPath,
                projectPath.CombineFileInfo(ContentGeneratorConstants.Endpoints, $"{controllerParameters.EndpointName}.cs"),
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
        bool removeNamespaceGroupSeparatorInGlobalUsings,
        bool useProblemDetailsAsDefaultBody)
    {
        var requiredUsings = new List<string>
        {
            "System.CodeDom.Compiler",
            "System.ComponentModel.DataAnnotations",
            "Microsoft.AspNetCore.Authorization",
            "Microsoft.AspNetCore.Http",
            "Microsoft.AspNetCore.Mvc",
            "Atc.Rest.Results",
        };

        if (openApiDocument.IsUsingRequiredForSystemNet(useProblemDetailsAsDefaultBody))
        {
            requiredUsings.Add("System.Net");
        }

        if (operationSchemaMappings.Any(apiOperation => apiOperation.Model.IsShared))
        {
            requiredUsings.Add($"{projectName}.Contracts");
        }

        var apiGroupNames = openApiDocument.GetApiGroupNames();

        requiredUsings.AddRange(apiGroupNames.Select(x => $"{projectName}.Contracts.{x}"));

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
}