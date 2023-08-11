// ReSharper disable ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
// ReSharper disable SuggestBaseTypeForParameter
// ReSharper disable ReplaceSubstringWithRangeIndexer
// ReSharper disable ReturnTypeCanBeEnumerable.Local
// ReSharper disable UseObjectOrCollectionInitializer
namespace Atc.Rest.ApiGenerator.Generators;

public class ServerApiGenerator
{
    private readonly ILogger logger;
    private readonly IApiOperationExtractor apiOperationExtractor;
    private readonly INugetPackageReferenceProvider nugetPackageReferenceProvider;
    private readonly ApiProjectOptions projectOptions;

    private readonly string codeGeneratorContentHeader;
    private readonly AttributeParameters codeGeneratorAttribute;

    public ServerApiGenerator(
        ILogger logger,
        IApiOperationExtractor apiOperationExtractor,
        INugetPackageReferenceProvider nugetPackageReferenceProvider,
        ApiProjectOptions projectOptions)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.apiOperationExtractor = apiOperationExtractor ?? throw new ArgumentNullException(nameof(apiOperationExtractor));
        this.nugetPackageReferenceProvider = nugetPackageReferenceProvider ?? throw new ArgumentNullException(nameof(nugetPackageReferenceProvider));
        this.projectOptions = projectOptions ?? throw new ArgumentNullException(nameof(projectOptions));

        // TODO: Optimize codeGeneratorContentHeader & codeGeneratorAttribute
        var codeHeaderGenerator = new GeneratedCodeHeaderGenerator(
            new GeneratedCodeGeneratorParameters(
                projectOptions.ApiGeneratorVersion));
        codeGeneratorContentHeader = codeHeaderGenerator.Generate();

