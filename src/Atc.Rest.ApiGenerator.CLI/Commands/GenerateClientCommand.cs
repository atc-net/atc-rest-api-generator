using System.Diagnostics.CodeAnalysis;
using McMaster.Extensions.CommandLineUtils;

// ReSharper disable LocalizableElement
namespace Atc.Rest.ApiGenerator.CLI.Commands
{
    [Command("client", Description = "Generate client project.")]
    [Subcommand(
        typeof(GenerateClientCSharpCommand))]
    public class GenerateClientCommand
    {
        [SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "OK.")]
        public int OnExecute(CommandLineApplication configCmd)
        {
            ConsoleHelper.WriteHelp(configCmd, "Specify a sub-command");
            return ExitStatusCodes.Failure;
        }
    }
}