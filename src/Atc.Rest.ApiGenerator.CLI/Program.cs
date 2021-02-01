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
            ////args = new[]
            ////{
            ////    "generate",
            ////    "server",
            ////    "all",
            ////    "--validate-strictMode", "false",
            ////    "--specificationPath", @"C:\Code\atc-net\atc-rest-api-generator\sample\Demo.ApiDesign\SingleFileVersion\Api.v1.yaml",
            ////    "--projectPrefixName", "Demo",
            ////    "--outputSlnPath", @"C:\Code\atc-net\atc-rest-api-generator\sample",
            ////    "--outputSrcPath", @"C:\Code\atc-net\atc-rest-api-generator\sample",
            ////    "--outputTestPath", @"C:\Code\atc-net\atc-rest-api-generator\sample",
            ////    "--optionsPath", @"C:\Code\atc-net\atc-rest-api-generator\sample\Demo.ApiDesign\ApiGeneratorOptions.json",
            ////    "-v", "true",
            ////};

            //// ERUT - SERVER
            //args = new[]
            //{
            //    "generate",
            //    "server",
            //    "all",
            //    "--validate-strictMode", "false",
            //    "--specificationPath", @"C:\Code\LEGO-CrystalBall\data-federation-erut\src\ERUT.Api.Spec\Api.v1.yaml",
            //    "--projectPrefixName", "ERUT",
            //    "--outputSlnPath", @"C:\Code\LEGO-CrystalBall\data-federation-erut",
            //    "--outputSrcPath", @"C:\Code\LEGO-CrystalBall\data-federation-erut\src",
            //    "--outputTestPath", @"C:\Code\LEGO-CrystalBall\data-federation-erut\test",
            //    "--optionsPath", @"C:\Code\LEGO-CrystalBall\data-federation-erut\src\ERUT.Api.Spec\DelegateApiGeneratorOptions.json",
            //    "-v", "true",
            //};

            //// ERUT - CLIENT
            args = new[]
            {
                "generate",
                "client",
                "csharp",
                "--validate-strictMode", "false",
                "--specificationPath", @"C:\Code\LEGO-CrystalBall\data-federation-erut\src\ERUT.Api.Spec\Api.v1.yaml",
                "--projectPrefixName", "ERUT",
                "--outputPath", @"C:\Code\LEGO-CrystalBall\data-federation-erut\src",
                "--optionsPath", @"C:\Code\LEGO-CrystalBall\data-federation-erut\src\ERUT.Api.Spec\DelegateApiGeneratorOptions.json",
                "-v", "true",
            };

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
                Colorful.Console.WriteLine($@"Error: {ex.InnerException.Message}", Color.Red);
                return ExitStatusCodes.Failure;
            }
            catch (Exception ex)
            {
                Colorful.Console.WriteLine($@"Error: {ex.Message}", Color.Red);
                return ExitStatusCodes.Failure;
            }
        }
    }
}