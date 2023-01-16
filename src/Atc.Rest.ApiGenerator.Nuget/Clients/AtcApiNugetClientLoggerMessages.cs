namespace Atc.Rest.ApiGenerator.Nuget.Clients;

/// <summary>
/// AtcApiNugetClient LoggerMessages.
/// </summary>
[SuppressMessage("Design", "MA0048:File name must match type name", Justification = "OK - By Design")]
public sealed partial class AtcApiNugetClient
{
    private readonly ILogger<AtcApiNugetClient> logger;

    [LoggerMessage(
        EventId = LoggingEventIdConstants.RetrievingVersionForPackage,
        Level = LogLevel.Trace,
        Message = "{prefix}Retrieving version for package '{packageId}'")]
    private partial void LogRetrievingVersionForPackage(
        string prefix,
        string packageId);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.RetrievedVersionForPackage,
        Level = LogLevel.Trace,
        Message = "{prefix}Retrieved version for package '{packageId}' took '{duration}'")]
    private partial void LogRetrievedVersionForPackage(
        string prefix,
        string packageId,
        string duration);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.RetrieveVersionForPackageFailed,
        Level = LogLevel.Trace,
        Message = "{prefix}Failed to retrieve version for package '{packageId}': '{errorMessage}'")]
    private partial void LogRetrieveVersionForPackageFailed(
        string prefix,
        string packageId,
        string errorMessage);
}