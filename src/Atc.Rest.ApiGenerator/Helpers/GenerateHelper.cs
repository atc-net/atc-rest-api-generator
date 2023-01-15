// ReSharper disable ReturnTypeCanBeEnumerable.Global
namespace Atc.Rest.ApiGenerator.Helpers;

public static class GenerateHelper
{
    private static readonly Version AtcApiGeneratorVersion = new(1, 1, 405, 0); // TODO: Fix version

    public static Version GetAtcVersion()
    {
        var version = AtcApiNugetClientHelper.GetLatestVersionForPackageId(
            NullLogger.Instance,
            "Atc",
            CancellationToken.None) ?? new Version(2, 0, 81, 0);
        return version;
    }

    public static string GetAtcVersionAsString3()
    {
        var atcVersion = GetAtcVersion();
        return $"{atcVersion.Major}.{atcVersion.Minor}.{atcVersion.Build}";
    }

    public static string GetAtcVersionAsString4()
    {
        var atcVersion = GetAtcVersion();
        return $"{atcVersion.Major}.{atcVersion.Minor}.{atcVersion.Build}.{atcVersion.Revision}";
    }

    public static Version GetAtcApiGeneratorVersion()
    {
        var version = AtcApiNugetClientHelper.GetLatestVersionForPackageId(
            NullLogger.Instance,
            "Atc.Rest.ApiGenerator",
            CancellationToken.None) ?? new Version(2, 0, 101, 0);

        var assemblyVersion = CliHelper.GetCurrentVersion();

        return assemblyVersion.GreaterThan(version)
            ? assemblyVersion
            : AtcApiGeneratorVersion;
    }

    public static string GetAtcApiGeneratorVersionAsString3()
    {
        var atcApiGeneratorVersion = GetAtcApiGeneratorVersion();
        return $"{atcApiGeneratorVersion.Major}.{atcApiGeneratorVersion.Minor}.{atcApiGeneratorVersion.Build}";
    }

    public static string GetAtcApiGeneratorVersionAsString4()
    {
        var atcApiGeneratorVersion = GetAtcApiGeneratorVersion();
        return $"{atcApiGeneratorVersion.Major}.{atcApiGeneratorVersion.Minor}.{atcApiGeneratorVersion.Build}.{atcApiGeneratorVersion.Revision}";
    }

    public static bool GenerateServerApi(
        ILogger logger,
        IApiOperationExtractor apiOperationExtractor,
        string projectPrefixName,
        DirectoryInfo outputPath,
        DirectoryInfo? outputTestPath,
        OpenApiDocumentContainer apiDocumentContainer,
        ApiOptions apiOptions,
        bool useCodingRules)
    {
        ArgumentNullException.ThrowIfNull(logger);
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
            useCodingRules);
        var serverApiGenerator = new ServerApiGenerator(logger, apiOperationExtractor, projectOptions);
        return serverApiGenerator.Generate();
    }

    public static bool GenerateServerDomain(
        ILogger logger,
        string projectPrefixName,
        DirectoryInfo outputSourcePath,
        DirectoryInfo? outputTestPath,
        OpenApiDocumentContainer apiDocumentContainer,
        ApiOptions apiOptions,
        bool useCodingRules,
        DirectoryInfo apiPath)
    {
        ArgumentNullException.ThrowIfNull(logger);
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
            apiPath);
        var serverDomainGenerator = new ServerDomainGenerator(logger, domainProjectOptions);
        return serverDomainGenerator.Generate();
    }

    public static bool GenerateServerHost(
        ILogger logger,
        IApiOperationExtractor apiOperationExtractor,
        string projectPrefixName,
        DirectoryInfo outputSourcePath,
        DirectoryInfo? outputTestPath,
        OpenApiDocumentContainer apiDocumentContainer,
        ApiOptions apiOptions,
        bool usingCodingRules,
        DirectoryInfo apiPath,
        DirectoryInfo domainPath)
    {
        ArgumentNullException.ThrowIfNull(logger);
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
            apiPath,
            domainPath);
        var serverHostGenerator = new ServerHostGenerator(logger, hostProjectOptions);
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
        string projectPrefixName,
        string? clientFolderName,
        DirectoryInfo outputPath,
        OpenApiDocumentContainer apiDocumentContainer,
        bool excludeEndpointGeneration,
        ApiOptions apiOptions,
        bool useCodingRules)
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
            excludeEndpointGeneration,
            apiOptions,
            useCodingRules);
        var clientCSharpApiGenerator = new ClientCSharpApiGenerator(logger, apiOperationExtractor, clientCSharpApiProjectOptions);
        return clientCSharpApiGenerator.Generate();
    }
}