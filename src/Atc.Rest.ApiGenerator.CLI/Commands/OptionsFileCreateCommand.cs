namespace Atc.Rest.ApiGenerator.CLI.Commands;

public class OptionsFileCreateCommand : AsyncCommand<BaseOptionsCommandSettings>
{
    private readonly ILogger<OptionsFileCreateCommand> logger;

    public OptionsFileCreateCommand(ILogger<OptionsFileCreateCommand> logger) => this.logger = logger;

    public override Task<int> ExecuteAsync(
        CommandContext context,
        BaseOptionsCommandSettings settings)
    {
        ArgumentNullException.ThrowIfNull(settings);
        return ExecuteInternalAsync(settings);
    }

    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "OK.")]
    private async Task<int> ExecuteInternalAsync(
        BaseOptionsCommandSettings settings)
    {
        ConsoleHelper.WriteHeader();

        try
        {
            var (isSuccessful, error) = await ApiOptionsHelper.CreateOptionsFile(settings.OptionsPath);
            if (isSuccessful)
            {
                logger.LogInformation("The options file is created");
            }
            else
            {
                logger.LogError(error);
            }
        }
        catch (Exception ex)
        {
            logger.LogError($"{EmojisConstants.Error} {ex.GetMessage()}");
            return ConsoleExitStatusCodes.Failure;
        }

        logger.LogInformation($"{EmojisConstants.Success} Done");
        return ConsoleExitStatusCodes.Success;
    }
}