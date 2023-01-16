namespace Atc.Rest.ApiGenerator.CodingRules;

/// <summary>
/// AtcCodingRulesUpdater LoggerMessages.
/// </summary>
[SuppressMessage("Design", "MA0048:File name must match type name", Justification = "OK - By Design")]
public sealed partial class AtcCodingRulesUpdater
{
    private readonly ILogger<AtcCodingRulesUpdater> logger;

    [LoggerMessage(
        EventId = LoggingEventIdConstants.WorkingOnCodingRules,
        Level = LogLevel.Trace,
        Message = "{prefix}Working on Coding Rules")]
    private partial void LogWorkingOnCodingRules(
        string prefix);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.WorkingOnEditorConfigFiles,
        Level = LogLevel.Trace,
        Message = "{prefix}Working on EditorConfig files")]
    private partial void LogWorkingOnEditorConfigFiles(
        string prefix);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.WorkingOnDirectoryBuildPropsFiles,
        Level = LogLevel.Trace,
        Message = "{prefix}Working on Directory.Build.props files")]
    private partial void LogWorkingOnDirectoryBuildPropsFiles(
        string prefix);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.FileCreated,
        Level = LogLevel.Debug,
        Message = "{prefix}{area}: {fileName} created")]
    private partial void LogFileCreated(
        string prefix,
        string area,
        string fileName);

    [LoggerMessage(
        EventId = LoggingEventIdConstants.FileSkipped,
        Level = LogLevel.Error,
        Message = "{prefix}{area}: {fileName} skipped: '{errorMessage}'")]
    private partial void LogFileSkipped(
        string prefix,
        string area,
        string fileName,
        string errorMessage);
}