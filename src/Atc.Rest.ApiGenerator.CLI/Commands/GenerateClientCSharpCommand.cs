namespace Atc.Rest.ApiGenerator.CLI.Commands;

public class GenerateClientCSharpCommand : AsyncCommand<ClientApiCommandSettings>
{
    private readonly ILogger<GenerateClientCSharpCommand> logger;

    public GenerateClientCSharpCommand(ILogger<GenerateClientCSharpCommand> logger) => this.logger = logger;

    public override Task<int> ExecuteAsync(
        CommandContext context,
        ClientApiCommandSettings settings)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(settings);
        return ExecuteInternalAsync(settings);
    }

    private async Task<int> ExecuteInternalAsync(
        ClientApiCommandSettings settings)
    {
        ConsoleHelper.WriteHeader();

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
            logItems.AddRange(GenerateHelper.GenerateServerCSharpClient(
                settings.ProjectPrefixName,
                settings.ClientFolderName,
                new DirectoryInfo(settings.OutputPath),
                apiDocument,
                settings.ExcludeEndpointGeneration,
                apiOptions,
                usingCodingRules));

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