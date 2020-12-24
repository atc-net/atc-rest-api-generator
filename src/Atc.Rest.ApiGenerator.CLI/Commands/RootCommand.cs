using System.Diagnostics.CodeAnalysis;
using McMaster.Extensions.CommandLineUtils;

// ReSharper disable once ClassNeverInstantiated.Global
namespace Atc.Rest.ApiGenerator.CLI.Commands
{
    [Subcommand(
        typeof(ValidateCommand),
        typeof(GenerateCommand))]
    public class RootCommand
    {
        [SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "OK.")]
        public int OnExecute(CommandLineApplication configCmd)
        {
            ConsoleHelper.WriteHelp(configCmd, "Specify a command");
            return ExitStatusCodes.Failure;
        }
    }
}