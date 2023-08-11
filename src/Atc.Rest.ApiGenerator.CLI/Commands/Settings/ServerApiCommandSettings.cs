namespace Atc.Rest.ApiGenerator.CLI.Commands.Settings;

public class ServerApiCommandSettings : BaseServerCommandSettings
{
    [CommandOption($"{ArgumentCommandConstants.ShortOutputPath}|{ArgumentCommandConstants.LongOutputPath} <OUTPUTPATH>")]
    [Description("Path to generated project (directory)")]
    public string OutputPath { get; init; } = string.Empty;

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

        return string.IsNullOrEmpty(OutputPath)
            ? ValidationResult.Error("OutputPath is missing.")
            : ValidationResult.Success();
    }
}