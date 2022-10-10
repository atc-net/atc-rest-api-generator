namespace Atc.Rest.ApiGenerator.CLI.Commands;

public class GenerateServerApiCommand : AsyncCommand<ServerApiCommandSettings>
{
    private readonly ILogger<GenerateServerApiCommand> logger;
    private readonly IApiOperationExtractor apiOperationExtractor;

    public GenerateServerApiCommand(
        ILogger<GenerateServerApiCommand> logger,
        IApiOperationExtractor apiOperationExtractor)
    {
        this.logger = logger;
        this.apiOperationExtractor = apiOperationExtractor;
    }

    public override Task<int> ExecuteAsync(
        CommandContext context,
        ServerApiCommandSettings settings)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(settings);
        return ExecuteInternalAsync(settings);
    }

    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "OK.")]
    [SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "OK.")]
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

        var apiOptions = await ApiOptionsHelper.CreateDefault(settings);
        var apiDocument = OpenApiDocumentHelper.CombineAndGetApiDocument(logger, settings.SpecificationPath);
        var shouldScaffoldCodingRules = CodingRulesHelper.ShouldScaffoldCodingRules(settings.OutputPath, settings.DisableCodingRules);
        var isUsingCodingRules = CodingRulesHelper.IsUsingCodingRules(settings.OutputPath, settings.DisableCodingRules);

        if (shouldScaffoldCodingRules &&
            !NetworkInformationHelper.HasHttpConnection())
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
                    apiOperationExtractor,
                    settings.ProjectPrefixName,
                    new DirectoryInfo(settings.OutputPath),
                    outputTestPath,
                    apiDocument,
                    apiOptions,
                    isUsingCodingRules))
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