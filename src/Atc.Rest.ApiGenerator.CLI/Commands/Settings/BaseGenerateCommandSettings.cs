namespace Atc.Rest.ApiGenerator.CLI.Commands.Settings;

public class BaseGenerateCommandSettings : BaseConfigurationCommandSettings
{
    [CommandOption($"{CommandConstants.ArgumentShortOutputPath}|{CommandConstants.ArgumentLongOutputPath} <OUTPUTPATH>")]
    [Description("Path to generated project (directory)")]
    public string OutputPath { get; init; }

    [CommandOption($"{CommandConstants.ArgumentShortProjectPrefixName}|{CommandConstants.ArgumentLongProjectPrefixName} <PROJECTPREFIXNAME>")]
    [Description("Project prefix name (e.g. 'PetStore' becomes 'PetStore.Api.Generated')")]
    public string ProjectPrefixName { get; init; }
}