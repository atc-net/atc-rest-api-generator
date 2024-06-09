// ReSharper disable ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
// ReSharper disable ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
namespace Atc.Rest.ApiGenerator.Generators;

public class ClientCSharpApiGenerator
{
    private readonly ILogger logger;
    private readonly INugetPackageReferenceProvider nugetPackageReferenceProvider;
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
        this.nugetPackageReferenceProvider = nugetPackageReferenceProvider ?? throw new ArgumentNullException(nameof(nugetPackageReferenceProvider));
        this.projectOptions = projectOptions ?? throw new ArgumentNullException(nameof(projectOptions));

        var operationSchemaMappings = apiOperationExtractor.Extract(projectOptions.Document);

        clientCSharpApiGenerator = new Client.CSharp.ProjectGenerator.ClientCSharpApiGenerator(
            loggerFactory,
            nugetPackageReferenceProvider,
            projectOptions.ApiGeneratorVersion,
            projectOptions.ProjectName,
            projectOptions.PathForSrcGenerate,
            projectOptions.Document,
            operationSchemaMappings,
            projectOptions.ApiOptions.Generator.Response.UseProblemDetailsAsDefaultBody)
        {
            HttpClientName = projectOptions.HttpClientName,
            ClientFolderName = projectOptions.ClientFolderName,
        };
    }

    public async Task<bool> Generate()
    {
        logger.LogInformation($"{ContentWriterConstants.AreaGenerateCode} Working on client api generation ({projectOptions.ProjectName})");

        await clientCSharpApiGenerator.ScaffoldProjectFile();

        clientCSharpApiGenerator.GenerateModels();

        clientCSharpApiGenerator.GenerateParameters();

        if (!projectOptions.ExcludeEndpointGeneration)
        {
            clientCSharpApiGenerator.GenerateEndpointInterfaces();

            clientCSharpApiGenerator.GenerateEndpoints();

            clientCSharpApiGenerator.GenerateEndpointResultInterfaces();

            clientCSharpApiGenerator.GenerateEndpointResults();
        }

        clientCSharpApiGenerator.MaintainGlobalUsings(
            projectOptions.RemoveNamespaceGroupSeparatorInGlobalUsings);

        return true;
    }
}