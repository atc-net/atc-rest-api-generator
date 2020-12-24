using System.ComponentModel.DataAnnotations;
using McMaster.Extensions.CommandLineUtils;

namespace Atc.Rest.ApiGenerator.CLI.Commands.Options
{
    public class ServerHostCommandOptions : BaseGenerateCommandOptions
    {
        [Required]
        [Option("--outputPath", "Path to generated project (directory)", CommandOptionType.SingleValue, ShortName = "o")]
        public string? OutputPath { get; set; }

        [Option("--outputTestPath", "Path to generated test project (directory)", CommandOptionType.SingleValue)]
        public string? OutputTestPath { get; set; }

        [Required]
        [Option("--apiPath", "Path to api project (directory)", CommandOptionType.SingleValue)]
        public string? ApiPath { get; set; }

        [Required]
        [Option("--domainPath", "Path to domain project (directory)", CommandOptionType.SingleValue)]
        public string? DomainPath { get; set; }
    }
}