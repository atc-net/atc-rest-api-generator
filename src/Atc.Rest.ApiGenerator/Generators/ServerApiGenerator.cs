// ReSharper disable ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
// ReSharper disable SuggestBaseTypeForParameter
// ReSharper disable ReplaceSubstringWithRangeIndexer
// ReSharper disable ReturnTypeCanBeEnumerable.Local
// ReSharper disable UseObjectOrCollectionInitializer
namespace Atc.Rest.ApiGenerator.Generators;

public class ServerApiGenerator
{
    private readonly ILogger logger;
    private readonly INugetPackageReferenceProvider nugetPackageReferenceProvider;
    private readonly ApiProjectOptions projectOptions;

    private readonly IServerApiGenerator serverApiGeneratorMvc;
    private readonly IServerApiGenerator serverApiGeneratorMinimalApi;

    public ServerApiGenerator(
        ILoggerFactory loggerFactory,
        IApiOperationExtractor apiOperationExtractor,
        INugetPackageReferenceProvider nugetPackageReferenceProvider,
        ApiProjectOptions projectOptions)
    {
        ArgumentNullException.ThrowIfNull(loggerFactory);
        ArgumentNullException.ThrowIfNull(apiOperationExtractor);

        logger = loggerFactory.CreateLogger<ServerDomainGenerator>();
        this.nugetPackageReferenceProvider = nugetPackageReferenceProvider ?? throw new ArgumentNullException(nameof(nugetPackageReferenceProvider));
        this.projectOptions = projectOptions ?? throw new ArgumentNullException(nameof(projectOptions));

        var operationSchemaMappings = apiOperationExtractor.Extract(projectOptions.Document);

        serverApiGeneratorMvc = new Framework.Mvc.ProjectGenerator.ServerApiGenerator(
            loggerFactory,
            nugetPackageReferenceProvider,
            projectOptions.ApiGeneratorVersion,
            projectOptions.ProjectName,
            projectOptions.PathForSrcGenerate,
            projectOptions.Document,
            operationSchemaMappings,
            projectOptions.RouteBase)
        {
            UseProblemDetailsAsDefaultBody = projectOptions.ApiOptions.Generator.Response.UseProblemDetailsAsDefaultBody,
        };

        serverApiGeneratorMinimalApi = new Framework.Minimal.ProjectGenerator.ServerApiGenerator(
            loggerFactory,
            nugetPackageReferenceProvider,
            projectOptions.ApiGeneratorVersion,
            projectOptions.ProjectName,
            projectOptions.PathForSrcGenerate,
            projectOptions.Document,
            operationSchemaMappings,
            projectOptions.RouteBase);
    }

    public async Task<bool> Generate()
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

        if (projectOptions.ApiOptions.Generator.AspNetOutputType == AspNetOutputType.Mvc)
        {
            await serverApiGeneratorMvc.ScaffoldProjectFile();

            serverApiGeneratorMvc.GenerateAssemblyMarker();
            serverApiGeneratorMvc.GenerateModels();
            serverApiGeneratorMvc.GenerateParameters();
            serverApiGeneratorMvc.GenerateResults();
            serverApiGeneratorMvc.GenerateInterfaces();
            serverApiGeneratorMvc.GenerateEndpoints();

            serverApiGeneratorMvc.MaintainApiSpecification(projectOptions.DocumentFile);
            serverApiGeneratorMvc.MaintainGlobalUsings(
                projectOptions.RemoveNamespaceGroupSeparatorInGlobalUsings);
        }
        else
        {
            await serverApiGeneratorMinimalApi.ScaffoldProjectFile();

            serverApiGeneratorMinimalApi.GenerateAssemblyMarker();
            serverApiGeneratorMinimalApi.GenerateModels();
            serverApiGeneratorMinimalApi.GenerateParameters();
            serverApiGeneratorMinimalApi.GenerateResults();
            serverApiGeneratorMinimalApi.GenerateInterfaces();
            serverApiGeneratorMinimalApi.GenerateEndpoints();

            serverApiGeneratorMinimalApi.MaintainApiSpecification(projectOptions.DocumentFile);
            serverApiGeneratorMinimalApi.MaintainGlobalUsings(
                projectOptions.RemoveNamespaceGroupSeparatorInGlobalUsings);
        }

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
}