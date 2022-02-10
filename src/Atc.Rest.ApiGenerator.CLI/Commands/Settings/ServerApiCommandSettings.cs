namespace Atc.Rest.ApiGenerator.CLI.Commands.Settings;

public class ServerApiCommandSettings : BaseGenerateCommandSettings
{
    [CommandOption($"{CommandConstants.ArgumentShortOutputPath}|{CommandConstants.ArgumentLongOutputPath} <OUTPUTPATH>")]
    [Description("Path to generated project (directory)")]
    public string OutputPath { get; init; }

    [CommandOption($"{CommandConstants.ArgumentLongServerOutputTestPath} [OUTPUTTESTPATH]")]
    [Description("Path to generated test projects (directory)")]
    public FlagValue<string>? OutputTestPath { get; init; }
}