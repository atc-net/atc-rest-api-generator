namespace Atc.Rest.ApiGenerator.CLI.Commands.Settings;

public class BaseCommandSettings : CommandSettings
{
    [CommandOption($"{CommandConstants.ArgumentShortVerbose}|{CommandConstants.ArgumentLongVerbose}")]
    [Description("Use verbose for more debug/trace information")]
    public bool Verbose { get; init; }

    [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "OK.")]
    public bool IsOptionValueTrue(
        bool? value)
        => value is not null &&
           value.Value;
}