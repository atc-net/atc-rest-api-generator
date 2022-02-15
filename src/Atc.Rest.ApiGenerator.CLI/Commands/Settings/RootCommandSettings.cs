namespace Atc.Rest.ApiGenerator.CLI.Commands.Settings;

public class RootCommandSettings : BaseCommandSettings
{
    [CommandOption(CommandConstants.ArgumentLongVersion)]
    [Description("Display version")]
    public bool? Version { get; init; }
}