        codeGeneratorAttribute = new AttributeParameters(
            "GeneratedCode",
            $"\"{ContentWriterConstants.ApiGeneratorName}\", \"{projectOptions.ApiGeneratorVersion}\"");
    }

    public bool Generate()
    {
        logger.LogInformation($"{ContentWriterConstants.AreaGenerateCode} Working on server api generation ({projectOptions.ProjectName})");

        var isVersionValid = ValidateVersioning();
        if (!isVersionValid)
        {
            return false;
        }

        ScaffoldSrc();

        CopyApiSpecification();

        var operationSchemaMappings = apiOperationExtractor.Extract(projectOptions.Document);

        GenerateModels(projectOptions.Document, operationSchemaMappings);
        GenerateParameters(projectOptions.Document);
        GenerateResults(projectOptions.Document);
        GenerateInterfaces(projectOptions.Document);

        GenerateEndpoints(operationSchemaMappings);
        GenerateSrcGlobalUsings(projectOptions.RemoveNamespaceGroupSeparatorInGlobalUsings);

        return true;
    }

    private bool ValidateVersioning()
    {
        if (!Directory.Exists(projectOptions.PathForSrcGenerate.FullName))
        {
            logger.LogInformation($"     {ValidationRuleNameConstants.ProjectApiGenerated01} - Old project does not exist");
            return true;
        }

        var apiGeneratedFile = Path.Combine(projectOptions.PathForSrcGenerate.FullName, "ApiRegistration.cs");
        if (!File.Exists(apiGeneratedFile))
        {
            logger.LogInformation($"     {ValidationRuleNameConstants.ProjectApiGenerated02} - Old ApiRegistration.cs in project does not exist.");
            return true;
        }

        var lines = File.ReadLines(apiGeneratedFile).ToList();

        Version? newVersion = null;
        TaskHelper.RunSync(async () =>
        {
            newVersion = await nugetPackageReferenceProvider.GetAtcApiGeneratorVersion();
        });

        foreach (var line in lines)
        {
            var indexOfApiGeneratorName = line.IndexOf(projectOptions.ApiGeneratorName, StringComparison.Ordinal);
            if (indexOfApiGeneratorName == -1)
            {
                continue;
            }

            var oldVersion = line.Substring(indexOfApiGeneratorName + projectOptions.ApiGeneratorName.Length);
            if (oldVersion.EndsWith('.'))
            {
                oldVersion = oldVersion.Substring(0, oldVersion.Length - 1);
            }

            if (!Version.TryParse(oldVersion, out var oldVersionResult))
            {
                logger.LogError($"     {ValidationRuleNameConstants.ProjectApiGenerated03} - Existing project version is invalid.");
                return false;
            }

            if (newVersion >= oldVersionResult)
            {
                logger.LogInformation($"     {ValidationRuleNameConstants.ProjectApiGenerated04} - The generate project version is the same or newer.");
                return true;
            }

            logger.LogError($"     {ValidationRuleNameConstants.ProjectApiGenerated05} - Existing project version is never than this tool version.");
            return false;
        }

        logger.LogError($"     {ValidationRuleNameConstants.ProjectApiGenerated06} - Existing project did not contain a version.");
        return false;
    }

    private void ScaffoldSrc()
    {
        if (!Directory.Exists(projectOptions.PathForSrcGenerate.FullName))
        {
            Directory.CreateDirectory(projectOptions.PathForSrcGenerate.FullName);
        }

        if (projectOptions.PathForSrcGenerate.Exists &&
            projectOptions.ProjectSrcCsProj.Exists)
        {
            var hasUpdates = SolutionAndProjectHelper.EnsureLatestPackageReferencesVersionInProjFile(
                logger,
                projectOptions.ProjectSrcCsProj,
                projectOptions.ProjectSrcCsProjDisplayLocation,
                ProjectType.ServerApi,
                isTestProject: false);
            if (!hasUpdates)
            {
                logger.LogDebug($"{EmojisConstants.FileNotUpdated}   No updates for csproj");
            }
        }
        else
        {
            IList<(string PackageId, string PackageVersion, string? SubElements)>? packageReferencesBaseLineForApiProject = null;
            TaskHelper.RunSync(async () =>
            {
                packageReferencesBaseLineForApiProject = await nugetPackageReferenceProvider.GetPackageReferencesBaseLineForApiProject();
            });

            SolutionAndProjectHelper.ScaffoldProjFile(
                logger,
                projectOptions.ProjectSrcCsProj,
                projectOptions.ProjectSrcCsProjDisplayLocation,
                ProjectType.ServerApi,
                createAsWeb: false,
                createAsTestProject: false,
                projectOptions.ProjectName,
                "net6.0",
                new List<string> { "Microsoft.AspNetCore.App" },
                packageReferencesBaseLineForApiProject,
                projectReferences: null,
                includeApiSpecification: true,
                usingCodingRules: projectOptions.UsingCodingRules);

            ScaffoldBasicFileApiGenerated();
        }
    }

    private void CopyApiSpecification()
    {
        var resourceFolder = new DirectoryInfo(Path.Combine(projectOptions.PathForSrcGenerate.FullName, "Resources"));
        if (!resourceFolder.Exists)
        {
            Directory.CreateDirectory(resourceFolder.FullName);
        }

        var resourceFile = new FileInfo(Path.Combine(resourceFolder.FullName, "ApiSpecification.yaml"));
        if (File.Exists(resourceFile.FullName))
        {
            File.Delete(resourceFile.FullName);
        }

        if (projectOptions.DocumentFile.Extension.Equals(".json", StringComparison.OrdinalIgnoreCase))
        {
            using var writeFile = new StreamWriter(resourceFile.FullName);
            projectOptions.Document.SerializeAsV3(new OpenApiYamlWriter(writeFile));
        }
        else
        {
            File.Copy(projectOptions.DocumentFile.FullName, resourceFile.FullName);
        }
    }

    private void GenerateModels(
        OpenApiDocument document,
        IList<ApiOperation> operationSchemaMappings)
    {
        ArgumentNullException.ThrowIfNull(operationSchemaMappings);

        foreach (var apiGroupName in projectOptions.ApiGroupNames)
        {
            var apiOperations = operationSchemaMappings
                .Where(x => x.LocatedArea is ApiSchemaMapLocatedAreaType.Response or ApiSchemaMapLocatedAreaType.RequestBody &&
                            x.ApiGroupName.Equals(apiGroupName, StringComparison.OrdinalIgnoreCase))
                .ToList();

            var apiOperationModels = GetDistinctApiOperationModels(apiOperations);

            foreach (var apiOperationModel in apiOperationModels)
            {
                var apiSchema = document.Components.Schemas.First(x => x.Key.Equals(apiOperationModel.Name, StringComparison.OrdinalIgnoreCase));

                var modelName = apiSchema.Key.EnsureFirstCharacterToUpper();

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

    private void GenerateEnumerationType(
        string enumerationName,
        OpenApiSchema openApiSchemaEnumeration)
    {
        var fullNamespace = $"{projectOptions.ProjectName}.{ContentGeneratorConstants.Contracts}";

        // Generate
        var enumParameters = ContentGeneratorServerClientEnumParametersFactory.Create(
            codeGeneratorContentHeader,
            fullNamespace,
            codeGeneratorAttribute,
            enumerationName,
            openApiSchemaEnumeration.Enum);

        var contentGeneratorEnum = new GenerateContentForEnum(
            new CodeDocumentationTagsGenerator(),
            enumParameters);

        var enumContent = contentGeneratorEnum.Generate();

        // Write
        var file = new FileInfo(
            Helpers.DirectoryInfoHelper.GetCsFileNameForContractEnumTypes(
                projectOptions.PathForContracts,
                enumerationName));

        var contentWriter = new ContentWriter(logger);
        contentWriter.Write(
            projectOptions.PathForSrcGenerate,
            file,
            ContentWriterArea.Src,
            enumContent);
    }

    private void GenerateModel(
        string modelName,
        OpenApiSchema apiSchemaModel,
        string apiGroupName,
        bool isSharedContract)
    {
        var fullNamespace = isSharedContract
            ? $"{projectOptions.ProjectName}.{ContentGeneratorConstants.Contracts}"
            : $"{projectOptions.ProjectName}.{ContentGeneratorConstants.Contracts}.{apiGroupName}";

        string? content = string.Empty;
        if (projectOptions.AspNetOutputType == AspNetOutputType.Mvc)
        {
            var parameters = ContentGeneratorServerClientModelParametersFactory.CreateForClass(
                codeGeneratorContentHeader,
                fullNamespace,
                codeGeneratorAttribute,
                modelName,
                apiSchemaModel);

            var contentGeneratorClass = new GenerateContentForClass(
                new CodeDocumentationTagsGenerator(),
                parameters);

            content = contentGeneratorClass.Generate();
        }
        else
        {
            var parameters = ContentGeneratorServerClientModelParametersFactory.CreateForRecord(
                codeGeneratorContentHeader,
                fullNamespace,
                codeGeneratorAttribute,
                modelName,
                apiSchemaModel);

            var contentGeneratorRecord = new GenerateContentForRecords(
                new CodeDocumentationTagsGenerator(),
                parameters);

            content = contentGeneratorRecord.Generate();
        }

        // Write
        FileInfo file;
        if (isSharedContract)
        {
            file = new FileInfo(
                Helpers.DirectoryInfoHelper.GetCsFileNameForContractShared(
                    projectOptions.PathForContractsShared,
                    modelName));
        }
        else
        {
            file = new FileInfo(
                Helpers.DirectoryInfoHelper.GetCsFileNameForContract(
                    projectOptions.PathForContracts,
                    apiGroupName,
                    ContentGeneratorConstants.Models,
                    modelName));
        }

        var contentWriter = new ContentWriter(logger);
        contentWriter.Write(
            projectOptions.PathForSrcGenerate,
            file,
            ContentWriterArea.Src,
            content);
    }

    private void GenerateParameters(
        OpenApiDocument document)
    {
        foreach (var openApiPath in document.Paths)
        {
            var apiGroupName = openApiPath.GetApiGroupName();

            var fullNamespace = $"{projectOptions.ProjectName}.{ContentGeneratorConstants.Contracts}.{apiGroupName}";

            foreach (var openApiOperation in openApiPath.Value.Operations)
            {
                if (!openApiPath.Value.HasParameters() && !openApiOperation.Value.HasParametersOrRequestBody())
                {
                    continue;
                }

                // Generate
                var parameterParameters = ContentGeneratorServerParameterParametersFactory.Create(
                    fullNamespace,
                    openApiOperation.Value,
                    openApiPath.Value.Parameters);

                var contentGeneratorParameter = new ContentGeneratorServerParameter(
                    new GeneratedCodeHeaderGenerator(new GeneratedCodeGeneratorParameters(projectOptions.ApiGeneratorVersion)),
                    new GeneratedCodeAttributeGenerator(new GeneratedCodeGeneratorParameters(projectOptions.ApiGeneratorVersion)),
                    new CodeDocumentationTagsGenerator(),
                    parameterParameters);

                var parameterContent = contentGeneratorParameter.Generate();

                // Write
                var file = new FileInfo(
                    Helpers.DirectoryInfoHelper.GetCsFileNameForContract(
                        projectOptions.PathForContracts,
                        apiGroupName,
                        ContentGeneratorConstants.Parameters,
                        parameterParameters.ParameterName));

                var contentWriter = new ContentWriter(logger);
                contentWriter.Write(
                    projectOptions.PathForSrcGenerate,
                    file,
                    ContentWriterArea.Src,
                    parameterContent);
            }
        }
    }

    private void GenerateResults(
        OpenApiDocument document)
    {
        foreach (var openApiPath in document.Paths)
        {
            var apiGroupName = openApiPath.GetApiGroupName();

            var fullNamespace = $"{projectOptions.ProjectName}.{ContentGeneratorConstants.Contracts}.{apiGroupName}";

            foreach (var openApiOperation in openApiPath.Value.Operations)
            {
                // Generate
                var resultParameters = ContentGeneratorServerResultParametersFactory.Create(
                    fullNamespace,
                    openApiOperation.Value,
                    projectOptions.ApiOptions.Generator.Response.UseProblemDetailsAsDefaultBody);

                var contentGeneratorResult = new ContentGeneratorServerResult(
                    new GeneratedCodeHeaderGenerator(new GeneratedCodeGeneratorParameters(projectOptions.ApiGeneratorVersion)),
                    new GeneratedCodeAttributeGenerator(new GeneratedCodeGeneratorParameters(projectOptions.ApiGeneratorVersion)),
                    new CodeDocumentationTagsGenerator(),
                    resultParameters);

                var resultContent = contentGeneratorResult.Generate();

                // Write
                var file = new FileInfo(
                    Helpers.DirectoryInfoHelper.GetCsFileNameForContract(
                        projectOptions.PathForContracts,
                        apiGroupName,
                        ContentGeneratorConstants.Results,
                        resultParameters.ResultName));

                var contentWriter = new ContentWriter(logger);
                contentWriter.Write(
                    projectOptions.PathForSrcGenerate,
                    file,
                    ContentWriterArea.Src,
                    resultContent);
            }
        }
    }

    private void GenerateInterfaces(
        OpenApiDocument document)
    {
        foreach (var openApiPath in document.Paths)
        {
            var apiGroupName = openApiPath.GetApiGroupName();

            var fullNamespace = $"{projectOptions.ProjectName}.{ContentGeneratorConstants.Contracts}.{apiGroupName}";

            foreach (var openApiOperation in openApiPath.Value.Operations)
            {
                // Generate
                var interfaceParameters = ContentGeneratorServerHandlerInterfaceParametersFactory.Create(
                    codeGeneratorContentHeader,
                    fullNamespace,
                    codeGeneratorAttribute,
                    openApiPath.Value,
                    openApiOperation.Value);

                var contentGeneratorInterface = new GenerateContentForInterface(
                    new CodeDocumentationTagsGenerator(),
                    interfaceParameters);

                var interfaceContent = contentGeneratorInterface.Generate();

                // Write
                var file = new FileInfo(
                    Helpers.DirectoryInfoHelper.GetCsFileNameForContract(
                        projectOptions.PathForContracts,
                        apiGroupName,
                        ContentGeneratorConstants.Interfaces,
                        interfaceParameters.TypeName));

                var contentWriter = new ContentWriter(logger);
                contentWriter.Write(
                    projectOptions.PathForSrcGenerate,
                    file,
                    ContentWriterArea.Src,
                    interfaceContent);
            }
        }
    }

    private void GenerateEndpoints(
        IList<ApiOperation> operationSchemaMappings)
    {
        ArgumentNullException.ThrowIfNull(operationSchemaMappings);

        foreach (var apiGroupName in projectOptions.ApiGroupNames)
        {
            var controllerParameters = ContentGeneratorServerControllerParametersFactory.Create(
                operationSchemaMappings,
                projectOptions.ProjectName,
                projectOptions.ApiOptions.Generator.Response.UseProblemDetailsAsDefaultBody,
                $"{projectOptions.ProjectName}.{ContentGeneratorConstants.Endpoints}",
                apiGroupName,
                GetRouteByApiGroupName(apiGroupName),
                projectOptions.Document);

            var contentGenerator = new ContentGeneratorServerController(
                new GeneratedCodeHeaderGenerator(new GeneratedCodeGeneratorParameters(projectOptions.ApiGeneratorVersion)),
                new GeneratedCodeAttributeGenerator(new GeneratedCodeGeneratorParameters(projectOptions.ApiGeneratorVersion)),
                new CodeDocumentationTagsGenerator(),
                controllerParameters);

            var content = contentGenerator.Generate();

            var file = new FileInfo(
                Helpers.DirectoryInfoHelper.GetCsFileNameForEndpoints(
                    projectOptions.PathForEndpoints,
                    controllerParameters.ControllerName));

            var contentWriter = new ContentWriter(logger);
            contentWriter.Write(
                projectOptions.PathForSrcGenerate,
                file,
                ContentWriterArea.Src,
                content);
        }
    }

    private static List<ApiOperationModel> GetDistinctApiOperationModels(
        List<ApiOperation> apiOperations)
    {
        var result = new List<ApiOperationModel>();

        foreach (var apiOperation in apiOperations)
        {
            var apiOperationModel = result.FirstOrDefault(x => x.Name.Equals(apiOperation.Model.Name, StringComparison.Ordinal));
            if (apiOperationModel is null)
            {
                result.Add(apiOperation.Model);
            }
        }

        return result;
    }

    private string GetRouteByApiGroupName(
        string apiGroupName)
    {
        var (key, _) = projectOptions.Document.Paths.FirstOrDefault(x => x.IsPathStartingSegmentName(apiGroupName));
        if (key is null)
        {
            throw new NotSupportedException($"{nameof(apiGroupName)} was not found in any route.");
        }

        var routeSuffix = key
            .Split('/', StringSplitOptions.RemoveEmptyEntries)
            .FirstOrDefault();

        return $"{projectOptions.RouteBase}/{routeSuffix}";
    }

    private void ScaffoldBasicFileApiGenerated()
    {
        var classParameters = ClassParametersFactory.Create(
            codeGeneratorContentHeader,
            projectOptions.ProjectName,
            codeGeneratorAttribute,
            "ApiRegistration");

        var contentGeneratorClass = new GenerateContentForClass(
            new CodeDocumentationTagsGenerator(),
            classParameters);

        var classContent = contentGeneratorClass.Generate();

        var file = new FileInfo(Path.Combine(projectOptions.PathForSrcGenerate.FullName, "ApiRegistration.cs"));

        var contentWriter = new ContentWriter(logger);
        contentWriter.Write(
            projectOptions.PathForSrcGenerate,
            file,
            ContentWriterArea.Src,
            classContent);
    }

    private void GenerateSrcGlobalUsings(
        bool removeNamespaceGroupSeparatorInGlobalUsings)
    {
        var requiredUsings = new List<string>
        {
            "System.CodeDom.Compiler",
            "System.ComponentModel.DataAnnotations",
            "System.Net",
            "Microsoft.AspNetCore.Authorization",
            "Microsoft.AspNetCore.Http",
            "Microsoft.AspNetCore.Mvc",
            "Atc.Rest.Results",
            $"{projectOptions.ProjectName}.Contracts",
        };

        foreach (var apiGroupName in projectOptions.ApiGroupNames)
        {
            requiredUsings.Add($"{projectOptions.ProjectName}.Contracts.{apiGroupName}");
        }

        GlobalUsingsHelper.CreateOrUpdate(
            logger,
            ContentWriterArea.Src,
            projectOptions.PathForSrcGenerate,
            requiredUsings,
            removeNamespaceGroupSeparatorInGlobalUsings);
    }
}