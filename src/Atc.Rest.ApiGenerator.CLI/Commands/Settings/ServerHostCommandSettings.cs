namespace Atc.Rest.ApiGenerator.CLI.Commands.Settings;

public class ServerHostCommandSettings : BaseGenerateCommandSettings
{
    [CommandOption($"{CommandConstants.ArgumentLongServerOutputApiPath} <APIPATH>")]
    [Description("Path to api project (directory)")]
    public string ApiPath { get; init; } = string.Empty;

    [CommandOption($"{CommandConstants.ArgumentLongServerOutputDomainPath} <DOMAINPATH>")]
    [Description("Path to domain project (directory)")]
    public string DomainPath { get; init; } = string.Empty;

    [CommandOption($"{CommandConstants.ArgumentLongServerOutputTestPath} [OUTPUTTESTPATH]")]
    [Description("Path to generated test projects (directory)")]
    public FlagValue<string>? OutputTestPath { get; init; }
}