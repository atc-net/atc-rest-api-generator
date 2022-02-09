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

        var logItems = new List<LogKeyValueItem>();

        try
        {
            logItems.AddRange(OpenApiDocumentHelper.Validate(apiDocument, apiOptions.Validation));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Validation failed.");
            return ConsoleExitStatusCodes.Failure;
        }

        if (logItems.HasAnyErrorsLogIfNeeded(logger))
        {
            return ConsoleExitStatusCodes.Failure;
        }

        logger.LogInformation("Done");
        return ConsoleExitStatusCodes.Success;
    }
}