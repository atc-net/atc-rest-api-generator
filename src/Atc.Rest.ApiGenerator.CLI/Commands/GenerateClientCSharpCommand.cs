namespace Atc.Rest.ApiGenerator.CLI.Commands;

public class GenerateClientCSharpCommand : AsyncCommand<ClientApiCommandSettings>
{
    private readonly ILogger<GenerateClientCSharpCommand> logger;
    private readonly IApiOperationExtractor apiOperationExtractor;

    public GenerateClientCSharpCommand(
        ILogger<GenerateClientCSharpCommand> logger,
        IApiOperationExtractor apiOperationExtractor)
    {
        this.logger = logger;
        this.apiOperationExtractor = apiOperationExtractor;
    }

    public override Task<int> ExecuteAsync(
        CommandContext context,
        ClientApiCommandSettings settings)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(settings);
        return ExecuteInternalAsync(settings);
    }

    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "OK.")]
    [SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "OK.")]
    private async Task<int> ExecuteInternalAsync(
        ClientApiCommandSettings settings)
    {
        ConsoleHelper.WriteHeader();

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

            if (!GenerateHelper.GenerateServerCSharpClient(
                    logger,
                    apiOperationExtractor,
                    settings.ProjectPrefixName,
                    settings.ClientFolderName is not null && settings.ClientFolderName.IsSet ? settings.ClientFolderName.Value : string.Empty,
                    new DirectoryInfo(settings.OutputPath),
                    apiDocument,
                    settings.ExcludeEndpointGeneration,
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