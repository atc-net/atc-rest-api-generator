// ReSharper disable ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
// ReSharper disable ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
namespace Atc.Rest.ApiGenerator.Generators;

public class ClientCSharpApiGenerator
{
    private readonly ILogger logger;
    private readonly ClientCSharpApiProjectOptions projectOptions;

    private readonly IClientCSharpApiGenerator clientCSharpApiGenerator;

    public ClientCSharpApiGenerator(
        ILoggerFactory loggerFactory,
        IApiOperationExtractor apiOperationExtractor,
        INugetPackageReferenceProvider nugetPackageReferenceProvider,
        ClientCSharpApiProjectOptions projectOptions)
    {
        ArgumentNullException.ThrowIfNull(loggerFactory);
        ArgumentNullException.ThrowIfNull(apiOperationExtractor);

        logger = loggerFactory.CreateLogger<ServerDomainGenerator>();
        this.projectOptions = projectOptions ?? throw new ArgumentNullException(nameof(projectOptions));

        var operationSchemaMappings = apiOperationExtractor.Extract(projectOptions.Document);

        var generatorSettings = GeneratorSettingsFactory.Create(projectOptions.ApiOptions.Generator);

        clientCSharpApiGenerator = new Client.CSharp.ProjectGenerator.ClientCSharpApiGenerator(
            loggerFactory,
            nugetPackageReferenceProvider,
            projectOptions.ApiGeneratorVersion,
            projectOptions.ProjectName,
            projectOptions.PathForSrcGenerate,
            projectOptions.Document,
            operationSchemaMappings,
            generatorSettings,
            projectOptions.ApiOptions.Generator.Response.CustomErrorResponseModel);

        if (projectOptions.ApiOptions.Generator.Client is not null)
        {
            clientCSharpApiGenerator.HttpClientName = projectOptions.ApiOptions.Generator.Client.HttpClientName;
        }
    }

    public async Task<bool> Generate()
    {
        logger.LogInformation($"{ContentWriterConstants.AreaGenerateCode} Working on client api generation ({projectOptions.ProjectName})");

        await clientCSharpApiGenerator.ScaffoldProjectFile();

        clientCSharpApiGenerator.GenerateModels();

        clientCSharpApiGenerator.GenerateParameters();

        if (projectOptions.ApiOptions.Generator.Client is not null &&
            !projectOptions.ApiOptions.Generator.Client.ExcludeEndpointGeneration)
        {
            clientCSharpApiGenerator.GenerateEndpointInterfaces();

            clientCSharpApiGenerator.GenerateEndpoints();

            clientCSharpApiGenerator.GenerateEndpointResultInterfaces();

            clientCSharpApiGenerator.GenerateEndpointResults();
        }

        clientCSharpApiGenerator.MaintainGlobalUsings(
            projectOptions.ApiOptions.Generator.RemoveNamespaceGroupSeparatorInGlobalUsings);

        return true;
    }
}