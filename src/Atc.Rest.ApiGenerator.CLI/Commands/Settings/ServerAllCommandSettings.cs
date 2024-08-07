namespace Atc.Rest.ApiGenerator.CLI.Commands.Settings;

public class ServerAllCommandSettings : BaseServerCommandSettings
{
    [CommandOption($"{ArgumentCommandConstants.LongServerOutputSolutionPath} <OUTPUTSLNPATH>")]
    [Description("Path to solution file (directory or file)")]
    public string OutputSlnPath { get; init; } = string.Empty;

    [CommandOption($"{ArgumentCommandConstants.LongServerOutputSourcePath} <OUTPUTSRCPATH>")]
    [Description("Path to generated src projects (directory)")]
    public string OutputSrcPath { get; init; } = string.Empty;

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

        if (string.IsNullOrEmpty(OutputSlnPath))
        {
            return ValidationResult.Error($"{nameof(OutputSlnPath)} is missing.");
        }

        return string.IsNullOrEmpty(OutputSrcPath)
            ? ValidationResult.Error($"{nameof(OutputSrcPath)} is missing.")
            : ValidationResult.Success();
    }
}