namespace Atc.Rest.ApiGenerator.CLI.Commands;

public class GenerateServerHostCommand : AsyncCommand<ServerHostCommandSettings>
{
    private readonly ILogger<GenerateServerHostCommand> logger;

    public GenerateServerHostCommand(ILogger<GenerateServerHostCommand> logger) => this.logger = logger;

    public override Task<int> ExecuteAsync(
        CommandContext context,
        ServerHostCommandSettings settings)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(settings);
        return ExecuteInternalAsync(settings);
    }

    private async Task<int> ExecuteInternalAsync(
        ServerHostCommandSettings settings)
    {
        ConsoleHelper.WriteHeader();

        DirectoryInfo? outputTestPath = null;

        if (settings.OutputTestPath is not null &&
            settings.OutputTestPath.IsSet)
        {
            outputTestPath = new DirectoryInfo(settings.OutputTestPath.Value);
        }

        var apiOptions = await ApiOptionsHelper.CreateApiOptions(settings);
        var apiDocument = OpenApiDocumentHelper.CombineAndGetApiDocument(settings.SpecificationPath);

        var usingCodingRules = settings.DisableCodingRules; // TODO: Detect

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
            logItems.AddRange(GenerateHelper.GenerateServerHost(
                settings.ProjectPrefixName,
                new DirectoryInfo(settings.OutputPath),
                outputTestPath,
                apiDocument,
                apiOptions,
                usingCodingRules,
                new DirectoryInfo(settings.ApiPath),
                new DirectoryInfo(settings.DomainPath)));

            if (logItems.HasAnyErrors())
            {
                logItems.LogAndClear(logger);
                return ConsoleExitStatusCodes.Failure;
            }

            logItems.LogAndClear(logger);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Generation failed.");
            return ConsoleExitStatusCodes.Failure;
        }

        logger.LogInformation("Done");
        return ConsoleExitStatusCodes.Success;
    }
}