namespace Atc.Rest.ApiGenerator.CLI.Extensions;

public static class CommandAppExtensions
{
    public static void ConfigureCommands(
        this CommandApp<RootCommand> app)
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
            ConfigureGenerateServerCommands(node);
        };

    private static Action<IConfigurator<CommandSettings>> ConfigureValidateCommands()
        => node =>
        {
            node.SetDescription("Operations related to validation of specifications.");

            node.AddCommand<ValidateSchemaCommand>("schema")
                .WithDescription("Validate OpenApi Specification");
        };

    private static void ConfigureGenerateClientCommands(
        IConfigurator<CommandSettings> node)
        => node.AddBranch("client", client =>
        {
            client.SetDescription("Operations related to generating client project(s).");

            client.AddCommand<GenerateClientCSharpCommand>("csharp")
                .WithDescription("Generate client project in C#.");
        });

    private static void ConfigureGenerateServerCommands(
        IConfigurator<CommandSettings> node)
        => node.AddBranch("server", client =>
        {
            client.SetDescription("Operations related to generating server project(s).");

            client.AddCommand<GenerateServerAllCommand>("all")
                .WithDescription("Creates API, domain and host projects.");

            client.AddCommand<GenerateServerApiCommand>("api")
                .WithDescription("Create API project.");

            client.AddCommand<GenerateServerDomainCommand>("domain")
                .WithDescription("Create domain project (requires API project).");

            client.AddCommand<GenerateServerHostCommand>("host")
                .WithDescription("Create ASP.NET Core host project (requires API and domain projects).");
        });
}