using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Atc.Data.Models;
using Atc.Rest.ApiGenerator.CLI.Commands.Options;
using Atc.Rest.ApiGenerator.Helpers;
using McMaster.Extensions.CommandLineUtils;

// ReSharper disable LocalizableElement
namespace Atc.Rest.ApiGenerator.CLI.Commands
{
    [Command("csharp", Description = "Generate client project in C#.")]
    public class GenerateClientCSharpCommand : ClientApiCommandOptions
    {
        private const string CommandArea = "Client-CSharp";

        [SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "OK.")]
        public int OnExecute(CommandLineApplication configCmd)
        {
            ConsoleHelper.WriteHeader();

            var verboseMode = CommandLineApplicationHelper.GetVerboseMode(configCmd);
            var apiOptions = ApiOptionsHelper.CreateDefault(configCmd);
            ApiOptionsHelper.ApplyValidationOverrides(apiOptions, configCmd);

            var specificationPath = CommandLineApplicationHelper.GetSpecificationPath(configCmd);
            var apiDocument = OpenApiDocumentHelper.CombineAndGetApiDocument(specificationPath);

            var logItems = new List<LogKeyValueItem>();
            logItems.AddRange(OpenApiDocumentHelper.Validate(apiDocument, apiOptions.Validation));

            if (logItems.Any(x => x.LogCategory == LogCategoryType.Error))
            {
                return ConsoleHelper.WriteLogItemsAndExit(logItems, verboseMode, CommandArea);
            }

            var projectPrefixName = CommandLineApplicationHelper.GetProjectPrefixName(configCmd);
            var clientFolderName = CommandLineApplicationHelper.GetClientFolderName(configCmd);
            var outputPath = CommandLineApplicationHelper.GetOutputPath(configCmd);
            var excludeEndpointGeneration = CommandLineApplicationHelper.GetExcludeEndpointGeneration(configCmd);

            logItems.AddRange(GenerateHelper.GenerateServerCSharpClient(
                projectPrefixName,
                clientFolderName,
                outputPath,
                apiDocument,
                excludeEndpointGeneration,
                apiOptions));

            return ConsoleHelper.WriteLogItemsAndExit(logItems, verboseMode, CommandArea);
        }
    }
}