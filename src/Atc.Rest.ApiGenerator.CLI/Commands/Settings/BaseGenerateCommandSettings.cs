namespace Atc.Rest.ApiGenerator.CLI.Commands.Settings;

public class BaseGenerateCommandSettings : BaseConfigurationCommandSettings
{
    [CommandOption($"{ArgumentCommandConstants.ShortProjectPrefixName}|{ArgumentCommandConstants.LongProjectPrefixName} <PROJECTPREFIXNAME>")]
    [Description("Project prefix name (e.g. 'PetStore' becomes 'PetStore.Api.Generated')")]
    public string ProjectPrefixName { get; init; } = string.Empty;

    [CommandOption($"{ArgumentCommandConstants.LongServerDisableCodingRules}")]
    [Description("Disable ATC-Coding-Rules")]
    public bool DisableCodingRules { get; init; }

    [CommandOption($"{ArgumentCommandConstants.LongUseProblemDetailsAsDefaultResponseBody}")]
    [Description("Use ProblemDetails as default responsen body")]
    public bool UseProblemDetailsAsDefaultResponseBody { get; init; }

    [CommandOption($"{ArgumentCommandConstants.LongRemoveNamespaceGroupSeparatorInGlobalUsings}")]
    [Description("Remove space between namespace groups in GlobalUsing.cs")]
    public bool RemoveNamespaceGroupSeparatorInGlobalUsings { get; init; }

    public override ValidationResult Validate()
    {
        var validationResult = base.Validate();
        if (!validationResult.Successful)
        {
            return validationResult;
        }

        return string.IsNullOrEmpty(ProjectPrefixName)
            ? ValidationResult.Error($"{nameof(ProjectPrefixName)} is missing.")
            : ValidationResult.Success();
    }
}