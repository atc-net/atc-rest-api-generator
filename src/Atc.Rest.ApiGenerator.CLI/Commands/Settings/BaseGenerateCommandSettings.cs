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

    [CommandOption($"{ArgumentCommandConstants.LongContractsLocation} [CONTRACTSLOCATION]")]
    [Description($"If contracts-localtion is provided, generated files will be placed here instead of the {ContentGeneratorConstants.Contracts} folder.")]
    public FlagValue<string>? ContractsLocation { get; init; }

    [CommandOption($"{ArgumentCommandConstants.LongEndpointsLocation} [ENDPOINTSLOCATION]")]
    [Description($"If endpoints-localtion is provided, generated files will be placed here instead of the {ContentGeneratorConstants.Endpoints} folder.")]
    public FlagValue<string>? EndpointsLocation { get; init; }

    [CommandOption(ArgumentCommandConstants.LongUsePartialClassForContracts)]
    [Description("Use Partial-Class for contracts")]
    public bool UsePartialClassForContracts { get; init; }

    [CommandOption(ArgumentCommandConstants.LongUsePartialClassForEndpoints)]
    [Description("Use Partial-Class for endpoints")]
    public bool UsePartialClassForEndpoints { get; init; }

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