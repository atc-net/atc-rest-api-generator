// ReSharper disable InvertIf
namespace Atc.Rest.ApiGenerator.CLI;

[ExcludeFromCodeCoverage]
public static class Program
{
    public static Task<int> Main(
        string[] args)
    {
        ArgumentNullException.ThrowIfNull(args);

        ////args = new[]
        ////{
        ////    "generate", "server", "all",
        ////    "-p", "SletMig",
        ////    "-s", @"C:\Temp\sletmig\api.v1.yaml",
        ////    ////"-s", @"C:\Code\atc-net\atc-rest-api-generator\sample\Demo.ApiDesign\SingleFileVersion\api.v1.yaml",
        ////    "--outputSlnPath", @"C:\Temp\sletmig\crap",
        ////    "--outputSrcPath", @"C:\Temp\sletmig\crap\src",
        ////    "--outputTestPath", @"C:\Temp\sletmig\crap\test",
        ////    "--optionsPath", @"C:\Temp\sletmig\DelegateApiGeneratorOptions.json",
        ////    "-v",
        ////};

        ////args = new[]
        ////{
        ////    "generate", "server", "all",
        ////    "-p", "Demo",
        ////    "-s", @"C:\Code\atc-net\atc-rest-api-generator\sample\Demo.ApiDesign\SingleFileVersion\api.v1.yaml",
        ////    "--outputSlnPath", @"C:\Code\atc-net\atc-rest-api-generator\sample\",
        ////    "--outputSrcPath", @"C:\Code\atc-net\atc-rest-api-generator\sample\src",
        ////    "--outputTestPath", @"C:\Code\atc-net\atc-rest-api-generator\sample\test",
        ////    "--optionsPath", @"C:\Code\atc-net\atc-rest-api-generator\sample\Demo.ApiDesign\DemoApiGeneratorOptions.json",
        ////    "-v",
        ////};

        args = SetOutputPathFromDotArgumentIfNeeded(args);
        args = SetHelpArgumentIfNeeded(args);

        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
            .Build();

        var consoleLoggerConfiguration = new ConsoleLoggerConfiguration();
        configuration.GetRequiredSection("ConsoleLogger").Bind(consoleLoggerConfiguration);

        ProgramCsHelper.SetMinimumLogLevelIfNeeded(args, consoleLoggerConfiguration);

        var serviceCollection = ServiceCollectionFactory.Create(consoleLoggerConfiguration);

        serviceCollection.AddSingleton<ILogItemFactory, LogItemFactory>();
        serviceCollection.AddSingleton<IOpenApiDocumentValidator, OpenApiDocumentValidator>();
        serviceCollection.AddSingleton<IApiOperationExtractor, ApiOperationExtractor>();

        var app = CommandAppFactory.CreateWithRootCommand<RootCommand>(serviceCollection);
        app.ConfigureCommands();

        return app.RunAsync(args);
    }

    private static string[] SetOutputPathFromDotArgumentIfNeeded(
        string[] args)
    {
        if (!args.Contains(".", StringComparer.Ordinal))
        {
            return args;
        }

        var newArgs = new List<string>();
        foreach (var s in args)
        {
            if (".".Equals(s, StringComparison.Ordinal))
            {
                if (!args.Contains("all", StringComparer.OrdinalIgnoreCase) &&
                    !args.Contains("host", StringComparer.OrdinalIgnoreCase) &&
                    !args.Contains("api", StringComparer.OrdinalIgnoreCase) &&
                    !args.Contains("domain", StringComparer.OrdinalIgnoreCase))
                {
                    newArgs.Add("all");
                }

                if (newArgs.Contains("all", StringComparer.OrdinalIgnoreCase))
                {
                    if (!args.Contains(ArgumentCommandConstants.LongServerOutputSolutionPath, StringComparer.OrdinalIgnoreCase))
                    {
                        newArgs.Add(ArgumentCommandConstants.LongServerOutputSolutionPath);
                    }

                    newArgs.Add(Environment.CurrentDirectory);

                    if (!args.Contains(ArgumentCommandConstants.LongServerOutputSourcePath, StringComparer.OrdinalIgnoreCase))
                    {
                        newArgs.Add(ArgumentCommandConstants.LongServerOutputSourcePath);
                        newArgs.Add(Path.Combine(Environment.CurrentDirectory, "src"));
                    }

                    if (!args.Contains(ArgumentCommandConstants.LongServerOutputTestPath, StringComparer.OrdinalIgnoreCase))
                    {
                        newArgs.Add(ArgumentCommandConstants.LongServerOutputTestPath);
                        newArgs.Add(Path.Combine(Environment.CurrentDirectory, "test"));
                    }
                }
                else
                {
                    if (!(args.Contains(ArgumentCommandConstants.ShortOutputPath, StringComparer.OrdinalIgnoreCase) ||
                          args.Contains(ArgumentCommandConstants.LongOutputPath, StringComparer.OrdinalIgnoreCase)))
                    {
                        newArgs.Add(ArgumentCommandConstants.ShortOutputPath);
                    }

                    newArgs.Add(Environment.CurrentDirectory);
                }
            }
            else
            {
                newArgs.Add(s);
            }
        }

        if (!newArgs.Contains(ArgumentCommandConstants.LongConfigurationValidateStrictMode, StringComparer.OrdinalIgnoreCase))
        {
            newArgs.Add(ArgumentCommandConstants.LongConfigurationValidateStrictMode);
        }

        if (!newArgs.Contains(CommandConstants.ArgumentShortVerbose, StringComparer.OrdinalIgnoreCase) ||
            !newArgs.Contains(CommandConstants.ArgumentLongVerbose, StringComparer.OrdinalIgnoreCase))
        {
            newArgs.Add(CommandConstants.ArgumentShortVerbose);
        }

        return newArgs.ToArray();
    }

    private static string[] SetHelpArgumentIfNeeded(
        string[] args)
    {
        if (args.Length == 0)
        {
            return new[] { CommandConstants.ArgumentShortHelp };
        }

        // TODO: Add multiple validations
        return args;
    }
}