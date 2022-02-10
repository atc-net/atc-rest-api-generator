namespace Atc.Rest.ApiGenerator.CLI.Commands.Settings;

public class ServerAllCommandSettings : BaseGenerateCommandSettings
{
    [CommandOption($"{CommandConstants.ArgumentLongServerOutputSolutionPath} <OUTPUTSLNPATH>")]
    [Description("Path to solution file (directory or file)")]
    public string OutputSlnPath { get; init; } = string.Empty;

    [CommandOption($"{CommandConstants.ArgumentLongServerOutputSourcePath} <OUTPUTSRCPATH>")]
    [Description("Path to generated src projects (directory)")]
    public string OutputSrcPath { get; init; } = string.Empty;

    [CommandOption($"{CommandConstants.ArgumentLongServerOutputTestPath} [OUTPUTTESTPATH]")]
    [Description("Path to generated test projects (directory)")]
    public FlagValue<string>? OutputTestPath { get; init; }

    [CommandOption($"{CommandConstants.ArgumentLongServerDisableCodingRules}")]
    [Description("Disable ATC-Coding-Rules")]
    public bool DisableCodingRules { get; init; }
}