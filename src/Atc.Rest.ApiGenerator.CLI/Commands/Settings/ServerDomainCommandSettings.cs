namespace Atc.Rest.ApiGenerator.CLI.Commands.Settings;

public class ServerDomainCommandSettings : BaseServerCommandSettings
{
    [CommandOption($"{ArgumentCommandConstants.ShortOutputPath}|{ArgumentCommandConstants.LongOutputPath} <OUTPUTPATH>")]
    [Description("Path to generated project (directory)")]
    public string OutputPath { get; init; } = string.Empty;

    [CommandOption($"{ArgumentCommandConstants.LongServerOutputApiPath} <APIPATH>")]
    [Description("Path to api project (directory)")]
    public string ApiPath { get; init; } = string.Empty;

    [CommandOption($"{ArgumentCommandConstants.LongServerOutputTestPath} [OUTPUTTESTPATH]")]
    [Description("Path to generated test projects (directory)")]
    public FlagValue<string>? OutputTestPath { get; init; }

    public override ValidationResult Validate()
    {
        var validationResult = base.Validate();
        if (!validationResult.Successful)
        {
            return validationResult;
        }

        if (string.IsNullOrEmpty(OutputPath))
        {
            return ValidationResult.Error($"{nameof(OutputPath)} is missing.");
        }

        return string.IsNullOrEmpty(ApiPath)
            ? ValidationResult.Error($"{nameof(ApiPath)} is missing.")
            : ValidationResult.Success();
    }
}