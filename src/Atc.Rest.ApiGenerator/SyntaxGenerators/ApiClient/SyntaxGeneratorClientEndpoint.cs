// ReSharper disable UseObjectOrCollectionInitializer
// ReSharper disable InvertIf
namespace Atc.Rest.ApiGenerator.SyntaxGenerators.ApiClient;

public class SyntaxGeneratorClientEndpoint : SyntaxGeneratorClientEndpointBase, ISyntaxCodeGenerator
{
    public SyntaxGeneratorClientEndpoint(
        ILogger logger,
        ApiProjectOptions apiProjectOptions,
        List<ApiOperationSchemaMap> operationSchemaMappings,
        IList<OpenApiParameter> globalPathParameters,
        OperationType apiOperationType,
        OpenApiOperation apiOperation,
        string focusOnSegmentName,
        string urlPath,
        bool hasParametersOrRequestBody)
        : base(
            logger,
            apiProjectOptions,
            operationSchemaMappings,
            globalPathParameters,
            apiOperationType,
            apiOperation,
            focusOnSegmentName,
            urlPath,
            hasParametersOrRequestBody)
    {
    }

    public CompilationUnitSyntax? Code { get; private set; }

    public string InterfaceTypeName => "I" + ApiOperation.GetOperationName() + NameConstants.Endpoint;

    public string ParameterTypeName => ApiOperation.GetOperationName() + NameConstants.ContractParameters;

    public string EndpointTypeName => ApiOperation.GetOperationName() + NameConstants.Endpoint;

    public string EndpointResultTypeName => ApiOperation.GetOperationName() + NameConstants.EndpointResult;

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
                HasParametersOrRequestBody,
                OpenApiDocumentSchemaModelNameHelper.HasList(ResultTypeName),
                OpenApiDocumentSchemaModelNameHelper.HasSharedResponseContract(
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
        if (Code is null)
        {
            GenerateCode();
        }

        if (Code is null)
        {
            return $"Syntax generate problem for client-endpoint for apiOperation: {ApiOperation}";
        }

        return Code
            .NormalizeWhitespace()
            .ToFullString()
            .EnsureEnvironmentNewLines()
            .FormatClientEndpointNewLineSpaceBefore8()
            .FormatClientEndpointNewLineSpaceAfter12();
    }

    public void ToFile()
    {
        var area = FocusOnSegmentName.EnsureFirstCharacterToUpper();
        var endpointName = ApiOperation.GetOperationName() + NameConstants.Endpoint;
        var file = Util.GetCsFileNameForContract(ApiProjectOptions.PathForEndpoints, area, endpointName);
        ToFile(new FileInfo(file));
    }

    public void ToFile(
        FileInfo file)
    {
        ArgumentNullException.ThrowIfNull(file);

        var fileDisplayLocation = file.FullName.Replace(ApiProjectOptions.PathForSrcGenerate.FullName, "src: ", StringComparison.Ordinal);
        TextFileHelper.Save(Logger, file.FullName, fileDisplayLocation, ToCodeAsString());
    }

    public override string ToString()
        => $"OperationType: {ApiOperationType}, OperationName: {ApiOperation.GetOperationName()}, SegmentName: {FocusOnSegmentName}";

    private MemberDeclarationSyntax CreateFieldIHttpClientFactory()
        => SyntaxFactory.FieldDeclaration(
                SyntaxFactory
                    .VariableDeclaration(SyntaxFactory.IdentifierName(nameof(IHttpClientFactory)))
                    .WithVariables(
                        SyntaxFactory.SingletonSeparatedList(
                            SyntaxFactory.VariableDeclarator(
                                SyntaxFactory.Identifier("factory")))))
            .WithModifiers(
                SyntaxTokenListFactory.PrivateReadonlyKeyword());

    private MemberDeclarationSyntax CreateFieldIHttpMessageFactory()
        => SyntaxFactory.FieldDeclaration(
                SyntaxFactory.VariableDeclaration(SyntaxFactory.IdentifierName(nameof(IHttpMessageFactory)))
                    .WithVariables(
                        SyntaxFactory.SingletonSeparatedList(
                            SyntaxFactory.VariableDeclarator(
                                SyntaxFactory.Identifier("httpMessageFactory")))))
            .WithModifiers(
                SyntaxTokenListFactory.PrivateReadonlyKeyword());

