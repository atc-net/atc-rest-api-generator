namespace Atc.Rest.ApiGenerator.CLI.Commands.Settings;

public class ClientApiCommandSettings : BaseGenerateCommandSettings
{
    [CommandOption($"{ArgumentCommandConstants.ShortOutputPath}|{ArgumentCommandConstants.LongOutputPath} <OUTPUTPATH>")]
    [Description("Path to generated project (directory)")]
    public string OutputPath { get; init; } = string.Empty;

    [CommandOption($"{ArgumentCommandConstants.LongClientFolderName} [CLIENTFOLDERNAME]")]
    [Description("If client folder is provided, generated files will be placed here instead of the project root.")]
    public FlagValue<string>? ClientFolderName { get; init; }

    [CommandOption(ArgumentCommandConstants.LongExcludeEndpointGeneration)]
    [Description("Exclude endpoint generation")]
    public bool ExcludeEndpointGeneration { get; init; }

    public override ValidationResult Validate()
    {
        var validationResult = base.Validate();
        if (!validationResult.Successful)
        {
            return validationResult;
        }

        return string.IsNullOrEmpty(OutputPath)
            ? ValidationResult.Error($"{nameof(OutputPath)} is missing.")
            : ValidationResult.Success();
    }
}