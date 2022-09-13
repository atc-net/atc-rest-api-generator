namespace Atc.Rest.ApiGenerator.CLI.Commands.Settings;

public class BaseSchemaCommandSettings : BaseCommandSettings
{
    [CommandOption($"{ArgumentCommandConstants.ShortConfigurationSpecificationPath}|{ArgumentCommandConstants.LongConfigurationSpecificationPath} <SPECIFICATIONPATH>")]
    [Description("Path to Open API specification (directory, file or url)")]
    public string SpecificationPath { get; init; } = string.Empty;

    [CommandOption($"{ArgumentCommandConstants.LongConfigurationOptionsPath} [OPTIONSPATH]")]
    [Description("Path to options json-file")]
    public FlagValue<string>? OptionsPath { get; init; }

    public override ValidationResult Validate()
    {
        var validationResult = base.Validate();
        if (!validationResult.Successful)
        {
            return validationResult;
        }

        return string.IsNullOrEmpty(SpecificationPath)
            ? ValidationResult.Error($"{nameof(SpecificationPath)} is missing.")
            : ValidationResult.Success();
    }

    internal string GetOptionsPath()
    {
        var optionsPath = string.Empty;
        if (OptionsPath is not null && OptionsPath.IsSet)
        {
            optionsPath = OptionsPath.Value;
        }

        return optionsPath;
    }
}