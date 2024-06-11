// ReSharper disable ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
// ReSharper disable SuggestBaseTypeForParameter
// ReSharper disable ReturnTypeCanBeEnumerable.Local
// ReSharper disable StringLiteralTypo
namespace Atc.Rest.ApiGenerator.Generators;

public class ServerHostGenerator
{
    private readonly ILogger logger;
    private readonly HostProjectOptions projectOptions;

    private readonly IServerHostGenerator serverHostGeneratorMvc;
    private readonly IServerHostGenerator serverHostGeneratorMinimalApi;
    private readonly IServerHostTestGenerator? serverHostTestGeneratorMvc;
    private readonly IServerHostTestGenerator? serverHostTestGeneratorMinimalApi;

    public ServerHostGenerator(
        ILoggerFactory loggerFactory,
        IApiOperationExtractor apiOperationExtractor,
        INugetPackageReferenceProvider nugetPackageReferenceProvider,
        HostProjectOptions projectOptions)
    {
        ArgumentNullException.ThrowIfNull(loggerFactory);
        ArgumentNullException.ThrowIfNull(apiOperationExtractor);
        ArgumentNullException.ThrowIfNull(nugetPackageReferenceProvider);

        logger = loggerFactory.CreateLogger<ServerDomainGenerator>();
        this.projectOptions = projectOptions ?? throw new ArgumentNullException(nameof(projectOptions));

        var apiProjectName = projectOptions.ProjectName.Replace(".Api", ".Api.Generated", StringComparison.Ordinal);
        var domainProjectName = projectOptions.ProjectName.Replace(".Api", ".Domain", StringComparison.Ordinal);
        var operationSchemaMappings = apiOperationExtractor.Extract(projectOptions.Document);

        serverHostGeneratorMvc = new Framework.Mvc.ProjectGenerator.ServerHostGenerator(
            loggerFactory,
            nugetPackageReferenceProvider,
            projectOptions.ApiGeneratorVersion,
            projectOptions.ProjectName,
            apiProjectName,
            domainProjectName,
            projectOptions.PathForSrcGenerate,
            projectOptions.Document)
        {
            UseRestExtended = projectOptions.UseRestExtended,
        };

        serverHostGeneratorMinimalApi = new Framework.Minimal.ProjectGenerator.ServerHostGenerator(
            loggerFactory,
            nugetPackageReferenceProvider,
            projectOptions.ApiGeneratorVersion,
            projectOptions.ProjectName,
            apiProjectName,
            domainProjectName,
            projectOptions.PathForSrcGenerate,
            projectOptions.Document);

        if (projectOptions.PathForTestGenerate is not null)
        {
            serverHostTestGeneratorMvc = new Framework.Mvc.ProjectGenerator.ServerHostTestGenerator(
                loggerFactory,
                nugetPackageReferenceProvider,
                projectOptions.ApiGeneratorVersion,
                $"{projectOptions.ProjectName}.{ContentGeneratorConstants.Tests}",
                projectOptions.ProjectName,
                apiProjectName,
                domainProjectName,
                projectOptions.PathForTestGenerate,
                projectOptions.Document,
                operationSchemaMappings);

            serverHostTestGeneratorMinimalApi = new Framework.Minimal.ProjectGenerator.ServerHostTestGenerator(
                loggerFactory,
                nugetPackageReferenceProvider,
                projectOptions.ApiGeneratorVersion,
                $"{projectOptions.ProjectName}.{ContentGeneratorConstants.Tests}",
                projectOptions.ProjectName,
                apiProjectName,
                domainProjectName,
                projectOptions.PathForTestGenerate,
                projectOptions.Document,
                operationSchemaMappings);
        }
    }

