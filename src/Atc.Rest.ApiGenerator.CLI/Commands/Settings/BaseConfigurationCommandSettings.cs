using Spectre.Console;

namespace Atc.Rest.ApiGenerator.CLI.Commands.Settings;

public class BaseConfigurationCommandSettings : BaseCommandSettings
{
    [CommandOption($"{CommandConstants.ArgumentLongConfigurationSpecificationPath} <SPECIFICATIONPATH>")]
    [Description("Path to Open API specification (directory, file or url)")]
    public string SpecificationPath { get; init; } = string.Empty;

    [CommandOption($"{CommandConstants.ArgumentLongConfigurationOptionsPath} [OPTIONSPATH]")]
    [Description("Path to options json-file")]
    public FlagValue<string>? OptionsPath { get; init; }

    [CommandOption($"{CommandConstants.ArgumentLongConfigurationValidateStrictMode}")]
    [Description("Use strictmode")]
    public bool StrictMode { get; init; }

    [CommandOption($"{CommandConstants.ArgumentLongConfigurationValidateOperationIdCasingStyle} [OPERATIONIDCASINGSTYLE]")]
    [CasingStyleDescription(Default = CasingStyle.CamelCase, Prefix = "Set casingStyle for operationId")]
    public FlagValue<CasingStyle> OperationIdCasingStyle { get; init; } = new ();

    [CommandOption($"{CommandConstants.ArgumentLongConfigurationValidateModelNameCasingStyle} [MODELNAMECASINGSTYLE]")]
    [CasingStyleDescription(Default = CasingStyle.PascalCase, Prefix = "Set casingStyle for model name")]
    public FlagValue<CasingStyle> ModelNameCasingStyle { get; init; } = new ();

    [CommandOption($"{CommandConstants.ArgumentLongConfigurationValidateModelPropertyNameCasingStyle} [MODELPROPERTYNAMECASINGSTYLE]")]
    [CasingStyleDescription(Default = CasingStyle.CamelCase, Prefix = "Set casingStyle for model property name")]
    public FlagValue<CasingStyle> ModelPropertyNameCasingStyle { get; init; } = new ();

    [CommandOption($"{CommandConstants.ArgumentLongConfigurationAuthorization}")]
    [Description("Use authorization")]
    public bool UseAuthorization { get; init; }

    public override ValidationResult Validate()
    {
        var validationResult = base.Validate();
        if (!validationResult.Successful)
        {
            return validationResult;
        }

        return string.IsNullOrEmpty(SpecificationPath)
            ? ValidationResult.Error("SpecificationPath is missing.")
            : ValidationResult.Success();
    }
}