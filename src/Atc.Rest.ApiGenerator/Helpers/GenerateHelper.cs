// ReSharper disable ReturnTypeCanBeEnumerable.Global
namespace Atc.Rest.ApiGenerator.Helpers;

public static class GenerateHelper
{
    public static bool GenerateServerApi(
        ILoggerFactory loggerFactory,
        IApiOperationExtractor apiOperationExtractor,
        INugetPackageReferenceProvider nugetPackageReferenceProvider,
        string projectPrefixName,
        DirectoryInfo outputPath,
        DirectoryInfo? outputTestPath,
        OpenApiDocumentContainer apiDocumentContainer,
        ApiOptions apiOptions,
        bool useCodingRules,
        bool removeNamespaceGroupSeparatorInGlobalUsings)
    {
        ArgumentNullException.ThrowIfNull(apiOperationExtractor);
        ArgumentNullException.ThrowIfNull(projectPrefixName);
        ArgumentNullException.ThrowIfNull(outputPath);
        ArgumentNullException.ThrowIfNull(apiDocumentContainer);
        ArgumentNullException.ThrowIfNull(apiOptions);

        var projectOptions = new ApiProjectOptions(
            outputPath,
            outputTestPath,
            apiDocumentContainer.Document!,
            apiDocumentContainer.DocFile,
            projectPrefixName,
            "Api.Generated",
            apiOptions,
            useCodingRules,
            removeNamespaceGroupSeparatorInGlobalUsings);
        var serverApiGenerator = new ServerApiGenerator(loggerFactory, apiOperationExtractor, nugetPackageReferenceProvider, projectOptions);
        return serverApiGenerator.Generate();
    }

    public static bool GenerateServerDomain(
        ILoggerFactory loggerFactory,
        IApiOperationExtractor apiOperationExtractor,
        INugetPackageReferenceProvider nugetPackageReferenceProvider,
        string projectPrefixName,
        DirectoryInfo outputSourcePath,
        DirectoryInfo? outputTestPath,
        OpenApiDocumentContainer apiDocumentContainer,
        ApiOptions apiOptions,
        bool useCodingRules,
        bool removeNamespaceGroupSeparatorInGlobalUsings,
        DirectoryInfo apiPath)
    {
        ArgumentNullException.ThrowIfNull(projectPrefixName);
        ArgumentNullException.ThrowIfNull(outputSourcePath);
        ArgumentNullException.ThrowIfNull(apiDocumentContainer);
        ArgumentNullException.ThrowIfNull(apiOptions);
        ArgumentNullException.ThrowIfNull(apiPath);

        var domainProjectOptions = new DomainProjectOptions(
            outputSourcePath,
            outputTestPath,
            apiDocumentContainer.Document!,
            apiDocumentContainer.DocFile,
            projectPrefixName,
            apiOptions,
            useCodingRules,
            removeNamespaceGroupSeparatorInGlobalUsings,
            apiPath);
        var serverDomainGenerator = new ServerDomainGenerator(loggerFactory, apiOperationExtractor, nugetPackageReferenceProvider, domainProjectOptions);
        return serverDomainGenerator.Generate();
    }

    public static bool GenerateServerHost(
        ILoggerFactory loggerFactory,
        IApiOperationExtractor apiOperationExtractor,
        INugetPackageReferenceProvider nugetPackageReferenceProvider,
        string projectPrefixName,
        DirectoryInfo outputSourcePath,
        DirectoryInfo? outputTestPath,
        OpenApiDocumentContainer apiDocumentContainer,
        ApiOptions apiOptions,
        bool usingCodingRules,
        bool removeNamespaceGroupSeparatorInGlobalUsings,
        DirectoryInfo apiPath,
        DirectoryInfo domainPath)
    {
        ArgumentNullException.ThrowIfNull(projectPrefixName);
        ArgumentNullException.ThrowIfNull(outputSourcePath);
        ArgumentNullException.ThrowIfNull(apiDocumentContainer);
        ArgumentNullException.ThrowIfNull(apiOptions);
        ArgumentNullException.ThrowIfNull(apiPath);
        ArgumentNullException.ThrowIfNull(domainPath);

        var hostProjectOptions = new HostProjectOptions(
            outputSourcePath,
            outputTestPath,
            apiDocumentContainer.Document!,
            apiDocumentContainer.DocFile,
            projectPrefixName,
            apiOptions,
            usingCodingRules,
            removeNamespaceGroupSeparatorInGlobalUsings,
            apiPath,
            domainPath);
        var serverHostGenerator = new ServerHostGenerator(loggerFactory, apiOperationExtractor, nugetPackageReferenceProvider, hostProjectOptions);
        return serverHostGenerator.Generate();
    }

