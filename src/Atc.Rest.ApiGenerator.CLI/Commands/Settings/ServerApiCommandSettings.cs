namespace Atc.Rest.ApiGenerator.CLI.Commands.Settings;

public class ServerApiCommandSettings : BaseGenerateCommandSettings
{
    [CommandOption($"{CommandConstants.ArgumentLongServerOutputTestPath} [OUTPUTTESTPATH]")]
    [Description("Path to generated test projects (directory)")]
    public FlagValue<string>? OutputTestPath { get; init; }
}