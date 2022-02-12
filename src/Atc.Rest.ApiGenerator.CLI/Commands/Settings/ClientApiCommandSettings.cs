namespace Atc.Rest.ApiGenerator.CLI.Commands.Settings;

public class ClientApiCommandSettings : BaseGenerateCommandSettings
{
    [CommandOption($"{ArgumentCommandConstants.ShortOutputPath}|{ArgumentCommandConstants.LongOutputPath} <OUTPUTPATH>")]
    [Description("Path to generated project (directory)")]
    public string OutputPath { get; init; }

    [CommandOption($"{ArgumentCommandConstants.LongClientFolderName} <CLIENTFOLDERNAME>")]
    [Description("If client folder is provided, generated files will be placed here instead of the project root.")]
    public string ClientFolderName { get; init; } = string.Empty;

    [CommandOption(ArgumentCommandConstants.LongExcludeEndpointGeneration)]
    [Description("Exclude endpoint generation")]
    public bool ExcludeEndpointGeneration { get; init; }
}