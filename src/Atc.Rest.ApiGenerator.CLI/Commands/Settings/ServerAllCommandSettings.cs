namespace Atc.Rest.ApiGenerator.CLI.Commands.Settings;

public class ServerAllCommandSettings : BaseGenerateCommandSettings
{
    [CommandOption($"{ArgumentCommandConstants.LongServerOutputSolutionPath} <OUTPUTSLNPATH>")]
    [Description("Path to solution file (directory or file)")]
    public string OutputSlnPath { get; init; } = string.Empty;

    [CommandOption($"{ArgumentCommandConstants.LongServerOutputSourcePath} <OUTPUTSRCPATH>")]
    [Description("Path to generated src projects (directory)")]
    public string OutputSrcPath { get; init; } = string.Empty;

    [CommandOption($"{ArgumentCommandConstants.LongServerOutputTestPath} [OUTPUTTESTPATH]")]
    [Description("Path to generated test projects (directory)")]
    public FlagValue<string>? OutputTestPath { get; init; }
}