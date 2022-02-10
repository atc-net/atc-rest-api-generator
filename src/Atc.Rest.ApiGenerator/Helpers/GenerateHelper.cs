// ReSharper disable ReturnTypeCanBeEnumerable.Global
namespace Atc.Rest.ApiGenerator.Helpers;

public static class GenerateHelper
{
    private static readonly Version AtcVersion = new (1, 1, 349, 0);
    private static readonly Version AtcToolVersion = new (1, 1, 371, 0);

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "OK.")]
    public static Version GetAtcVersion() => AtcVersion;

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

    public static Version GetAtcToolVersion()
    {
        var assembly = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();

        return assembly.GetName().Version.GreaterThan(AtcToolVersion)
            ? assembly.GetName().Version
            : AtcToolVersion;
    }

    public static string GetAtcToolVersionAsString3()
    {
        var atcToolVersion = GetAtcToolVersion();
        return $"{atcToolVersion.Major}.{atcToolVersion.Minor}.{atcToolVersion.Build}";
    }

    public static string GetAtcToolVersionAsString4()
    {
        var atcToolVersion = GetAtcToolVersion();
        return $"{atcToolVersion.Major}.{atcToolVersion.Minor}.{atcToolVersion.Build}.{atcToolVersion.Revision}";
    }

    public static List<LogKeyValueItem> GenerateServerApi(
        string projectPrefixName,
        DirectoryInfo outputPath,
        DirectoryInfo? outputTestPath,
        Tuple<OpenApiDocument, OpenApiDiagnostic, FileInfo> apiDocument,
        ApiOptions apiOptions)
    {
        ArgumentNullException.ThrowIfNull(projectPrefixName);
        ArgumentNullException.ThrowIfNull(outputPath);
        ArgumentNullException.ThrowIfNull(apiDocument);
        ArgumentNullException.ThrowIfNull(apiOptions);

        var projectOptions = new ApiProjectOptions(
            outputPath,
            outputTestPath,
            apiDocument.Item1,
            apiDocument.Item3,
            projectPrefixName,
            "Api.Generated",
            apiOptions);
        var serverApiGenerator = new ServerApiGenerator(projectOptions);
        return serverApiGenerator.Generate();
    }

    public static List<LogKeyValueItem> GenerateServerDomain(
        string projectPrefixName,
        DirectoryInfo outputSourcePath,
        DirectoryInfo? outputTestPath,
        Tuple<OpenApiDocument, OpenApiDiagnostic, FileInfo> apiDocument,
        ApiOptions apiOptions,
        DirectoryInfo apiPath)
    {
        ArgumentNullException.ThrowIfNull(projectPrefixName);
        ArgumentNullException.ThrowIfNull(outputSourcePath);
        ArgumentNullException.ThrowIfNull(apiDocument);
        ArgumentNullException.ThrowIfNull(apiOptions);
        ArgumentNullException.ThrowIfNull(apiPath);

        var domainProjectOptions = new DomainProjectOptions(outputSourcePath, outputTestPath, apiDocument.Item1, apiDocument.Item3, projectPrefixName, apiOptions, apiPath);
        var serverDomainGenerator = new ServerDomainGenerator(domainProjectOptions);
        return serverDomainGenerator.Generate();
    }

    public static List<LogKeyValueItem> GenerateServerHost(
        string projectPrefixName,
        DirectoryInfo outputSourcePath,
        DirectoryInfo? outputTestPath,
        Tuple<OpenApiDocument, OpenApiDiagnostic, FileInfo> apiDocument,
        ApiOptions apiOptions,
        DirectoryInfo apiPath,
        DirectoryInfo domainPath)
    {
        ArgumentNullException.ThrowIfNull(projectPrefixName);
        ArgumentNullException.ThrowIfNull(outputSourcePath);
        ArgumentNullException.ThrowIfNull(apiDocument);
        ArgumentNullException.ThrowIfNull(apiOptions);
        ArgumentNullException.ThrowIfNull(apiPath);
        ArgumentNullException.ThrowIfNull(domainPath);

        var hostProjectOptions = new HostProjectOptions(outputSourcePath, outputTestPath, apiDocument.Item1, apiDocument.Item3, projectPrefixName, apiOptions, apiPath, domainPath);
        var serverHostGenerator = new ServerHostGenerator(hostProjectOptions);
        return serverHostGenerator.Generate();
    }

    public static List<LogKeyValueItem> GenerateServerSln(
        string projectPrefixName,
        string outputSlnPath,
        DirectoryInfo outputSourcePath,
        DirectoryInfo? outputTestPath)
    {
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

            return SolutionAndProjectHelper.ScaffoldSlnFile(
                slnFile,
                projectName,
                apiPath,
                domainPath,
                hostPath,
                domainTestPath,
                hostTestPath);
        }

        return SolutionAndProjectHelper.ScaffoldSlnFile(
            slnFile,
            projectName,
            apiPath,
            domainPath,
            hostPath);
    }

    public static List<LogKeyValueItem> GenerateServerCSharpClient(
        string projectPrefixName,
        string? clientFolder,
        DirectoryInfo outputPath,
        Tuple<OpenApiDocument, OpenApiDiagnostic, FileInfo> apiDocument,
        bool excludeEndpointGeneration,
        ApiOptions apiOptions)
    {
        ArgumentNullException.ThrowIfNull(projectPrefixName);
        ArgumentNullException.ThrowIfNull(outputPath);
        ArgumentNullException.ThrowIfNull(apiDocument);
        ArgumentNullException.ThrowIfNull(apiOptions);

        var clientCSharpApiProjectOptions = new ClientCSharpApiProjectOptions(
            outputPath,
            clientFolder,
            apiDocument.Item1,
            apiDocument.Item3,
            projectPrefixName,
            "ApiClient.Generated",
            excludeEndpointGeneration,
            apiOptions);
        var clientCSharpApiGenerator = new ClientCSharpApiGenerator(clientCSharpApiProjectOptions);
        return clientCSharpApiGenerator.Generate();
    }
}