namespace Atc.Rest.ApiGenerator.CLI.Commands;

public class ValidateSchemaCommand : AsyncCommand<BaseConfigurationCommandSettings>
{
    private readonly ILogger<ValidateSchemaCommand> logger;

    public ValidateSchemaCommand(ILogger<ValidateSchemaCommand> logger) => this.logger = logger;

    public override Task<int> ExecuteAsync(
        CommandContext context,
        BaseConfigurationCommandSettings settings)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(settings);
        return ExecuteInternalAsync(settings);
    }

    private async Task<int> ExecuteInternalAsync(
        BaseConfigurationCommandSettings settings)
    {
        ConsoleHelper.WriteHeader();

        var apiOptions = await ApiOptionsHelper.CreateApiOptions(settings);
        var apiDocument = OpenApiDocumentHelper.CombineAndGetApiDocument(settings.SpecificationPath);

        try
        {
            var logItems = new List<LogKeyValueItem>();
            logItems.AddRange(OpenApiDocumentHelper.Validate(apiDocument, apiOptions.Validation));
            if (logItems.HasAnyErrors())
            {
                logItems.LogAndClear(logger);
                return ConsoleExitStatusCodes.Failure;
            }

            logItems.LogAndClear(logger);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Validation failed.");
            return ConsoleExitStatusCodes.Failure;
        }

        logger.LogInformation("Schema validated successfully.");
        return ConsoleExitStatusCodes.Success;
    }
}