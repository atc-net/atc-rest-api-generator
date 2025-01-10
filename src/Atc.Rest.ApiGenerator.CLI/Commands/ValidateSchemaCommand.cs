namespace Atc.Rest.ApiGenerator.CLI.Commands;

public class ValidateSchemaCommand : AsyncCommand<BaseSchemaCommandSettings>
{
    private readonly ILogger<ValidateSchemaCommand> logger;
    private readonly IOpenApiDocumentValidator openApiDocumentValidator;

    public ValidateSchemaCommand(
        ILogger<ValidateSchemaCommand> logger,
        IOpenApiDocumentValidator openApiDocumentValidator)
    {
        this.logger = logger;
        this.openApiDocumentValidator = openApiDocumentValidator;
    }

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
        var apiSpecificationContentReader = new ApiSpecificationContentReader();
        var apiDocumentContainer = apiSpecificationContentReader.CombineAndGetApiDocumentContainer(logger, settings.SpecificationPath);
        if (apiDocumentContainer.Exception is not null)
        {
            logger.LogError(apiDocumentContainer.Exception, $"{EmojisConstants.Error} Reading specification failed.");
            return ConsoleExitStatusCodes.Failure;
        }

        try
        {
            if (!openApiDocumentValidator.IsValid(
                    apiOptions.Validation,
                    apiOptions.IncludeDeprecatedOperations,
                    apiDocumentContainer))
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