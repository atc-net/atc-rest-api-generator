namespace Atc.Rest.ApiGenerator.CLI.Commands.Settings;

public class BaseServerCommandSettings : BaseGenerateCommandSettings
{
    [CommandOption($"{ArgumentCommandConstants.LongConfigurationAspNetOutputType} [ASPNETOUTPUTTYPE]")]
    [AspNetOutputType(Default = Framework.Contracts.AspNetOutputType.Mvc, Prefix = "Set AspNet output type for the generated api")]
    public FlagValue<AspNetOutputType> AspNetOutputType { get; init; } = new();

    [CommandOption($"{ArgumentCommandConstants.LongConfigurationSwaggerTheme} [SWAGGERTHEME]")]
    [SwaggerThemeMode(Default = Framework.Contracts.SwaggerThemeMode.Default, Prefix = "Set Swagger theme for the hosting api")]
    public FlagValue<SwaggerThemeMode> SwaggerThemeMode { get; init; } = new();

    public override ValidationResult Validate()
    {
        var validationResult = base.Validate();

        return validationResult.Successful
            ? ValidationResult.Success()
            : validationResult;
    }
}