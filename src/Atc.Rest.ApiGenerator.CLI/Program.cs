using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Reflection;
using Atc.Rest.ApiGenerator.CLI.Commands;
using Microsoft.Extensions.Hosting;

namespace Atc.Rest.ApiGenerator.CLI
{
    [ExcludeFromCodeCoverage]
    public static class Program
    {
        public static int Main(string[] args)
        {
            //// ATC-DEMO - SERVER
            args = new[]
            {
                "generate",
                "server",
                "all",
                "--validate-strictMode",
                "--specificationPath", @"C:\Code\atc-net\atc-rest-api-generator\sample\Demo.ApiDesign\SingleFileVersion\Api.v1.yaml",
                "--projectPrefixName", "Demo",
                "--outputSlnPath", @"C:\Temp\@X\sample",
                "--outputSrcPath", @"C:\Temp\@X\sample",
                "--outputTestPath", @"C:\Temp\@X\sample",
                "--optionsPath", @"C:\Code\atc-net\atc-rest-api-generator\sample\Demo.ApiDesign\DemoApiGeneratorOptions.json",
                "-v",
            };

            //////// ATC-DEMO - CLIENT
            ////args = new[]
            ////{
            ////    "generate",
            ////    "client",
            ////    "csharp",
            ////    "--validate-strictMode",
            ////    "--specificationPath", @"C:\Code\atc-net\atc-rest-api-generator\sample\Demo.ApiDesign\SingleFileVersion\Api.v1.yaml",
            ////    "--projectPrefixName", "Demo",
            ////    "--clientFolderName", "DemoApiClient",
            ////    "--outputPath", @"C:\Temp\@X",
            ////    "--optionsPath", @"C:\Code\atc-net\atc-rest-api-generator\sample\Demo.ApiDesign\DemoApiGeneratorOptions.json",
            ////    "--excludeEndpointGeneration",
            ////    //"--excludeContractRequestParameterGeneration",
            ////    "-v",
            ////};

            var builder = new HostBuilder();

            try
            {
                return builder
                    .RunCommandLineApplicationAsync<RootCommand>(args)
                    .GetAwaiter()
                    .GetResult();
            }
            catch (TargetInvocationException ex) when (ex.InnerException != null)
            {
                Colorful.Console.WriteLine($"Error: {ex.InnerException.Message}", Color.Red);
                return ExitStatusCodes.Failure;
            }
            catch (Exception ex)
            {
                Colorful.Console.WriteLine($"Error: {ex.Message}", Color.Red);
                return ExitStatusCodes.Failure;
            }
        }
    }
}