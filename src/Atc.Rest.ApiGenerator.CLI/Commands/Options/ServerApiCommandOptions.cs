using System.ComponentModel.DataAnnotations;
using McMaster.Extensions.CommandLineUtils;

namespace Atc.Rest.ApiGenerator.CLI.Commands.Options
{
    public class ServerApiCommandOptions : BaseGenerateCommandOptions
    {
        [Required]
        [Option("--outputPath", "Path to generated project (directory)", CommandOptionType.SingleValue, ShortName = "o")]
        public string? OutputPath { get; set; }

        [Option("--outputTestPath", "Path to generated test project (directory)", CommandOptionType.SingleValue)]
        public string? OutputTestPath { get; set; }
    }
}