    public static bool GenerateServerSln(
        ILogger logger,
        string projectPrefixName,
        string outputSlnPath,
        DirectoryInfo outputSourcePath,
        DirectoryInfo? outputTestPath)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(projectPrefixName);
        ArgumentNullException.ThrowIfNull(outputSlnPath);
        ArgumentNullException.ThrowIfNull(outputSourcePath);

        var projectName = projectPrefixName
            .Replace(" ", ".", StringComparison.Ordinal)
            .Replace("-", ".", StringComparison.Ordinal)
            .Trim();

        var apiPath = new DirectoryInfo(Path.Combine(outputSourcePath.FullName, $"{projectName}.Api.Generated"));
        var domainPath = new DirectoryInfo(Path.Combine(outputSourcePath.FullName, $"{projectName}.Domain"));
        var hostPath = new DirectoryInfo(Path.Combine(outputSourcePath.FullName, $"{projectName}.Api"));

        var slnFile = outputSlnPath.EndsWith(".sln", StringComparison.OrdinalIgnoreCase)
            ? new FileInfo(outputSlnPath)
            : new FileInfo(Path.Combine(outputSlnPath, $"{projectName}.sln"));

        if (outputTestPath is not null)
        {
            var domainTestPath = new DirectoryInfo(Path.Combine(outputTestPath.FullName, $"{projectName}.Domain"));
            var hostTestPath = new DirectoryInfo(Path.Combine(outputTestPath.FullName, $"{projectName}.Api"));

            SolutionAndProjectHelper.ScaffoldSlnFile(
                logger,
                slnFile,
                projectName,
                apiPath,
                domainPath,
                hostPath,
                domainTestPath,
                hostTestPath);

            return true;
        }

        SolutionAndProjectHelper.ScaffoldSlnFile(
            logger,
            slnFile,
            projectName,
            apiPath,
            domainPath,
            hostPath);

        return true;
    }

    public static bool GenerateServerCSharpClient(
        ILogger logger,
        IApiOperationExtractor apiOperationExtractor,
        INugetPackageReferenceProvider nugetPackageReferenceProvider,
        string projectPrefixName,
        string? httpClientName,
        string? clientFolderName,
        DirectoryInfo outputPath,
        OpenApiDocumentContainer apiDocumentContainer,
        bool excludeEndpointGeneration,
        ApiOptions apiOptions,
        bool useCodingRules,
        bool removeNamespaceGroupSeparatorInGlobalUsings)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(projectPrefixName);
        ArgumentNullException.ThrowIfNull(outputPath);
        ArgumentNullException.ThrowIfNull(apiDocumentContainer);
        ArgumentNullException.ThrowIfNull(apiOptions);

        var clientCSharpApiProjectOptions = new ClientCSharpApiProjectOptions(
            outputPath,
            clientFolderName,
            apiDocumentContainer.Document!,
            apiDocumentContainer.DocFile,
            projectPrefixName,
            "ApiClient.Generated",
            httpClientName,
            excludeEndpointGeneration,
            apiOptions,
            useCodingRules,
            removeNamespaceGroupSeparatorInGlobalUsings);
        var clientCSharpApiGenerator = new ClientCSharpApiGenerator(logger, apiOperationExtractor, nugetPackageReferenceProvider, clientCSharpApiProjectOptions);
        return clientCSharpApiGenerator.Generate();
    }
}