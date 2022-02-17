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

    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "OK.")]
    [SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "OK.")]
    private async Task<int> ExecuteInternalAsync(
        ClientApiCommandSettings settings)
    {
        ConsoleHelper.WriteHeader();

        var apiOptions = await ApiOptionsHelper.CreateDefault(settings);
        var apiDocument = OpenApiDocumentHelper.CombineAndGetApiDocument(logger, settings.SpecificationPath);

        var usingCodingRules = !settings.DisableCodingRules; // TODO: Detect

        if (usingCodingRules &&
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

            if (!GenerateHelper.GenerateServerCSharpClient(
                    logger,
                    settings.ProjectPrefixName,
                    settings.ClientFolderName,
                    new DirectoryInfo(settings.OutputPath),
                    apiDocument,
                    settings.ExcludeEndpointGeneration,
                    apiOptions,
                    usingCodingRules))
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