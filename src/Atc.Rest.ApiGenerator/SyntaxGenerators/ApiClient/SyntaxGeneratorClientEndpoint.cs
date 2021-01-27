using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Atc.CodeAnalysis.CSharp.SyntaxFactories;
using Atc.Data.Models;
using Atc.Rest.ApiGenerator.Extensions;
using Atc.Rest.ApiGenerator.Factories;
using Atc.Rest.ApiGenerator.Helpers;
using Atc.Rest.ApiGenerator.Models;
using Atc.Rest.ApiGenerator.ProjectSyntaxFactories;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.OpenApi.Models;

namespace Atc.Rest.ApiGenerator.SyntaxGenerators.ApiClient
{
    public class SyntaxGeneratorClientEndpoint : ISyntaxCodeGenerator
    {
        public SyntaxGeneratorClientEndpoint(
            ApiProjectOptions apiProjectOptions,
            List<ApiOperationSchemaMap> operationSchemaMappings,
            OperationType apiOperationType,
            OpenApiOperation apiOperation,
            string focusOnSegmentName,
            string urlPath,
            bool hasParametersOrRequestBody)
        {
            this.ApiProjectOptions = apiProjectOptions ?? throw new ArgumentNullException(nameof(apiProjectOptions));
            this.OperationSchemaMappings = operationSchemaMappings ?? throw new ArgumentNullException(nameof(apiProjectOptions));
            this.ApiOperationType = apiOperationType;
            this.ApiOperation = apiOperation ?? throw new ArgumentNullException(nameof(apiOperation));
            this.FocusOnSegmentName = focusOnSegmentName ?? throw new ArgumentNullException(nameof(focusOnSegmentName));
            this.ApiUrlPath = urlPath ?? throw new ArgumentNullException(nameof(urlPath));
            this.HasParametersOrRequestBody = hasParametersOrRequestBody;
        }

        public ApiProjectOptions ApiProjectOptions { get; }

        private List<ApiOperationSchemaMap> OperationSchemaMappings { get; }

        public OperationType ApiOperationType { get; }

        public OpenApiOperation ApiOperation { get; }

        public string ApiUrlPath { get; }

        public string FocusOnSegmentName { get; }

        public CompilationUnitSyntax? Code { get; private set; }

        public string InterfaceTypeName => "I" + ApiOperation.GetOperationName() + NameConstants.Endpoint;

        public string ParameterTypeName => ApiOperation.GetOperationName() + NameConstants.ContractParameters;

        public string EndpointTypeName => ApiOperation.GetOperationName() + NameConstants.Endpoint;

        public bool HasParametersOrRequestBody { get; }

        public bool GenerateCode()
        {
            // Create compilationUnit
            var compilationUnit = SyntaxFactory.CompilationUnit();

            // Create a namespace
            var @namespace = SyntaxProjectFactory.CreateNamespace(
                ApiProjectOptions,
                NameConstants.Endpoints,
                FocusOnSegmentName);

            // Create class
            var classDeclaration = SyntaxClassDeclarationFactory.CreateWithInterface(EndpointTypeName, InterfaceTypeName)
                .AddGeneratedCodeAttribute(ApiProjectOptions.ToolName, ApiProjectOptions.ToolVersion.ToString())
                .WithLeadingTrivia(SyntaxDocumentationFactory.CreateForResults(ApiOperation, FocusOnSegmentName));

            classDeclaration = classDeclaration.AddMembers(CreateFieldIHttpClientFactory());
            classDeclaration = classDeclaration.AddMembers(CreateFieldIHttpMessageFactory());
            classDeclaration = classDeclaration.AddMembers(CreateConstructor());
            classDeclaration = classDeclaration.AddMembers(CreateMembers());

            // TODO: Imp. this.
            var usedApiOperations = new List<OpenApiOperation>();

            // Add using statement to compilationUnit
            var includeRestResults = classDeclaration
                .Select<IdentifierNameSyntax>()
                .Any(x => x.Identifier.ValueText.Contains(
                    $"({Microsoft.OpenApi.Models.NameConstants.Pagination}<",
                    StringComparison.Ordinal));

            compilationUnit = compilationUnit.AddUsingStatements(
                ProjectEndpointsFactory.CreateUsingList(
                    ApiProjectOptions,
                    FocusOnSegmentName,
                    usedApiOperations,
                    includeRestResults,
                    HasSharedResponseContract(),
                    forClient: true));

            // Add the class to the namespace.
            @namespace = @namespace.AddMembers(classDeclaration);

            // Add namespace to compilationUnit
            compilationUnit = compilationUnit.AddMembers(@namespace);

            // Set code property
            Code = compilationUnit;
            return true;
        }

