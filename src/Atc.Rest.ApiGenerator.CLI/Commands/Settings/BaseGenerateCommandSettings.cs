namespace Atc.Rest.ApiGenerator.CLI.Commands.Settings;

public class BaseGenerateCommandSettings : BaseConfigurationCommandSettings
{
    [CommandOption($"{CommandConstants.ArgumentShortProjectPrefixName}|{CommandConstants.ArgumentLongProjectPrefixName} <PROJECTPREFIXNAME>")]
    [Description("Project prefix name (e.g. 'PetStore' becomes 'PetStore.Api.Generated')")]
    public string ProjectPrefixName { get; init; }

    [CommandOption($"{CommandConstants.ArgumentLongServerDisableCodingRules}")]
    [Description("Disable ATC-Coding-Rules")]
    public bool DisableCodingRules { get; init; }
}