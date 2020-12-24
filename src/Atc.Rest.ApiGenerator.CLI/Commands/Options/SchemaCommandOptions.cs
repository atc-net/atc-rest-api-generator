using McMaster.Extensions.CommandLineUtils;

namespace Atc.Rest.ApiGenerator.CLI.Commands.Options
{
    public class SchemaCommandOptions : BaseCommandOptions
    {
        [Option("--strictMode", "Use strictMode - Treat Warnings As Errors", CommandOptionType.SingleValue)]
        public bool StrictMode { get; set; }

        [Option("--operationIdCasingStyle", "Set casingStyle for operationId", CommandOptionType.SingleValue)]
        public CasingStyle OperationIdCasingStyle { get; set; } = CasingStyle.CamelCase;

        [Option("--modelNameCasingStyle", "Set casingStyle for model name", CommandOptionType.SingleValue)]
        public CasingStyle ModelNameCasingStyle { get; set; } = CasingStyle.PascalCase;

        [Option("--modelPropertyNameCasingStyle", "Set casingStyle for model property name", CommandOptionType.SingleValue)]
        public CasingStyle ModelPropertyNameCasingStyle { get; set; } = CasingStyle.CamelCase;
    }
}