        public string ToCodeAsString()
        {
            if (Code == null)
            {
                GenerateCode();
            }

            if (Code == null)
            {
                return $"Syntax generate problem for client-endpoint for apiOperation: {ApiOperation}";
            }

            return Code
                .NormalizeWhitespace()
                .ToFullString()
                .FormatClientEndpointNewLineOnFluentMethod()
                .FormatClientEndpointNewLineSpace();
        }

        public LogKeyValueItem ToFile()
        {
            var area = FocusOnSegmentName.EnsureFirstCharacterToUpper();
            var endpointName = ApiOperation.GetOperationName() + NameConstants.Endpoint;
            var file = Util.GetCsFileNameForContract(ApiProjectOptions.PathForEndpoints, area, endpointName);
            return TextFileHelper.Save(file, ToCodeAsString());
        }

        public void ToFile(FileInfo file)
        {
            if (file == null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            TextFileHelper.Save(file, ToCodeAsString());
        }

        public override string ToString()
        {
            return $"OperationType: {ApiOperationType}, OperationName: {ApiOperation.GetOperationName()}, SegmentName: {FocusOnSegmentName}";
        }

        // CopyFrom - SyntaxGeneratorEndpointControllers
        private bool HasSharedResponseContract()
        {
            foreach (var (_, value) in ApiProjectOptions.Document.GetPathsByBasePathSegmentName(FocusOnSegmentName))
            {
                foreach (var apiOperation in value.Operations)
                {
                    if (apiOperation.Value.Responses == null)
                    {
                        continue;
                    }

                    var responseModelName = apiOperation.Value.Responses.GetModelNameForStatusCode(HttpStatusCode.OK);
                    var isSharedResponseModel = !string.IsNullOrEmpty(responseModelName) &&
                                                OperationSchemaMappings.IsShared(responseModelName);
                    if (isSharedResponseModel)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private MemberDeclarationSyntax CreateFieldIHttpClientFactory()
        {
            return
                SyntaxFactory.FieldDeclaration(
                    SyntaxFactory
                        .VariableDeclaration(
                            SyntaxFactory.IdentifierName("IHttpClientFactory"))
                        .WithVariables(
                            SyntaxFactory.SingletonSeparatedList(
                                SyntaxFactory.VariableDeclarator(
                                    SyntaxFactory.Identifier("factory")))))
                .WithModifiers(
                    SyntaxTokenListFactory.PrivateReadonlyKeyword());
        }

        private MemberDeclarationSyntax CreateFieldIHttpMessageFactory()
        {
            return
                SyntaxFactory.FieldDeclaration(
                    SyntaxFactory.VariableDeclaration(
                            SyntaxFactory.IdentifierName("IHttpMessageFactory"))
                        .WithVariables(
                            SyntaxFactory.SingletonSeparatedList(
                                SyntaxFactory.VariableDeclarator(
                                    SyntaxFactory.Identifier("httpMessageFactory")))))
                .WithModifiers(
                    SyntaxTokenListFactory.PrivateReadonlyKeyword());
        }

        private MemberDeclarationSyntax CreateConstructor()
        {
            return
                SyntaxFactory.ConstructorDeclaration(
                        SyntaxFactory.Identifier(EndpointTypeName))
                    .WithModifiers(
                        SyntaxFactory.TokenList(
                            SyntaxTokenFactory.PublicKeyword()))
                    .WithParameterList(
                        SyntaxFactory.ParameterList(
                            SyntaxFactory.SeparatedList<ParameterSyntax>(
                                new SyntaxNodeOrToken[]
                                {
                                    SyntaxFactory.Parameter(SyntaxFactory.Identifier("factory"))
                                        .WithType(SyntaxFactory.IdentifierName("IHttpClientFactory")),
                                    SyntaxTokenFactory.Comma(), SyntaxFactory
                                        .Parameter(SyntaxFactory.Identifier("httpMessageFactory"))
                                        .WithType(SyntaxFactory.IdentifierName("IHttpMessageFactory")),
                                })))
                    .WithBody(
                        SyntaxFactory.Block(
                            SyntaxFactory.ExpressionStatement(
                                SyntaxFactory.AssignmentExpression(
                                    SyntaxKind.SimpleAssignmentExpression,
                                    SyntaxFactory.MemberAccessExpression(
                                        SyntaxKind.SimpleMemberAccessExpression,
                                        SyntaxFactory.ThisExpression(),
                                        SyntaxFactory.IdentifierName("factory")),
                                    SyntaxFactory.IdentifierName("factory"))),
                            SyntaxFactory.ExpressionStatement(
                                SyntaxFactory.AssignmentExpression(
                                    SyntaxKind.SimpleAssignmentExpression,
                                    SyntaxFactory.MemberAccessExpression(
                                        SyntaxKind.SimpleMemberAccessExpression,
                                        SyntaxFactory.ThisExpression(),
                                        SyntaxFactory.IdentifierName("httpMessageFactory")),
                                    SyntaxFactory.IdentifierName("httpMessageFactory")))));
        }

        private MemberDeclarationSyntax[] CreateMembers()
        {
            var responseTypes = ApiOperation.Responses.GetResponseTypes(
                FocusOnSegmentName,
                OperationSchemaMappings,
                ApiProjectOptions.ProjectName);

            string resultTypeName = responseTypes
                .FirstOrDefault(x => x.Item1 == HttpStatusCode.OK)?.Item2 ?? responseTypes
                .FirstOrDefault(x => x.Item1 == HttpStatusCode.Created)?.Item2 ?? "string";

            var result = new List<MemberDeclarationSyntax>
            {
                CreateExecuteAsyncMethod(ParameterTypeName, resultTypeName, HasParametersOrRequestBody),
            };

            if (HasParametersOrRequestBody)
            {
                result.Add(CreateInvokeExecuteAsyncMethod(ParameterTypeName, resultTypeName, HasParametersOrRequestBody));
            }

            return result.ToArray();
        }

        private MemberDeclarationSyntax CreateExecuteAsyncMethod(string parameterTypeName, string resultTypeName, bool hasParameters)
        {
            var arguments = hasParameters
                ? new SyntaxNodeOrToken[]
                {
                    SyntaxParameterFactory.Create(parameterTypeName, "parameters"),
                    SyntaxTokenFactory.Comma(),
                    SyntaxParameterFactory.Create(nameof(CancellationToken), nameof(CancellationToken).EnsureFirstCharacterToLower())
                        .WithDefault(SyntaxFactory.EqualsValueClause(
                            SyntaxFactory.LiteralExpression(SyntaxKind.DefaultLiteralExpression, SyntaxTokenFactory.DefaultKeyword()))),
                }
                : new SyntaxNodeOrToken[]
                {
                    SyntaxParameterFactory.Create(nameof(CancellationToken), nameof(CancellationToken).EnsureFirstCharacterToLower())
                        .WithDefault(SyntaxFactory.EqualsValueClause(
                            SyntaxFactory.LiteralExpression(SyntaxKind.DefaultLiteralExpression, SyntaxTokenFactory.DefaultKeyword()))),
                };

            var codeBody = hasParameters
                ? SyntaxFactory.Block(
                    SyntaxIfStatementFactory.CreateParameterArgumentNullCheck("parameters"),
                    SyntaxFactory.ReturnStatement(
                        SyntaxFactory.InvocationExpression(
                                SyntaxFactory.IdentifierName("InvokeExecuteAsync"))
                            .WithArgumentList(
                                SyntaxFactory.ArgumentList(
                                    SyntaxFactory.SeparatedList<ArgumentSyntax>(
                                        new SyntaxNodeOrToken[]
                                        {
                                            SyntaxFactory.Argument(SyntaxFactory.IdentifierName("parameters")),
                                            SyntaxTokenFactory.Comma(),
                                            SyntaxFactory.Argument(SyntaxFactory.IdentifierName(nameof(CancellationToken).EnsureFirstCharacterToLower())),
                                        })))))
                : SyntaxFactory.Block(
                    SyntaxThrowStatementFactory.CreateNotImplementedException());

            return SyntaxFactory.MethodDeclaration(
                    SyntaxFactory.GenericName(SyntaxFactory.Identifier(nameof(Task)))
                        .WithTypeArgumentList(
                            SyntaxFactory.TypeArgumentList(
                                SyntaxFactory.SingletonSeparatedList<TypeSyntax>(
                                    SyntaxFactory.GenericName(
                                            SyntaxFactory.Identifier("EndpointResult"))
                                        .WithTypeArgumentList(
                                            SyntaxFactory.TypeArgumentList(
                                                SyntaxFactory.SingletonSeparatedList<TypeSyntax>(
                                                    SyntaxFactory.IdentifierName(resultTypeName))))))),
                    SyntaxFactory.Identifier("ExecuteAsync"))
                .WithModifiers(SyntaxTokenListFactory.PublicKeyword())
                .WithParameterList(SyntaxFactory.ParameterList(SyntaxFactory.SeparatedList<ParameterSyntax>(arguments)))
                .WithBody(codeBody);
        }

        private MemberDeclarationSyntax CreateInvokeExecuteAsyncMethod(string parameterTypeName, string resultTypeName, bool hasParameters)
        {
            var arguments = hasParameters
                ? new SyntaxNodeOrToken[]
                {
                    SyntaxParameterFactory.Create(parameterTypeName, "parameters"),
                    SyntaxTokenFactory.Comma(),
                    SyntaxParameterFactory.Create(nameof(CancellationToken), nameof(CancellationToken).EnsureFirstCharacterToLower()),
                }
                : new SyntaxNodeOrToken[]
                {
                    SyntaxParameterFactory.Create(nameof(CancellationToken), nameof(CancellationToken).EnsureFirstCharacterToLower()),
                };

            return SyntaxFactory.MethodDeclaration(
                    SyntaxFactory.GenericName(SyntaxFactory.Identifier(nameof(Task)))
                        .WithTypeArgumentList(
                            SyntaxFactory.TypeArgumentList(
                                SyntaxFactory.SingletonSeparatedList<TypeSyntax>(
                                    SyntaxFactory.GenericName(
                                            SyntaxFactory.Identifier("EndpointResult"))
                                        .WithTypeArgumentList(
                                            SyntaxFactory.TypeArgumentList(
                                                SyntaxFactory.SingletonSeparatedList<TypeSyntax>(
                                                    SyntaxFactory.IdentifierName(resultTypeName))))))),
                    SyntaxFactory.Identifier("InvokeExecuteAsync"))
                .WithModifiers(SyntaxTokenListFactory.PrivateAsyncKeyword())
                .WithParameterList(
                    SyntaxFactory.ParameterList(SyntaxFactory.SeparatedList<ParameterSyntax>(arguments)))
                .WithBody(
                    SyntaxFactory.Block(
                        CreateInvokeExecuteAsyncMethodBlockLocalClient(),
                        CreateInvokeExecuteAsyncMethodBlockLocalRequestBuilder(),
                        CreateInvokeExecuteAsyncMethodBlockLocalRequestMessage(),
                        CreateInvokeExecuteAsyncMethodBlockLocalResponse(),
                        CreateInvokeExecuteAsyncMethodBlockReturn(resultTypeName)));
        }

        private LocalDeclarationStatementSyntax CreateInvokeExecuteAsyncMethodBlockLocalClient()
        {
            return SyntaxFactory.LocalDeclarationStatement(
                SyntaxFactory.VariableDeclaration(
                        SyntaxFactory.IdentifierName("var"))
                    .WithVariables(
                        SyntaxFactory.SingletonSeparatedList(
                            SyntaxFactory.VariableDeclarator(
                                    SyntaxFactory.Identifier("client"))
                                .WithInitializer(
                                    SyntaxFactory.EqualsValueClause(
                                        SyntaxFactory.InvocationExpression(
                                                SyntaxFactory.MemberAccessExpression(
                                                    SyntaxKind.SimpleMemberAccessExpression,
                                                    SyntaxFactory.IdentifierName("factory"),
                                                    SyntaxFactory.IdentifierName("CreateClient")))
                                            .WithArgumentList(
                                                SyntaxFactory.ArgumentList(
                                                    SyntaxFactory.SingletonSeparatedList(
                                                        SyntaxFactory.Argument(
                                                            SyntaxFactory.LiteralExpression(
                                                                SyntaxKind.StringLiteralExpression,
                                                                SyntaxFactory.Literal(
                                                                    $"{ApiProjectOptions.ProjectPrefixName}-ApiClient")))))))))));
        }

        private LocalDeclarationStatementSyntax CreateInvokeExecuteAsyncMethodBlockLocalRequestBuilder()
        {
            ////var xxx = SyntaxFactory.SingletonList<MemberDeclarationSyntax>(
            ////    SyntaxFactory.GlobalStatement(
            ////        SyntaxFactory.LocalDeclarationStatement(
            ////            SyntaxFactory.VariableDeclaration(
            ////                SyntaxFactory.IdentifierName("var"))
            ////            .WithVariables(
            ////                SyntaxFactory.SingletonSeparatedList<VariableDeclaratorSyntax>(
            ////                    SyntaxFactory.VariableDeclarator(
            ////                        SyntaxFactory.Identifier("requestBuilder"))
            ////                    .WithInitializer(
            ////                        SyntaxFactory.EqualsValueClause(
            ////                            SyntaxFactory.InvocationExpression(
            ////                                SyntaxFactory.MemberAccessExpression(
            ////                                    SyntaxKind.SimpleMemberAccessExpression,
            ////                                    SyntaxFactory.InvocationExpression(
            ////                                        SyntaxFactory.MemberAccessExpression(
            ////                                            SyntaxKind.SimpleMemberAccessExpression,
            ////                                            SyntaxFactory.IdentifierName("httpMessageFactory"),
            ////                                            SyntaxFactory.IdentifierName("FromTemplate")))
            ////                                    .WithArgumentList(
            ////                                        SyntaxFactory.ArgumentList(
            ////                                            SyntaxFactory.SingletonSeparatedList<ArgumentSyntax>(
            ////                                                SyntaxFactory.Argument(
            ////                                                    SyntaxFactory.LiteralExpression(
            ////                                                        SyntaxKind.StringLiteralExpression,
            ////                                                        SyntaxFactory.Literal("/api/v1/locations/{locationId}")))))),
            ////                                    SyntaxFactory.IdentifierName("FromHest")))
            ////                            .WithArgumentList(
            ////                                SyntaxFactory.ArgumentList(
            ////                                    SyntaxFactory.SeparatedList<ArgumentSyntax>(
            ////                                        new SyntaxNodeOrToken[]{
            ////                                            SyntaxFactory.Argument(
            ////                                                SyntaxFactory.LiteralExpression(
            ////                                                    SyntaxKind.StringLiteralExpression,
            ////                                                    SyntaxFactory.Literal("GRIS"))),
            ////                                            SyntaxFactory.Token(SyntaxKind.CommaToken),
            ////                                            SyntaxFactory.Argument(
            ////                                                SyntaxFactory.MemberAccessExpression(
            ////                                                    SyntaxKind.SimpleMemberAccessExpression,
            ////                                                    SyntaxFactory.IdentifierName("parameters"),
            ////                                                    SyntaxFactory.IdentifierName("HALLO"))),
            ////                                        }))))))))));

            ////var mae1 = SyntaxFactory.InvocationExpression(
            ////    SyntaxFactory.MemberAccessExpression(
            ////        SyntaxKind.SimpleMemberAccessExpression,
            ////        SyntaxFactory.IdentifierName("httpMessageFactory"),
            ////        SyntaxFactory.IdentifierName("FromTemplate")));

            var equalsClauseSyntax = SyntaxFactory.EqualsValueClause(
                SyntaxFactory.InvocationExpression(
                        SyntaxFactory.MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            SyntaxFactory.IdentifierName("httpMessageFactory"),
                            SyntaxFactory.IdentifierName("FromTemplate")))
                    .WithArgumentList(CreateOneStringArg($"/api/{ApiProjectOptions.ApiVersion}{ApiUrlPath}")));

            return SyntaxFactory.LocalDeclarationStatement(
                SyntaxFactory.VariableDeclaration(
                    SyntaxFactory.IdentifierName("var"))
                .WithVariables(
                    SyntaxFactory.SingletonSeparatedList(
                        SyntaxFactory.VariableDeclarator(
                            SyntaxFactory.Identifier("requestBuilder"))
                        .WithInitializer(equalsClauseSyntax))));

            ////return SyntaxFactory.LocalDeclarationStatement(
            ////    SyntaxFactory.VariableDeclaration(
            ////        SyntaxFactory.IdentifierName("var"))
            ////    .WithVariables(
            ////        SyntaxFactory.SingletonSeparatedList(
            ////            SyntaxFactory.VariableDeclarator(
            ////                SyntaxFactory.Identifier("requestBuilder"))
            ////            .WithInitializer(
            ////                SyntaxFactory.EqualsValueClause(
            ////                    SyntaxFactory.InvocationExpression(
            ////                        SyntaxFactory.MemberAccessExpression(
            ////                            SyntaxKind.SimpleMemberAccessExpression,
            ////                            SyntaxFactory.InvocationExpression(
            ////                                SyntaxFactory.MemberAccessExpression(
            ////                                    SyntaxKind.SimpleMemberAccessExpression,
            ////                                    SyntaxFactory.IdentifierName("httpMessageFactory"),
            ////                                    SyntaxFactory.IdentifierName("FromTemplate")))
            ////                            .WithArgumentList(
            ////                                SyntaxFactory.ArgumentList(
            ////                                    SyntaxFactory.SingletonSeparatedList<ArgumentSyntax>(
            ////                                        SyntaxFactory.Argument(
            ////                                            SyntaxFactory.LiteralExpression(
            ////                                                SyntaxKind.StringLiteralExpression,
            ////                                                SyntaxFactory.Literal($"/api/{ApiProjectOptions.ApiVersion}{ApiUrlPath}")))))),
            ////                            SyntaxFactory.IdentifierName("WithPathParameter")))
            ////                    .WithArgumentList(CreateOneParameterArg("serverId", "ServerId")))))));
        }

        private LocalDeclarationStatementSyntax CreateInvokeExecuteAsyncMethodBlockLocalRequestMessage()
        {
            return SyntaxFactory.LocalDeclarationStatement(
                    SyntaxFactory.VariableDeclaration(
                            SyntaxFactory.IdentifierName("var"))
                        .WithVariables(
                            SyntaxFactory.SingletonSeparatedList(
                                SyntaxFactory.VariableDeclarator(
                                        SyntaxFactory.Identifier("requestMessage"))
                                    .WithInitializer(
                                        SyntaxFactory.EqualsValueClause(
                                            SyntaxFactory.InvocationExpression(
                                                    SyntaxFactory.MemberAccessExpression(
                                                        SyntaxKind.SimpleMemberAccessExpression,
                                                        SyntaxFactory.IdentifierName("requestBuilder"),
                                                        SyntaxFactory.IdentifierName("Build"))) // TODO: nameof
                                                .WithArgumentList(
                                                    SyntaxFactory.ArgumentList(
                                                        SyntaxFactory.SingletonSeparatedList(
                                                            SyntaxFactory.Argument(
                                                                SyntaxFactory.MemberAccessExpression(
                                                                    SyntaxKind.SimpleMemberAccessExpression,
                                                                    SyntaxFactory.IdentifierName(nameof(HttpMethod)),
                                                                    SyntaxFactory.IdentifierName(ApiOperationType.ToString())))))))))))
                .WithUsingKeyword(
                    SyntaxFactory.Token(SyntaxKind.UsingKeyword));
        }

        private LocalDeclarationStatementSyntax CreateInvokeExecuteAsyncMethodBlockLocalResponse()
        {
            return SyntaxFactory.LocalDeclarationStatement(
                    SyntaxFactory.VariableDeclaration(
                            SyntaxFactory.IdentifierName("var"))
                        .WithVariables(
                            SyntaxFactory.SingletonSeparatedList(
                                SyntaxFactory.VariableDeclarator(
                                        SyntaxFactory.Identifier("response"))
                                    .WithInitializer(
                                        SyntaxFactory.EqualsValueClause(
                                            SyntaxFactory.AwaitExpression(
                                                SyntaxFactory.InvocationExpression(
                                                        SyntaxFactory.MemberAccessExpression(
                                                            SyntaxKind.SimpleMemberAccessExpression,
                                                            SyntaxFactory.IdentifierName("client"),
                                                            SyntaxFactory.IdentifierName(nameof(HttpClient.SendAsync))))
                                                    .WithArgumentList(
                                                        SyntaxFactory.ArgumentList(
                                                            SyntaxFactory.SeparatedList<ArgumentSyntax>(
                                                                new SyntaxNodeOrToken[]
                                                                {
                                                                    SyntaxFactory.Argument(SyntaxFactory.IdentifierName("requestMessage")),
                                                                    SyntaxFactory.Token(SyntaxKind.CommaToken),
                                                                    SyntaxFactory.Argument(SyntaxFactory.IdentifierName("cancellationToken")),
                                                                })))))))))
                .WithUsingKeyword(
                    SyntaxFactory.Token(SyntaxKind.UsingKeyword));
        }

        private ReturnStatementSyntax CreateInvokeExecuteAsyncMethodBlockReturn(string resultTypeName)
        {
            return SyntaxFactory.ReturnStatement(
                SyntaxFactory.AwaitExpression(
                    SyntaxFactory.InvocationExpression(
                        SyntaxFactory.MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            SyntaxFactory.InvocationExpression(
                                SyntaxFactory.MemberAccessExpression(
                                    SyntaxKind.SimpleMemberAccessExpression,
                                    SyntaxFactory.InvocationExpression(
                                        SyntaxFactory.MemberAccessExpression(
                                            SyntaxKind.SimpleMemberAccessExpression,
                                            SyntaxFactory.InvocationExpression(
                                                SyntaxFactory.MemberAccessExpression(
                                                    SyntaxKind.SimpleMemberAccessExpression,
                                                    SyntaxFactory.InvocationExpression(
                                                        SyntaxFactory.MemberAccessExpression(
                                                            SyntaxKind.SimpleMemberAccessExpression,
                                                            SyntaxFactory.IdentifierName("httpMessageFactory"),
                                                            SyntaxFactory.IdentifierName("FromResponse")))
                                                    .WithArgumentList(
                                                        SyntaxFactory.ArgumentList(
                                                            SyntaxFactory.SingletonSeparatedList(
                                                                SyntaxFactory.Argument(
                                                                    SyntaxFactory.IdentifierName("response"))))),
                                                    SyntaxFactory.GenericName(
                                                            SyntaxFactory.Identifier("AddSuccessResponse")) // TODO: nameof
                                                    .WithTypeArgumentList(
                                                        SyntaxFactory.TypeArgumentList(
                                                            SyntaxFactory.SingletonSeparatedList<TypeSyntax>(
                                                                SyntaxFactory.IdentifierName(resultTypeName))))))
                                                    .WithArgumentList(CreateResponseArgHttpStatusCode(HttpStatusCode.OK)),
                                            SyntaxFactory.GenericName(
                                                    SyntaxFactory.Identifier("AddErrorResponse")) // TODO: nameof
                                            .WithTypeArgumentList(
                                                SyntaxFactory.TypeArgumentList(
                                                    SyntaxFactory.SingletonSeparatedList<TypeSyntax>(
                                                        SyntaxFactory.IdentifierName("ProblemDetails")))))) // TODO: nameof
                                            .WithArgumentList(CreateResponseArgHttpStatusCode(HttpStatusCode.BadRequest)),
                                    SyntaxFactory.IdentifierName("AddErrorResponse"))) // TODO: nameof
                                .WithArgumentList(CreateResponseArgHttpStatusCode(HttpStatusCode.NotFound)),
                            SyntaxFactory.IdentifierName("BuildResponseAsync"))) // TODO: nameof
                        .WithArgumentList(
                            SyntaxFactory.ArgumentList(
                                SyntaxFactory.SeparatedList<ArgumentSyntax>(
                                    new SyntaxNodeOrToken[]
                                    {
                                        SyntaxFactory.Argument(
                                            SyntaxFactory.SimpleLambdaExpression(
                                                SyntaxFactory.Parameter(
                                                    SyntaxFactory.Identifier("x")))
                                                .WithExpressionBody(
                                                    SyntaxFactory.ObjectCreationExpression(
                                                        SyntaxFactory.GenericName(
                                                                SyntaxFactory.Identifier("EndpointResult")) // TODO: nameof
                                                            .WithTypeArgumentList(
                                                                SyntaxFactory.TypeArgumentList(
                                                                    SyntaxFactory.SingletonSeparatedList<TypeSyntax>(
                                                                        SyntaxFactory.IdentifierName(resultTypeName)))))
                                                        .WithArgumentList(
                                                            SyntaxFactory.ArgumentList(
                                                                SyntaxFactory.SingletonSeparatedList(
                                                                    SyntaxFactory.Argument(
                                                                        SyntaxFactory.IdentifierName("x"))))))),
                                        SyntaxTokenFactory.Comma(),
                                        SyntaxFactory.Argument(SyntaxFactory.IdentifierName("cancellationToken")),
                                    })))));
        }

        private static ArgumentListSyntax CreateResponseArgHttpStatusCode(HttpStatusCode httpStatusCode) =>
            SyntaxFactory.ArgumentList(
                SyntaxFactory.SingletonSeparatedList(
                    SyntaxFactory.Argument(
                        SyntaxFactory.MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            SyntaxFactory.IdentifierName(nameof(HttpStatusCode)),
                            SyntaxFactory.IdentifierName(httpStatusCode.ToString())))));

        private ArgumentListSyntax CreateOneStringArg(string value) =>
            SyntaxFactory.ArgumentList(
                SyntaxFactory.SingletonSeparatedList(
                    SyntaxFactory.Argument(
                        SyntaxFactory.LiteralExpression(
                            SyntaxKind.StringLiteralExpression,
                            SyntaxFactory.Literal(value)))));

        private static ArgumentListSyntax CreateOneParameterArg(string parameterNameTo, string parameterNameFrom) =>
            SyntaxFactory.ArgumentList(
                SyntaxFactory.SeparatedList<ArgumentSyntax>(
                    new SyntaxNodeOrToken[]
                    {
                        SyntaxFactory.Argument(
                            SyntaxFactory.LiteralExpression(
                                SyntaxKind.StringLiteralExpression,
                                SyntaxFactory.Literal(parameterNameTo))),
                        SyntaxTokenFactory.Comma(),
                        SyntaxFactory.Argument(
                            SyntaxFactory.MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                SyntaxFactory.IdentifierName("parameters"),
                                SyntaxFactory.IdentifierName(parameterNameFrom))),
                    }));
    }
}