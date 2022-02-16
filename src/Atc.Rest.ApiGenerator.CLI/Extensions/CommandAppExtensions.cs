namespace Atc.Rest.ApiGenerator.CLI.Extensions;

public static class CommandAppExtensions
{
    public static void ConfigureCommands(
        this CommandApp<RootCommand> app)
    {
        ArgumentNullException.ThrowIfNull(app);

        app.Configure(config =>
        {
            config.AddBranch(CommandConstants.NameOptionsFile, ConfigureOptionsFileCommands());
            config.AddBranch(NameCommandConstants.Generate, ConfigureGenerateCommands());
            config.AddBranch(NameCommandConstants.Validate, ConfigureValidateCommands());
        });
    }

    private static Action<IConfigurator<CommandSettings>> ConfigureOptionsFileCommands()
        => node =>
        {
            node.SetDescription("Commands for the options file 'ApiGeneratorOptions.json'");

            node
                .AddCommand<OptionsFileCreateCommand>(CommandConstants.NameOptionsFileCreate)
                .WithDescription("Create default options file 'ApiGeneratorOptions.json' if it doesn't exist")
                .WithExample(new[]
                {
                    CreateArgumentCommandsOptionsFileWithCreate(),
                    CreateArgumentConfigurationOptionsPath(),
                });

            node
                .AddCommand<OptionsFileValidateCommand>(CommandConstants.NameOptionsFileValidate)
                .WithDescription("Validate the options file 'ApiGeneratorOptions.json'")
                .WithExample(new[]
                {
                    CreateArgumentCommandsOptionsFileWithValidate(),
                    CreateArgumentConfigurationOptionsPath(),
                });
        };

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

            node.AddCommand<ValidateSchemaCommand>(NameCommandConstants.ValidateSchema)
                .WithDescription("Validate OpenApi Specification")
                .WithExample(new[]
                {
                    NameCommandConstants.Validate,
                    NameCommandConstants.ValidateSchema,
                    CreateArgumentConfigurationSpecificationPath(),
                });
        };

    private static void ConfigureGenerateClientCommands(
        IConfigurator<CommandSettings> node)
        => node.AddBranch(NameCommandConstants.GenerateClient, client =>
        {
            client.SetDescription("Operations related to generating client project(s).");

            client.AddCommand<GenerateClientCSharpCommand>(NameCommandConstants.GenerateClientCsharp)
                .WithDescription("Generate client project in C#.")
                .WithExample(new[]
                {
                    NameCommandConstants.Generate,
                    NameCommandConstants.GenerateClient,
                    NameCommandConstants.GenerateClientCsharp,
                    CreateArgumentConfigurationSpecificationPath(),
                });
        });

    private static void ConfigureGenerateServerCommands(
        IConfigurator<CommandSettings> node)
        => node.AddBranch(NameCommandConstants.GenerateServer, client =>
        {
            client.SetDescription("Operations related to generating server project(s).");

            client.AddCommand<GenerateServerAllCommand>(NameCommandConstants.GenerateServerAll)
                .WithDescription("Creates API, domain and host projects.")
                .WithExample(new[]
                {
                    NameCommandConstants.Generate,
                    NameCommandConstants.GenerateServer,
                    NameCommandConstants.GenerateServerAll,
                    CreateArgumentConfigurationSpecificationPath(),
                    CreateArgumentProjectPrefixName(),
                    CreateArgumentConfigurationOutputSolutionPath(),
                    CreateArgumentConfigurationSourcePath(),
                });

            client.AddCommand<GenerateServerApiCommand>(NameCommandConstants.GenerateServerApi)
                .WithDescription("Create API project.")
                .WithExample(new[]
                {
                    NameCommandConstants.Generate,
                    NameCommandConstants.GenerateServer,
                    NameCommandConstants.GenerateServerApi,
                    CreateArgumentConfigurationSpecificationPath(),
                    CreateArgumentProjectPrefixName(),
                    CreateArgumentConfigurationOutputPath(),
                });

            client.AddCommand<GenerateServerDomainCommand>(NameCommandConstants.GenerateServerDomain)
                .WithDescription("Create domain project (requires API project).")
                .WithExample(new[]
                {
                    NameCommandConstants.Generate,
                    NameCommandConstants.GenerateServer,
                    NameCommandConstants.GenerateServerDomain,
                    CreateArgumentConfigurationSpecificationPath(),
                    CreateArgumentProjectPrefixName(),
                    CreateArgumentConfigurationOutputPath(),
                });

            client.AddCommand<GenerateServerHostCommand>(NameCommandConstants.GenerateServerHost)
                .WithDescription("Create ASP.NET Core host project (requires API and domain projects).")
                .WithExample(new[]
                {
                    NameCommandConstants.Generate,
                    NameCommandConstants.GenerateServer,
                    NameCommandConstants.GenerateServerHost,
                    CreateArgumentConfigurationSpecificationPath(),
                    CreateArgumentProjectPrefixName(),
                    CreateArgumentConfigurationOutputPath(),
                    CreateArgumentConfigurationApiPath(),
                    CreateArgumentConfigurationDomainPath(),
                });
        });

    private static string CreateArgumentCommandsOptionsFileWithCreate()
        => $"{CommandConstants.NameOptionsFile} {CommandConstants.NameOptionsFileCreate}";

    private static string CreateArgumentCommandsOptionsFileWithValidate()
        => $"{CommandConstants.NameOptionsFile} {CommandConstants.NameOptionsFileValidate}";

    private static string CreateArgumentConfigurationSpecificationPath()
        => @$"{ArgumentCommandConstants.ShortConfigurationSpecificationPath} c:\temp\MyProject\api.yml";

    private static string CreateArgumentConfigurationOptionsPath()
        => @$"{ArgumentCommandConstants.LongConfigurationOptionsPath} c:\temp\MyProject\ApiGeneratorOptions.json";

    private static string CreateArgumentConfigurationOutputSolutionPath()
        => @$"{ArgumentCommandConstants.LongServerOutputSolutionPath} c:\temp\MyProject";

    private static string CreateArgumentConfigurationSourcePath()
        => @$"{ArgumentCommandConstants.LongServerOutputSourcePath} c:\temp\MyProject\src";

    private static string CreateArgumentConfigurationOutputPath()
        => @$"{ArgumentCommandConstants.ShortOutputPath} c:\temp\MyProject";

    private static string CreateArgumentConfigurationApiPath()
        => @$"{ArgumentCommandConstants.LongServerOutputApiPath} c:\temp\MyProject\src";

    private static string CreateArgumentConfigurationDomainPath()
        => @$"{ArgumentCommandConstants.LongServerOutputDomainPath} c:\temp\MyProject\src";

    private static string CreateArgumentProjectPrefixName()
        => $@"{ArgumentCommandConstants.ShortProjectPrefixName} MyApi";
}