using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Xml.Linq;
using Atc.CodeAnalysis.CSharp.SyntaxFactories;
using Atc.Data.Models;
using Atc.Rest.ApiGenerator.Factories;
using Atc.Rest.ApiGenerator.Helpers;
using Atc.Rest.ApiGenerator.Helpers.XunitTest;
using Atc.Rest.ApiGenerator.Models;
using Atc.Rest.ApiGenerator.ProjectSyntaxFactories;
using Atc.Rest.ApiGenerator.SyntaxGenerators.Api;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Hosting;

// ReSharper disable SuggestBaseTypeForParameter
// ReSharper disable ReturnTypeCanBeEnumerable.Local
namespace Atc.Rest.ApiGenerator.Generators
{
    public class ServerHostGenerator
    {
        private readonly HostProjectOptions projectOptions;

        public ServerHostGenerator(HostProjectOptions projectOptions)
        {
            this.projectOptions = projectOptions ?? throw new ArgumentNullException(nameof(projectOptions));
        }

        public List<LogKeyValueItem> Generate()
        {
            var logItems = new List<LogKeyValueItem>();

            logItems.AddRange(projectOptions.SetPropertiesAfterValidationsOfProjectReferencesPathAndFiles());
            if (logItems.Any(x => x.LogCategory == LogCategoryType.Error))
            {
                return logItems;
            }

            logItems.AddRange(ScaffoldSrc(projectOptions));
            if (projectOptions.PathForTestGenerate != null)
            {
                logItems.AddRange(ScaffoldTest(projectOptions));
            }

            if (projectOptions.PathForTestGenerate != null)
            {
                logItems.AddRange(GenerateTestEndpoints(projectOptions));
            }

            return logItems;
        }

        private static List<LogKeyValueItem> ScaffoldSrc(HostProjectOptions hostProjectOptions)
        {
            if (hostProjectOptions == null)
            {
                throw new ArgumentNullException(nameof(hostProjectOptions));
            }

            if (!Directory.Exists(hostProjectOptions.PathForSrcGenerate.FullName))
            {
                Directory.CreateDirectory(hostProjectOptions.PathForSrcGenerate.FullName);
            }

            var logItems = new List<LogKeyValueItem>();

            if (hostProjectOptions.PathForSrcGenerate.Exists && hostProjectOptions.ProjectSrcCsProj.Exists)
            {
                var element = XElement.Load(hostProjectOptions.ProjectSrcCsProj.FullName);
                var originalNullableValue = SolutionAndProjectHelper.GetBoolFromNullableString(SolutionAndProjectHelper.GetNullableValueFromProject(element));

                bool hasUpdates = false;
                if (hostProjectOptions.ApiOptions.Generator.UseNullableReferenceTypes != originalNullableValue)
                {
                    var newNullableValue = SolutionAndProjectHelper.GetNullableStringFromBool(hostProjectOptions.ApiOptions.Generator.UseNullableReferenceTypes);
                    SolutionAndProjectHelper.SetNullableValueForProject(element, newNullableValue);
                    element.Save(hostProjectOptions.ProjectSrcCsProj.FullName);
                    logItems.Add(new LogKeyValueItem(LogCategoryType.Debug, "FileUpdate", "#", $"Update host csproj - Nullable value={newNullableValue}"));
                    hasUpdates = true;
                }

                if (!hasUpdates)
                {
                    logItems.Add(new LogKeyValueItem(LogCategoryType.Debug, "FileSkip", "#", "No updates for host csproj"));
                }
            }
            else
            {
                var projectReferences = new List<FileInfo>();
                if (hostProjectOptions.ApiProjectSrcCsProj != null)
                {
                    projectReferences.Add(hostProjectOptions.ApiProjectSrcCsProj);
                }

                if (hostProjectOptions.DomainProjectSrcCsProj != null)
                {
                    projectReferences.Add(hostProjectOptions.DomainProjectSrcCsProj);
                }

                logItems.Add(SolutionAndProjectHelper.ScaffoldProjFile(
                    hostProjectOptions.ProjectSrcCsProj,
                    true,
                    false,
                    hostProjectOptions.ProjectName,
                    hostProjectOptions.ApiOptions.Generator.UseNullableReferenceTypes,
                    null,
                    NugetPackageReferenceHelper.CreateForHostProject(hostProjectOptions.UseRestExtended),
                    projectReferences,
                    false));

                logItems.Add(ScaffoldPropertiesLaunchSettingsFile(
                    hostProjectOptions.PathForSrcGenerate,
                    hostProjectOptions.UseRestExtended));
                logItems.Add(ScaffoldProgramFile(hostProjectOptions));
                logItems.Add(ScaffoldStartupFile(hostProjectOptions));
            }

            return logItems;
        }

