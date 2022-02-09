// ReSharper disable LocalizableElement
namespace Atc.Rest.ApiGenerator.CLI.Commands;

public class GenerateClientCSharpCommand : AsyncCommand<ClientApiCommandSettings>
{
    private readonly ILogger<GenerateClientCSharpCommand> logger;
    ////private const string CommandArea = "Client-CSharp";

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

        var logItems = new List<LogKeyValueItem>();

        try
        {
            logItems.AddRange(OpenApiDocumentHelper.Validate(apiDocument, apiOptions.Validation));

            if (logItems.HasAnyErrorsLogIfNeeded(logger))
            {
                return ConsoleExitStatusCodes.Failure;
            }

            logItems.AddRange(GenerateHelper.GenerateServerCSharpClient(
                settings.ProjectPrefixName,
                settings.ClientFolderName,
                new DirectoryInfo(settings.OutputPath),
                apiDocument,
                settings.ExcludeEndpointGeneration,
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