// ReSharper disable ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
// ReSharper disable InvertIf
// ReSharper disable SuggestBaseTypeForParameter
// ReSharper disable ReturnTypeCanBeEnumerable.Local
// ReSharper disable ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
namespace Atc.Rest.ApiGenerator.Generators;

public class ServerDomainGenerator
{
    private readonly ILogger logger;
    private readonly INugetPackageReferenceProvider nugetPackageReferenceProvider;
    private readonly DomainProjectOptions projectOptions;

    private readonly IServerDomainGenerator serverDomainGeneratorMvc;
    private readonly IServerDomainGenerator serverDomainGeneratorMinimalApi;
    private readonly IServerDomainTestGenerator? serverDomainTestGeneratorMvc;

    public ServerDomainGenerator(
        ILoggerFactory loggerFactory,
        IApiOperationExtractor apiOperationExtractor,
        INugetPackageReferenceProvider nugetPackageReferenceProvider,
        DomainProjectOptions projectOptions)
    {
        ArgumentNullException.ThrowIfNull(loggerFactory);

        logger = loggerFactory.CreateLogger<ServerDomainGenerator>();
        this.nugetPackageReferenceProvider = nugetPackageReferenceProvider ?? throw new ArgumentNullException(nameof(nugetPackageReferenceProvider));
        this.projectOptions = projectOptions ?? throw new ArgumentNullException(nameof(projectOptions));

        var apiProjectName = projectOptions.ProjectName.Replace(".Domain", ".Api.Generated", StringComparison.Ordinal);
        var operationSchemaMappings = apiOperationExtractor.Extract(projectOptions.Document);

        serverDomainGeneratorMvc = new Framework.Mvc.ProjectGenerator.ServerDomainGenerator(
            loggerFactory,
            projectOptions.ApiGeneratorVersion,
            projectOptions.ProjectName,
            apiProjectName,
            projectOptions.PathForSrcGenerate,
            projectOptions.Document);

        serverDomainGeneratorMinimalApi = new Framework.Minimal.ProjectGenerator.ServerDomainGenerator(
            loggerFactory,
            projectOptions.ApiGeneratorVersion,
            projectOptions.ProjectName,
            apiProjectName,
            projectOptions.PathForSrcGenerate,
            projectOptions.Document,
            operationSchemaMappings);

        if (projectOptions.PathForTestGenerate is not null)
        {
            serverDomainTestGeneratorMvc = new Framework.Mvc.ProjectGenerator.ServerDomainTestGenerator(
                loggerFactory,
                projectOptions.ApiGeneratorVersion,
                $"{projectOptions.ProjectName}.{ContentGeneratorConstants.Tests}",
                apiProjectName,
                projectOptions.ProjectName,
                projectOptions.PathForTestGenerate,
                projectOptions.Document);
        }
    }

    public bool Generate()
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

        var operationSchemaMappings = apiOperationExtractor.Extract(projectOptions.Document);

        if (projectOptions.ApiOptions.Generator.AspNetOutputType == AspNetOutputType.Mvc)
        {
            serverDomainGeneratorMvc.ScaffoldProjectFile();

            serverDomainGeneratorMvc.GenerateAssemblyMarker();
            serverDomainGeneratorMvc.GenerateHandlers();

            serverDomainGeneratorMvc.MaintainGlobalUsings(
                projectOptions.RemoveNamespaceGroupSeparatorInGlobalUsings);

            if (serverDomainTestGeneratorMvc is not null &&
                projectOptions.PathForTestGenerate is not null)
            {
                logger.LogInformation($"{ContentWriterConstants.AreaGenerateTest} Working on server domain unit-test generation ({projectOptions.ProjectName}.Tests)");

                serverDomainTestGeneratorMvc.ScaffoldProjectFile();

                serverDomainTestGeneratorMvc.GenerateHandlers();

                serverDomainTestGeneratorMvc.MaintainGlobalUsings(
                    projectOptions.UsingCodingRules,
                    projectOptions.RemoveNamespaceGroupSeparatorInGlobalUsings);
            }
        }
        else
        {
            serverDomainGeneratorMinimalApi.ScaffoldProjectFile();

            serverDomainGeneratorMinimalApi.GenerateAssemblyMarker();
            serverDomainGeneratorMinimalApi.GenerateServiceCollectionExtensions();
            serverDomainGeneratorMinimalApi.GenerateHandlers();

            serverDomainGeneratorMinimalApi.MaintainGlobalUsings(
                projectOptions.RemoveNamespaceGroupSeparatorInGlobalUsings);
        }

        return true;
    }
}