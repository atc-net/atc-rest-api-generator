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

    private readonly IServerApiGenerator serverApiGeneratorMvc;
    private readonly IServerApiGenerator serverApiGeneratorMinimalApi;

    private readonly string codeGeneratorContentHeader;
    private readonly AttributeParameters codeGeneratorAttribute;

    public ServerApiGenerator(
        ILoggerFactory loggerFactory,
        IApiOperationExtractor apiOperationExtractor,
        INugetPackageReferenceProvider nugetPackageReferenceProvider,
        ApiProjectOptions projectOptions)
    {
        ArgumentNullException.ThrowIfNull(loggerFactory);

        logger = loggerFactory.CreateLogger<ServerDomainGenerator>();
        this.apiOperationExtractor = apiOperationExtractor ?? throw new ArgumentNullException(nameof(apiOperationExtractor));
        this.nugetPackageReferenceProvider = nugetPackageReferenceProvider ?? throw new ArgumentNullException(nameof(nugetPackageReferenceProvider));
        this.projectOptions = projectOptions ?? throw new ArgumentNullException(nameof(projectOptions));

        var operationSchemaMappings = apiOperationExtractor.Extract(projectOptions.Document);

        serverApiGeneratorMvc = new Framework.Mvc.ProjectGenerator.ServerApiGenerator(
            loggerFactory,
            projectOptions.ApiGeneratorVersion,
            projectOptions.ProjectName,
            projectOptions.PathForSrcGenerate,
            projectOptions.Document,
            operationSchemaMappings,
            projectOptions.RouteBase);

        serverApiGeneratorMinimalApi = new Framework.Minimal.ProjectGenerator.ServerApiGenerator(
            loggerFactory,
            projectOptions.ApiGeneratorVersion,
            projectOptions.ProjectName,
            projectOptions.PathForSrcGenerate,
            projectOptions.Document,
            operationSchemaMappings,
            projectOptions.RouteBase);

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

        if (projectOptions.ApiOptions.Generator.AspNetOutputType == AspNetOutputType.Mvc)
        {
            var isVersionValid = ValidateVersioning();
            if (!isVersionValid)
            {
                return false;
            }
        }

        var operationSchemaMappings = apiOperationExtractor.Extract(projectOptions.Document);

        if (projectOptions.ApiOptions.Generator.AspNetOutputType == AspNetOutputType.Mvc)
        {
            serverApiGeneratorMvc.ScaffoldProjectFile();

            serverApiGeneratorMvc.GenerateAssemblyMarker();
            serverApiGeneratorMvc.GenerateModels();
            serverApiGeneratorMvc.GenerateParameters();
            serverApiGeneratorMvc.GenerateResults();
            serverApiGeneratorMvc.GenerateInterfaces();
            serverApiGeneratorMvc.GenerateEndpoints(
                projectOptions.RemoveNamespaceGroupSeparatorInGlobalUsings);

            serverApiGeneratorMvc.MaintainApiSpecification(projectOptions.DocumentFile);
            serverApiGeneratorMvc.MaintainGlobalUsings(
                projectOptions.RemoveNamespaceGroupSeparatorInGlobalUsings,
                projectOptions.ApiOptions.Generator.Response.UseProblemDetailsAsDefaultBody);
        }
        else
        {
            serverApiGeneratorMinimalApi.ScaffoldProjectFile();

            serverApiGeneratorMinimalApi.GenerateAssemblyMarker();
            serverApiGeneratorMinimalApi.GenerateModels();
            serverApiGeneratorMinimalApi.GenerateParameters();
            serverApiGeneratorMinimalApi.GenerateInterfaces();
            serverApiGeneratorMinimalApi.GenerateEndpoints(
                projectOptions.RemoveNamespaceGroupSeparatorInGlobalUsings);

            serverApiGeneratorMinimalApi.MaintainApiSpecification(projectOptions.DocumentFile);
            serverApiGeneratorMinimalApi.MaintainGlobalUsings(
                projectOptions.RemoveNamespaceGroupSeparatorInGlobalUsings,
                useProblemDetailsAsDefaultBody: false);
        }

        GenerateModels(projectOptions.Document, operationSchemaMappings);
        GenerateParameters(projectOptions.Document);

        if (projectOptions.ApiOptions.Generator.AspNetOutputType == AspNetOutputType.Mvc)
        {
            GenerateResults(projectOptions.Document);
        }

        GenerateInterfaces(projectOptions.Document);

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

        string content;
        if (projectOptions.ApiOptions.Generator.AspNetOutputType == AspNetOutputType.Mvc)
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

                string modelName;
                string content;
                if (projectOptions.ApiOptions.Generator.AspNetOutputType == AspNetOutputType.Mvc)
                {
                    var parameterParameters = ContentGeneratorServerParameterParametersFactory.CreateForClass(
                        fullNamespace,
                        openApiOperation.Value,
                        openApiPath.Value.Parameters);

                    modelName = parameterParameters.ParameterName;

                    var contentGeneratorParameter = new Framework.Mvc.ContentGenerators.Server.ContentGeneratorServerParameter(
                        new GeneratedCodeHeaderGenerator(new GeneratedCodeGeneratorParameters(projectOptions.ApiGeneratorVersion)),
                        new GeneratedCodeAttributeGenerator(new GeneratedCodeGeneratorParameters(projectOptions.ApiGeneratorVersion)),
                        new CodeDocumentationTagsGenerator(),
                        parameterParameters);

                    content = contentGeneratorParameter.Generate();
                }
                else
                {
                    var parameterParameters = ContentGeneratorServerParameterParametersFactory.CreateForRecord(
                        fullNamespace,
                        openApiOperation.Value,
                        openApiPath.Value.Parameters);

                    modelName = parameterParameters.ParameterName;

                    var contentGeneratorParameter = new Framework.Minimal.ContentGenerators.Server.ContentGeneratorServerParameter(
                        new GeneratedCodeHeaderGenerator(new GeneratedCodeGeneratorParameters(projectOptions.ApiGeneratorVersion)),
                        new GeneratedCodeAttributeGenerator(new GeneratedCodeGeneratorParameters(projectOptions.ApiGeneratorVersion)),
                        new CodeDocumentationTagsGenerator(),
                        parameterParameters);

                    content = contentGeneratorParameter.Generate();
                }

                // Write
                var file = new FileInfo(
                    Helpers.DirectoryInfoHelper.GetCsFileNameForContract(
                        projectOptions.PathForContracts,
                        apiGroupName,
                        ContentGeneratorConstants.Parameters,
                        modelName));

                var contentWriter = new ContentWriter(logger);
                contentWriter.Write(
                    projectOptions.PathForSrcGenerate,
                    file,
                    ContentWriterArea.Src,
                    content);
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

                var contentGeneratorResult = new Framework.Mvc.ContentGenerators.Server.ContentGeneratorServerResult(
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
                InterfaceParameters interfaceParameters;

                if (projectOptions.ApiOptions.Generator.AspNetOutputType == AspNetOutputType.Mvc)
                {
                    interfaceParameters = Framework.Mvc.Factories.Parameters.Server.ContentGeneratorServerHandlerInterfaceParametersFactory.Create(
                        codeGeneratorContentHeader,
                        fullNamespace,
                        codeGeneratorAttribute,
                        openApiPath.Value,
                        openApiOperation.Value);
                }
                else
                {
                    interfaceParameters = Framework.Minimal.Factories.Parameters.Server.ContentGeneratorServerHandlerInterfaceParametersFactory.Create(
                        codeGeneratorContentHeader,
                        fullNamespace,
                        codeGeneratorAttribute,
                        openApiPath.Value,
                        openApiOperation.Value);
                }

                var contentGeneratorInterface = new GenerateContentForInterface(
                    new CodeDocumentationTagsGenerator(),
                    interfaceParameters);

                var content = contentGeneratorInterface.Generate();

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
                    content);
            }
        }
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