        [SuppressMessage("Critical Code Smell", "S3776:Cognitive Complexity of methods should not be too high", Justification = "OK.")]
        private static List<LogKeyValueItem> ScaffoldTest(HostProjectOptions hostProjectOptions)
        {
            if (hostProjectOptions == null)
            {
                throw new ArgumentNullException(nameof(hostProjectOptions));
            }

            var logItems = new List<LogKeyValueItem>();

            if (hostProjectOptions.PathForTestGenerate == null || hostProjectOptions.ProjectTestCsProj == null)
            {
                return logItems;
            }

            if (hostProjectOptions.PathForTestGenerate.Exists && hostProjectOptions.ProjectTestCsProj.Exists)
            {
                // Update
            }
            else
            {
                if (!Directory.Exists(hostProjectOptions.PathForTestGenerate.FullName))
                {
                    Directory.CreateDirectory(hostProjectOptions.PathForTestGenerate.FullName);
                }

                var projectReferences = new List<FileInfo>();
                if (hostProjectOptions.ApiProjectSrcCsProj != null)
                {
                    projectReferences.Add(hostProjectOptions.ProjectSrcCsProj);
                    projectReferences.Add(hostProjectOptions.ApiProjectSrcCsProj);
                }

                if (hostProjectOptions.DomainProjectSrcCsProj != null)
                {
                    projectReferences.Add(hostProjectOptions.DomainProjectSrcCsProj);
                }

                logItems.Add(SolutionAndProjectHelper.ScaffoldProjFile(
                    hostProjectOptions.ProjectTestCsProj,
                    false,
                    true,
                    $"{hostProjectOptions.ProjectName}.Tests",
                    hostProjectOptions.ApiOptions.Generator.UseNullableReferenceTypes,
                    null,
                    NugetPackageReferenceHelper.CreateForTestProject(true),
                    projectReferences,
                    true));
            }

            logItems.Add(GenerateTestWebApiStartupFactory(hostProjectOptions));
            logItems.Add(GenerateTestWebApiControllerBaseTest(hostProjectOptions));

            return logItems;
        }

        private List<LogKeyValueItem> GenerateTestEndpoints(HostProjectOptions hostProjectOptions)
        {
            if (hostProjectOptions == null)
            {
                throw new ArgumentNullException(nameof(hostProjectOptions));
            }

            var apiProjectOptions = new ApiProjectOptions(
                hostProjectOptions.ApiProjectSrcPath,
                null,
                hostProjectOptions.Document,
                hostProjectOptions.DocumentFile,
                hostProjectOptions.ProjectName.Replace(".Api", string.Empty, StringComparison.Ordinal),
                hostProjectOptions.ApiOptions);

            var operationSchemaMappings = OpenApiOperationSchemaMapHelper.CollectMappings(projectOptions.Document);
            var sgEndpointControllers = new List<SyntaxGeneratorEndpointControllers>();
            foreach (var segmentName in hostProjectOptions.BasePathSegmentNames)
            {
                var generator = new SyntaxGeneratorEndpointControllers(apiProjectOptions, operationSchemaMappings, segmentName);
                generator.GenerateCode();
                sgEndpointControllers.Add(generator);
            }

            var logItems = new List<LogKeyValueItem>();
            foreach (var sgEndpointController in sgEndpointControllers)
            {
                var metadataForMethods = sgEndpointController.GetMetadataForMethods();
                foreach (var endpointMethodMetadata in metadataForMethods)
                {
                    logItems.Add(GenerateServerApiXunitTestEndpointHandlerStubHelper.Generate(hostProjectOptions, endpointMethodMetadata));
                    logItems.Add(GenerateServerApiXunitTestEndpointTestHelper.Generate(hostProjectOptions, endpointMethodMetadata));
                }
            }

            return logItems;
        }

