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
using Atc.Rest.ApiGenerator.Factories;
using Atc.Rest.ApiGenerator.Helpers;
using Atc.Rest.ApiGenerator.Models;
using Atc.Rest.ApiGenerator.ProjectSyntaxFactories;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.OpenApi.Models;

// ReSharper disable UseObjectOrCollectionInitializer
// ReSharper disable InvertIf
namespace Atc.Rest.ApiGenerator.SyntaxGenerators.ApiClient
{
    public class SyntaxGeneratorClientEndpoint : ISyntaxCodeGenerator
    {
        public SyntaxGeneratorClientEndpoint(
            ApiProjectOptions apiProjectOptions,
            List<ApiOperationSchemaMap> operationSchemaMappings,
            IList<OpenApiParameter> globalPathParameters,
            OperationType apiOperationType,
            OpenApiOperation apiOperation,
            string focusOnSegmentName,
            string urlPath,
            bool hasParametersOrRequestBody)
        {
            this.ApiProjectOptions = apiProjectOptions ?? throw new ArgumentNullException(nameof(apiProjectOptions));
            this.OperationSchemaMappings = operationSchemaMappings ?? throw new ArgumentNullException(nameof(apiProjectOptions));
            this.GlobalPathParameters = globalPathParameters ?? throw new ArgumentNullException(nameof(globalPathParameters));
            this.ApiOperationType = apiOperationType;
            this.ApiOperation = apiOperation ?? throw new ArgumentNullException(nameof(apiOperation));
            this.FocusOnSegmentName = focusOnSegmentName ?? throw new ArgumentNullException(nameof(focusOnSegmentName));
            this.ApiUrlPath = urlPath ?? throw new ArgumentNullException(nameof(urlPath));
            this.HasParametersOrRequestBody = hasParametersOrRequestBody;
        }

        public ApiProjectOptions ApiProjectOptions { get; }

        private List<ApiOperationSchemaMap> OperationSchemaMappings { get; }

        public IList<OpenApiParameter> GlobalPathParameters { get; }

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

            // Add using statement to compilationUnit
            var includeRestResults = classDeclaration
                .Select<IdentifierNameSyntax>()
                .Any(x => x.Identifier.ValueText.Contains(
                    $"({Microsoft.OpenApi.Models.NameConstants.Pagination}<",
                    StringComparison.Ordinal));

            compilationUnit = compilationUnit.AddUsingStatements(
                ProjectApiClientFactory.CreateUsingListForEndpoint(
                    ApiProjectOptions,
                    includeRestResults,
                    ContractHelper.HasSharedResponseContract(
                        ApiProjectOptions.Document,
                        OperationSchemaMappings,
                        FocusOnSegmentName)));

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
                .FormatClientEndpointNewLineSpaceBefore8()
                .FormatClientEndpointNewLineSpaceAfter12();
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

