namespace Atc.Rest.ApiGenerator.CLI.Commands;

[SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "OK.")]
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

    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "OK.")]
    private static async Task<int> ExecuteInternalAsync(
        RootCommandSettings settings)
    {
        if (!NetworkInformationHelper.HasHttpConnection())
        {
            System.Console.WriteLine("This tool requires internet connection!");
            return ConsoleExitStatusCodes.Failure;
        }

        if (settings.IsOptionValueTrue(settings.Version))
        {
            try
            {
                HandleVersionOption();
            }
            catch
            {
                return ConsoleExitStatusCodes.Failure;
            }
        }

        await Task.Delay(1);
        return ConsoleExitStatusCodes.Success;
    }

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