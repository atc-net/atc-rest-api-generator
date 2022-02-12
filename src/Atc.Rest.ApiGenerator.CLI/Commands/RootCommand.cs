namespace Atc.Rest.ApiGenerator.CLI.Commands;

public class RootCommand : AsyncCommand<RootCommandSettings>
{
    public override Task<int> ExecuteAsync(
        CommandContext context,
        RootCommandSettings settings)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(settings);
        return ExecuteInternalAsync(settings);
    }

    private static async Task<int> ExecuteInternalAsync(
        RootCommandSettings settings)
    {
        if (settings.IsOptionValueTrue(settings.Version))
        {
            HandleVersionOption();
        }

        await Task.Delay(1);
        return ConsoleExitStatusCodes.Success;
    }

    [SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "OK.")]
    private static void HandleVersionOption()
    {
        System.Console.WriteLine(CliHelper.GetCurrentVersion().ToString());
        if (CliVersionHelper.IsLatestVersion())
        {
            return;
        }

        var latestVersion = CliVersionHelper.GetLatestVersion()!;
        System.Console.WriteLine(string.Empty);
        System.Console.WriteLine($"Version {latestVersion} of ATC-Rest-Api-Generator is available!");
        System.Console.WriteLine(string.Empty);
        System.Console.WriteLine("To update run the following command:");
        System.Console.WriteLine("   dotnet tool update --global atc-rest-api-generator");
    }
}