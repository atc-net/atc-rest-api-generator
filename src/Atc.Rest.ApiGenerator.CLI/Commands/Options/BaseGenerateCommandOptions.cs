using System.ComponentModel.DataAnnotations;
using McMaster.Extensions.CommandLineUtils;

namespace Atc.Rest.ApiGenerator.CLI.Commands.Options
{
    public abstract class BaseGenerateCommandOptions : BaseCommandOptions
    {
        [Option("--validate-strictMode", "Use strictMode", CommandOptionType.SingleValue)]
        public bool StrictMode { get; set; }

        [Option("--validate-operationIdCasingStyle", "Set casingStyle for operationId", CommandOptionType.SingleValue)]
        public CasingStyle OperationIdCasingStyle { get; set; } = CasingStyle.CamelCase;

        [Option("--validate-modelNameCasingStyle", "Set casingStyle for model name", CommandOptionType.SingleValue)]
        public CasingStyle ModelNameCasingStyle { get; set; } = CasingStyle.PascalCase;

        [Option("--validate-modelPropertyNameCasingStyle", "Set casingStyle for model property name", CommandOptionType.SingleValue)]
        public CasingStyle ModelPropertyNameCasingStyle { get; set; } = CasingStyle.CamelCase;

        [Required]
        [Option("--projectPrefixName", "Project prefix name (e.g. 'PetStore' becomes 'PetStore.Api.Generated')", CommandOptionType.SingleValue, ShortName = "p")]
        public string? ProjectPrefixName { get; set; }

        [Option("--useNullableReferenceTypes", "Use nullable reference types in csproj", CommandOptionType.SingleValue)]
        public bool UseNullableReferenceTypes { get; set; } = true;

        [Option("--useAuthorization", "Use authorization", CommandOptionType.SingleValue)]
        public bool UseAuthorization { get; set; } = true;
    }
}