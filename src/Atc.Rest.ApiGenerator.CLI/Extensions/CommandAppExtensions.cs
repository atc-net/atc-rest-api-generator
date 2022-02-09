using Atc.Rest.ApiGenerator.CLI.Commands;
using Spectre.Console.Cli;

namespace Atc.Rest.ApiGenerator.CLI.Extensions
{
    public static class CommandAppExtensions
    {
        public static void ConfigureCommands(this CommandApp<RootCommand> app)
        {
            ArgumentNullException.ThrowIfNull(app);

            app.Configure(config =>
            {
                config.AddBranch("generate", ConfigureGenerateCommands());
                config.AddBranch("validate", ConfigureValidateCommands());
            });
        }

        private static Action<IConfigurator<CommandSettings>> ConfigureGenerateCommands()
            => node =>
            {
                node.SetDescription("Operations related to generation of code.");

                ConfigureGenerateClientCommands(node);
            };

        private static Action<IConfigurator<CommandSettings>> ConfigureValidateCommands()
            => node =>
            {
                node.SetDescription("Operations related to validation of specifications.");

                node.AddCommand<ValidateSchemaCommand>("schema")
                    .WithDescription("Validate OpenApi Specification");
            };

        private static void ConfigureGenerateClientCommands(IConfigurator<CommandSettings> node)
            => node.AddBranch("client", client =>
            {
                client.SetDescription("Operations related to generating clients.");

                client.AddCommand<GenerateClientCSharpCommand>("csharp")
                    .WithDescription("Generate client project in C#.");
            });
    }
}