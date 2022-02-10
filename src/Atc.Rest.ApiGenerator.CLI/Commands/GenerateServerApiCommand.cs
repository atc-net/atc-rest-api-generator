namespace Atc.Rest.ApiGenerator.CLI.Commands;

public class GenerateServerApiCommand : AsyncCommand<ServerApiCommandSettings>
{
    private readonly ILogger<GenerateServerApiCommand> logger;

    public GenerateServerApiCommand(ILogger<GenerateServerApiCommand> logger) => this.logger = logger;

    public override Task<int> ExecuteAsync(
        CommandContext context,
        ServerApiCommandSettings settings)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(settings);
        return ExecuteInternalAsync(settings);
    }

    private async Task<int> ExecuteInternalAsync(
        ServerApiCommandSettings settings)
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

        var logItems = new List<LogKeyValueItem>();

        try
        {
            logItems.AddRange(OpenApiDocumentHelper.Validate(apiDocument, apiOptions.Validation));

            if (logItems.HasAnyErrorsLogIfNeeded(logger))
            {
                return ConsoleExitStatusCodes.Failure;
            }

            logItems.AddRange(GenerateHelper.GenerateServerApi(
                settings.ProjectPrefixName,
                new DirectoryInfo(settings.OutputPath),
                outputTestPath,
                apiDocument,
                apiOptions));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Generation failed.");
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