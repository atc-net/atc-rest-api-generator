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

    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "OK.")]
    [SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "OK.")]
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

        var apiOptions = await ApiOptionsHelper.CreateDefault(settings);
        var apiDocument = OpenApiDocumentHelper.CombineAndGetApiDocument(logger, settings.SpecificationPath);
        var shouldScaffoldCodingRules = CodingRulesHelper.ShouldScaffoldCodingRules(outputSlnPath, settings.DisableCodingRules);
        var isUsingCodingRules = CodingRulesHelper.IsUsingCodingRules(outputSlnPath, settings.DisableCodingRules);

        if (shouldScaffoldCodingRules &&
            !NetworkInformationHelper.HasConnection())
        {
            System.Console.WriteLine("This tool requires internet connection!");
            return ConsoleExitStatusCodes.Failure;
        }

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
                    isUsingCodingRules))
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
                    isUsingCodingRules,
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
                    isUsingCodingRules,
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

            if (shouldScaffoldCodingRules &&
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