    public async Task<bool> Generate()
    {
        logger.LogInformation($"{ContentWriterConstants.AreaGenerateCode} Working on server host generation ({projectOptions.ProjectName})");

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
            await serverHostGeneratorMvc.ScaffoldProjectFile();
            serverHostGeneratorMvc.ScaffoldPropertiesLaunchSettingsFile();
            serverHostGeneratorMvc.ScaffoldProgramFile(
                projectOptions.ApiOptions.Generator.SwaggerThemeMode);
            serverHostGeneratorMvc.ScaffoldStartupFile();
            serverHostGeneratorMvc.ScaffoldWebConfig();

            serverHostGeneratorMvc.GenerateConfigureSwaggerDocOptions();

            serverHostGeneratorMvc.MaintainGlobalUsings(
                projectOptions.RemoveNamespaceGroupSeparatorInGlobalUsings);

            if (serverHostTestGeneratorMvc is not null &&
                projectOptions.PathForTestGenerate is not null)
            {
                logger.LogInformation($"{ContentWriterConstants.AreaGenerateTest} Working on server host unit-test generation ({projectOptions.ProjectName}.Tests)");

                await serverHostTestGeneratorMvc.ScaffoldProjectFile();
                serverHostTestGeneratorMvc.ScaffoldAppSettingsIntegrationTestFile();

                serverHostTestGeneratorMvc.GenerateWebApiStartupFactoryFile();
                serverHostTestGeneratorMvc.GenerateWebApiControllerBaseTestFile();
                serverHostTestGeneratorMvc.GenerateEndpointHandlerStubs();
                serverHostTestGeneratorMvc.GenerateEndpointTests();

                serverHostTestGeneratorMvc.MaintainGlobalUsings(
                    projectOptions.UsingCodingRules,
                    projectOptions.RemoveNamespaceGroupSeparatorInGlobalUsings);
            }
        }
        else
        {
            await serverHostGeneratorMinimalApi.ScaffoldProjectFile();
            serverHostGeneratorMinimalApi.ScaffoldPropertiesLaunchSettingsFile();
            serverHostGeneratorMinimalApi.ScaffoldProgramFile(
                projectOptions.ApiOptions.Generator.SwaggerThemeMode);
            serverHostGeneratorMinimalApi.ScaffoldWebConfig();

            serverHostGeneratorMinimalApi.ScaffoldJsonSerializerOptionsExtensions();
            serverHostGeneratorMinimalApi.ScaffoldServiceCollectionExtensions();
            serverHostGeneratorMinimalApi.ScaffoldWebApplicationBuilderExtensions();
            serverHostGeneratorMinimalApi.ScaffoldWebApplicationExtensions(
                projectOptions.ApiOptions.Generator.SwaggerThemeMode);
            serverHostGeneratorMinimalApi.ScaffoldConfigureSwaggerOptions();

            serverHostGeneratorMinimalApi.GenerateConfigureSwaggerDocOptions();

            serverHostGeneratorMinimalApi.MaintainGlobalUsings(
                projectOptions.RemoveNamespaceGroupSeparatorInGlobalUsings);
            serverHostGeneratorMinimalApi.MaintainWwwResources();

            if (serverHostTestGeneratorMinimalApi is not null &&
                projectOptions.PathForTestGenerate is not null)
            {
                logger.LogInformation($"{ContentWriterConstants.AreaGenerateTest} Working on server host unit-test generation ({projectOptions.ProjectName}.Tests)");

                await serverHostTestGeneratorMinimalApi.ScaffoldProjectFile();
                serverHostTestGeneratorMinimalApi.ScaffoldAppSettingsIntegrationTestFile();

                serverHostTestGeneratorMinimalApi.GenerateWebApiStartupFactoryFile();
                serverHostTestGeneratorMinimalApi.GenerateWebApiControllerBaseTestFile();
                serverHostTestGeneratorMinimalApi.GenerateEndpointHandlerStubs();
                serverHostTestGeneratorMinimalApi.GenerateEndpointTests();

                serverHostTestGeneratorMinimalApi.MaintainGlobalUsings(
                    projectOptions.UsingCodingRules,
                    projectOptions.RemoveNamespaceGroupSeparatorInGlobalUsings);
            }
        }

        return true;
    }
}