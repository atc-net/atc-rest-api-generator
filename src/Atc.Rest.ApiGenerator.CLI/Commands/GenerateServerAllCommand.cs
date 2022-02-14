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
            if (!OpenApiDocumentHelper.Validate(
                    logger,
                    apiDocument,
                    apiOptions.Validation))
            {
                return ConsoleExitStatusCodes.Failure;
            }

            if (!GenerateHelper.GenerateServerApi(
                    logger,
                    projectPrefixName,
                    outputSrcPath,
                    outputTestPath,
                    apiDocument,
                    apiOptions,
                    usingCodingRules))
            {
                return ConsoleExitStatusCodes.Failure;
            }

            if (!GenerateHelper.GenerateServerDomain(
                    logger,
                    projectPrefixName,
                    outputSrcPath,
                    outputTestPath,
                    apiDocument,
                    apiOptions,
                    usingCodingRules,
                    outputSrcPath))
            {
                return ConsoleExitStatusCodes.Failure;
            }

            if (!GenerateHelper.GenerateServerHost(
                    logger,
                    projectPrefixName,
                    outputSrcPath,
                    outputTestPath,
                    apiDocument,
                    apiOptions,
                    usingCodingRules,
                    outputSrcPath,
                    outputSrcPath))
            {
                return ConsoleExitStatusCodes.Failure;
            }

            if (!GenerateHelper.GenerateServerSln(
                    logger,
                    projectPrefixName,
                    outputSlnPath,
                    outputSrcPath,
                    outputTestPath))
            {
                return ConsoleExitStatusCodes.Failure;
            }

            if (!settings.DisableCodingRules &&
                !GenerateAtcCodingRulesHelper.Generate(
                    logger,
                    outputSlnPath,
                    outputSrcPath,
                    outputTestPath))
            {
                return ConsoleExitStatusCodes.Failure;
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"{EmojisConstants.Error} Generation failed.");
            return ConsoleExitStatusCodes.Failure;
        }

        logger.LogInformation($"{EmojisConstants.Success} Done");
        return ConsoleExitStatusCodes.Success;
    }
}