using System;
using System.Collections.Generic;
using System.Linq;
using Atc.Data.Models;
using Atc.Rest.ApiGenerator.CLI.Commands.Options;
using Atc.Rest.ApiGenerator.Helpers;
using McMaster.Extensions.CommandLineUtils;

// ReSharper disable LocalizableElement
namespace Atc.Rest.ApiGenerator.CLI.Commands
{
    [Command("all", Description = "Creates API, domain and host projects.")]
    public class GenerateServerAllCommand : ServerAllCommandOptions
    {
        private const string CommandArea = "Server-API-Domain-Host";

        public int OnExecute(CommandLineApplication configCmd)
        {
            if (configCmd == null)
            {
                throw new ArgumentNullException(nameof(configCmd));
            }

            ConsoleHelper.WriteHeader();

            var verboseMode = CommandLineApplicationHelper.GetVerboseMode(configCmd);
            var apiOptions = ApiOptionsHelper.CreateDefault(configCmd);
            ApiOptionsHelper.ApplyValidationOverrides(apiOptions, configCmd);
            ApiOptionsHelper.ApplyGeneratorOverrides(apiOptions, configCmd);

            var specificationPath = CommandLineApplicationHelper.GetSpecificationPath(configCmd);
            var apiDocument = OpenApiDocumentHelper.CombineAndGetApiDocument(specificationPath);

            var logItems = new List<LogKeyValueItem>();
            logItems.AddRange(OpenApiDocumentHelper.Validate(apiDocument, apiOptions.Validation));

            if (logItems.Any(x => x.LogCategory == LogCategoryType.Error))
            {
                return ConsoleHelper.WriteLogItemsAndExit(logItems, verboseMode, CommandArea);
            }

            var projectPrefixName = CommandLineApplicationHelper.GetProjectPrefixName(configCmd);
            var outputSlnPath = CommandLineApplicationHelper.GetOutputSlnPath(configCmd);
            var outputSrcPath = CommandLineApplicationHelper.GetOutputSrcPath(configCmd);
            var outputTestPath = CommandLineApplicationHelper.GetOutputTestPath(configCmd);
            var useCodingRules = CommandLineApplicationHelper.GetDisableCodingRules(configCmd);

            logItems.AddRange(GenerateHelper.GenerateServerApi(
                projectPrefixName,
                outputSrcPath,
                outputTestPath,
                apiDocument,
                apiOptions));

            if (logItems.Any(x => x.LogCategory == LogCategoryType.Error))
            {
                return ConsoleHelper.WriteLogItemsAndExit(logItems, verboseMode, CommandArea);
            }

            logItems.AddRange(GenerateHelper.GenerateServerDomain(
                projectPrefixName,
                outputSrcPath,
                outputTestPath,
                apiDocument,
                apiOptions,
                outputSrcPath));

            if (logItems.Any(x => x.LogCategory == LogCategoryType.Error))
            {
                return ConsoleHelper.WriteLogItemsAndExit(logItems, verboseMode, CommandArea);
            }

            logItems.AddRange(GenerateHelper.GenerateServerHost(
                projectPrefixName,
                outputSrcPath,
                outputTestPath,
                apiDocument,
                apiOptions,
                outputSrcPath,
                outputSrcPath));

            logItems.AddRange(GenerateHelper.GenerateServerSln(
                projectPrefixName,
                outputSlnPath,
                outputSrcPath,
                outputTestPath));

            if (useCodingRules)
            {
                logItems.AddRange(GenerateAtcCodingRulesHelper.Generate(
                    outputSlnPath,
                    outputSrcPath,
                    outputTestPath));
            }

            return ConsoleHelper.WriteLogItemsAndExit(logItems, verboseMode, CommandArea);
        }
    }
}