        private MemberDeclarationSyntax CreateFieldIHttpClientFactory()
        {
            return
                SyntaxFactory.FieldDeclaration(
                    SyntaxFactory
                        .VariableDeclaration(SyntaxFactory.IdentifierName("IHttpClientFactory"))
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
                    SyntaxFactory.VariableDeclaration(SyntaxFactory.IdentifierName("IHttpMessageFactory"))
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
                OperationSchemaMappings,
                FocusOnSegmentName,
                ApiProjectOptions.ProjectName,
                ensureModelNameWithNamespaceIfNeeded: false,
                useProblemDetailsAsDefaultResponseBody: false,
                includeEmptyResponseTypes: false);

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

            SyntaxTokenList methodModifiers;
            BlockSyntax codeBlockSyntax;
            if (hasParameters)
            {
                methodModifiers = SyntaxTokenListFactory.PublicKeyword();

                codeBlockSyntax = SyntaxFactory.Block(
                    SyntaxIfStatementFactory.CreateParameterArgumentNullCheck("parameters", false),
                    SyntaxFactory.ReturnStatement(
                        SyntaxFactory.InvocationExpression(SyntaxFactory.IdentifierName("InvokeExecuteAsync"))
                            .WithArgumentList(
                                SyntaxArgumentListFactory.CreateWithTwoItems("parameters", nameof(CancellationToken).EnsureFirstCharacterToLower()))));
            }
            else
            {
                methodModifiers = SyntaxTokenListFactory.PublicAsyncKeyword();

                var bodyBlockSyntaxStatements = CreateBodyBlockSyntaxStatements(resultTypeName);
                codeBlockSyntax = SyntaxFactory.Block(bodyBlockSyntaxStatements);
            }

            return SyntaxFactory.MethodDeclaration(
                    SyntaxFactory.GenericName(SyntaxFactory.Identifier(nameof(Task)))
                        .WithTypeArgumentList(
                            SyntaxFactory.TypeArgumentList(
                                SyntaxFactory.SingletonSeparatedList<TypeSyntax>(
                                    SyntaxFactory.GenericName(
                                            SyntaxFactory.Identifier("EndpointResult"))
                                        .WithTypeArgumentList(
                                            SyntaxTypeArgumentListFactory.CreateWithOneItem(resultTypeName))))),
                    SyntaxFactory.Identifier("ExecuteAsync"))
                .WithModifiers(methodModifiers)
                .WithParameterList(SyntaxFactory.ParameterList(SyntaxFactory.SeparatedList<ParameterSyntax>(arguments)))
                .WithBody(codeBlockSyntax);
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

            var bodyBlockSyntaxStatements = CreateBodyBlockSyntaxStatements(resultTypeName);

            return SyntaxFactory.MethodDeclaration(
                    SyntaxFactory.GenericName(SyntaxFactory.Identifier(nameof(Task)))
                        .WithTypeArgumentList(
                            SyntaxFactory.TypeArgumentList(
                                SyntaxFactory.SingletonSeparatedList<TypeSyntax>(
                                    SyntaxFactory.GenericName(
                                            SyntaxFactory.Identifier("EndpointResult"))
                                        .WithTypeArgumentList(
                                            SyntaxTypeArgumentListFactory.CreateWithOneItem(resultTypeName))))),
                    SyntaxFactory.Identifier("InvokeExecuteAsync"))
                .WithModifiers(SyntaxTokenListFactory.PrivateAsyncKeyword())
                .WithParameterList(
                    SyntaxFactory.ParameterList(SyntaxFactory.SeparatedList<ParameterSyntax>(arguments)))
                .WithBody(
                    SyntaxFactory.Block(bodyBlockSyntaxStatements));
        }

        private List<StatementSyntax> CreateBodyBlockSyntaxStatements(string resultTypeName)
        {
            var bodyBlockSyntaxStatements = new List<StatementSyntax>();
            bodyBlockSyntaxStatements.Add(CreateInvokeExecuteAsyncMethodBlockLocalClient());
            bodyBlockSyntaxStatements.AddRange(CreateInvokeExecuteAsyncMethodBlockLocalRequestBuilder());
            bodyBlockSyntaxStatements.Add(CreateInvokeExecuteAsyncMethodBlockLocalRequestMessage());
            bodyBlockSyntaxStatements.Add(CreateInvokeExecuteAsyncMethodBlockLocalResponse());
            bodyBlockSyntaxStatements.AddRange(CreateInvokeExecuteAsyncMethodBlockLocalResponseBuilder(resultTypeName));
            return bodyBlockSyntaxStatements;
        }

        private LocalDeclarationStatementSyntax CreateInvokeExecuteAsyncMethodBlockLocalClient()
        {
            return SyntaxFactory.LocalDeclarationStatement(
                SyntaxFactory.VariableDeclaration(SyntaxFactory.IdentifierName("var"))
                    .WithVariables(
                        SyntaxFactory.SingletonSeparatedList(
                            SyntaxFactory.VariableDeclarator(
                                    SyntaxFactory.Identifier("client"))
                                .WithInitializer(
                                    SyntaxFactory.EqualsValueClause(
                                        SyntaxFactory.InvocationExpression(
                                                SyntaxMemberAccessExpressionFactory.Create("CreateClient", "factory"))
                                            .WithArgumentList(
                                                SyntaxFactory.ArgumentList(
                                                    SyntaxFactory.SingletonSeparatedList(
                                                        SyntaxFactory.Argument(
                                                            SyntaxFactory.LiteralExpression(
                                                                SyntaxKind.StringLiteralExpression,
                                                                SyntaxFactory.Literal(
                                                                    $"{ApiProjectOptions.ProjectPrefixName}-ApiClient")))))))))));
        }

