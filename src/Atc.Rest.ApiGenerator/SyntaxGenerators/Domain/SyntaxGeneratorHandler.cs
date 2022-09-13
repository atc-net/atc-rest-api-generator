namespace Atc.Rest.ApiGenerator.SyntaxGenerators.Domain;

public class SyntaxGeneratorHandler
{
    private readonly ILogger logger;

    public SyntaxGeneratorHandler(
        ILogger logger,
        DomainProjectOptions domainProjectOptions,
        OperationType apiOperationType,
        OpenApiOperation apiOperation,
        string focusOnSegmentName,
        bool hasParametersOrRequestBody)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        DomainProjectOptions = domainProjectOptions ?? throw new ArgumentNullException(nameof(domainProjectOptions));
        ApiOperationType = apiOperationType;
        ApiOperation = apiOperation ?? throw new ArgumentNullException(nameof(apiOperation));
        FocusOnSegmentName = focusOnSegmentName ?? throw new ArgumentNullException(nameof(focusOnSegmentName));
        HasParametersOrRequestBody = hasParametersOrRequestBody;
    }

    public DomainProjectOptions DomainProjectOptions { get; }

    public OperationType ApiOperationType { get; }

    public OpenApiOperation ApiOperation { get; }

    public string FocusOnSegmentName { get; }

    public CompilationUnitSyntax? Code { get; private set; }

    public string InterfaceTypeName => "I" + ApiOperation.GetOperationName() + NameConstants.Handler;

    public string ParameterTypeName => ApiOperation.GetOperationName() + NameConstants.ContractParameters;

    public string ResultTypeName => ApiOperation.GetOperationName() + NameConstants.ContractResult;

    public string HandlerTypeName => ApiOperation.GetOperationName() + NameConstants.Handler;

    public bool HasParametersOrRequestBody { get; }

    public bool GenerateCode()
    {
        // Create compilationUnit
        var compilationUnit = SyntaxFactory.CompilationUnit();

        // Create a namespace
        var @namespace = SyntaxProjectFactory.CreateNamespace(
            DomainProjectOptions,
            NameConstants.Handlers,
            FocusOnSegmentName,
            false);

        // Create class
        var classDeclaration = SyntaxClassDeclarationFactory.CreateWithInterface(HandlerTypeName, InterfaceTypeName)
            .WithLeadingTrivia(SyntaxDocumentationFactory.CreateForHandlers(ApiOperation, FocusOnSegmentName));

        // Create members
        var memberDeclarations = CreateMembers();

        // Add members to class
        classDeclaration = memberDeclarations.Aggregate(
            classDeclaration,
            (current, memberDeclaration) => current.AddMembers(memberDeclaration));

        // Add using statement to compilationUnit
        compilationUnit = compilationUnit.AddUsingStatements(
            ProjectDomainFactory.CreateUsingListForHandler(
                DomainProjectOptions,
                FocusOnSegmentName));

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
            return $"Syntax generate problem for handler for apiOperation: {ApiOperation}";
        }

        return Code
            .NormalizeWhitespace()
            .ToFullString()
            .EnsureEnvironmentNewLines()
            .FormatCs1998();
    }

    public FileInfo GetFilePath()
    {
        var area = FocusOnSegmentName.EnsureFirstCharacterToUpper();
        var file = DirectoryInfoHelper.GetCsFileNameForHandler(DomainProjectOptions.PathForSrcHandlers!, area, HandlerTypeName);
        return new FileInfo(file);
    }

    public void ToFile()
        => ToFile(GetFilePath());

    public void ToFile(
        FileInfo file)
    {
        ArgumentNullException.ThrowIfNull(file);

        var fileDisplayLocation = file.FullName.Replace(DomainProjectOptions.PathForSrcGenerate.FullName, "src: ", StringComparison.Ordinal);
        TextFileHelper.Save(logger, file.FullName, fileDisplayLocation, ToCodeAsString());
    }

    public override string ToString()
        => $"OperationType: {ApiOperationType}, OperationName: {ApiOperation.GetOperationName()}, SegmentName: {FocusOnSegmentName}";

    private List<MemberDeclarationSyntax> CreateMembers()
    {
        var result = new List<MemberDeclarationSyntax>
        {
            CreateExecuteAsyncMethod(ParameterTypeName, ResultTypeName, HasParametersOrRequestBody),
        };

        if (HasParametersOrRequestBody)
        {
            result.Add(CreateInvokeExecuteAsyncMethod(ParameterTypeName, ResultTypeName, HasParametersOrRequestBody));
        }

        return result;
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

        var codeBody = hasParameters
            ? SyntaxFactory.Block(
                SyntaxIfStatementFactory.CreateParameterArgumentNullCheck("parameters"),
                SyntaxFactory.ReturnStatement(
                    SyntaxFactory.InvocationExpression(SyntaxFactory.IdentifierName("InvokeExecuteAsync"))
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
                    .WithTypeArgumentList(SyntaxTypeArgumentListFactory.CreateWithOneItem(resultTypeName)),
                SyntaxFactory.Identifier("ExecuteAsync"))
            .WithModifiers(SyntaxTokenListFactory.PublicKeyword())
            .WithParameterList(SyntaxFactory.ParameterList(SyntaxFactory.SeparatedList<ParameterSyntax>(arguments)))
            .WithBody(codeBody);
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

        return SyntaxFactory.MethodDeclaration(
                SyntaxFactory.GenericName(SyntaxFactory.Identifier(nameof(Task)))
                    .WithTypeArgumentList(SyntaxTypeArgumentListFactory.CreateWithOneItem(resultTypeName)),
                SyntaxFactory.Identifier("InvokeExecuteAsync"))
            .WithModifiers(
                SyntaxFactory.TokenList(
                    SyntaxFactory.Token(
                        CreatePragmaWarningCodeStyle1998(true),
                        SyntaxKind.PrivateKeyword,
                        SyntaxFactory.TriviaList()), SyntaxFactory.Token(SyntaxKind.AsyncKeyword)))
            .WithParameterList(SyntaxFactory.ParameterList(SyntaxFactory.SeparatedList<ParameterSyntax>(arguments)))
            .WithBody(
                SyntaxFactory.Block(SyntaxThrowStatementFactory.CreateNotImplementedException())
                    .WithOpenBraceToken(
                        SyntaxFactory.Token(
                            CreatePragmaWarningCodeStyle1998(false),
                            SyntaxKind.OpenBraceToken,
                            SyntaxFactory.TriviaList())));
    }

    private static SyntaxTriviaList CreatePragmaWarningCodeStyle1998(
        bool disable)
        => CreatePragmaWarningCodeStyle(
            disable,
            1998,
            "// Async method lacks 'await' operators and will run synchronously");

    private static SyntaxTriviaList CreatePragmaWarningCodeStyle(
        bool disable,
        int checkId,
        string comment)
    {
        var keyword = disable
            ? SyntaxKind.DisableKeyword
            : SyntaxKind.RestoreKeyword;

        return SyntaxFactory.TriviaList(
            SyntaxFactory.Trivia(
                SyntaxFactory.PragmaWarningDirectiveTrivia(SyntaxFactory.Token(keyword), true)
                    .WithErrorCodes(
                        SyntaxFactory.SingletonSeparatedList<ExpressionSyntax>(
                            SyntaxFactory.IdentifierName(
                                SyntaxFactory.Identifier(
                                    SyntaxFactory.TriviaList(),
                                    "CS" + checkId,
                                    SyntaxFactory.TriviaList(SyntaxFactory.Comment(comment))))))));
    }
}