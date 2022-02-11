namespace Atc.Rest.ApiGenerator.CLI.Commands;

public class GenerateServerAllCommand : AsyncCommand<ServerAllCommandSettings>
{
    private readonly ILogger<GenerateServerAllCommand> logger;

    public GenerateServerAllCommand(ILogger<GenerateServerAllCommand> logger) => this.logger = logger;

    public override Task<int> ExecuteAsync(
        CommandContext context,
        ServerAllCommandSettings settings)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(settings);
        return ExecuteInternalAsync(settings);
    }

    private async Task<int> ExecuteInternalAsync(
        ServerAllCommandSettings settings)
    {
        ConsoleHelper.WriteHeader();

        var projectPrefixName = settings.ProjectPrefixName;
        var outputSlnPath = settings.OutputSlnPath;
        var outputSrcPath = new DirectoryInfo(settings.OutputSrcPath);

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
            logItems.AddRange(GenerateHelper.GenerateServerApi(
                projectPrefixName,
                outputSrcPath,
                outputTestPath,
                apiDocument,
                apiOptions,
                usingCodingRules));

            if (logItems.HasAnyErrors())
            {
                logItems.LogAndClear(logger);
                return ConsoleExitStatusCodes.Failure;
            }

            logItems.LogAndClear(logger);
            logItems.AddRange(GenerateHelper.GenerateServerDomain(
                projectPrefixName,
                outputSrcPath,
                outputTestPath,
                apiDocument,
                apiOptions,
                usingCodingRules,
                outputSrcPath));

            if (logItems.HasAnyErrors())
            {
                logItems.LogAndClear(logger);
                return ConsoleExitStatusCodes.Failure;
            }

            logItems.LogAndClear(logger);
            logItems.AddRange(GenerateHelper.GenerateServerHost(
                projectPrefixName,
                outputSrcPath,
                outputTestPath,
                apiDocument,
                apiOptions,
                usingCodingRules,
                outputSrcPath,
                outputSrcPath));

            if (logItems.HasAnyErrors())
            {
                logItems.LogAndClear(logger);
                return ConsoleExitStatusCodes.Failure;
            }

            logItems.LogAndClear(logger);
            logItems.AddRange(GenerateHelper.GenerateServerSln(
                projectPrefixName,
                outputSlnPath,
                outputSrcPath,
                outputTestPath));

            if (logItems.HasAnyErrors())
            {
                logItems.LogAndClear(logger);
                return ConsoleExitStatusCodes.Failure;
            }

            logItems.LogAndClear(logger);
            if (!settings.DisableCodingRules)
            {
                logItems.AddRange(GenerateAtcCodingRulesHelper.Generate(
                    outputSlnPath,
                    outputSrcPath,
                    outputTestPath));

                if (logItems.HasAnyErrors())
                {
                    logItems.LogAndClear(logger);
                    return ConsoleExitStatusCodes.Failure;
                }

                logItems.LogAndClear(logger);
            }
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