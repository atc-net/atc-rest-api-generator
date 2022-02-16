namespace Atc.Rest.ApiGenerator.CLI.Commands;

public class ValidateSchemaCommand : AsyncCommand<BaseSchemaCommandSettings>
{
    private readonly ILogger<ValidateSchemaCommand> logger;

    public ValidateSchemaCommand(ILogger<ValidateSchemaCommand> logger) => this.logger = logger;

    public override Task<int> ExecuteAsync(
        CommandContext context,
        BaseSchemaCommandSettings settings)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(settings);
        return ExecuteInternalAsync(settings);
    }

    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "OK.")]
    private async Task<int> ExecuteInternalAsync(
        BaseSchemaCommandSettings settings)
    {
        ConsoleHelper.WriteHeader();

        var apiOptions = await ApiOptionsHelper.CreateDefault(settings.GetOptionsPath());
        var apiDocument = OpenApiDocumentHelper.CombineAndGetApiDocument(logger, settings.SpecificationPath);

        try
        {
            if (!OpenApiDocumentHelper.Validate(
                    logger,
                    apiDocument,
                    apiOptions.Validation))
            {
                return ConsoleExitStatusCodes.Failure;
            }
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