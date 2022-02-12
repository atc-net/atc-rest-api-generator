namespace Atc.Rest.ApiGenerator.CLI.Commands.Settings;

public class BaseGenerateCommandSettings : BaseConfigurationCommandSettings
{
    [CommandOption($"{ArgumentCommandConstants.ShortProjectPrefixName}|{ArgumentCommandConstants.LongProjectPrefixName} <PROJECTPREFIXNAME>")]
    [Description("Project prefix name (e.g. 'PetStore' becomes 'PetStore.Api.Generated')")]
    public string ProjectPrefixName { get; init; }

    [CommandOption($"{ArgumentCommandConstants.LongServerDisableCodingRules}")]
    [Description("Disable ATC-Coding-Rules")]
    public bool DisableCodingRules { get; init; }
}