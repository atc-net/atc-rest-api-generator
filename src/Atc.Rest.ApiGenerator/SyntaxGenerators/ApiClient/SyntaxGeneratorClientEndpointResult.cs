using Atc.Rest.ApiGenerator.Factories;

// ReSharper disable UseObjectOrCollectionInitializer
// ReSharper disable InvertIf
namespace Atc.Rest.ApiGenerator.SyntaxGenerators.ApiClient;

public class SyntaxGeneratorClientEndpointResult : SyntaxGeneratorClientEndpointBase, ISyntaxCodeGenerator
{
    public SyntaxGeneratorClientEndpointResult(
        ApiProjectOptions apiProjectOptions,
        List<ApiOperationSchemaMap> operationSchemaMappings,
        IList<OpenApiParameter> globalPathParameters,
        OperationType apiOperationType,
        OpenApiOperation apiOperation,
        string focusOnSegmentName,
        string urlPath,
        bool hasParametersOrRequestBody)
        : base(
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

    public string InterfaceTypeName => "I" + ApiOperation.GetOperationName() + NameConstants.EndpointResult;

    public string ParameterTypeName => ApiOperation.GetOperationName() + NameConstants.ContractParameters;

    public string EndpointTypeName => ApiOperation.GetOperationName() + NameConstants.EndpointResult;

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
        var classDeclaration = SyntaxClassDeclarationFactory.CreateWithInheritClassType(EndpointTypeName, "EndpointResponse")
            .AddGeneratedCodeAttribute(ApiProjectOptions.ToolName, ApiProjectOptions.ToolVersion.ToString())
            .WithLeadingTrivia(SyntaxDocumentationFactory.CreateForResults(ApiOperation, FocusOnSegmentName));

        classDeclaration = classDeclaration.AddMembers(CreateConstructor());
        classDeclaration = classDeclaration.AddMembers(CreatePropertiesForIsStatusCode());
        classDeclaration = classDeclaration.AddMembers(CreatePropertiesForStatusCodeContent());

        // Add using statement to compilationUnit
        var includeRestResults = classDeclaration
            .Select<IdentifierNameSyntax>()
            .Any(x => x.Identifier.ValueText.Contains(
                $"({Microsoft.OpenApi.Models.NameConstants.Pagination}<",
                StringComparison.Ordinal));

        compilationUnit = compilationUnit.AddUsingStatements(
            ProjectApiClientFactory.CreateUsingListForEndpointResult(
                ApiProjectOptions,
                includeRestResults,
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
            return $"Syntax generate problem for client-endpointResult for apiOperation: {ApiOperation}";
        }

        return Code
            .NormalizeWhitespace()
            .ToFullString()
            .EnsureEnvironmentNewLines()
            .FormatClientEndpointResult();
    }

    public LogKeyValueItem ToFile()
    {
        var area = FocusOnSegmentName.EnsureFirstCharacterToUpper();
        var endpointName = ApiOperation.GetOperationName() + NameConstants.EndpointResult;
        var file = Util.GetCsFileNameForContract(ApiProjectOptions.PathForEndpoints, area, endpointName);
        return TextFileHelper.Save(file, ToCodeAsString());
    }

    public void ToFile(
        FileInfo file)
    {
        ArgumentNullException.ThrowIfNull(file);

        TextFileHelper.Save(file, ToCodeAsString());
    }

    public override string ToString()
        => $"OperationType: {ApiOperationType}, OperationName: {ApiOperation.GetOperationName()}, SegmentName: {FocusOnSegmentName}";

    private MemberDeclarationSyntax CreateConstructor()
        => SyntaxFactory.ConstructorDeclaration(
                SyntaxFactory.Identifier(EndpointTypeName))
            .WithModifiers(
                SyntaxTokenListFactory.PublicKeyword())
            .WithParameterList(
                SyntaxFactory.ParameterList(
                    SyntaxFactory.SingletonSeparatedList(
                        SyntaxFactory.Parameter(
                                SyntaxFactory.Identifier("response"))
                            .WithType(
                                SyntaxFactory.IdentifierName("EndpointResponse")))))
            .WithInitializer(
                SyntaxFactory.ConstructorInitializer(
                    SyntaxKind.BaseConstructorInitializer,
                    SyntaxFactory.ArgumentList(
                        SyntaxFactory.SingletonSeparatedList(
                            SyntaxFactory.Argument(
                                SyntaxFactory.IdentifierName("response"))))))
            .WithBody(SyntaxFactory.Block());

    private MemberDeclarationSyntax[] CreatePropertiesForIsStatusCode()
        => ResponseTypes
            .Select(x => CreatePropertyForIsStatusCode(x.Item1))
            .ToArray();

    private MemberDeclarationSyntax CreatePropertyForIsStatusCode(
        HttpStatusCode statusCode)
        => SyntaxFactory.PropertyDeclaration(
                SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.BoolKeyword)),
                SyntaxFactory.Identifier("Is" + statusCode.ToNormalizedString()))
            .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword)))
            .WithExpressionBody(
                SyntaxFactory.ArrowExpressionClause(
                    SyntaxFactory.BinaryExpression(
                        SyntaxKind.EqualsExpression,
                        SyntaxFactory.IdentifierName("StatusCode"),
                        SyntaxMemberAccessExpressionFactory.Create(statusCode.ToString(), nameof(HttpStatusCode)))))
            .WithSemicolonToken(SyntaxTokenFactory.Semicolon());

    private MemberDeclarationSyntax[] CreatePropertiesForStatusCodeContent()
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

        return responseTypes
            .Select(x => CreatePropertyForStatusCodeContent(x.Item1, x.Item2))
            .ToArray();
    }

    private MemberDeclarationSyntax CreatePropertyForStatusCodeContent(
        HttpStatusCode statusCode,
        string resultTypeName)
        => SyntaxFactory.PropertyDeclaration(
                SyntaxFactory.IdentifierName(resultTypeName),
                SyntaxFactory.Identifier(statusCode.ToNormalizedString() + "Content"))
            .WithModifiers(SyntaxTokenListFactory.PublicKeyword())
            .WithExpressionBody(
                SyntaxFactory.ArrowExpressionClause(
                    SyntaxFactory.ConditionalExpression(
                        SyntaxFactory.BinaryExpression(
                            SyntaxKind.LogicalAndExpression,
                            SyntaxFactory.IdentifierName("Is" + statusCode.ToNormalizedString()),
                            SyntaxFactory.IsPatternExpression(
                                SyntaxFactory.IdentifierName("ContentObject"),
                                SyntaxFactory.DeclarationPattern(
                                    SyntaxFactory.IdentifierName(resultTypeName),
                                    SyntaxFactory.SingleVariableDesignation(
                                        SyntaxFactory.Identifier("result"))))),
                        SyntaxFactory.IdentifierName("result"),
                        SyntaxFactory.ThrowExpression(
                            SyntaxFactory.ObjectCreationExpression(SyntaxFactory.IdentifierName(nameof(InvalidOperationException)))
                                .WithArgumentList(
                                    SyntaxFactory.ArgumentList(
                                        SyntaxFactory.SingletonSeparatedList(
                                            SyntaxFactory.Argument(
                                                SyntaxFactory.LiteralExpression(
                                                    SyntaxKind.StringLiteralExpression,
                                                    SyntaxFactory.Literal($"Content is not the expected type - please use the Is{statusCode.ToNormalizedString()} property first."))))))))))
            .WithSemicolonToken(SyntaxTokenFactory.Semicolon());
}