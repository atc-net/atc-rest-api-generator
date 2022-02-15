namespace Atc.Rest.ApiGenerator.SyntaxGenerators.ApiClient;

public class SyntaxGeneratorClientEndpointInterface : SyntaxGeneratorClientEndpointBase, ISyntaxCodeGenerator
{
    public SyntaxGeneratorClientEndpointInterface(
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

        // Create interface
        var interfaceDeclaration = SyntaxInterfaceDeclarationFactory.Create(InterfaceTypeName)
            .AddGeneratedCodeAttribute(ApiProjectOptions.ToolName, ApiProjectOptions.ToolVersion.ToString())
            .WithLeadingTrivia(SyntaxDocumentationFactory.CreateForResults(ApiOperation, FocusOnSegmentName));

        // Create interface-method
        interfaceDeclaration = interfaceDeclaration.AddMembers(CreateMembers());
        //// TODO: var methodDeclaration = ...

        // Add using statement to compilationUnit
        var includeRestResults = interfaceDeclaration
            .Select<IdentifierNameSyntax>()
            .Any(x => x.Identifier.ValueText.Contains(
                $"({Microsoft.OpenApi.Models.NameConstants.Pagination}<",
                StringComparison.Ordinal));
        compilationUnit = compilationUnit.AddUsingStatements(
            ProjectApiClientFactory.CreateUsingListForEndpointInterface(
                ApiProjectOptions,
                includeRestResults,
                OpenApiDocumentSchemaModelNameHelper.HasList(ResultTypeName),
                OpenApiDocumentSchemaModelNameHelper.HasSharedResponseContract(
                    ApiProjectOptions.Document,
                    OperationSchemaMappings,
                    FocusOnSegmentName)));

        // Add interface-method to interface
        //// TODO: interfaceDeclaration = interfaceDeclaration.AddMembers(methodDeclaration);

        // Add the interface to the namespace.
        @namespace = @namespace.AddMembers(interfaceDeclaration);

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
            return $"Syntax generate problem for client-endpoint-interface for apiOperation: {ApiOperation}";
        }

        return Code
            .NormalizeWhitespace()
            .ToFullString()
            .EnsureEnvironmentNewLines();
    }

    public void ToFile()
    {
        var area = FocusOnSegmentName.EnsureFirstCharacterToUpper();
        var file = DirectoryInfoHelper.GetCsFileNameForContract(ApiProjectOptions.PathForEndpoints, area, NameConstants.EndpointInterfaces, InterfaceTypeName);
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

    private MemberDeclarationSyntax[] CreateMembers()
    {
        var result = new List<MemberDeclarationSyntax>
        {
            CreateExecuteAsyncMethod(ParameterTypeName, HasParametersOrRequestBody),
        };

        return result.ToArray();
    }

    private MemberDeclarationSyntax CreateExecuteAsyncMethod(
        string parameterTypeName,
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

        return SyntaxFactory.MethodDeclaration(
                SyntaxFactory.GenericName(SyntaxFactory.Identifier(nameof(Task)))
                    .WithTypeArgumentList(
                        SyntaxFactory.TypeArgumentList(
                            SyntaxFactory.SingletonSeparatedList<TypeSyntax>(
                                SyntaxFactory.IdentifierName(EndpointResultTypeName)))),
                SyntaxFactory.Identifier("ExecuteAsync"))
            .WithModifiers(SyntaxTokenListFactory.PublicKeyword())
            .WithParameterList(SyntaxFactory.ParameterList(SyntaxFactory.SeparatedList<ParameterSyntax>(arguments)))
            .WithSemicolonToken(SyntaxTokenFactory.Semicolon());
    }
}