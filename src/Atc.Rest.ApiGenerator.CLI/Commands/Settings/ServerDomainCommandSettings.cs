namespace Atc.Rest.ApiGenerator.CLI.Commands.Settings;

public class ServerDomainCommandSettings : BaseGenerateCommandSettings
{
    [CommandOption($"{CommandConstants.ArgumentShortOutputPath}|{CommandConstants.ArgumentLongOutputPath} <OUTPUTPATH>")]
    [Description("Path to generated project (directory)")]
    public string OutputPath { get; init; }

    [CommandOption($"{CommandConstants.ArgumentLongServerOutputApiPath} <APIPATH>")]
    [Description("Path to api project (directory)")]
    public string ApiPath { get; init; } = string.Empty;

    [CommandOption($"{CommandConstants.ArgumentLongServerOutputTestPath} [OUTPUTTESTPATH]")]
    [Description("Path to generated test projects (directory)")]
    public FlagValue<string>? OutputTestPath { get; init; }
}