    private MemberDeclarationSyntax CreateConstructor()
        => SyntaxFactory.ConstructorDeclaration(
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
                                .WithType(SyntaxFactory.IdentifierName(nameof(IHttpClientFactory))),
                            SyntaxTokenFactory.Comma(), SyntaxFactory
                                .Parameter(SyntaxFactory.Identifier("httpMessageFactory"))
                                .WithType(SyntaxFactory.IdentifierName(nameof(IHttpMessageFactory))),
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

    private MemberDeclarationSyntax[] CreateMembers()
    {
        var result = new List<MemberDeclarationSyntax>
        {
            CreateExecuteAsyncMethod(ParameterTypeName, ResultTypeName, HasParametersOrRequestBody),
        };

        if (HasParametersOrRequestBody)
        {
            result.Add(CreateInvokeExecuteAsyncMethod(ParameterTypeName, ResultTypeName, HasParametersOrRequestBody));
        }

        return result.ToArray();
    }

    private MemberDeclarationSyntax CreateExecuteAsyncMethod(
        string parameterTypeName,
        string resultTypeName,
        bool hasParameters)
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
                                SyntaxFactory.IdentifierName(EndpointResultTypeName)))),
                SyntaxFactory.Identifier("ExecuteAsync"))
            .WithModifiers(methodModifiers)
            .WithParameterList(SyntaxFactory.ParameterList(SyntaxFactory.SeparatedList<ParameterSyntax>(arguments)))
            .WithBody(codeBlockSyntax);
    }

    private MemberDeclarationSyntax CreateInvokeExecuteAsyncMethod(
        string parameterTypeName,
        string resultTypeName,
        bool hasParameters)
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
                                SyntaxFactory.IdentifierName(EndpointResultTypeName)))),
                SyntaxFactory.Identifier("InvokeExecuteAsync"))
            .WithModifiers(SyntaxTokenListFactory.PrivateAsyncKeyword())
            .WithParameterList(
                SyntaxFactory.ParameterList(SyntaxFactory.SeparatedList<ParameterSyntax>(arguments)))
            .WithBody(
                SyntaxFactory.Block(bodyBlockSyntaxStatements));
    }

    private List<StatementSyntax> CreateBodyBlockSyntaxStatements(
        string resultTypeName)
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
        => SyntaxFactory.LocalDeclarationStatement(
            SyntaxFactory.VariableDeclaration(SyntaxFactory.IdentifierName("var"))
                .WithVariables(
                    SyntaxFactory.SingletonSeparatedList(
                        SyntaxFactory.VariableDeclarator(
                                SyntaxFactory.Identifier("client"))
                            .WithInitializer(
                                SyntaxFactory.EqualsValueClause(
                                    SyntaxFactory.InvocationExpression(
                                            SyntaxMemberAccessExpressionFactory.Create(nameof(IHttpClientFactory.CreateClient), "factory"))
                                        .WithArgumentList(
                                            SyntaxFactory.ArgumentList(
                                                SyntaxFactory.SingletonSeparatedList(
                                                    SyntaxFactory.Argument(
                                                        SyntaxFactory.LiteralExpression(
                                                            SyntaxKind.StringLiteralExpression,
                                                            SyntaxFactory.Literal(
                                                                $"{ApiProjectOptions.ProjectPrefixName}-ApiClient")))))))))));

    private StatementSyntax[] CreateInvokeExecuteAsyncMethodBlockLocalRequestBuilder()
    {
        var equalsClauseSyntax = SyntaxFactory.EqualsValueClause(
            SyntaxFactory.InvocationExpression(
                    SyntaxMemberAccessExpressionFactory.Create(nameof(IHttpMessageFactory.FromTemplate), "httpMessageFactory"))
                .WithArgumentList(CreateOneStringArg($"{ApiProjectOptions.RouteBase}{ApiUrlPath}")));

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

            if (ApiOperation.Parameters is not null)
            {
                result.AddRange(ApiOperation.Parameters.Select(CreateExpressionStatementForWithMethodParameterMap));
            }

            var bodySchema = ApiOperation.RequestBody?.Content.GetSchemaByFirstMediaType();
            if (bodySchema is not null)
            {
                result.Add(CreateExpressionStatementForWithMethodBodyMap());
            }
        }

        return result.ToArray();
    }

    private StatementSyntax CreateExpressionStatementForWithMethodParameterMap(
        OpenApiParameter parameter)
    {
        var methodName = parameter.In switch
        {
            ParameterLocation.Query => nameof(IMessageRequestBuilder.WithQueryParameter),
            ParameterLocation.Header => nameof(IMessageRequestBuilder.WithHeaderParameter),
            ParameterLocation.Path => nameof(IMessageRequestBuilder.WithPathParameter),
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
        => SyntaxFactory.ExpressionStatement(
            SyntaxFactory.InvocationExpression(
                    SyntaxMemberAccessExpressionFactory.Create(nameof(IMessageRequestBuilder.WithBody), "requestBuilder"))
                .WithArgumentList(
                    SyntaxFactory.ArgumentList(
                        SyntaxFactory.SingletonSeparatedList(
                            SyntaxFactory.Argument(
                                SyntaxMemberAccessExpressionFactory.Create("Request", "parameters"))))));

    private LocalDeclarationStatementSyntax CreateInvokeExecuteAsyncMethodBlockLocalRequestMessage()
        => SyntaxFactory.LocalDeclarationStatement(
                SyntaxFactory.VariableDeclaration(SyntaxFactory.IdentifierName("var"))
                    .WithVariables(
                        SyntaxFactory.SingletonSeparatedList(
                            SyntaxFactory.VariableDeclarator(SyntaxFactory.Identifier("requestMessage"))
                                .WithInitializer(
                                    SyntaxFactory.EqualsValueClause(
                                        SyntaxFactory.InvocationExpression(
                                                SyntaxMemberAccessExpressionFactory.Create(nameof(IMessageRequestBuilder.Build), "requestBuilder"))
                                            .WithArgumentList(
                                                SyntaxFactory.ArgumentList(
                                                    SyntaxFactory.SingletonSeparatedList(
                                                        SyntaxFactory.Argument(
                                                            SyntaxMemberAccessExpressionFactory.Create(ApiOperationType.ToString(), nameof(HttpMethod)))))))))))
            .WithUsingKeyword(SyntaxFactory.Token(SyntaxKind.UsingKeyword));

    private LocalDeclarationStatementSyntax CreateInvokeExecuteAsyncMethodBlockLocalResponse()
        => SyntaxFactory.LocalDeclarationStatement(
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
                                                                SyntaxFactory.Argument(SyntaxFactory.IdentifierName(nameof(CancellationToken).EnsureFirstCharacterToLower())),
                                                            })))))))))
            .WithUsingKeyword(SyntaxFactory.Token(SyntaxKind.UsingKeyword));

    private StatementSyntax[] CreateInvokeExecuteAsyncMethodBlockLocalResponseBuilder(
        string resultTypeName)
    {
        var result = new List<StatementSyntax>();
        result.Add(CreateInvokeExecuteAsyncMethodBlockLocalResponseBuilderFromResponse());
        result.Add(CreateInvokeExecuteAsyncMethodBlockLocalResponseBuilderAddSuccess(resultTypeName));
        result.AddRange(CreateInvokeExecuteAsyncMethodBlockLocalResponseBuilderAddErrors());
        result.Add(CreateInvokeExecuteAsyncMethodBlockLocalResponseBuilderReturnBuildResponse());
        return result.ToArray();
    }

    private LocalDeclarationStatementSyntax CreateInvokeExecuteAsyncMethodBlockLocalResponseBuilderFromResponse()
        => SyntaxFactory.LocalDeclarationStatement(
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
                                                SyntaxFactory.IdentifierName(nameof(IHttpMessageFactory.FromResponse))))
                                        .WithArgumentList(
                                            SyntaxFactory.ArgumentList(
                                                SyntaxFactory.SingletonSeparatedList(
                                                    SyntaxFactory.Argument(
                                                        SyntaxFactory.IdentifierName("response"))))))))));

    private ExpressionStatementSyntax CreateInvokeExecuteAsyncMethodBlockLocalResponseBuilderAddSuccess(
        string resultTypeName)
        => SyntaxFactory.ExpressionStatement(
            SyntaxFactory.InvocationExpression(
                    SyntaxFactory.MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        SyntaxFactory.IdentifierName("responseBuilder"),
                        SyntaxFactory.GenericName(
                                SyntaxFactory.Identifier(nameof(IMessageResponseBuilder.AddSuccessResponse)))
                            .WithTypeArgumentList(
                                SyntaxFactory.TypeArgumentList(
                                    SyntaxFactory.SingletonSeparatedList<TypeSyntax>(
                                        SyntaxFactory.IdentifierName(resultTypeName))))))
                .WithArgumentList(
                    SyntaxFactory.ArgumentList(
                        SyntaxFactory.SingletonSeparatedList(
                            SyntaxFactory.Argument(
                                SyntaxFactory.MemberAccessExpression(
                                    SyntaxKind.SimpleMemberAccessExpression,
                                    SyntaxFactory.IdentifierName(nameof(HttpStatusCode)),
                                    SyntaxFactory.IdentifierName(nameof(HttpStatusCode.OK))))))));

    private StatementSyntax[] CreateInvokeExecuteAsyncMethodBlockLocalResponseBuilderAddErrors()
    {
        var responseTypes = ApiOperation.Responses.GetResponseTypes(
            OperationSchemaMappings,
            FocusOnSegmentName,
            ApiProjectOptions.ProjectName,
            useProblemDetailsAsDefaultResponseBody: true,
            includeEmptyResponseTypes: false,
            HasParametersOrRequestBody,
            ApiProjectOptions.ApiOptions.Generator.UseAuthorization,
            includeIfNotDefinedInternalServerError: true,
            isClient: true);

        var result = new List<StatementSyntax>();
        foreach (var responseType in responseTypes
                     .Where(x => x.Item1.IsClientOrServerError())
                     .OrderBy(x => x.Item1))
        {
            result.Add(CreateInvokeExecuteAsyncMethodBlockLocalResponseBuilderAddError(responseType));
        }

        return result.ToArray();
    }

    private ExpressionStatementSyntax CreateInvokeExecuteAsyncMethodBlockLocalResponseBuilderAddError(
        Tuple<HttpStatusCode, string> responseType)
        => SyntaxFactory.ExpressionStatement(
            SyntaxFactory.InvocationExpression(
                    SyntaxFactory.MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        SyntaxFactory.IdentifierName("responseBuilder"),
                        SyntaxFactory.GenericName(
                                SyntaxFactory.Identifier(nameof(IMessageResponseBuilder.AddErrorResponse)))
                            .WithTypeArgumentList(
                                SyntaxFactory.TypeArgumentList(
                                    SyntaxFactory.SingletonSeparatedList<TypeSyntax>(
                                        SyntaxFactory.IdentifierName(responseType.Item2))))))
                .WithArgumentList(
                    SyntaxFactory.ArgumentList(
                        SyntaxFactory.SingletonSeparatedList(
                            SyntaxFactory.Argument(
                                SyntaxMemberAccessExpressionFactory.Create(responseType.Item1.ToString(), nameof(HttpStatusCode)))))));

    private ReturnStatementSyntax CreateInvokeExecuteAsyncMethodBlockLocalResponseBuilderReturnBuildResponse()
        => SyntaxFactory.ReturnStatement(
            SyntaxFactory.AwaitExpression(
                SyntaxFactory.InvocationExpression(
                        SyntaxFactory.MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            SyntaxFactory.IdentifierName("responseBuilder"),
                            SyntaxFactory.IdentifierName(nameof(IMessageResponseBuilder.BuildResponseAsync))))
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
                                                        SyntaxFactory.IdentifierName(EndpointResultTypeName))
                                                    .WithArgumentList(
                                                        SyntaxFactory.ArgumentList(
                                                            SyntaxFactory.SingletonSeparatedList(
                                                                SyntaxFactory.Argument(
                                                                    SyntaxFactory.IdentifierName("x"))))))),
                                    SyntaxFactory.Token(SyntaxKind.CommaToken),
                                    SyntaxFactory.Argument(
                                        SyntaxFactory.IdentifierName(nameof(CancellationToken).EnsureFirstCharacterToLower())),
                                })))));

    private ArgumentListSyntax CreateOneStringArg(
        string value)
        => SyntaxFactory.ArgumentList(
            SyntaxFactory.SingletonSeparatedList(
                SyntaxFactory.Argument(
                    SyntaxFactory.LiteralExpression(
                        SyntaxKind.StringLiteralExpression,
                        SyntaxFactory.Literal(value)))));
}