        private StatementSyntax[] CreateInvokeExecuteAsyncMethodBlockLocalRequestBuilder()
        {
            var equalsClauseSyntax = SyntaxFactory.EqualsValueClause(
                SyntaxFactory.InvocationExpression(
                        SyntaxMemberAccessExpressionFactory.Create("FromTemplate", "httpMessageFactory"))
                    .WithArgumentList(CreateOneStringArg($"/api/{ApiProjectOptions.ApiVersion}{ApiUrlPath}")));

            var requestBuilderSyntax = SyntaxFactory.LocalDeclarationStatement(
                SyntaxFactory.VariableDeclaration(
                    SyntaxFactory.IdentifierName("var"))
                .WithVariables(
                    SyntaxFactory.SingletonSeparatedList(
                        SyntaxFactory.VariableDeclarator(
                            SyntaxFactory.Identifier("requestBuilder"))
                        .WithInitializer(equalsClauseSyntax))));

            var result = new List<StatementSyntax>
            {
                requestBuilderSyntax,
            };

            if (HasParametersOrRequestBody)
            {
                if (GlobalPathParameters.Any())
                {
                    result.AddRange(GlobalPathParameters.Select(CreateExpressionStatementForWithMethodParameterMap));
                }

                if (ApiOperation.Parameters != null)
                {
                    result.AddRange(ApiOperation.Parameters.Select(CreateExpressionStatementForWithMethodParameterMap));
                }

                var bodySchema = ApiOperation.RequestBody?.Content.GetSchema();
                if (bodySchema != null)
                {
                    result.Add(CreateExpressionStatementForWithMethodBodyMap());
                }
            }

            return result.ToArray();
        }

        private StatementSyntax CreateExpressionStatementForWithMethodParameterMap(OpenApiParameter parameter)
        {
            string methodName = parameter.In switch
            {
                ParameterLocation.Query => "WithQueryParameter",
                ParameterLocation.Header => "WithHeaderParameter",
                ParameterLocation.Path => "WithPathParameter",
                _ => throw new NotSupportedException(nameof(parameter.In))
            };

            var parameterMapName = parameter.Name;
            var parameterName = parameter.Name.EnsureFirstCharacterToUpper();

            return SyntaxFactory.ExpressionStatement(
                SyntaxFactory.InvocationExpression(
                    SyntaxMemberAccessExpressionFactory.Create(methodName, "requestBuilder"))
                .WithArgumentList(
                    SyntaxFactory.ArgumentList(
                        SyntaxFactory.SeparatedList<ArgumentSyntax>(
                            new SyntaxNodeOrToken[]
                            {
                                SyntaxFactory.Argument(
                                    SyntaxFactory.LiteralExpression(
                                        SyntaxKind.StringLiteralExpression,
                                        SyntaxFactory.Literal(parameterMapName))),
                                SyntaxTokenFactory.Comma(),
                                SyntaxFactory.Argument(
                                    SyntaxMemberAccessExpressionFactory.Create(parameterName, "parameters")),
                            }))));
        }

        private StatementSyntax CreateExpressionStatementForWithMethodBodyMap()
        {
            return SyntaxFactory.ExpressionStatement(
                SyntaxFactory.InvocationExpression(
                    SyntaxMemberAccessExpressionFactory.Create("WithBody", "requestBuilder"))
                .WithArgumentList(
                    SyntaxFactory.ArgumentList(
                        SyntaxFactory.SingletonSeparatedList(
                            SyntaxFactory.Argument(
                                SyntaxMemberAccessExpressionFactory.Create("Request", "parameters"))))));
        }

        private LocalDeclarationStatementSyntax CreateInvokeExecuteAsyncMethodBlockLocalRequestMessage()
        {
            return SyntaxFactory.LocalDeclarationStatement(
                    SyntaxFactory.VariableDeclaration(SyntaxFactory.IdentifierName("var"))
                        .WithVariables(
                            SyntaxFactory.SingletonSeparatedList(
                                SyntaxFactory.VariableDeclarator(SyntaxFactory.Identifier("requestMessage"))
                                    .WithInitializer(
                                        SyntaxFactory.EqualsValueClause(
                                            SyntaxFactory.InvocationExpression(
                                                    SyntaxMemberAccessExpressionFactory.Create("Build", "requestBuilder")) // TODO: nameof
                                                .WithArgumentList(
                                                    SyntaxFactory.ArgumentList(
                                                        SyntaxFactory.SingletonSeparatedList(
                                                            SyntaxFactory.Argument(
                                                                SyntaxMemberAccessExpressionFactory.Create(ApiOperationType.ToString(), nameof(HttpMethod)))))))))))
                .WithUsingKeyword(
                    SyntaxFactory.Token(SyntaxKind.UsingKeyword));
        }

