namespace Atc.Rest.ApiGenerator.CLI.Commands.Settings;

public class BaseOptionsCommandSettings : BaseCommandSettings
{
    [CommandOption($"{ArgumentCommandConstants.LongConfigurationOptionsPath} <OPTIONSPATH>")]
    [Description("Path to options json-file")]
    public string OptionsPath { get; init; } = string.Empty;

    public override ValidationResult Validate()
    {
        var validationResult = base.Validate();
        if (!validationResult.Successful)
        {
            return validationResult;
        }

        return string.IsNullOrEmpty(OptionsPath)
            ? ValidationResult.Error($"{nameof(OptionsPath)} is missing.")
            : ValidationResult.Success();
    }
}