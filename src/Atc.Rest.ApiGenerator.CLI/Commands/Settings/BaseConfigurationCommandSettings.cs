namespace Atc.Rest.ApiGenerator.CLI.Commands.Settings;

public class BaseConfigurationCommandSettings : BaseCommandSettings
{
    [CommandOption($"{ArgumentCommandConstants.ShortConfigurationSpecificationPath}|{ArgumentCommandConstants.LongConfigurationSpecificationPath} <SPECIFICATIONPATH>")]
    [Description("Path to Open API specification (directory, file or url)")]
    public string SpecificationPath { get; init; } = string.Empty;

    [CommandOption($"{ArgumentCommandConstants.LongConfigurationOptionsPath} [OPTIONSPATH]")]
    [Description("Path to options json-file")]
    public FlagValue<string>? OptionsPath { get; init; }

    [CommandOption($"{ArgumentCommandConstants.LongConfigurationValidateStrictMode}")]
    [Description("Use strictmode")]
    public bool StrictMode { get; init; }

    [CommandOption($"{ArgumentCommandConstants.LongConfigurationValidateOperationIdValidation}")]
    [Description("Use operationId validation")]
    public bool OperationIdValidation { get; init; }

    [CommandOption($"{ArgumentCommandConstants.LongConfigurationValidateOperationIdCasingStyle} [OPERATIONIDCASINGSTYLE]")]
    [CasingStyleDescription(Default = CasingStyle.CamelCase, Prefix = "Set casingStyle for operationId")]
    public FlagValue<CasingStyle> OperationIdCasingStyle { get; init; } = new();

    [CommandOption($"{ArgumentCommandConstants.LongConfigurationValidateModelNameCasingStyle} [MODELNAMECASINGSTYLE]")]
    [CasingStyleDescription(Default = CasingStyle.PascalCase, Prefix = "Set casingStyle for model name")]
    public FlagValue<CasingStyle> ModelNameCasingStyle { get; init; } = new();

    [CommandOption($"{ArgumentCommandConstants.LongConfigurationValidateModelPropertyNameCasingStyle} [MODELPROPERTYNAMECASINGSTYLE]")]
    [CasingStyleDescription(Default = CasingStyle.CamelCase, Prefix = "Set casingStyle for model property name")]
    public FlagValue<CasingStyle> ModelPropertyNameCasingStyle { get; init; } = new();

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