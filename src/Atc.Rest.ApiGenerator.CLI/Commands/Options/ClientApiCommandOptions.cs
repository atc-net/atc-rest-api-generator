using System.ComponentModel.DataAnnotations;
using McMaster.Extensions.CommandLineUtils;

namespace Atc.Rest.ApiGenerator.CLI.Commands.Options
{
    public class ClientApiCommandOptions : BaseGenerateCommandOptions
    {
        [Option("--clientFolderName", "If client folder is provided, generated files be placed here instead of the project root", CommandOptionType.SingleValue)]
        public string? ClientFolderName { get; set; }

        [Required]
        [Option("--outputPath", "Path to generated project (directory)", CommandOptionType.SingleValue, ShortName = "o")]
        public string? OutputPath { get; set; }

        [Option("--excludeEndpointGeneration", "Exclude endpoint generation", CommandOptionType.NoValue)]
        public bool ExcludeEndpointGeneration { get; set; }
    }
}