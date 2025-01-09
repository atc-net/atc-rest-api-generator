// ReSharper disable ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
// ReSharper disable InvertIf
// ReSharper disable SuggestBaseTypeForParameter
// ReSharper disable ReturnTypeCanBeEnumerable.Local
// ReSharper disable ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
namespace Atc.Rest.ApiGenerator.Generators;

public class ServerDomainGenerator
{
    private readonly ILogger logger;
    private readonly DomainProjectOptions projectOptions;

    private readonly IServerDomainGenerator serverDomainGeneratorMvc;
    private readonly IServerDomainGenerator serverDomainGeneratorMinimalApi;
    private readonly IServerDomainTestGenerator? serverDomainTestGenerator;

    public ServerDomainGenerator(
        ILoggerFactory loggerFactory,
        IApiOperationExtractor apiOperationExtractor,
        INugetPackageReferenceProvider nugetPackageReferenceProvider,
        DomainProjectOptions projectOptions)
    {
        ArgumentNullException.ThrowIfNull(loggerFactory);
        ArgumentNullException.ThrowIfNull(apiOperationExtractor);
        ArgumentNullException.ThrowIfNull(nugetPackageReferenceProvider);

        logger = loggerFactory.CreateLogger<ServerDomainGenerator>();
        this.projectOptions = projectOptions ?? throw new ArgumentNullException(nameof(projectOptions));

        var apiProjectName = projectOptions.ProjectName.Replace(".Domain", ".Api.Generated", StringComparison.Ordinal);

        var generatorSettings = GeneratorSettingsFactory.Create(
            projectOptions.ApiGeneratorVersion,
            projectOptions.ProjectName,
            projectOptions.PathForSrcGenerate,
            projectOptions.ApiOptions.Generator);

        serverDomainGeneratorMvc = new Framework.Mvc.ProjectGenerator.ServerDomainGenerator(
            loggerFactory,
            apiProjectName,
            projectOptions.Document,
            generatorSettings);

        serverDomainGeneratorMinimalApi = new Framework.Minimal.ProjectGenerator.ServerDomainGenerator(
            loggerFactory,
            nugetPackageReferenceProvider,
            apiProjectName,
            projectOptions.Document,
            generatorSettings);

        if (projectOptions.PathForTestGenerate is not null)
        {
            var generatorTestSettings = GeneratorSettingsFactory.Create(
                projectOptions.ApiGeneratorVersion,
                $"{projectOptions.ProjectName}.{ContentGeneratorConstants.Tests}",
                projectOptions.PathForTestGenerate,
                projectOptions.ApiOptions.Generator);

            serverDomainTestGenerator = new ServerDomainTestGenerator(
                loggerFactory,
                nugetPackageReferenceProvider,
                apiProjectName,
                projectOptions.ProjectName,
                projectOptions.Document,
                generatorTestSettings);
        }
    }

    public async Task<bool> Generate()
    {
        logger.LogInformation($"{ContentWriterConstants.AreaGenerateCode} Working on server domain generation ({projectOptions.ProjectName})");

        if (projectOptions.ApiOptions.Generator.AspNetOutputType == AspNetOutputType.Mvc)
        {
            if (!projectOptions.SetPropertiesAfterValidationsOfProjectReferencesPathAndFilesForMvc(logger))
            {
                return false;
            }
        }
        else
        {
            if (!projectOptions.SetPropertiesAfterValidationsOfProjectReferencesPathAndFilesForMinimalApi(logger))
            {
                return false;
            }
        }

        if (projectOptions.ApiOptions.Generator.AspNetOutputType == AspNetOutputType.Mvc)
        {
            await serverDomainGeneratorMvc.ScaffoldProjectFile();
            serverDomainGeneratorMvc.ScaffoldHandlers();

            serverDomainGeneratorMvc.GenerateAssemblyMarker();

            serverDomainGeneratorMvc.MaintainGlobalUsings(
                projectOptions.ApiOptions.Generator.RemoveNamespaceGroupSeparatorInGlobalUsings);
        }
        else
        {
            await serverDomainGeneratorMinimalApi.ScaffoldProjectFile();
            serverDomainGeneratorMinimalApi.ScaffoldHandlers();

            serverDomainGeneratorMinimalApi.GenerateAssemblyMarker();
            serverDomainGeneratorMinimalApi.GenerateServiceCollectionEndpointHandlerExtensions();

            serverDomainGeneratorMinimalApi.MaintainGlobalUsings(
                projectOptions.ApiOptions.Generator.RemoveNamespaceGroupSeparatorInGlobalUsings);
        }

        if (serverDomainTestGenerator is not null &&
            projectOptions.PathForTestGenerate is not null)
        {
            logger.LogInformation($"{ContentWriterConstants.AreaGenerateTest} Working on server domain unit-test generation ({projectOptions.ProjectName}.Tests)");

            await serverDomainTestGenerator.ScaffoldProjectFile();

            serverDomainTestGenerator.ScaffoldHandlers();

            serverDomainTestGenerator.MaintainGlobalUsings(
                projectOptions.UsingCodingRules,
                projectOptions.ApiOptions.Generator.RemoveNamespaceGroupSeparatorInGlobalUsings);
        }

        return true;
    }
}