        private LocalDeclarationStatementSyntax CreateInvokeExecuteAsyncMethodBlockLocalResponse()
        {
            return SyntaxFactory.LocalDeclarationStatement(
                    SyntaxFactory.VariableDeclaration(SyntaxFactory.IdentifierName("var"))
                        .WithVariables(
                            SyntaxFactory.SingletonSeparatedList(
                                SyntaxFactory.VariableDeclarator(SyntaxFactory.Identifier("response"))
                                    .WithInitializer(
                                        SyntaxFactory.EqualsValueClause(
                                            SyntaxFactory.AwaitExpression(
                                                SyntaxFactory.InvocationExpression(
                                                        SyntaxMemberAccessExpressionFactory.Create(nameof(HttpClient.SendAsync), "client"))
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

        private StatementSyntax[] CreateInvokeExecuteAsyncMethodBlockLocalResponseBuilder(string resultTypeName)
        {
            var result = new List<StatementSyntax>();
            result.Add(CreateInvokeExecuteAsyncMethodBlockLocalResponseBuilderFromResponse());
            result.Add(CreateInvokeExecuteAsyncMethodBlockLocalResponseBuilderAddSuccess(resultTypeName));
            result.AddRange(CreateInvokeExecuteAsyncMethodBlockLocalResponseBuilderAddErrors());
            result.Add(CreateInvokeExecuteAsyncMethodBlockLocalResponseBuilderReturnBuildResponse(resultTypeName));
            return result.ToArray();
        }

        private LocalDeclarationStatementSyntax CreateInvokeExecuteAsyncMethodBlockLocalResponseBuilderFromResponse()
        {
            return SyntaxFactory.LocalDeclarationStatement(
                SyntaxFactory.VariableDeclaration(
                        SyntaxFactory.IdentifierName("var"))
                    .WithVariables(
                        SyntaxFactory.SingletonSeparatedList(
                            SyntaxFactory.VariableDeclarator(
                                    SyntaxFactory.Identifier("responseBuilder"))
                                .WithInitializer(
                                    SyntaxFactory.EqualsValueClause(
                                        SyntaxFactory.InvocationExpression(
                                                SyntaxFactory.MemberAccessExpression(
                                                    SyntaxKind.SimpleMemberAccessExpression,
                                                    SyntaxFactory.IdentifierName("httpMessageFactory"),
                                                    SyntaxFactory.IdentifierName("FromResponse")))
                                            .WithArgumentList(
                                                SyntaxFactory.ArgumentList(
                                                    SyntaxFactory.SingletonSeparatedList(
                                                        SyntaxFactory.Argument(
                                                            SyntaxFactory.IdentifierName("response"))))))))));
        }

        private ExpressionStatementSyntax CreateInvokeExecuteAsyncMethodBlockLocalResponseBuilderAddSuccess(string resultTypeName)
        {
            return SyntaxFactory.ExpressionStatement(
                SyntaxFactory.InvocationExpression(
                        SyntaxFactory.MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            SyntaxFactory.IdentifierName("responseBuilder"),
                            SyntaxFactory.GenericName(
                                    SyntaxFactory.Identifier("AddSuccessResponse"))
                                .WithTypeArgumentList(
                                    SyntaxFactory.TypeArgumentList(
                                        SyntaxFactory.SingletonSeparatedList<TypeSyntax>(
                                            SyntaxFactory.IdentifierName(resultTypeName))))))
                    .WithArgumentList(
                        SyntaxFactory.ArgumentList(
                            SyntaxFactory.SingletonSeparatedList<ArgumentSyntax>(
                                SyntaxFactory.Argument(
                                    SyntaxFactory.MemberAccessExpression(
                                        SyntaxKind.SimpleMemberAccessExpression,
                                        SyntaxFactory.IdentifierName("HttpStatusCode"),
                                        SyntaxFactory.IdentifierName("OK")))))));
        }

        private StatementSyntax[] CreateInvokeExecuteAsyncMethodBlockLocalResponseBuilderAddErrors()
        {
            var responseTypes = ApiOperation.Responses.GetResponseTypes(
                OperationSchemaMappings,
                FocusOnSegmentName,
                ApiProjectOptions.ProjectName,
                ensureModelNameWithNamespaceIfNeeded: false,
                useProblemDetailsAsDefaultResponseBody: true,
                includeEmptyResponseTypes: false);

            // TODO: If HasParametersOrRequestBody-AND-Minimum-1-required-or-1-that-is-not-string
            if (HasParametersOrRequestBody &&
                responseTypes.All(x => x.Item1 != HttpStatusCode.BadRequest))
            {
                responseTypes.Add(new Tuple<HttpStatusCode, string>(HttpStatusCode.BadRequest, "ValidationProblemDetails"));
            }

            if (ApiProjectOptions.ApiOptions.Generator.UseAuthorization &&
                responseTypes.All(x => x.Item1 != HttpStatusCode.Unauthorized))
            {
                responseTypes.Add(new Tuple<HttpStatusCode, string>(HttpStatusCode.Unauthorized, "ProblemDetails"));
            }

            var result = new List<StatementSyntax>();
            foreach (var responseType in responseTypes
                .Where(x => x.Item1.IsClientOrServerError())
                .OrderBy(x => x.Item1))
            {
                // TODO: incomment.. trying out stuff..
                ////result.Add(CreateInvokeExecuteAsyncMethodBlockLocalResponseBuilderAddError(responseType));
            }

            return result.ToArray();
        }

        private ExpressionStatementSyntax CreateInvokeExecuteAsyncMethodBlockLocalResponseBuilderAddError(Tuple<HttpStatusCode, string> responseType)
        {
            return SyntaxFactory.ExpressionStatement(
                SyntaxFactory.InvocationExpression(
                        SyntaxFactory.MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            SyntaxFactory.IdentifierName("responseBuilder"),
                            SyntaxFactory.GenericName(
                                    SyntaxFactory.Identifier("AddErrorResponse"))
                                .WithTypeArgumentList(
                                    SyntaxFactory.TypeArgumentList(
                                        SyntaxFactory.SingletonSeparatedList<TypeSyntax>(
                                            SyntaxFactory.IdentifierName(responseType.Item2))))))
                    .WithArgumentList(
                        SyntaxFactory.ArgumentList(
                            SyntaxFactory.SingletonSeparatedList(
                                SyntaxFactory.Argument(
                                    SyntaxMemberAccessExpressionFactory.Create(responseType.Item1.ToString(), nameof(HttpStatusCode)))))));
        }

        private ReturnStatementSyntax CreateInvokeExecuteAsyncMethodBlockLocalResponseBuilderReturnBuildResponse(string resultTypeName)
        {
            return SyntaxFactory.ReturnStatement(
                SyntaxFactory.AwaitExpression(
                    SyntaxFactory.InvocationExpression(
                        SyntaxFactory.MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            SyntaxFactory.IdentifierName("responseBuilder"),
                            SyntaxFactory.IdentifierName("BuildResponseAsync")))
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
                                                    SyntaxFactory.Identifier("EndpointResult"))
                                                .WithTypeArgumentList(
                                                    SyntaxFactory.TypeArgumentList(
                                                        SyntaxFactory.SingletonSeparatedList<TypeSyntax>(
                                                            SyntaxFactory.IdentifierName(resultTypeName)))))
                                            .WithArgumentList(
                                                SyntaxFactory.ArgumentList(
                                                    SyntaxFactory.SingletonSeparatedList<ArgumentSyntax>(
                                                        SyntaxFactory.Argument(
                                                            SyntaxFactory.IdentifierName("x"))))))),
                                    SyntaxFactory.Token(SyntaxKind.CommaToken),
                                    SyntaxFactory.Argument(
                                        SyntaxFactory.IdentifierName("cancellationToken")),
                                })))));
        }

        private ArgumentListSyntax CreateOneStringArg(string value) =>
            SyntaxFactory.ArgumentList(
                SyntaxFactory.SingletonSeparatedList(
                    SyntaxFactory.Argument(
                        SyntaxFactory.LiteralExpression(
                            SyntaxKind.StringLiteralExpression,
                            SyntaxFactory.Literal(value)))));
    }
}