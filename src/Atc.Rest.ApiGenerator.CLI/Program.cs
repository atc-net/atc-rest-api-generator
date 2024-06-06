// ReSharper disable InvertIf
namespace Atc.Rest.ApiGenerator.CLI;

[ExcludeFromCodeCoverage]
public static class Program
{
    public static Task<int> Main(
        string[] args)
    {
        //args = new[]
        //{
        //    "generate", "server", "all",
        //    "--specificationPath", @"D:\Code\atc-net\atc-rest-api-generator\test\Atc.Rest.ApiGenerator.CLI.Tests\GenericPaginationApi\GenericPaginationApi.yaml",
        //    "--projectPrefixName", "DemoOctopus",
        //    "--outputSlnPath", @"C:\Temp\SletMig\GenericPaginationApi",
        //    "--outputSrcPath", @"C:\Temp\SletMig\GenericPaginationApi\src",
        //    "--outputTestPath", @"C:\Temp\SletMig\GenericPaginationApi\test",
        //    "--verbose",
        //};

        ////args = new[]
        ////{
        ////    "generate", "server", "all",
        ////    "--specificationPath", @"C:\Temp\SletMig\SimpleExampleApi.yaml",
        ////    "--projectPrefixName", "DemoOctopus",
        ////    "--outputSlnPath", @"C:\Temp\SletMig\SimpleExampleApi\MinimalApi",
        ////    "--outputSrcPath", @"C:\Temp\SletMig\SimpleExampleApi\MinimalApi\src",
        ////    "--outputTestPath", @"C:\Temp\SletMig\SimpleExampleApi\MinimalApi\test",
        ////    "--verbose",
        ////    "--aspnet-output-type", "MinimalApi",
        ////    "--swagger-theme", "Dark",
        ////    "--useProblemDetailsAsDefaultResponseBody",
        ////};

        ////args = new[]
        ////{
        ////    "generate", "server", "all",
        ////    "--specificationPath", @"C:\Temp\SletMig\DEMO-Octopus\api.v1.yaml",
        ////    "--projectPrefixName", "DemoOctopus",
        ////    "--outputSlnPath", @"C:\Temp\SletMig\DEMO-Octopus\MinimalApi",
        ////    "--outputSrcPath", @"C:\Temp\SletMig\DEMO-Octopus\MinimalApi\src",
        ////    "--outputTestPath", @"C:\Temp\SletMig\DEMO-Octopus\MinimalApi\test",
        ////    "--verbose",
        ////    "--aspnet-output-type", "MinimalApi",
        ////    "--swagger-theme", "Dark",
        ////    "--useProblemDetailsAsDefaultResponseBody",
        ////};

        ////args = new[]
        ////{
        ////    "generate", "server", "all",
        ////    "--specificationPath", @"C:\Temp\SletMig\DEMO-Octopus\api.v1.yaml",
        ////    "--projectPrefixName", "DemoOctopus",
        ////    "--outputSlnPath", @"C:\Temp\SletMig\DEMO-Octopus\Mvc",
        ////    "--outputSrcPath", @"C:\Temp\SletMig\DEMO-Octopus\Mvc\src",
        ////    "--outputTestPath", @"C:\Temp\SletMig\DEMO-Octopus\Mvc\test",
        ////    "--verbose",
        ////    //"--useProblemDetailsAsDefaultResponseBody",
        ////};

        ////args = new[]
        ////{
        ////    "generate", "client", "csharp",
        ////    "--specificationPath", @"C:\Temp\SletMig\DEMO-Octopus\api.v1.yaml",
        ////    "--projectPrefixName", "DemoOctopus",
        ////    "--outputPath", @"C:\Temp\SletMig\DEMO-Octopus\GenOutput_Client",
        ////    "--verbose",
        ////};

        ////args = new[]
        ////{
        ////    "generate",
        ////    "server",
        ////    "all",
        ////    "-h",
        ////};

        ////args = new[]
        ////{
        ////    "generate", "server", "all",
        ////    "--specificationPath", @"D:\Code\atc-net\atc-rest-api-generator\test\Atc.Rest.ApiGenerator.CLI.Tests\DemoSampleApi\DemoSampleApi.yaml",
        ////    "--projectPrefixName", "DemoSampleApi",
        ////    "--outputSlnPath", @"C:\Users\David\AppData\Local\Temp\atc-rest-api-generator-cli-test\DemoSampleApi",
        ////    "--outputSrcPath", @"C:\Users\David\AppData\Local\Temp\atc-rest-api-generator-cli-test\DemoSampleApi\src",
        ////    "--outputTestPath", @"C:\Users\David\AppData\Local\Temp\atc-rest-api-generator-cli-test\DemoSampleApi\test",
        ////    "--verbose",
        ////};

        ////args = new[]
        ////{
        ////    "generate", "client", "csharp",
        ////    "-p", "Demo",
        ////    ////"-s", @"D:\Code\atc-net\atc-rest-api-generator\sample-mvc\Demo.ApiDesign\SingleFileVersion\api.v1.yaml",
        ////    ////"-s", @"C:\Temp\SletMig\api.v1_small.yaml",
        ////    "--outputPath", @"C:\Temp\SletMig\CSharpClient\",
        ////    "-s", @"C:\Temp\SletMig\api.v1_big.yaml",
        ////    "--optionsPath", @"D:\Code\atc-net\atc-rest-api-generator\sample-mvc\Demo.ApiDesign\DemoApiGeneratorOptions.json",
        ////    "--verbose",
        ////};

        //var useMinimalApi = true;
        //if (useMinimalApi)
        //{
        //    args = new[]
        //    {
        //        "generate", "server", "all",
        //        "-p", "Demo",
        //        ////"-s", @"D:\Code\atc-net\atc-rest-api-generator\sample-minimal\Demo.ApiDesign\api.v1.yaml",
        //        ////"-s", @"C:\Temp\SletMig\api.v1_small.yaml",
        //        "-s", @"C:\Temp\SletMig\api.v1_big.yaml",
        //        "--outputSlnPath", @"C:\Temp\SletMig\MinimalApi\",
        //        "--outputSrcPath", @"C:\Temp\SletMig\MinimalApi\src",
        //        "--outputTestPath", @"C:\Temp\SletMig\MinimalApi\test",
        //        "--optionsPath", @"D:\Code\atc-net\atc-rest-api-generator\sample-minimal\Demo.ApiDesign\DemoApiGeneratorOptions.json",
        //        "--aspnet-output-type", "MinimalApi",
        //        "--swagger-theme", "Dark",
        //        "--verbose",
        //    };
        //}
        //else
        //{
        ////args = new[]
        ////{
        ////        "generate", "server", "all",
        ////        "-p", "Demo",
        ////        "-s", @"D:\Code\atc-net\atc-rest-api-generator\sample-mvc\Demo.ApiDesign\SingleFileVersion\api.v1.yaml",
        ////        ////"-s", @"C:\Temp\SletMig\api.v1_small.yaml",
        ////        ////"-s", @"C:\Temp\SletMig\api.v1_big.yaml",
        ////        "--outputSlnPath", @"C:\Temp\SletMig\Mvc\",
        ////        "--outputSrcPath", @"C:\Temp\SletMig\Mvc\src",
        ////        "--outputTestPath", @"C:\Temp\SletMig\Mvc\test",
        ////        "--optionsPath", @"D:\Code\atc-net\atc-rest-api-generator\sample-mvc\Demo.ApiDesign\DemoApiGeneratorOptions.json",
        ////        "--aspnet-output-type", "Mvc",
        ////        "--verbose",
        ////    };
        //}

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
        serviceCollection.AddSingleton<INugetPackageReferenceProvider, NugetPackageReferenceProvider>();
        serviceCollection.AddSingleton<IAtcApiNugetClient, AtcApiNugetClient>();
        serviceCollection.AddSingleton<IAtcCodingRulesUpdater, AtcCodingRulesUpdater>();

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

        if (!newArgs.Contains(CommandConstants.ArgumentLongVerbose, StringComparer.OrdinalIgnoreCase))
        {
            newArgs.Add(CommandConstants.ArgumentLongVerbose);
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