using Atc.Console.Spectre;

// ReSharper disable ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
// ReSharper disable SuggestBaseTypeForParameter
// ReSharper disable ReturnTypeCanBeEnumerable.Local
namespace Atc.Rest.ApiGenerator.Generators;

public class ServerHostGenerator
{
    private readonly ILogger logger;
    private readonly HostProjectOptions projectOptions;

    public ServerHostGenerator(
        ILogger logger,
        HostProjectOptions projectOptions)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.projectOptions = projectOptions ?? throw new ArgumentNullException(nameof(projectOptions));
    }

    public bool Generate()
    {
        logger.LogInformation($"{AppEmojisConstants.AreaGenerateCode} Working on server host generation ({projectOptions.ProjectName})");

        if (!projectOptions.SetPropertiesAfterValidationsOfProjectReferencesPathAndFiles(logger))
        {
            return false;
        }

        ScaffoldSrc();

        if (projectOptions.PathForTestGenerate is not null)
        {
            logger.LogInformation($"{AppEmojisConstants.AreaGenerateTest} Working on server host unit-test generation ({projectOptions.ProjectName}.Tests)");
            ScaffoldTest();
            GenerateTestEndpoints();
        }

        return true;
    }

    private void ScaffoldSrc()
    {
        if (!Directory.Exists(projectOptions.PathForSrcGenerate.FullName))
        {
            Directory.CreateDirectory(projectOptions.PathForSrcGenerate.FullName);
        }

        if (projectOptions.PathForSrcGenerate.Exists &&
            projectOptions.ProjectSrcCsProj.Exists)
        {
            var hasUpdates = SolutionAndProjectHelper.EnsureLatestPackageReferencesVersionInProjFile(
                logger,
                projectOptions.ProjectSrcCsProj,
                projectOptions.ProjectSrcCsProjDisplayLocation,
                ProjectType.ServerHost,
                isTestProject: false);
            if (!hasUpdates)
            {
                logger.LogDebug($"{EmojisConstants.FileNotUpdated}   No updates for csproj");
            }
        }
        else
        {
            var projectReferences = new List<FileInfo>();
            if (projectOptions.ApiProjectSrcCsProj is not null)
            {
                projectReferences.Add(projectOptions.ApiProjectSrcCsProj);
            }

            if (projectOptions.DomainProjectSrcCsProj is not null)
            {
                projectReferences.Add(projectOptions.DomainProjectSrcCsProj);
            }

            SolutionAndProjectHelper.ScaffoldProjFile(
                logger,
                projectOptions.ProjectSrcCsProj,
                projectOptions.ProjectSrcCsProjDisplayLocation,
                ProjectType.ServerHost,
                createAsWeb: true,
                createAsTestProject: false,
                projectOptions.ProjectName,
                "net6.0",
                frameworkReferences: null,
                NugetPackageReferenceHelper.CreateForHostProject(projectOptions.UseRestExtended),
                projectReferences,
                includeApiSpecification: false,
                usingCodingRules: projectOptions.UsingCodingRules);

            ScaffoldPropertiesLaunchSettingsFile(
                projectOptions.PathForSrcGenerate,
                projectOptions.UseRestExtended);

            ScaffoldProgramFile();
            ScaffoldStartupFile();
            ScaffoldWebConfig();

            if (projectOptions.UseRestExtended)
            {
                ScaffoldConfigureSwaggerDocOptions();
            }
        }
    }

    private void ScaffoldPropertiesLaunchSettingsFile(
        DirectoryInfo pathForSrcGenerate,
        bool useExtended)
    {
        var propertiesPath = new DirectoryInfo(Path.Combine(pathForSrcGenerate.FullName, "Properties"));
        propertiesPath.Create();

        var resourceName = "Atc.Rest.ApiGenerator.Resources.launchSettings.json";
        if (useExtended)
        {
            resourceName = "Atc.Rest.ApiGenerator.Resources.launchSettingsExtended.json";
        }

        var resourceStream = typeof(ServerHostGenerator).Assembly.GetManifestResourceStream(resourceName);
        var json = resourceStream!.ToStringData();

        var file = new FileInfo(Path.Combine(propertiesPath.FullName, "launchSettings.json"));

        if (file.Exists)
        {
            logger.LogTrace($"{EmojisConstants.FileNotUpdated}   {file.FullName} nothing to update");
        }
        else
        {
            var fileDisplayLocation = file.FullName.Replace(projectOptions.PathForSrcGenerate.FullName, "src: ", StringComparison.Ordinal);
            TextFileHelper.Save(logger, file, fileDisplayLocation, json);
        }
    }

    private static MemberDeclarationSyntax CreateProgramMain()
    {
        var codeBody = SyntaxFactory.Block(
            SyntaxFactory.SingletonList<StatementSyntax>(
                SyntaxFactory.ExpressionStatement(
                    SyntaxFactory.InvocationExpression(
                        SyntaxFactory.MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            SyntaxFactory.InvocationExpression(
                                SyntaxFactory.MemberAccessExpression(
                                    SyntaxKind.SimpleMemberAccessExpression,
                                    SyntaxFactory.InvocationExpression(SyntaxFactory.IdentifierName("CreateHostBuilder"))
                                        .WithArgumentList(SyntaxArgumentListFactory.CreateWithOneItem("args")),
                                    SyntaxFactory.IdentifierName("Build"))),
                            SyntaxFactory.IdentifierName("Run"))))));

        return SyntaxFactory.MethodDeclaration(
                SyntaxFactory.PredefinedType(SyntaxTokenFactory.VoidKeyword()),
                SyntaxFactory.Identifier("Main"))
            .WithModifiers(SyntaxTokenListFactory.PublicStaticKeyword())
            .WithParameterList(
                SyntaxFactory.ParameterList(
                    SyntaxFactory.SingletonSeparatedList(
                        SyntaxFactory.Parameter(SyntaxFactory.Identifier("args"))
                            .WithType(
                                SyntaxFactory.ArrayType(
                                        SyntaxFactory.PredefinedType(
                                            SyntaxTokenFactory.StringKeyword()))
                                    .WithRankSpecifiers(
                                        SyntaxFactory.SingletonList(
                                            SyntaxFactory.ArrayRankSpecifier(
                                                SyntaxFactory.SingletonSeparatedList<ExpressionSyntax>(
                                                    SyntaxFactory.OmittedArraySizeExpression()))))))))
            .WithBody(codeBody);
    }

    private static MemberDeclarationSyntax CreateProgramHostBuilder()
    {
        var codeBody = SyntaxFactory.Block(
            SyntaxFactory.LocalDeclarationStatement(
                SyntaxFactory.VariableDeclaration(SyntaxFactory.IdentifierName("var"))
                    .WithVariables(
                        SyntaxFactory.SingletonSeparatedList(
                            SyntaxFactory.VariableDeclarator(SyntaxFactory.Identifier("builder"))
                                .WithInitializer(
                                    SyntaxFactory.EqualsValueClause(
                                        SyntaxFactory.InvocationExpression(
                                                SyntaxFactory.MemberAccessExpression(
                                                    SyntaxKind.SimpleMemberAccessExpression,
                                                    SyntaxFactory.InvocationExpression(
                                                            SyntaxMemberAccessExpressionFactory.Create("CreateDefaultBuilder", "Host"))
                                                        .WithArgumentList(
                                                            SyntaxArgumentListFactory.CreateWithOneItem("args")),
                                                    SyntaxFactory.IdentifierName("ConfigureWebHostDefaults")))
                                            .WithArgumentList(
                                                SyntaxFactory.ArgumentList(
                                                    SyntaxFactory.SingletonSeparatedList(
                                                        SyntaxFactory.Argument(
                                                            SyntaxFactory.SimpleLambdaExpression(
                                                                    SyntaxFactory.Parameter(SyntaxFactory.Identifier("webBuilder")))
                                                                .WithBlock(
                                                                    SyntaxFactory.Block(
                                                                        SyntaxFactory
                                                                            .SingletonList<StatementSyntax>(
                                                                                SyntaxFactory.ExpressionStatement(
                                                                                    SyntaxFactory.InvocationExpression(
                                                                                        SyntaxFactory.MemberAccessExpression(
                                                                                            SyntaxKind.SimpleMemberAccessExpression,
                                                                                            SyntaxFactory.IdentifierName("webBuilder"),
                                                                                            SyntaxFactory.GenericName(SyntaxFactory.Identifier("UseStartup"))
                                                                                                .WithTypeArgumentList(SyntaxTypeArgumentListFactory.CreateWithOneItem("Startup"))))))))))))))))),
            SyntaxFactory.ReturnStatement(SyntaxFactory.IdentifierName("builder")));

        return SyntaxFactory.MethodDeclaration(
                SyntaxFactory.IdentifierName(nameof(IHostBuilder)),
                SyntaxFactory.Identifier("CreateHostBuilder"))
            .WithModifiers(SyntaxTokenListFactory.PublicStaticKeyword())
            .WithParameterList(
                SyntaxFactory.ParameterList(
                    SyntaxFactory.SingletonSeparatedList(
                        SyntaxFactory.Parameter(SyntaxFactory.Identifier("args"))
                            .WithType(
                                SyntaxFactory.ArrayType(
                                        SyntaxFactory.PredefinedType(
                                            SyntaxTokenFactory.StringKeyword()))
                                    .WithRankSpecifiers(
                                        SyntaxFactory.SingletonList(
                                            SyntaxFactory.ArrayRankSpecifier(
                                                SyntaxFactory.SingletonSeparatedList<ExpressionSyntax>(
                                                    SyntaxFactory.OmittedArraySizeExpression()))))))))
            .WithBody(codeBody);
    }

    private static MemberDeclarationSyntax CreateStartupPropertyPrivateOptions(
        bool useRestExtended)
    {
        var optionTypeName = "RestApiOptions";
        if (useRestExtended)
        {
            optionTypeName = "RestApiExtendedOptions";
        }

        return SyntaxFactory.FieldDeclaration(
                SyntaxFactory.VariableDeclaration(
                        SyntaxFactory.IdentifierName(optionTypeName))
                    .WithVariables(
                        SyntaxFactory.SingletonSeparatedList(
                            SyntaxFactory.VariableDeclarator(
                                SyntaxFactory.Identifier("restApiOptions")))))
            .WithModifiers(SyntaxTokenListFactory.PrivateReadonlyKeyword());
    }

    private static MemberDeclarationSyntax CreateStartupConstructor(
        bool useRestExtended)
    {
        var optionTypeName = "RestApiOptions";
        if (useRestExtended)
        {
            optionTypeName = "RestApiExtendedOptions";
        }

        var codeBody = SyntaxFactory.Block(
            SyntaxFactory.ExpressionStatement(
                SyntaxFactory.AssignmentExpression(
                    SyntaxKind.SimpleAssignmentExpression,
                    SyntaxFactory.IdentifierName("Configuration"),
                    SyntaxFactory.IdentifierName("configuration"))),
            SyntaxFactory.ExpressionStatement(
                SyntaxFactory.AssignmentExpression(
                    SyntaxKind.SimpleAssignmentExpression,
                    SyntaxFactory.IdentifierName("restApiOptions"),
                    SyntaxFactory.ObjectCreationExpression(
                            SyntaxFactory.IdentifierName(optionTypeName))
                        .WithInitializer(
                            SyntaxFactory.InitializerExpression(
                                SyntaxKind.ObjectInitializerExpression)))),
            SyntaxFactory.ExpressionStatement(
                SyntaxFactory.InvocationExpression(
                        SyntaxMemberAccessExpressionFactory.Create("AddAssemblyPairs", "restApiOptions"))
                    .WithArgumentList(
                        SyntaxFactory.ArgumentList(
                            SyntaxFactory.SeparatedList<ArgumentSyntax>(
                                new SyntaxNodeOrToken[]
                                {
                                    SyntaxFactory.Argument(
                                        SyntaxFactory.InvocationExpression(
                                                SyntaxMemberAccessExpressionFactory.Create("GetAssembly", "Assembly"))
                                            .WithArgumentList(
                                                SyntaxFactory.ArgumentList(
                                                    SyntaxFactory.SingletonSeparatedList(
                                                        SyntaxFactory.Argument(
                                                            SyntaxFactory.TypeOfExpression(
                                                                SyntaxFactory.IdentifierName("ApiRegistration"))))))),
                                    SyntaxTokenFactory.Comma(),
                                    SyntaxFactory.Argument(
                                        SyntaxFactory.InvocationExpression(
                                                SyntaxMemberAccessExpressionFactory.Create("GetAssembly", "Assembly"))
                                            .WithArgumentList(
                                                SyntaxFactory.ArgumentList(
                                                    SyntaxFactory.SingletonSeparatedList(
                                                        SyntaxFactory.Argument(
                                                            SyntaxFactory.TypeOfExpression(
                                                                SyntaxFactory.IdentifierName("DomainRegistration"))))))),
                                })))));

        return SyntaxFactory.MethodDeclaration(
                SyntaxFactory.IdentifierName("Startup"),
                SyntaxFactory.MissingToken(SyntaxKind.IdentifierToken))
            .WithModifiers(SyntaxTokenListFactory.PublicKeyword())
            .WithParameterList(
                SyntaxFactory.ParameterList(
                    SyntaxFactory.SingletonSeparatedList(
                        SyntaxFactory.Parameter(SyntaxFactory.Identifier("configuration"))
                            .WithType(SyntaxFactory.IdentifierName("IConfiguration")))))
            .WithBody(codeBody);
    }

    private static MemberDeclarationSyntax CreateStartupPropertyPublicConfiguration()
        => SyntaxFactory.PropertyDeclaration(
                SyntaxFactory.IdentifierName("IConfiguration"),
                SyntaxFactory.Identifier("Configuration"))
            .WithModifiers(SyntaxFactory.TokenList(SyntaxTokenFactory.PublicKeyword()))
            .WithAccessorList(
                SyntaxFactory.AccessorList(
                    SyntaxFactory.SingletonList(
                        SyntaxFactory.AccessorDeclaration(
                                SyntaxKind.GetAccessorDeclaration)
                            .WithSemicolonToken(SyntaxTokenFactory.Semicolon()))));

    private static MemberDeclarationSyntax CreateStartupConfigureServices(
        bool useRestExtended)
    {
        ArgumentListSyntax argumentList;
        BlockSyntax bodyBlock;
        if (useRestExtended)
        {
            argumentList = SyntaxFactory.ArgumentList(
                SyntaxFactory.SeparatedList<ArgumentSyntax>(
                    new SyntaxNodeOrToken[]
                    {
                        SyntaxFactory.Argument(SyntaxFactory.IdentifierName("restApiOptions")),
                        SyntaxTokenFactory.Comma(),
                        SyntaxFactory.Argument(SyntaxFactory.IdentifierName("Configuration")),
                    }));

            bodyBlock = SyntaxFactory.Block(
                SyntaxFactory.ExpressionStatement(
                    SyntaxFactory.InvocationExpression(
                        SyntaxFactory.MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            SyntaxFactory.IdentifierName("services"),
                            SyntaxFactory.GenericName(
                                    SyntaxFactory.Identifier("ConfigureOptions"))
                                .WithTypeArgumentList(
                                    SyntaxFactory.TypeArgumentList(
                                        SyntaxFactory.SingletonSeparatedList<TypeSyntax>(
                                            SyntaxFactory.IdentifierName("ConfigureSwaggerDocOptions"))))))),
                SyntaxFactory.ExpressionStatement(
                    SyntaxFactory.InvocationExpression(
                            SyntaxFactory.MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                SyntaxFactory.IdentifierName("services"),
                                SyntaxFactory.GenericName(SyntaxFactory.Identifier("AddRestApi"))
                                    .WithTypeArgumentList(
                                        SyntaxFactory.TypeArgumentList(
                                            SyntaxFactory.SingletonSeparatedList<TypeSyntax>(
                                                SyntaxFactory.IdentifierName("Startup"))))))
                        .WithArgumentList(argumentList)));
        }
        else
        {
            argumentList = SyntaxFactory.ArgumentList(
                SyntaxFactory.SingletonSeparatedList(
                    SyntaxFactory.Argument(SyntaxFactory.IdentifierName("restApiOptions"))));

            bodyBlock = SyntaxFactory.Block(
                SyntaxFactory.ExpressionStatement(
                    SyntaxFactory.InvocationExpression(
                            SyntaxFactory.MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                SyntaxFactory.IdentifierName("services"),
                                SyntaxFactory.GenericName(SyntaxFactory.Identifier("AddRestApi"))
                                    .WithTypeArgumentList(
                                        SyntaxFactory.TypeArgumentList(
                                            SyntaxFactory.SingletonSeparatedList<TypeSyntax>(
                                                SyntaxFactory.IdentifierName("Startup"))))))
                        .WithArgumentList(argumentList)));
        }

        return SyntaxFactory.MethodDeclaration(
                SyntaxFactory.PredefinedType(SyntaxTokenFactory.VoidKeyword()),
                SyntaxFactory.Identifier("ConfigureServices"))
            .WithModifiers(
                SyntaxFactory.TokenList(SyntaxTokenFactory.PublicKeyword()))
            .WithParameterList(
                SyntaxFactory.ParameterList(
                    SyntaxFactory.SingletonSeparatedList(
                        SyntaxFactory.Parameter(SyntaxFactory.Identifier("services"))
                            .WithType(SyntaxFactory.IdentifierName("IServiceCollection")))))
            .WithBody(bodyBlock);
    }

    private static MemberDeclarationSyntax CreateStartupConfigure()
        => SyntaxFactory.MethodDeclaration(
                SyntaxFactory.PredefinedType(SyntaxTokenFactory.VoidKeyword()),
                SyntaxFactory.Identifier("Configure"))
            .WithModifiers(SyntaxTokenList.Create(SyntaxTokenFactory.PublicKeyword()))
            .WithParameterList(
                SyntaxFactory.ParameterList(
                    SyntaxFactory.SeparatedList<ParameterSyntax>(
                        new SyntaxNodeOrToken[]
                        {
                            SyntaxFactory.Parameter(SyntaxFactory.Identifier("app"))
                                .WithType(SyntaxFactory.IdentifierName("IApplicationBuilder")),
                            SyntaxTokenFactory.Comma(), SyntaxFactory.Parameter(SyntaxFactory.Identifier("env"))
                                .WithType(SyntaxFactory.IdentifierName("IWebHostEnvironment")),
                        })))
            .WithBody(
                SyntaxFactory.Block(
                    SyntaxFactory.SingletonList<StatementSyntax>(
                        SyntaxFactory.ExpressionStatement(
                            SyntaxFactory.InvocationExpression(
                                    SyntaxMemberAccessExpressionFactory.Create("ConfigureRestApi", "app"))
                                .WithArgumentList(
                                    SyntaxArgumentListFactory.CreateWithTwoArgumentItems(
                                        SyntaxFactory.Argument(SyntaxFactory.IdentifierName("env")),
                                        SyntaxFactory.Argument(SyntaxFactory.IdentifierName("restApiOptions"))))))));

    private static MemberDeclarationSyntax CreateWebApplicationFactoryConfigureWebHost()
        => SyntaxFactory.MethodDeclaration(
                SyntaxFactory.PredefinedType(SyntaxTokenFactory.VoidKeyword()),
                SyntaxFactory.Identifier("ConfigureWebHost"))
            .WithModifiers(
                SyntaxFactory.TokenList(
                    SyntaxTokenFactory.ProtectedKeyword(),
                    SyntaxTokenFactory.OverrideKeyword()))
            .WithParameterList(
                SyntaxFactory.ParameterList(
                    SyntaxFactory.SingletonSeparatedList(
                        SyntaxFactory.Parameter(SyntaxFactory.Identifier("builder"))
                            .WithType(SyntaxFactory.IdentifierName("IWebHostBuilder")))))
            .WithBody(
                SyntaxFactory.Block(
                    SyntaxFactory.ExpressionStatement(
                        SyntaxFactory.InvocationExpression(
                                SyntaxMemberAccessExpressionFactory.Create("ConfigureAppConfiguration", "builder"))
                            .WithArgumentList(
                                SyntaxFactory.ArgumentList(
                                    SyntaxFactory.SingletonSeparatedList(
                                        SyntaxFactory.Argument(
                                            SyntaxFactory.SimpleLambdaExpression(
                                                    SyntaxFactory.Parameter(SyntaxFactory.Identifier("config")))
                                                .WithBlock(
                                                    SyntaxFactory.Block(
                                                        SyntaxFactory.ExpressionStatement(
                                                            SyntaxFactory.InvocationExpression(
                                                                    SyntaxFactory.IdentifierName("ModifyConfiguration"))
                                                                .WithArgumentList(
                                                                    SyntaxArgumentListFactory.CreateWithOneItem("config"))),
                                                        SyntaxFactory.LocalDeclarationStatement(
                                                            SyntaxFactory.VariableDeclaration(SyntaxFactory.IdentifierName("var"))
                                                                .WithVariables(
                                                                    SyntaxFactory.SingletonSeparatedList<VariableDeclaratorSyntax>(
                                                                        SyntaxFactory.VariableDeclarator(
                                                                            SyntaxFactory.Identifier("integrationConfig"))
                                                                        .WithInitializer(
                                                                            SyntaxFactory.EqualsValueClause(
                                                                                SyntaxFactory.InvocationExpression(
                                                                                    SyntaxFactory.MemberAccessExpression(
                                                                                        SyntaxKind.SimpleMemberAccessExpression,
                                                                                        SyntaxFactory.InvocationExpression(
                                                                                            SyntaxFactory.MemberAccessExpression(
                                                                                                SyntaxKind.SimpleMemberAccessExpression,
                                                                                                SyntaxFactory.ObjectCreationExpression(SyntaxFactory.IdentifierName("ConfigurationBuilder"))
                                                                                                .WithArgumentList(
                                                                                                    SyntaxFactory.ArgumentList()),
                                                                                                SyntaxFactory.IdentifierName("AddJsonFile")))
                                                                                        .WithArgumentList(
                                                                                            SyntaxFactory.ArgumentList(
                                                                                                SyntaxFactory.SingletonSeparatedList(
                                                                                                    SyntaxFactory.Argument(
                                                                                                        SyntaxFactory.LiteralExpression(
                                                                                                            SyntaxKind.StringLiteralExpression,
                                                                                                            SyntaxFactory.Literal("appsettings.integrationtest.json")))))),
                                                                                        SyntaxFactory.IdentifierName("Build")))))))),
                                                        SyntaxFactory.ExpressionStatement(
                                                            SyntaxFactory.InvocationExpression(
                                                                    SyntaxMemberAccessExpressionFactory.Create("AddConfiguration", "config"))
                                                                .WithArgumentList(
                                                                    SyntaxArgumentListFactory.CreateWithOneItem("integrationConfig")))))))))),
                    SyntaxFactory.ExpressionStatement(
                        SyntaxFactory.InvocationExpression(
                                SyntaxMemberAccessExpressionFactory.Create("ConfigureTestServices", "builder"))
                            .WithArgumentList(
                                SyntaxFactory.ArgumentList(
                                    SyntaxFactory.SingletonSeparatedList(
                                        SyntaxFactory.Argument(
                                            SyntaxFactory.SimpleLambdaExpression(
                                                    SyntaxFactory.Parameter(
                                                        SyntaxFactory.Identifier("services")))
                                                .WithBlock(
                                                    SyntaxFactory.Block(
                                                        SyntaxFactory.ExpressionStatement(
                                                            SyntaxFactory.InvocationExpression(
                                                                    SyntaxFactory.IdentifierName("ModifyServices"))
                                                                .WithArgumentList(
                                                                    SyntaxArgumentListFactory.CreateWithOneItem("services"))),
                                                        SyntaxFactory.ExpressionStatement(
                                                            SyntaxFactory.InvocationExpression(
                                                                SyntaxFactory.MemberAccessExpression(
                                                                    SyntaxKind.SimpleMemberAccessExpression,
                                                                    SyntaxFactory.IdentifierName("services"),
                                                                    SyntaxFactory.GenericName(
                                                                            SyntaxFactory.Identifier("AddSingleton"))
                                                                        .WithTypeArgumentList(
                                                                            SyntaxTypeArgumentListFactory.CreateWithTwoItems(
                                                                                "RestApiOptions",
                                                                                "RestApiOptions"))))),
                                                        SyntaxFactory.ExpressionStatement(
                                                            SyntaxFactory.InvocationExpression(
                                                                    SyntaxMemberAccessExpressionFactory.Create("AutoRegistrateServices", "services"))
                                                                .WithArgumentList(
                                                                    SyntaxFactory.ArgumentList(
                                                                        SyntaxFactory.SeparatedList<ArgumentSyntax>(
                                                                            new SyntaxNodeOrToken[]
                                                                            {
                                                                                SyntaxFactory.Argument(
                                                                                    SyntaxFactory.PostfixUnaryExpression(
                                                                                        SyntaxKind.SuppressNullableWarningExpression,
                                                                                        SyntaxFactory.InvocationExpression(
                                                                                                SyntaxMemberAccessExpressionFactory.Create("GetAssembly", "Assembly"))
                                                                                            .WithArgumentList(
                                                                                                SyntaxFactory.ArgumentList(
                                                                                                    SyntaxFactory.SingletonSeparatedList(
                                                                                                        SyntaxFactory.Argument(
                                                                                                            SyntaxFactory.TypeOfExpression(
                                                                                                                SyntaxFactory.IdentifierName("ApiRegistration")))))))),
                                                                                SyntaxTokenFactory.Comma(),
                                                                                SyntaxFactory.Argument(
                                                                                    SyntaxFactory.PostfixUnaryExpression(
                                                                                        SyntaxKind.SuppressNullableWarningExpression,
                                                                                        SyntaxFactory.InvocationExpression(
                                                                                                SyntaxMemberAccessExpressionFactory.Create("GetAssembly", "Assembly"))
                                                                                            .WithArgumentList(
                                                                                                SyntaxFactory.ArgumentList(
                                                                                                    SyntaxFactory.SingletonSeparatedList(
                                                                                                        SyntaxFactory.Argument(
                                                                                                            SyntaxFactory.TypeOfExpression(
                                                                                                                SyntaxFactory.IdentifierName("WebApiStartupFactory")))))))),
                                                                            })))))))))))));

    private static MemberDeclarationSyntax CreateWebApplicationFactoryModifyConfiguration()
        => SyntaxFactory.MethodDeclaration(
                SyntaxFactory.PredefinedType(
                    SyntaxTokenFactory.VoidKeyword()),
                SyntaxFactory.Identifier("ModifyConfiguration"))
            .WithModifiers(
                SyntaxFactory.TokenList(
                    SyntaxTokenFactory.PartialKeyword()))
            .WithParameterList(
                SyntaxFactory.ParameterList(
                    SyntaxFactory.SingletonSeparatedList(
                        SyntaxFactory.Parameter(SyntaxFactory.Identifier("config"))
                            .WithType(SyntaxFactory.IdentifierName("IConfigurationBuilder")))))
            .WithSemicolonToken(SyntaxTokenFactory.Semicolon());

    private static MemberDeclarationSyntax CreateWebApplicationFactoryModifyServices()
        => SyntaxFactory.MethodDeclaration(
                SyntaxFactory.PredefinedType(
                    SyntaxTokenFactory.VoidKeyword()),
                SyntaxFactory.Identifier("ModifyServices"))
            .WithModifiers(SyntaxFactory.TokenList(SyntaxTokenFactory.PartialKeyword()))
            .WithParameterList(
                SyntaxFactory.ParameterList(
                    SyntaxFactory.SingletonSeparatedList(
                        SyntaxFactory.Parameter(SyntaxFactory.Identifier("services"))
                            .WithType(SyntaxFactory.IdentifierName("IServiceCollection")))))
            .WithSemicolonToken(SyntaxTokenFactory.Semicolon());

    private static MemberDeclarationSyntax CreateWebApiControllerBaseTestFactory()
        => SyntaxFactory.FieldDeclaration(
                SyntaxFactory.VariableDeclaration(SyntaxFactory.IdentifierName("WebApiStartupFactory"))
                    .WithVariables(
                        SyntaxFactory.SingletonSeparatedList(
                            SyntaxFactory.VariableDeclarator(SyntaxFactory.Identifier("Factory")))))
            .WithModifiers(
                SyntaxFactory.TokenList(
                    SyntaxTokenFactory.ProtectedKeyword(),
                    SyntaxTokenFactory.ReadOnlyKeyword()));

    private static MemberDeclarationSyntax CreateWebApiControllerBaseTestHttpClient()
        => SyntaxFactory.FieldDeclaration(
                SyntaxFactory.VariableDeclaration(SyntaxFactory.IdentifierName("HttpClient"))
                    .WithVariables(
                        SyntaxFactory.SingletonSeparatedList(
                            SyntaxFactory.VariableDeclarator(SyntaxFactory.Identifier("HttpClient")))))
            .WithModifiers(
                SyntaxFactory.TokenList(
                    SyntaxTokenFactory.ProtectedKeyword(),
                    SyntaxTokenFactory.ReadOnlyKeyword()));

    private static MemberDeclarationSyntax CreateWebApiControllerBaseTestConfiguration()
        => SyntaxFactory.FieldDeclaration(
                SyntaxFactory.VariableDeclaration(SyntaxFactory.IdentifierName("IConfiguration"))
                    .WithVariables(
                        SyntaxFactory.SingletonSeparatedList(
                            SyntaxFactory.VariableDeclarator(SyntaxFactory.Identifier("Configuration")))))
            .WithModifiers(SyntaxTokenListFactory.ProtectedReadOnlyKeyword());

    private static MemberDeclarationSyntax CreateWebApiControllerBaseTestJsonSerializerOptions()
        => SyntaxFactory.FieldDeclaration(
                SyntaxFactory.VariableDeclaration(SyntaxFactory.IdentifierName("JsonSerializerOptions"))
                    .WithVariables(
                        SyntaxFactory.SingletonSeparatedList(
                            SyntaxFactory.VariableDeclarator(SyntaxFactory.Identifier("JsonSerializerOptions")))))
            .WithModifiers(SyntaxTokenListFactory.ProtectedStaticKeyword());

    private static MemberDeclarationSyntax CreateWebApiControllerBaseTestConstructor()
        => SyntaxFactory.MethodDeclaration(
                SyntaxFactory.IdentifierName("WebApiControllerBaseTest"),
                SyntaxFactory.MissingToken(SyntaxKind.IdentifierToken))
            .WithModifiers(
                SyntaxFactory.TokenList(SyntaxTokenFactory.ProtectedKeyword()))
            .WithParameterList(
                SyntaxFactory.ParameterList(
                    SyntaxFactory.SingletonSeparatedList(
                        SyntaxFactory.Parameter(SyntaxFactory.Identifier("fixture"))
                            .WithType(SyntaxFactory.IdentifierName("WebApiStartupFactory")))))
            .WithBody(
                SyntaxFactory.Block(
                    SyntaxFactory.ExpressionStatement(
                        SyntaxFactory.AssignmentExpression(
                            SyntaxKind.SimpleAssignmentExpression,
                            SyntaxFactory.MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                SyntaxFactory.ThisExpression(),
                                SyntaxFactory.IdentifierName("Factory")),
                            SyntaxFactory.IdentifierName("fixture"))),
                    SyntaxFactory.ExpressionStatement(
                        SyntaxFactory.AssignmentExpression(
                            SyntaxKind.SimpleAssignmentExpression,
                            SyntaxFactory.MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                SyntaxFactory.ThisExpression(),
                                SyntaxFactory.IdentifierName("HttpClient")),
                            SyntaxFactory.InvocationExpression(
                                SyntaxFactory.MemberAccessExpression(
                                    SyntaxKind.SimpleMemberAccessExpression,
                                    SyntaxFactory.MemberAccessExpression(
                                        SyntaxKind.SimpleMemberAccessExpression,
                                        SyntaxFactory.ThisExpression(),
                                        SyntaxFactory.IdentifierName("Factory")),
                                    SyntaxFactory.IdentifierName("CreateClient"))))),
                    SyntaxFactory.ExpressionStatement(
                        SyntaxFactory.AssignmentExpression(
                            SyntaxKind.SimpleAssignmentExpression,
                            SyntaxFactory.MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                SyntaxFactory.ThisExpression(),
                                SyntaxFactory.IdentifierName("Configuration")),
                            SyntaxFactory.InvocationExpression(
                                SyntaxFactory.MemberAccessExpression(
                                    SyntaxKind.SimpleMemberAccessExpression,
                                    SyntaxFactory.ObjectCreationExpression(
                                            SyntaxFactory.IdentifierName("ConfigurationBuilder"))
                                        .WithArgumentList(
                                            SyntaxFactory.ArgumentList()),
                                    SyntaxFactory.IdentifierName("Build"))))),
                    SyntaxFactory.ExpressionStatement(
                        SyntaxFactory.AssignmentExpression(
                            SyntaxKind.SimpleAssignmentExpression,
                            SyntaxFactory.IdentifierName("JsonSerializerOptions"),
                            SyntaxFactory.ObjectCreationExpression(
                                    SyntaxFactory.IdentifierName("JsonSerializerOptions"))
                                .WithInitializer(
                                    SyntaxFactory.InitializerExpression(
                                        SyntaxKind.ObjectInitializerExpression,
                                        SyntaxFactory.SeparatedList<ExpressionSyntax>(
                                            new SyntaxNodeOrToken[]
                                            {
                                                SyntaxFactory.AssignmentExpression(
                                                    SyntaxKind.SimpleAssignmentExpression,
                                                    SyntaxFactory.IdentifierName("PropertyNameCaseInsensitive"),
                                                    SyntaxFactory.LiteralExpression(SyntaxKind.TrueLiteralExpression)),
                                                SyntaxTokenFactory.Comma(),
                                                SyntaxFactory.AssignmentExpression(
                                                    SyntaxKind.SimpleAssignmentExpression,
                                                    SyntaxFactory.IdentifierName("Converters"),
                                                    SyntaxFactory.InitializerExpression(
                                                        SyntaxKind.CollectionInitializerExpression,
                                                        SyntaxFactory.SingletonSeparatedList<ExpressionSyntax>(
                                                            SyntaxFactory.ObjectCreationExpression(
                                                                    SyntaxFactory.IdentifierName("JsonStringEnumConverter"))
                                                                .WithArgumentList(
                                                                    SyntaxFactory.ArgumentList())))),
                                                SyntaxTokenFactory.Comma(),
                                            })))))));

    private static MemberDeclarationSyntax CreateWebApiControllerBaseTestToJson()
        => SyntaxFactory.MethodDeclaration(
                SyntaxFactory.IdentifierName("StringContent"),
                SyntaxFactory.Identifier("ToJson"))
            .WithModifiers(SyntaxTokenListFactory.ProtectedStaticKeyword())
            .WithParameterList(
                SyntaxFactory.ParameterList(
                    SyntaxFactory.SingletonSeparatedList(
                        SyntaxFactory.Parameter(SyntaxFactory.Identifier("data"))
                            .WithType(SyntaxFactory.PredefinedType(SyntaxTokenFactory.ObjectKeyword())))))
            .WithExpressionBody(
                SyntaxFactory.ArrowExpressionClause(
                    SyntaxFactory.ObjectCreationExpression(SyntaxFactory.IdentifierName("StringContent"))
                        .WithArgumentList(
                            SyntaxFactory.ArgumentList(
                                SyntaxFactory.SeparatedList<ArgumentSyntax>(
                                    new SyntaxNodeOrToken[]
                                    {
                                        SyntaxFactory.Argument(
                                            SyntaxFactory.InvocationExpression(
                                                    SyntaxMemberAccessExpressionFactory.Create("Serialize", "JsonSerializer"))
                                                .WithArgumentList(
                                                    SyntaxArgumentListFactory.CreateWithTwoArgumentItems(
                                                        SyntaxFactory.Argument(SyntaxFactory.IdentifierName("data")),
                                                        SyntaxFactory.Argument(SyntaxFactory.IdentifierName("JsonSerializerOptions"))))),
                                        SyntaxTokenFactory.Comma(),
                                        SyntaxFactory.Argument(SyntaxMemberAccessExpressionFactory.Create("UTF8", "Encoding")),
                                        SyntaxTokenFactory.Comma(),
                                        SyntaxFactory.Argument(
                                            SyntaxFactory.LiteralExpression(
                                                SyntaxKind.StringLiteralExpression,
                                                SyntaxFactory.Literal(MediaTypeNames.Application.Json))),
                                    })))))
            .WithSemicolonToken(SyntaxTokenFactory.Semicolon());

    private static MemberDeclarationSyntax CreateWebApiControllerBaseTestJson()
        => SyntaxFactory.MethodDeclaration(
                SyntaxFactory.IdentifierName("StringContent"),
                SyntaxFactory.Identifier("Json"))
            .WithModifiers(SyntaxTokenListFactory.ProtectedStaticKeyword())
            .WithParameterList(
                SyntaxFactory.ParameterList(
                    SyntaxFactory.SingletonSeparatedList(
                        SyntaxFactory.Parameter(SyntaxFactory.Identifier("data"))
                            .WithType(SyntaxFactory.PredefinedType(SyntaxTokenFactory.StringKeyword())))))
            .WithExpressionBody(
                SyntaxFactory.ArrowExpressionClause(
                    SyntaxFactory.ObjectCreationExpression(
                            SyntaxFactory.IdentifierName("StringContent"))
                        .WithArgumentList(
                            SyntaxFactory.ArgumentList(
                                SyntaxFactory.SeparatedList<ArgumentSyntax>(
                                    new SyntaxNodeOrToken[]
                                    {
                                        SyntaxFactory.Argument(SyntaxFactory.IdentifierName("data")),
                                        SyntaxTokenFactory.Comma(),
                                        SyntaxFactory.Argument(SyntaxMemberAccessExpressionFactory.Create("UTF8", "Encoding")),
                                        SyntaxTokenFactory.Comma(),
                                        SyntaxFactory.Argument(
                                            SyntaxFactory.LiteralExpression(
                                                SyntaxKind.StringLiteralExpression,
                                                SyntaxFactory.Literal(MediaTypeNames.Application.Json))),
                                    })))))
            .WithSemicolonToken(SyntaxTokenFactory.Semicolon());

    private static MemberDeclarationSyntax CreateWebApiControllerBaseTestGetTestFile()
        => SyntaxFactory.MethodDeclaration(
                SyntaxFactory.IdentifierName("IFormFile"),
                SyntaxFactory.Identifier("GetTestFile"))
            .WithModifiers(SyntaxTokenListFactory.ProtectedStaticKeyword())
            .WithBody(
                SyntaxFactory.Block(
                    SyntaxFactory.LocalDeclarationStatement(
                        SyntaxFactory.VariableDeclaration(
                                SyntaxFactory.IdentifierName(
                                    SyntaxFactory.Identifier(
                                        SyntaxFactory.TriviaList(),
                                        SyntaxKind.VarKeyword,
                                        "var",
                                        "var",
                                        SyntaxFactory.TriviaList())))
                            .WithVariables(
                                SyntaxFactory.SingletonSeparatedList(
                                    SyntaxFactory.VariableDeclarator(SyntaxFactory.Identifier("bytes"))
                                        .WithInitializer(
                                            SyntaxFactory.EqualsValueClause(
                                                SyntaxFactory.InvocationExpression(
                                                        SyntaxFactory.MemberAccessExpression(
                                                            SyntaxKind.SimpleMemberAccessExpression,
                                                            SyntaxMemberAccessExpressionFactory.Create("UTF8", "Encoding"),
                                                            SyntaxFactory.IdentifierName("GetBytes")))
                                                    .WithArgumentList(
                                                        SyntaxFactory.ArgumentList(
                                                            SyntaxFactory.SingletonSeparatedList(
                                                                SyntaxFactory.Argument(
                                                                    SyntaxFactory.LiteralExpression(
                                                                        SyntaxKind.StringLiteralExpression,
                                                                        SyntaxFactory.Literal("Hello World"))))))))))),
                    SyntaxFactory.LocalDeclarationStatement(
                        SyntaxFactory.VariableDeclaration(
                                SyntaxFactory.IdentifierName(
                                    SyntaxFactory.Identifier(
                                        SyntaxFactory.TriviaList(),
                                        SyntaxKind.VarKeyword,
                                        "var",
                                        "var",
                                        SyntaxFactory.TriviaList())))
                            .WithVariables(
                                SyntaxFactory.SingletonSeparatedList(
                                    SyntaxFactory.VariableDeclarator(SyntaxFactory.Identifier("stream"))
                                        .WithInitializer(
                                            SyntaxFactory.EqualsValueClause(
                                                SyntaxFactory.ObjectCreationExpression(
                                                        SyntaxFactory.IdentifierName("MemoryStream"))
                                                    .WithArgumentList(
                                                        SyntaxFactory.ArgumentList(
                                                            SyntaxFactory.SingletonSeparatedList(
                                                                SyntaxFactory.Argument(
                                                                    SyntaxFactory.IdentifierName("bytes")))))))))),
                    SyntaxFactory.LocalDeclarationStatement(
                        SyntaxFactory.VariableDeclaration(
                                SyntaxFactory.IdentifierName(
                                    SyntaxFactory.Identifier(
                                        SyntaxFactory.TriviaList(),
                                        SyntaxKind.VarKeyword,
                                        "var",
                                        "var",
                                        SyntaxFactory.TriviaList())))
                            .WithVariables(
                                SyntaxFactory.SingletonSeparatedList(
                                    SyntaxFactory.VariableDeclarator(
                                            SyntaxFactory.Identifier("formFile"))
                                        .WithInitializer(
                                            SyntaxFactory.EqualsValueClause(
                                                SyntaxFactory.ObjectCreationExpression(
                                                        SyntaxFactory.IdentifierName("FormFile"))
                                                    .WithArgumentList(
                                                        SyntaxFactory.ArgumentList(
                                                            SyntaxFactory.SeparatedList<ArgumentSyntax>(
                                                                new SyntaxNodeOrToken[]
                                                                {
                                                                    SyntaxFactory.Argument(SyntaxFactory.IdentifierName("stream")),
                                                                    SyntaxTokenFactory.Comma(),
                                                                    SyntaxFactory.Argument(
                                                                        SyntaxFactory.LiteralExpression(
                                                                            SyntaxKind.NumericLiteralExpression,
                                                                            SyntaxFactory.Literal(0))),
                                                                    SyntaxTokenFactory.Comma(),
                                                                    SyntaxFactory.Argument(
                                                                        SyntaxFactory.MemberAccessExpression(
                                                                            SyntaxKind.SimpleMemberAccessExpression,
                                                                            SyntaxFactory.IdentifierName("stream"),
                                                                            SyntaxFactory.IdentifierName("Length"))),
                                                                    SyntaxTokenFactory.Comma(),
                                                                    SyntaxFactory.Argument(
                                                                        SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression)),
                                                                    SyntaxTokenFactory.Comma(),
                                                                    SyntaxFactory.Argument(
                                                                        SyntaxFactory.LiteralExpression(
                                                                            SyntaxKind.StringLiteralExpression,
                                                                            SyntaxFactory.Literal("dummy.txt"))),
                                                                })))
                                                    .WithInitializer(
                                                        SyntaxFactory.InitializerExpression(
                                                            SyntaxKind.ObjectInitializerExpression,
                                                            SyntaxFactory.SeparatedList<ExpressionSyntax>(
                                                                new SyntaxNodeOrToken[]
                                                                {
                                                                    SyntaxFactory.AssignmentExpression(
                                                                        SyntaxKind.SimpleAssignmentExpression,
                                                                        SyntaxFactory.IdentifierName("Headers"),
                                                                        SyntaxFactory.ObjectCreationExpression(SyntaxFactory.IdentifierName("HeaderDictionary"))
                                                                            .WithArgumentList(SyntaxFactory.ArgumentList())),
                                                                    SyntaxTokenFactory.Comma(),
                                                                    SyntaxFactory.AssignmentExpression(
                                                                        SyntaxKind.SimpleAssignmentExpression,
                                                                        SyntaxFactory.IdentifierName("ContentType"),
                                                                        SyntaxFactory.LiteralExpression(
                                                                            SyntaxKind.StringLiteralExpression,
                                                                            SyntaxFactory.Literal("application/octet-stream"))),
                                                                    SyntaxTokenFactory.Comma(),
                                                                })))))))),
                    SyntaxFactory.ReturnStatement(SyntaxFactory.IdentifierName("formFile"))));

    private static MemberDeclarationSyntax CreateWebApiControllerBaseTestGetTestFiles()
        => SyntaxFactory.MethodDeclaration(
                SyntaxFactory.GenericName(SyntaxFactory.Identifier("List"))
                    .WithTypeArgumentList(
                        SyntaxFactory.TypeArgumentList(
                            SyntaxFactory.SingletonSeparatedList<TypeSyntax>(
                                SyntaxFactory.IdentifierName("IFormFile")))),
                SyntaxFactory.Identifier("GetTestFiles"))
            .WithModifiers(SyntaxFactory.TokenList(SyntaxTokenFactory.ProtectedKeyword(), SyntaxTokenFactory.StaticKeyword()))
            .WithExpressionBody(
                SyntaxFactory.ArrowExpressionClause(
                    SyntaxFactory.ObjectCreationExpression(
                            SyntaxFactory.GenericName(
                                    SyntaxFactory.Identifier("List"))
                                .WithTypeArgumentList(
                                    SyntaxFactory.TypeArgumentList(
                                        SyntaxFactory.SingletonSeparatedList<TypeSyntax>(
                                            SyntaxFactory.IdentifierName("IFormFile")))))
                        .WithInitializer(
                            SyntaxFactory.InitializerExpression(
                                SyntaxKind.CollectionInitializerExpression,
                                SyntaxFactory.SeparatedList<ExpressionSyntax>(
                                    new SyntaxNodeOrToken[]
                                    {
                                        SyntaxFactory.InvocationExpression(SyntaxFactory.IdentifierName("GetTestFile")),
                                        SyntaxTokenFactory.Comma(),
                                        SyntaxFactory.InvocationExpression(SyntaxFactory.IdentifierName("GetTestFile")),
                                    })))))
            .WithSemicolonToken(SyntaxTokenFactory.Semicolon());

    private void ScaffoldTest()
    {
        if (projectOptions.PathForTestGenerate is null ||
            projectOptions.ProjectTestCsProj is null)
        {
            return;
        }

        if (projectOptions.PathForTestGenerate.Exists &&
            projectOptions.ProjectTestCsProj.Exists)
        {
            var hasUpdates = SolutionAndProjectHelper.EnsureLatestPackageReferencesVersionInProjFile(
                logger,
                projectOptions.ProjectTestCsProj,
                projectOptions.ProjectTestCsProjDisplayLocation,
                ProjectType.ServerHost,
                isTestProject: true);
            if (!hasUpdates)
            {
                logger.LogDebug($"{EmojisConstants.FileNotUpdated}   No updates for csproj");
            }
        }
        else
        {
            if (!Directory.Exists(projectOptions.PathForTestGenerate.FullName))
            {
                Directory.CreateDirectory(projectOptions.PathForTestGenerate.FullName);
            }

            var projectReferences = new List<FileInfo>();
            if (projectOptions.ApiProjectSrcCsProj is not null)
            {
                projectReferences.Add(projectOptions.ProjectSrcCsProj);
                projectReferences.Add(projectOptions.ApiProjectSrcCsProj);
            }

            if (projectOptions.DomainProjectSrcCsProj is not null)
            {
                projectReferences.Add(projectOptions.DomainProjectSrcCsProj);
            }

            SolutionAndProjectHelper.ScaffoldProjFile(
                logger,
                projectOptions.ProjectTestCsProj,
                projectOptions.ProjectTestCsProjDisplayLocation,
                ProjectType.ServerHost,
                createAsWeb: false,
                createAsTestProject: true,
                $"{projectOptions.ProjectName}.Tests",
                "net6.0",
                frameworkReferences: null,
                NugetPackageReferenceHelper.CreateForTestProject(true),
                projectReferences,
                includeApiSpecification: true,
                usingCodingRules: projectOptions.UsingCodingRules);
        }

        GenerateTestWebApiStartupFactory();
        GenerateTestWebApiControllerBaseTest();
        ScaffoldAppSettingsIntegrationTest();
    }

    private void GenerateTestEndpoints()
    {
        var apiProjectOptions = new ApiProjectOptions(
            projectOptions.ApiProjectSrcPath,
            projectTestGeneratePath: null,
            projectOptions.Document,
            projectOptions.DocumentFile,
            projectOptions.ProjectName.Replace(".Api", string.Empty, StringComparison.Ordinal),
            "Api.Generated",
            projectOptions.ApiOptions,
            projectOptions.UsingCodingRules);

        var operationSchemaMappings = OpenApiOperationSchemaMapHelper.CollectMappings(projectOptions.Document);
        var sgEndpointControllers = new List<SyntaxGeneratorEndpointControllers>();
        foreach (var segmentName in projectOptions.BasePathSegmentNames)
        {
            var generator = new SyntaxGeneratorEndpointControllers(logger, apiProjectOptions, operationSchemaMappings, segmentName);
            generator.GenerateCode();
            sgEndpointControllers.Add(generator);
        }

        foreach (var sgEndpointController in sgEndpointControllers)
        {
            var metadataForMethods = sgEndpointController.GetMetadataForMethods();
            foreach (var endpointMethodMetadata in metadataForMethods)
            {
                GenerateServerApiXunitTestEndpointHandlerStubHelper.Generate(logger, projectOptions, endpointMethodMetadata);
                GenerateServerApiXunitTestEndpointTestHelper.Generate(logger, projectOptions, endpointMethodMetadata);
            }
        }
    }

    private void ScaffoldProgramFile()
    {
        // Create compilationUnit
        var compilationUnit = SyntaxFactory.CompilationUnit();

        // Create a namespace
        var @namespace = SyntaxProjectFactory.CreateNamespace(projectOptions, false);

        // Create class
        var classDeclaration = SyntaxClassDeclarationFactory.CreateAsPublicStatic("Program");

        // Create method
        var methodDeclarationMain = CreateProgramMain();
        var methodDeclarationHostBuilder = CreateProgramHostBuilder();

        // Add method to class
        classDeclaration = classDeclaration.AddMembers(methodDeclarationMain);
        classDeclaration = classDeclaration.AddMembers(methodDeclarationHostBuilder);

        // Add class to namespace
        @namespace = @namespace.AddMembers(classDeclaration);

        // Add namespace to compilationUnit
        compilationUnit = compilationUnit.AddMembers(@namespace);

        // Add using to compilationUnit
        compilationUnit = compilationUnit.AddUsingStatements(ProjectHostFactory.CreateUsingListForProgram());

        var codeAsString = compilationUnit
            .NormalizeWhitespace()
            .ToFullString()
            .EnsureEnvironmentNewLines();

        var file = new FileInfo(Path.Combine(projectOptions.PathForSrcGenerate.FullName, "Program.cs"));
        if (file.Exists)
        {
            logger.LogTrace($"{EmojisConstants.FileNotUpdated}   {file.FullName} nothing to update");
        }
        else
        {
            var fileDisplayLocation = file.FullName.Replace(projectOptions.PathForSrcGenerate.FullName, "src: ", StringComparison.Ordinal);
            TextFileHelper.Save(logger, file, fileDisplayLocation, codeAsString);
        }
    }

    private void ScaffoldStartupFile()
    {
        // Create compilationUnit
        var compilationUnit = SyntaxFactory.CompilationUnit();

        // Create a namespace
        var @namespace = SyntaxProjectFactory.CreateNamespace(projectOptions, false);

        // Create class
        var classDeclaration = SyntaxClassDeclarationFactory.Create("Startup");

        // Create Member
        var memberDeclarationPropertyPrivateOptions = CreateStartupPropertyPrivateOptions(projectOptions.UseRestExtended);
        var memberDeclarationConstructor = CreateStartupConstructor(projectOptions.UseRestExtended);
        var memberDeclarationPropertyPublicConfiguration = CreateStartupPropertyPublicConfiguration();
        var memberDeclarationConfigureServices = CreateStartupConfigureServices(projectOptions.UseRestExtended);
        var memberDeclarationConfigure = CreateStartupConfigure();

        // Add member to class
        classDeclaration = classDeclaration.AddMembers(memberDeclarationPropertyPrivateOptions);
        classDeclaration = classDeclaration.AddMembers(memberDeclarationConstructor);
        classDeclaration = classDeclaration.AddMembers(memberDeclarationPropertyPublicConfiguration);
        classDeclaration = classDeclaration.AddMembers(memberDeclarationConfigureServices);
        classDeclaration = classDeclaration.AddMembers(memberDeclarationConfigure);

        // Add class to namespace
        @namespace = @namespace.AddMembers(classDeclaration);

        // Add namespace to compilationUnit
        compilationUnit = compilationUnit.AddMembers(@namespace);

        // Add using to compilationUnit
        compilationUnit = compilationUnit.AddUsingStatements(ProjectHostFactory.CreateUsingListForStartup(
            projectOptions.ProjectName,
            projectOptions.UseRestExtended));

        var codeAsString = compilationUnit
            .NormalizeWhitespace()
            .ToFullString()
            .EnsureEnvironmentNewLines()
            .FormatAutoPropertiesOnOneLine()
            .FormatRemoveEmptyBracketsInitialize()
            .FormatPublicPrivateLines();

        var file = new FileInfo(Path.Combine(projectOptions.PathForSrcGenerate.FullName, "Startup.cs"));

        if (file.Exists)
        {
            logger.LogTrace($"{EmojisConstants.FileNotUpdated}   {file.FullName} nothing to update");
        }
        else
        {
            var fileDisplayLocation = file.FullName.Replace(projectOptions.PathForSrcGenerate.FullName, "src: ", StringComparison.Ordinal);
            TextFileHelper.Save(logger, file, fileDisplayLocation, codeAsString);
        }
    }

    private void ScaffoldWebConfig()
    {
        var sb = new StringBuilder();
        sb.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
        sb.AppendLine("<configuration>");
        sb.AppendLine("  <system.webServer>");
        sb.AppendLine("    <security>");
        sb.AppendLine("      <requestFiltering>");
        sb.AppendLine("        <requestLimits maxAllowedContentLength=\"2147483647\" />");
        sb.AppendLine("      </requestFiltering>");
        sb.AppendLine("    </security>");
        sb.AppendLine("  </system.webServer>");
        sb.AppendLine("</configuration>");

        var file = new FileInfo(Path.Combine(projectOptions.PathForSrcGenerate.FullName, "web.config"));
        if (file.Exists)
        {
            logger.LogTrace($"{EmojisConstants.FileNotUpdated}   {file.FullName} nothing to update");
        }
        else
        {
            var fileDisplayLocation = file.FullName.Replace(projectOptions.PathForSrcGenerate.FullName, "src: ", StringComparison.Ordinal);
            TextFileHelper.Save(logger, file, fileDisplayLocation, sb.ToString());
        }
    }

    // TODO: FIX THIS - Use CompilationUnit
    private void ScaffoldConfigureSwaggerDocOptions()
{
    var fullNamespace = string.IsNullOrEmpty(projectOptions.ClientFolderName)
        ? $"{projectOptions.ProjectName}"
        : $"{projectOptions.ProjectName}.{projectOptions.ClientFolderName}";

    var syntaxGenerator = new SyntaxGeneratorSwaggerDocOptions(fullNamespace, projectOptions.Document);
    var file = new FileInfo(Path.Combine(projectOptions.PathForSrcGenerate.FullName, "ConfigureSwaggerDocOptions.cs"));

    var sb = new StringBuilder();
    GenerateCodeHelper.AppendGeneratedCodeWarningComment(sb, projectOptions.ToolNameAndVersion);
    sb.Append(syntaxGenerator.GenerateCode());
    var fileDisplayLocation = file.FullName.Replace(projectOptions.PathForSrcGenerate.FullName, "src: ", StringComparison.Ordinal);
    TextFileHelper.Save(logger, file, fileDisplayLocation, sb.ToString());
}

    private void GenerateTestWebApiStartupFactory()
    {
        // Create compilationUnit
        var compilationUnit = SyntaxFactory.CompilationUnit();

        // Create a namespace
        var @namespace = SyntaxProjectFactory.CreateNamespace(projectOptions, "Tests");

        // Create class
        var classDeclaration = SyntaxClassDeclarationFactory.CreateAsPublicPartial("WebApiStartupFactory")
            .AddGeneratedCodeAttribute(projectOptions.ToolName, projectOptions.ToolVersion.ToString())
            .WithBaseList(
                SyntaxFactory.BaseList(
                    SyntaxFactory.SingletonSeparatedList<BaseTypeSyntax>(
                        SyntaxFactory.SimpleBaseType(
                            SyntaxFactory.GenericName(
                                    SyntaxFactory.Identifier("WebApplicationFactory"))
                                .WithTypeArgumentList(
                                    SyntaxFactory.TypeArgumentList(
                                        SyntaxFactory.SingletonSeparatedList<TypeSyntax>(
                                            SyntaxFactory.IdentifierName("Startup"))))))))
            .WithLeadingTrivia(SyntaxDocumentationFactory.CreateSummary(new[]
            {
                "Factory for bootstrapping in memory tests.",
                string.Empty,
                "Includes options to override configuration and service collection using a partial class.",
            }));

        // Create members
        var memberDeclarationConfigureWebHost = CreateWebApplicationFactoryConfigureWebHost();
        var memberDeclarationModifyConfiguration = CreateWebApplicationFactoryModifyConfiguration();
        var memberDeclarationModifyServices = CreateWebApplicationFactoryModifyServices();

        // Add member to class
        classDeclaration = classDeclaration.AddMembers(memberDeclarationConfigureWebHost, memberDeclarationModifyConfiguration, memberDeclarationModifyServices);

        // Add class to namespace
        @namespace = @namespace.AddMembers(classDeclaration);

        // Add namespace to compilationUnit
        compilationUnit = compilationUnit.AddMembers(@namespace);

        // Add using to compilationUnit
        compilationUnit = compilationUnit.AddUsingStatements(
            ProjectHostFactory.CreateUsingListForWebApiStartupFactory(
                projectOptions.ProjectName));

        var codeAsString = compilationUnit
            .NormalizeWhitespace()
            .ToFullString()
            .EnsureEnvironmentNewLines()
            .EnsureNewlineAfterMethod("partial void ModifyConfiguration(IConfigurationBuilder config);");

        var file = new FileInfo(Path.Combine(projectOptions.PathForTestGenerate!.FullName, "WebApiStartupFactory.cs"));
        var fileDisplayLocation = file.FullName.Replace(projectOptions.PathForTestGenerate.FullName, "test: ", StringComparison.Ordinal);
        TextFileHelper.Save(logger, file, fileDisplayLocation, codeAsString);
    }

    private void GenerateTestWebApiControllerBaseTest()
    {
        // Create compilationUnit
        var compilationUnit = SyntaxFactory.CompilationUnit();

        // Create a namespace
        var @namespace = SyntaxProjectFactory.CreateNamespace(projectOptions, "Tests");

        // Create class
        var classDeclaration = SyntaxClassDeclarationFactory.Create("WebApiControllerBaseTest")
            .AddModifiers(SyntaxTokenFactory.AbstractKeyword())
            .AddGeneratedCodeAttribute(projectOptions.ToolName, projectOptions.ToolVersion.ToString())
            .WithBaseList(
                SyntaxFactory.BaseList(
                    SyntaxFactory.SingletonSeparatedList<BaseTypeSyntax>(
                        SyntaxFactory.SimpleBaseType(
                            SyntaxFactory.GenericName(
                                    SyntaxFactory.Identifier("IClassFixture"))
                                .WithTypeArgumentList(
                                    SyntaxTypeArgumentListFactory.CreateWithOneItem("WebApiStartupFactory"))))));

        // Create member
        var memberDeclarationFactory = CreateWebApiControllerBaseTestFactory();
        var memberDeclarationHttpClient = CreateWebApiControllerBaseTestHttpClient();
        var memberDeclarationConfiguration = CreateWebApiControllerBaseTestConfiguration();
        var memberDeclarationJsonSerializerOptions = CreateWebApiControllerBaseTestJsonSerializerOptions();
        var memberDeclarationConstructor = CreateWebApiControllerBaseTestConstructor();
        var memberDeclarationToJson = CreateWebApiControllerBaseTestToJson();
        var memberDeclarationJson = CreateWebApiControllerBaseTestJson();
        var memberDeclarationGetTestFile = CreateWebApiControllerBaseTestGetTestFile();
        var memberDeclarationGetTestFiles = CreateWebApiControllerBaseTestGetTestFiles();

        // Add member to class
        classDeclaration = classDeclaration.AddMembers(memberDeclarationFactory);
        classDeclaration = classDeclaration.AddMembers(memberDeclarationHttpClient);
        classDeclaration = classDeclaration.AddMembers(memberDeclarationConfiguration);
        classDeclaration = classDeclaration.AddMembers(memberDeclarationJsonSerializerOptions);
        classDeclaration = classDeclaration.AddMembers(memberDeclarationConstructor);
        classDeclaration = classDeclaration.AddMembers(memberDeclarationToJson);
        classDeclaration = classDeclaration.AddMembers(memberDeclarationJson);
        classDeclaration = classDeclaration.AddMembers(memberDeclarationGetTestFile);
        classDeclaration = classDeclaration.AddMembers(memberDeclarationGetTestFiles);

        // Add class to namespace
        @namespace = @namespace.AddMembers(classDeclaration);

        // Add namespace to compilationUnit
        compilationUnit = compilationUnit.AddMembers(@namespace);

        // Add using to compilationUnit
        compilationUnit = compilationUnit.AddUsingStatements(ProjectHostFactory.CreateUsingListForWebApiControllerBaseTest());

        var codeAsString = compilationUnit
            .NormalizeWhitespace()
            .ToFullString()
            .EnsureEnvironmentNewLines();

        var file = new FileInfo(Path.Combine(projectOptions.PathForTestGenerate!.FullName, "WebApiControllerBaseTest.cs"));
        var fileDisplayLocation = file.FullName.Replace(projectOptions.PathForTestGenerate.FullName, "test: ", StringComparison.Ordinal);
        TextFileHelper.Save(logger, file, fileDisplayLocation, codeAsString);
    }

    private void ScaffoldAppSettingsIntegrationTest()
    {
        var sb = new StringBuilder();
        sb.AppendLine("{");
        sb.AppendLine("}");

        var file = new FileInfo(Path.Combine(projectOptions.PathForTestGenerate!.FullName, "appsettings.integrationtest.json"));
        if (file.Exists)
        {
            return;
        }

        var fileDisplayLocation = file.FullName.Replace(projectOptions.PathForTestGenerate.FullName, "test: ", StringComparison.Ordinal);
        TextFileHelper.Save(logger, file, fileDisplayLocation, sb.ToString());
    }
}