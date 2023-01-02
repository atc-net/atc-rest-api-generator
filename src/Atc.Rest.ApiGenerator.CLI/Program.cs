// ReSharper disable InvertIf
namespace Atc.Rest.ApiGenerator.CLI;

[ExcludeFromCodeCoverage]
public static class Program
{
    public static Task<int> Main(
        string[] args)
    {
        ////args = new[]
        ////{
        ////    "generate", "server", "all",
        ////    "-p", "ERUT",
        ////    "-s", @"D:\Code\Lego-CrystalBall\data-federation-erut\src\ERUT.Api.Spec\api.v1.yaml",
        ////    "--outputSlnPath", @"C:\Temp\SletMig\",
        ////    "--outputSrcPath", @"C:\Temp\SletMig\src",
        ////    "--outputTestPath", @"C:\Temp\SletMig\test",
        ////    "--optionsPath", @"D:\Code\Lego-CrystalBall\data-federation-erut\src\ERUT.Api.Spec\DelegateApiGeneratorOptions.json",
        ////    "--disableCodingRules",
        ////    "-v",
        ////};

        ////args = new[]
        ////{
        ////    "generate", "server", "all",
        ////    "-p", "ERUT",
        ////    "-s", @"D:\Code\Lego-CrystalBall\data-federation-erut\src\ERUT.Api.Spec\api.v1.yaml",
        ////    "--outputSlnPath", @"D:\Code\Lego-CrystalBall\data-federation-erut\",
        ////    "--outputSrcPath", @"D:\Code\Lego-CrystalBall\data-federation-erut\src",
        ////    "--outputTestPath", @"D:\Code\Lego-CrystalBall\data-federation-erut\test",
        ////    "--optionsPath", @"D:\Code\Lego-CrystalBall\data-federation-erut\src\ERUT.Api.Spec\DelegateApiGeneratorOptions.json",
        ////    "--disableCodingRules",
        ////    "-v",
        ////};

        ////args = new[]
        ////{
        ////    "generate", "client", "csharp",
        ////    "-p", "ERUT",
        ////    "-s", @"D:\Code\Lego-CrystalBall\data-federation-erut\src\ERUT.Api.Spec\api.v1.yaml",
        ////    "--outputPath", @"D:\Code\Lego-CrystalBall\data-federation-erut\src",
        ////    "--optionsPath", @"D:\Code\Lego-CrystalBall\data-federation-erut\src\ERUT.Api.Spec\ERUT.Api.Spec\DelegateApiGeneratorOptions.json",
        ////    "-v",
        ////};

        ////args = new[]
        ////{
        ////    "generate", "server", "all",
        ////    "-p", "Demo",
        ////    "-s", @"D:\Code\atc-net\atc-rest-api-generator\sample\Demo.ApiDesign\SingleFileVersion\api.v1.yaml",
        ////    "--outputSlnPath", @"D:\Code\atc-net\atc-rest-api-generator\sample\",
        ////    "--outputSrcPath", @"D:\Code\atc-net\atc-rest-api-generator\sample\src",
        ////    "--outputTestPath", @"D:\Code\atc-net\atc-rest-api-generator\sample\test",
        ////    "--optionsPath", @"D:\Code\atc-net\atc-rest-api-generator\sample\Demo.ApiDesign\DelegateApiGeneratorOptions.json",
        ////    "--disableCodingRules",
        ////    "-v",
        ////};

        ////args = new[]
        ////{
        ////    "generate", "server", "all",
        ////    "-p", "Demo",
        ////    "-s", @"D:\Code\atc-net\atc-rest-api-generator\sample\Demo.ApiDesign\SingleFileVersion\api.v1.yaml",
        ////    "--outputSlnPath", @"C:\Temp\SletMig2\",
        ////    "--outputSrcPath", @"C:\Temp\SletMig2\src",
        ////    "--outputTestPath", @"C:\Temp\SletMig2\test",
        ////    "--optionsPath", @"D:\Code\atc-net\atc-rest-api-generator\sample\Demo.ApiDesign\DelegateApiGeneratorOptions.json",
        ////    //"--disableCodingRules",
        ////    "-v",
        ////};

        ////args = new[]
        ////{
        ////    "generate", "client", "csharp",
        ////    "-p", "Demo",
        ////    "-s", @"D:\Code\atc-net\atc-rest-api-generator\sample\Demo.ApiDesign\SingleFileVersion\api.v1.yaml",
        ////    "--outputPath", @"C:\Temp\SletMig2\src",
        ////    "--optionsPath", @"D:\Code\atc-net\atc-rest-api-generator\sample\Demo.ApiDesign\DelegateApiGeneratorOptions.json",
        ////    "-v",
        ////};

        ////args = new[]
        ////{
        ////    "generate", "client", "csharp",
        ////    "-p", "ERUT",
        ////    "-s", @"D:\Code\Lego-CrystalBall\data-federation-erut\src\ERUT.Api.Spec\api.v1.yaml",
        ////    "--outputPath", @"C:\Temp\SletMig2\src",
        ////    "--optionsPath", @"D:\Code\atc-net\atc-rest-api-generator\sample\Demo.ApiDesign\DelegateApiGeneratorOptions.json",
        ////    "-v",
        ////};

        ArgumentNullException.ThrowIfNull(args);

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