namespace Atc.Rest.ApiGenerator.CLI.Commands;

public class OptionsFileValidateCommand : AsyncCommand<BaseOptionsCommandSettings>
{
    private readonly ILogger<OptionsFileValidateCommand> logger;

    public OptionsFileValidateCommand(ILogger<OptionsFileValidateCommand> logger) => this.logger = logger;

    public override Task<int> ExecuteAsync(
        CommandContext context,
        BaseOptionsCommandSettings settings)
    {
        ArgumentNullException.ThrowIfNull(context);
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
            (bool isSuccessful, string error) = await ApiOptionsHelper.ValidateOptionsFile(settings.OptionsPath);
            if (isSuccessful)
            {
                logger.LogInformation("The options file is valid");
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