        private static LogKeyValueItem ScaffoldPropertiesLaunchSettingsFile(DirectoryInfo pathForSrcGenerate, bool useExtended)
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
            return File.Exists(file.FullName)
                ? new LogKeyValueItem(LogCategoryType.Debug, "FileSkip", "#", file.FullName)
                : TextFileHelper.Save(file, json);
        }

        private static LogKeyValueItem ScaffoldProgramFile(HostProjectOptions hostProjectOptions)
        {
            // Create compilationUnit
            var compilationUnit = SyntaxFactory.CompilationUnit();

            // Create a namespace
            var @namespace = SyntaxProjectFactory.CreateNamespace(hostProjectOptions, false);

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
                .ToFullString();

            var file = new FileInfo(Path.Combine(hostProjectOptions.PathForSrcGenerate.FullName, "Program.cs"));
            return File.Exists(file.FullName)
                ? new LogKeyValueItem(LogCategoryType.Debug, "FileSkip", "#", file.FullName)
                : TextFileHelper.Save(file, codeAsString);
        }

        private static LogKeyValueItem ScaffoldStartupFile(HostProjectOptions hostProjectOptions)
        {
            // Create compilationUnit
            var compilationUnit = SyntaxFactory.CompilationUnit();

            // Create a namespace
            var @namespace = SyntaxProjectFactory.CreateNamespace(hostProjectOptions, false);

            // Create class
            var classDeclaration = SyntaxClassDeclarationFactory.Create("Startup");

            // Create Member
            var memberDeclarationPropertyPrivateOptions = CreateStartupPropertyPrivateOptions(hostProjectOptions.UseRestExtended);
            var memberDeclarationConstructor = CreateStartupConstructor(hostProjectOptions.UseRestExtended);
            var memberDeclarationPropertyPublicConfiguration = CreateStartupPropertyPublicConfiguration();
            var memberDeclarationConfigureServices = CreateStartupConfigureServices(hostProjectOptions.UseRestExtended);
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
                hostProjectOptions.ProjectName,
                hostProjectOptions.UseRestExtended));

            var codeAsString = compilationUnit
                .NormalizeWhitespace()
                .ToFullString()
                .FormatAutoPropertiesOnOneLine()
                .FormatPublicPrivateLines();

            var file = new FileInfo(Path.Combine(hostProjectOptions.PathForSrcGenerate.FullName, "Startup.cs"));
            return File.Exists(file.FullName)
                ? new LogKeyValueItem(LogCategoryType.Debug, "FileSkip", "#", file.FullName)
                : TextFileHelper.Save(file, codeAsString);
        }

        private static LogKeyValueItem GenerateTestWebApiStartupFactory(HostProjectOptions hostProjectOptions)
        {
            // Create compilationUnit
            var compilationUnit = SyntaxFactory.CompilationUnit();

            // Create a namespace
            var @namespace = SyntaxProjectFactory.CreateNamespace(hostProjectOptions, "Tests");

            // Create class
            var classDeclaration = SyntaxClassDeclarationFactory.CreateAsPublicPartial("WebApiStartupFactory")
                .AddGeneratedCodeAttribute(hostProjectOptions.ToolName, hostProjectOptions.ToolVersion.ToString())
                .WithBaseList(
                    SyntaxFactory.BaseList(
                        SyntaxFactory.SingletonSeparatedList<BaseTypeSyntax>(
                            SyntaxFactory.SimpleBaseType(
                                SyntaxFactory.GenericName(
                                        SyntaxFactory.Identifier("WebApplicationFactory"))
                                    .WithTypeArgumentList(
                                        SyntaxFactory.TypeArgumentList(
                                            SyntaxFactory.SingletonSeparatedList<TypeSyntax>(
                                                SyntaxFactory.IdentifierName("Startup"))))))));

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
            compilationUnit = compilationUnit.AddUsingStatements(ProjectHostFactory.CreateUsingListForWebApiStartupFactory(
                hostProjectOptions.ProjectName));

            var codeAsString = compilationUnit
                .NormalizeWhitespace()
                .ToFullString();

            var file = new FileInfo(Path.Combine(hostProjectOptions.PathForTestGenerate!.FullName, "WebApiStartupFactory.cs"));
            return TextFileHelper.Save(file, codeAsString);
        }

        private static LogKeyValueItem GenerateTestWebApiControllerBaseTest(HostProjectOptions hostProjectOptions)
        {
            // Create compilationUnit
            var compilationUnit = SyntaxFactory.CompilationUnit();

            // Create a namespace
            var @namespace = SyntaxProjectFactory.CreateNamespace(hostProjectOptions, "Tests");

            // Create class
            var classDeclaration = SyntaxClassDeclarationFactory.Create("WebApiControllerBaseTest")
                .AddModifiers(SyntaxTokenFactory.AbstractKeyword())
                .AddGeneratedCodeAttribute(hostProjectOptions.ToolName, hostProjectOptions.ToolVersion.ToString())
                .WithBaseList(
                    SyntaxFactory.BaseList(
                        SyntaxFactory.SingletonSeparatedList<BaseTypeSyntax>(
                            SyntaxFactory.SimpleBaseType(
                                SyntaxFactory.GenericName(
                                        SyntaxFactory.Identifier("IClassFixture"))
                                    .WithTypeArgumentList(
                                        SyntaxFactory.TypeArgumentList(
                                            SyntaxFactory.SingletonSeparatedList<TypeSyntax>(
                                                SyntaxFactory.IdentifierName("WebApiStartupFactory"))))))));

            // Create member
            var memberDeclarationFactory = CreateWebApiControllerBaseTestFactory();
            var memberDeclarationHttpClient = CreateWebApiControllerBaseTestHttpClient();
            var memberDeclarationConfiguration = CreateWebApiControllerBaseTestConfiguration();
            var memberDeclarationJsonSerializerOptions = CreateWebApiControllerBaseTestJsonSerializerOptions();
            var memberDeclarationConstructor = CreateWebApiControllerBaseTestConstructor();
            var memberDeclarationToJson = CreateWebApiControllerBaseTestToJson();
            var memberDeclarationJson = CreateWebApiControllerBaseTestJson();

            // Add member to class
            classDeclaration = classDeclaration.AddMembers(memberDeclarationFactory);
            classDeclaration = classDeclaration.AddMembers(memberDeclarationHttpClient);
            classDeclaration = classDeclaration.AddMembers(memberDeclarationConfiguration);
            classDeclaration = classDeclaration.AddMembers(memberDeclarationJsonSerializerOptions);
            classDeclaration = classDeclaration.AddMembers(memberDeclarationConstructor);
            classDeclaration = classDeclaration.AddMembers(memberDeclarationToJson);
            classDeclaration = classDeclaration.AddMembers(memberDeclarationJson);

            // Add class to namespace
            @namespace = @namespace.AddMembers(classDeclaration);

            // Add namespace to compilationUnit
            compilationUnit = compilationUnit.AddMembers(@namespace);

            // Add using to compilationUnit
            compilationUnit = compilationUnit.AddUsingStatements(ProjectHostFactory.CreateUsingListForWebApiControllerBaseTest());

            var codeAsString = compilationUnit
                .NormalizeWhitespace()
                .ToFullString();

            var file = new FileInfo(Path.Combine(hostProjectOptions.PathForTestGenerate!.FullName, "WebApiControllerBaseTest.cs"));
            return TextFileHelper.Save(file, codeAsString);
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
                                        SyntaxFactory.InvocationExpression(
                                                SyntaxFactory.IdentifierName("CreateHostBuilder"))
                                            .WithArgumentList(
                                                SyntaxFactory.ArgumentList(
                                                    SyntaxFactory.SingletonSeparatedList(
                                                        SyntaxFactory.Argument(
                                                            SyntaxFactory.IdentifierName("args"))))),
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
                    SyntaxFactory.VariableDeclaration(
                        SyntaxFactory.IdentifierName("var"))
                    .WithVariables(
                        SyntaxFactory.SingletonSeparatedList(
                            SyntaxFactory.VariableDeclarator(
                                SyntaxFactory.Identifier("builder"))
                            .WithInitializer(
                                SyntaxFactory.EqualsValueClause(
                                    SyntaxFactory.InvocationExpression(
                                        SyntaxFactory.MemberAccessExpression(
                                            SyntaxKind.SimpleMemberAccessExpression,
                                            SyntaxFactory.InvocationExpression(
                                                SyntaxFactory.MemberAccessExpression(
                                                    SyntaxKind.SimpleMemberAccessExpression,
                                                    SyntaxFactory.IdentifierName("Host"),
                                                    SyntaxFactory.IdentifierName("CreateDefaultBuilder")))
                                            .WithArgumentList(
                                                SyntaxFactory.ArgumentList(
                                                    SyntaxFactory.SingletonSeparatedList(
                                                        SyntaxFactory.Argument(
                                                            SyntaxFactory.IdentifierName("args"))))),
                                            SyntaxFactory.IdentifierName("ConfigureWebHostDefaults")))
                                    .WithArgumentList(
                                        SyntaxFactory.ArgumentList(
                                            SyntaxFactory.SingletonSeparatedList(
                                                SyntaxFactory.Argument(
                                                    SyntaxFactory.SimpleLambdaExpression(
                                                        SyntaxFactory.Parameter(
                                                            SyntaxFactory.Identifier("webBuilder")))
                                                    .WithBlock(
                                                        SyntaxFactory.Block(
                                                            SyntaxFactory.SingletonList<StatementSyntax>(
                                                                SyntaxFactory.ExpressionStatement(
                                                                    SyntaxFactory.InvocationExpression(
                                                                        SyntaxFactory.MemberAccessExpression(
                                                                            SyntaxKind.SimpleMemberAccessExpression,
                                                                            SyntaxFactory.IdentifierName("webBuilder"),
                                                                            SyntaxFactory.GenericName(
                                                                                SyntaxFactory.Identifier("UseStartup"))
                                                                            .WithTypeArgumentList(
                                                                                SyntaxFactory.TypeArgumentList(
                                                                                    SyntaxFactory.SingletonSeparatedList<TypeSyntax>(
                                                                                        SyntaxFactory.IdentifierName("Startup"))))))))))))))))))),
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

        private static MemberDeclarationSyntax CreateStartupPropertyPrivateOptions(in bool useRestExtended)
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

        private static MemberDeclarationSyntax CreateStartupConstructor(bool useRestExtended)
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
                        SyntaxFactory.MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            SyntaxFactory.IdentifierName("restApiOptions"),
                            SyntaxFactory.IdentifierName("AddAssemblyPairs")))
                    .WithArgumentList(
                        SyntaxFactory.ArgumentList(
                            SyntaxFactory.SeparatedList<ArgumentSyntax>(
                                new SyntaxNodeOrToken[]
                                {
                                    SyntaxFactory.Argument(
                                        SyntaxFactory.InvocationExpression(
                                            SyntaxFactory.MemberAccessExpression(
                                                SyntaxKind.SimpleMemberAccessExpression,
                                                SyntaxFactory.IdentifierName("Assembly"),
                                                SyntaxFactory.IdentifierName("GetAssembly")))
                                        .WithArgumentList(
                                            SyntaxFactory.ArgumentList(
                                                SyntaxFactory.SingletonSeparatedList(
                                                    SyntaxFactory.Argument(
                                                        SyntaxFactory.TypeOfExpression(
                                                            SyntaxFactory.IdentifierName("ApiRegistration"))))))),
                                    SyntaxTokenFactory.Comma(),
                                    SyntaxFactory.Argument(
                                        SyntaxFactory.InvocationExpression(
                                            SyntaxFactory.MemberAccessExpression(
                                                SyntaxKind.SimpleMemberAccessExpression,
                                                SyntaxFactory.IdentifierName("Assembly"),
                                                SyntaxFactory.IdentifierName("GetAssembly")))
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
                            SyntaxFactory.Parameter(
                                    SyntaxFactory.Identifier("configuration"))
                                .WithType(
                                    SyntaxFactory.IdentifierName("IConfiguration")))))
                .WithBody(codeBody);
        }

        private static MemberDeclarationSyntax CreateStartupPropertyPublicConfiguration()
        {
            return SyntaxFactory.PropertyDeclaration(
                    SyntaxFactory.IdentifierName("IConfiguration"),
                    SyntaxFactory.Identifier("Configuration"))
                .WithModifiers(
                    SyntaxFactory.TokenList(
                        SyntaxTokenFactory.PublicKeyword()))
                .WithAccessorList(
                    SyntaxFactory.AccessorList(
                        SyntaxFactory.SingletonList(
                            SyntaxFactory.AccessorDeclaration(
                                    SyntaxKind.GetAccessorDeclaration)
                                .WithSemicolonToken(
                                    SyntaxTokenFactory.Semicolon()))));
        }

        private static MemberDeclarationSyntax CreateStartupConfigureServices(in bool useRestExtended)
        {
            ArgumentListSyntax argumentList;
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
            }
            else
            {
                argumentList = SyntaxFactory.ArgumentList(
                    SyntaxFactory.SingletonSeparatedList(
                        SyntaxFactory.Argument(
                            SyntaxFactory.IdentifierName("restApiOptions"))));
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
            .WithBody(
                SyntaxFactory.Block(
                    SyntaxFactory.SingletonList<StatementSyntax>(
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
                                .WithArgumentList(argumentList)))));
        }

        private static MemberDeclarationSyntax CreateStartupConfigure()
        {
            return SyntaxFactory.MethodDeclaration(
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
                            SyntaxTokenFactory.Comma(),
                            SyntaxFactory.Parameter(SyntaxFactory.Identifier("env"))
                                .WithType(SyntaxFactory.IdentifierName("IWebHostEnvironment")),
                        })))
            .WithBody(
                SyntaxFactory.Block(
                    SyntaxFactory.SingletonList<StatementSyntax>(
                        SyntaxFactory.ExpressionStatement(
                            SyntaxFactory.InvocationExpression(
                                SyntaxFactory.MemberAccessExpression(
                                    SyntaxKind.SimpleMemberAccessExpression,
                                    SyntaxFactory.IdentifierName("app"),
                                    SyntaxFactory.IdentifierName("ConfigureRestApi")))
                            .WithArgumentList(
                                SyntaxFactory.ArgumentList(
                                    SyntaxFactory.SeparatedList<ArgumentSyntax>(
                                        new SyntaxNodeOrToken[]
                                        {
                                            SyntaxFactory.Argument(SyntaxFactory.IdentifierName("env")),
                                            SyntaxTokenFactory.Comma(),
                                            SyntaxFactory.Argument(SyntaxFactory.IdentifierName("restApiOptions")),
                                        })))))));
        }

        private static MemberDeclarationSyntax CreateWebApplicationFactoryConfigureWebHost()
        {
            return SyntaxFactory.MethodDeclaration(
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
                            SyntaxFactory.MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                SyntaxFactory.IdentifierName("builder"),
                                SyntaxFactory.IdentifierName("ConfigureAppConfiguration")))
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
                                                        SyntaxFactory.SingletonSeparatedList(
                                                            SyntaxFactory.VariableDeclarator(SyntaxFactory.Identifier("integrationConfig"))
                                                            .WithInitializer(
                                                                SyntaxFactory.EqualsValueClause(
                                                                    SyntaxFactory.InvocationExpression(
                                                                        SyntaxFactory.MemberAccessExpression(
                                                                            SyntaxKind.SimpleMemberAccessExpression,
                                                                            SyntaxFactory.ObjectCreationExpression(SyntaxFactory.IdentifierName("ConfigurationBuilder"))
                                                                            .WithArgumentList(
                                                                                SyntaxFactory.ArgumentList()),
                                                                            SyntaxFactory.IdentifierName("Build")))))))),
                                                SyntaxFactory.ExpressionStatement(
                                                    SyntaxFactory.InvocationExpression(
                                                        SyntaxFactory.MemberAccessExpression(
                                                            SyntaxKind.SimpleMemberAccessExpression,
                                                            SyntaxFactory.IdentifierName("config"),
                                                            SyntaxFactory.IdentifierName("AddConfiguration")))
                                                    .WithArgumentList(
                                                        SyntaxFactory.ArgumentList(
                                                            SyntaxFactory.SingletonSeparatedList(
                                                                SyntaxFactory.Argument(
                                                                    SyntaxFactory.IdentifierName("integrationConfig"))))))))))))),
                    SyntaxFactory.ExpressionStatement(
                        SyntaxFactory.InvocationExpression(
                            SyntaxFactory.MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                SyntaxFactory.IdentifierName("builder"),
                                SyntaxFactory.IdentifierName("ConfigureTestServices")))
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
                                                                    SyntaxFactory.TypeArgumentList(
                                                                        SyntaxFactory.SeparatedList<TypeSyntax>(
                                                                            new SyntaxNodeOrToken[]
                                                                            {
                                                                                SyntaxFactory.IdentifierName("RestApiOptions"),
                                                                                SyntaxTokenFactory.Comma(),
                                                                                SyntaxFactory.IdentifierName("RestApiOptions"),
                                                                            })))))),
                                                SyntaxFactory.ExpressionStatement(
                                                    SyntaxFactory.InvocationExpression(
                                                        SyntaxFactory.MemberAccessExpression(
                                                            SyntaxKind.SimpleMemberAccessExpression,
                                                            SyntaxFactory.IdentifierName("services"),
                                                            SyntaxFactory.IdentifierName("AutoRegistrateServices")))
                                                    .WithArgumentList(
                                                        SyntaxFactory.ArgumentList(
                                                            SyntaxFactory.SeparatedList<ArgumentSyntax>(
                                                                new SyntaxNodeOrToken[]
                                                                {
                                                                    SyntaxFactory.Argument(
                                                                        SyntaxFactory.PostfixUnaryExpression(
                                                                            SyntaxKind.SuppressNullableWarningExpression,
                                                                            SyntaxFactory.InvocationExpression(
                                                                                SyntaxFactory.MemberAccessExpression(
                                                                                    SyntaxKind.SimpleMemberAccessExpression,
                                                                                    SyntaxFactory.IdentifierName("Assembly"),
                                                                                    SyntaxFactory.IdentifierName("GetAssembly")))
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
                                                                                SyntaxFactory.MemberAccessExpression(
                                                                                    SyntaxKind.SimpleMemberAccessExpression,
                                                                                    SyntaxFactory.IdentifierName("Assembly"),
                                                                                    SyntaxFactory.IdentifierName("GetAssembly")))
                                                                            .WithArgumentList(
                                                                                SyntaxFactory.ArgumentList(
                                                                                    SyntaxFactory.SingletonSeparatedList(
                                                                                        SyntaxFactory.Argument(
                                                                                            SyntaxFactory.TypeOfExpression(
                                                                                                SyntaxFactory.IdentifierName("WebApiStartupFactory")))))))),
                                                                })))))))))))));
        }

        private static MemberDeclarationSyntax CreateWebApplicationFactoryModifyConfiguration()
        {
            return SyntaxFactory.MethodDeclaration(
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
        }

        private static MemberDeclarationSyntax CreateWebApplicationFactoryModifyServices()
        {
            return SyntaxFactory.MethodDeclaration(
                    SyntaxFactory.PredefinedType(
                        SyntaxTokenFactory.VoidKeyword()),
                    SyntaxFactory.Identifier("ModifyServices"))
                .WithModifiers(
                    SyntaxFactory.TokenList(
                        SyntaxTokenFactory.PartialKeyword()))
                .WithParameterList(
                    SyntaxFactory.ParameterList(
                        SyntaxFactory.SingletonSeparatedList(
                            SyntaxFactory.Parameter(SyntaxFactory.Identifier("services"))
                            .WithType(SyntaxFactory.IdentifierName("IServiceCollection")))))
                .WithSemicolonToken(SyntaxTokenFactory.Semicolon());
        }

        private static MemberDeclarationSyntax CreateWebApiControllerBaseTestFactory()
        {
            return SyntaxFactory.FieldDeclaration(
                    SyntaxFactory.VariableDeclaration(SyntaxFactory.IdentifierName("WebApiStartupFactory"))
                        .WithVariables(
                            SyntaxFactory.SingletonSeparatedList(
                                SyntaxFactory.VariableDeclarator(SyntaxFactory.Identifier("Factory")))))
                .WithModifiers(
                    SyntaxFactory.TokenList(
                        SyntaxTokenFactory.ProtectedKeyword(),
                        SyntaxTokenFactory.ReadOnlyKeyword()));
        }

        private static MemberDeclarationSyntax CreateWebApiControllerBaseTestHttpClient()
        {
            return SyntaxFactory.FieldDeclaration(
                    SyntaxFactory.VariableDeclaration(SyntaxFactory.IdentifierName("HttpClient"))
                        .WithVariables(
                            SyntaxFactory.SingletonSeparatedList(
                                SyntaxFactory.VariableDeclarator(SyntaxFactory.Identifier("HttpClient")))))
                .WithModifiers(
                    SyntaxFactory.TokenList(
                        SyntaxTokenFactory.ProtectedKeyword(),
                        SyntaxTokenFactory.ReadOnlyKeyword()));
        }

        private static MemberDeclarationSyntax CreateWebApiControllerBaseTestConfiguration()
        {
            return SyntaxFactory.FieldDeclaration(
                    SyntaxFactory.VariableDeclaration(SyntaxFactory.IdentifierName("IConfiguration"))
                        .WithVariables(
                            SyntaxFactory.SingletonSeparatedList(
                                SyntaxFactory.VariableDeclarator(SyntaxFactory.Identifier("Configuration")))))
                .WithModifiers(SyntaxTokenListFactory.ProtectedReadOnlyKeyword());
        }

        private static MemberDeclarationSyntax CreateWebApiControllerBaseTestJsonSerializerOptions()
        {
            return SyntaxFactory.FieldDeclaration(
                    SyntaxFactory.VariableDeclaration(SyntaxFactory.IdentifierName("JsonSerializerOptions"))
                        .WithVariables(
                            SyntaxFactory.SingletonSeparatedList(
                                SyntaxFactory.VariableDeclarator(SyntaxFactory.Identifier("JsonSerializerOptions")))))
                .WithModifiers(SyntaxTokenListFactory.ProtectedStaticKeyword());
        }

        private static MemberDeclarationSyntax CreateWebApiControllerBaseTestConstructor()
        {
            return SyntaxFactory.MethodDeclaration(
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
                                                                .WithArgumentList(SyntaxFactory.ArgumentList())))),
                                                SyntaxTokenFactory.Comma(),
                                            })))))));
        }

        private static MemberDeclarationSyntax CreateWebApiControllerBaseTestToJson()
        {
            return SyntaxFactory.MethodDeclaration(
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
                    SyntaxFactory.ObjectCreationExpression(
                        SyntaxFactory.IdentifierName("StringContent"))
                    .WithArgumentList(
                        SyntaxFactory.ArgumentList(
                            SyntaxFactory.SeparatedList<ArgumentSyntax>(
                                new SyntaxNodeOrToken[]
                                {
                                    SyntaxFactory.Argument(
                                        SyntaxFactory.InvocationExpression(
                                            SyntaxFactory.MemberAccessExpression(
                                                SyntaxKind.SimpleMemberAccessExpression,
                                                SyntaxFactory.IdentifierName("JsonSerializer"),
                                                SyntaxFactory.IdentifierName("Serialize")))
                                        .WithArgumentList(
                                            SyntaxFactory.ArgumentList(
                                                SyntaxFactory.SeparatedList<ArgumentSyntax>(
                                                    new SyntaxNodeOrToken[]
                                                    {
                                                        SyntaxFactory.Argument(SyntaxFactory.IdentifierName("data")),
                                                        SyntaxTokenFactory.Comma(),
                                                        SyntaxFactory.Argument(SyntaxFactory.IdentifierName("JsonSerializerOptions")),
                                                    })))),
                                    SyntaxTokenFactory.Comma(),
                                    SyntaxFactory.Argument(
                                        SyntaxFactory.MemberAccessExpression(
                                            SyntaxKind.SimpleMemberAccessExpression,
                                            SyntaxFactory.IdentifierName("Encoding"),
                                            SyntaxFactory.IdentifierName("UTF8"))),
                                    SyntaxTokenFactory.Comma(),
                                    SyntaxFactory.Argument(
                                        SyntaxFactory.LiteralExpression(
                                            SyntaxKind.StringLiteralExpression,
                                            SyntaxFactory.Literal(MediaTypeNames.Application.Json))),
                                })))))
            .WithSemicolonToken(SyntaxTokenFactory.Semicolon());
        }

        private static MemberDeclarationSyntax CreateWebApiControllerBaseTestJson()
        {
            return SyntaxFactory.MethodDeclaration(
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
                                    SyntaxFactory.Argument(
                                        SyntaxFactory.IdentifierName("data")),
                                    SyntaxTokenFactory.Comma(),
                                    SyntaxFactory.Argument(
                                        SyntaxFactory.MemberAccessExpression(
                                            SyntaxKind.SimpleMemberAccessExpression,
                                            SyntaxFactory.IdentifierName("Encoding"),
                                            SyntaxFactory.IdentifierName("UTF8"))),
                                    SyntaxTokenFactory.Comma(),
                                    SyntaxFactory.Argument(
                                        SyntaxFactory.LiteralExpression(
                                            SyntaxKind.StringLiteralExpression,
                                            SyntaxFactory.Literal(MediaTypeNames.Application.Json))),
                                })))))
            .WithSemicolonToken(SyntaxTokenFactory.Semicolon());
        }
    }
}