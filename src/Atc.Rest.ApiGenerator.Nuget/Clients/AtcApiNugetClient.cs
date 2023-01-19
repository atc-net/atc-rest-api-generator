namespace Atc.Rest.ApiGenerator.Nuget.Clients;

/// <summary>
/// The main AtcApiNugetClient - Handles call execution.
/// </summary>
public sealed partial class AtcApiNugetClient : IAtcApiNugetClient
{
    private const string LogPrefix = "    ";
    private const string BaseAddress = "https://atc-api.azurewebsites.net/nuget-search";
    private static readonly ConcurrentDictionary<string, Version> Cache = new(StringComparer.Ordinal);

    public AtcApiNugetClient(
        ILogger<AtcApiNugetClient> logger)
    {
        this.logger = logger;
    }

    public async Task<Version?> RetrieveLatestVersionForPackageId(
        string packageId,
        CancellationToken cancellationToken = default)
    {
        var cacheValue = Cache.GetValueOrDefault(packageId);
        if (cacheValue is not null)
        {
            return cacheValue;
        }

        try
        {
            var stopwatch = Stopwatch.StartNew();
            var uri = new Uri($"{BaseAddress}/package?packageId={packageId}");
            LogRetrievingVersionForPackage(LogPrefix, packageId);

            using var client = new HttpClient();
            var response = await client.GetStringAsync(uri, cancellationToken);

            stopwatch.Stop();
            LogRetrievedVersionForPackage(LogPrefix, packageId, stopwatch.Elapsed.GetPrettyTime());

            if (string.IsNullOrEmpty(response) ||
                !Version.TryParse(response, out var version))
            {
                return null;
            }

            Cache.GetOrAdd(packageId, version);
            return version;
        }
        catch (WebException ex) when (ex.Status == WebExceptionStatus.ProtocolError &&
                                      ex.Message.Contains("404", StringComparison.Ordinal))
        {
            LogRetrieveVersionForPackageFailed(LogPrefix, packageId, "Atc-Api Nuget endpoint not found!");
        }
        catch (Exception ex)
        {
            LogRetrieveVersionForPackageFailed(LogPrefix, packageId, ex.GetLastInnerMessage());
        }

        return null;
    }
}