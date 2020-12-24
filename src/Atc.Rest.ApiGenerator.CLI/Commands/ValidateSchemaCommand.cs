using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Atc.Data.Models;
using Atc.Rest.ApiGenerator.CLI.Commands.Options;
using Atc.Rest.ApiGenerator.Helpers;
using McMaster.Extensions.CommandLineUtils;

// ReSharper disable LocalizableElement
namespace Atc.Rest.ApiGenerator.CLI.Commands
{
    [Command("schema", Description = "Validate OpenApi Specification.")]
    public class ValidateSchemaCommand : SchemaCommandOptions
    {
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

            return ConsoleHelper.WriteLogItemsAndExit(logItems, verboseMode, "Schema");
        }
    }
}