namespace Atc.Rest.ApiGenerator.CLI;

[ExcludeFromCodeCoverage]
public static class Program
{
    public static Task<int> Main(
        string[] args)
    {
        ArgumentNullException.ThrowIfNull(args);

        args = SetHelpArgumentIfNeeded(args);

        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
            .Build();

        var consoleLoggerConfiguration = new ConsoleLoggerConfiguration();
        configuration.GetRequiredSection("ConsoleLogger").Bind(consoleLoggerConfiguration);

        SetMinimumLogLevelIfNeeded(args, consoleLoggerConfiguration);

        var serviceCollection = ServiceCollectionFactory.Create(consoleLoggerConfiguration);

        var app = CommandAppFactory.CreateWithRootCommand<RootCommand>(serviceCollection);
        app.ConfigureCommands();

        return app.RunAsync(args);
    }

    private static string[] SetHelpArgumentIfNeeded(
        string[] args)
    {
        if (args.Length == 0)
        {
            return new[] { CommandConstants.ArgumentShortHelp, };
        }

        // TODO: Add multiple validations
        return args;
    }

    private static void SetMinimumLogLevelIfNeeded(
        string[] args,
        ConsoleLoggerConfiguration consoleLoggerConfiguration)
    {
        if (args.Any(x => x.Equals(CommandConstants.ArgumentShortVerbose, StringComparison.OrdinalIgnoreCase)) ||
            args.Any(x => x.Equals(CommandConstants.ArgumentLongVerbose, StringComparison.OrdinalIgnoreCase)))
        {
            consoleLoggerConfiguration.MinimumLogLevel = LogLevel.Trace;
        }
    }
}