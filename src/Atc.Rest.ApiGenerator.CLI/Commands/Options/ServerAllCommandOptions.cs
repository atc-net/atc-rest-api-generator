using System.ComponentModel.DataAnnotations;
using McMaster.Extensions.CommandLineUtils;

namespace Atc.Rest.ApiGenerator.CLI.Commands.Options
{
    public class ServerAllCommandOptions : BaseGenerateCommandOptions
    {
        [Required]
        [Option("--outputSlnPath", "Path to solution file (directory or file)", CommandOptionType.SingleValue)]
        public string? OutputSlnPath { get; set; }

        [Required]
        [Option("--outputSrcPath", "Path to generated src projects (directory)", CommandOptionType.SingleValue)]
        public string? OutputSrcPath { get; set; }

        [Option("--outputTestPath", "Path to generated test projects (directory)", CommandOptionType.SingleValue)]
        public string? OutputTestPath { get; set; }
    }
}