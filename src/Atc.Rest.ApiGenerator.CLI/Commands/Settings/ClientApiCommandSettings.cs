namespace Atc.Rest.ApiGenerator.CLI.Commands.Settings;

public class ClientApiCommandSettings : BaseGenerateCommandSettings
{
    [CommandOption($"{CommandConstants.ArgumentShortOutputPath}|{CommandConstants.ArgumentLongOutputPath} <OUTPUTPATH>")]
    [Description("Path to generated project (directory)")]
    public string OutputPath { get; init; }

    [CommandOption($"{CommandConstants.ArgumentLongClientFolderName} <CLIENTFOLDERNAME>")]
    [Description("If client folder is provided, generated files will be placed here instead of the project root.")]
    public string ClientFolderName { get; init; } = string.Empty;

    [CommandOption(CommandConstants.ArgumentLongExcludeEndpointGeneration)]
    [Description("Exclude endpoint generation")]
    public bool ExcludeEndpointGeneration { get; init; }
}