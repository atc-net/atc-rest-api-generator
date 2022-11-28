namespace Atc.Rest.ApiGenerator.SyntaxGenerators.ApiClient;

public class SyntaxGeneratorClientEndpointResultInterface : SyntaxGeneratorClientEndpointBase, ISyntaxCodeGenerator
{
    private readonly ILogger logger;

    public SyntaxGeneratorClientEndpointResultInterface(
        ILogger logger,
        ApiProjectOptions apiProjectOptions,
        List<ApiOperation> operationSchemaMappings,
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
        this.logger = logger;
    }

    public CompilationUnitSyntax? Code { get; private set; }

    public string InterfaceTypeName => "I" + ApiOperation.GetOperationName() + NameConstants.EndpointResult;

    public string ParameterTypeName => ApiOperation.GetOperationName() + NameConstants.ContractParameters;

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
            .AddBaseListTypes(SyntaxFactory.SimpleBaseType(SyntaxFactory.ParseTypeName(nameof(IEndpointResponse))))
            .AddGeneratedCodeAttribute(ApiProjectOptions.ApiGeneratorName, ApiProjectOptions.ApiGeneratorVersion.ToString())
            .WithLeadingTrivia(SyntaxDocumentationFactory.CreateForResults(ApiOperation, FocusOnSegmentName));

        interfaceDeclaration = interfaceDeclaration.AddMembers(CreatePropertiesForIsStatusCode());
        interfaceDeclaration = interfaceDeclaration.AddMembers(CreatePropertiesForStatusCodeContent());

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
            return $"Syntax generate problem for client-endpointResult-interface for apiOperation: {ApiOperation}";
        }

        return Code
            .NormalizeWhitespace()
            .ToFullString()
            .EnsureEnvironmentNewLines()
            .FormatAutoPropertiesOnOneLine()
            .EnsureFileScopedNamespace();
    }

    public void ToFile()
    {
        var area = FocusOnSegmentName.EnsureFirstCharacterToUpper();
        var file = DirectoryInfoHelper.GetCsFileNameForContract(ApiProjectOptions.PathForEndpoints, area, NameConstants.EndpointInterfaceResults, InterfaceTypeName);
        ToFile(new FileInfo(file));
    }

    public void ToFile(
        FileInfo file)
    {
        ArgumentNullException.ThrowIfNull(file);

        var contentWriter = new ContentWriter(logger);
        contentWriter.Write(
            ApiProjectOptions.PathForSrcGenerate,
            file,
            ContentWriterArea.Src,
            ToCodeAsString());
    }

    public override string ToString()
        => $"OperationType: {ApiOperationType}, OperationName: {ApiOperation.GetOperationName()}, SegmentName: {FocusOnSegmentName}";

    private MemberDeclarationSyntax[] CreatePropertiesForIsStatusCode()
        => ResponseTypes
            .Select(x => CreatePropertyForIsStatusCode(x.Item1))
            .ToArray();

    private static MemberDeclarationSyntax CreatePropertyForIsStatusCode(
        HttpStatusCode statusCode)
        => SyntaxFactory.PropertyDeclaration(
                SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.BoolKeyword)),
                SyntaxFactory.Identifier("Is" + statusCode.ToNormalizedString()))
            .WithAccessorList(
                SyntaxFactory.AccessorList(
                    SyntaxFactory.SingletonList(
                        SyntaxFactory.AccessorDeclaration(
                                SyntaxKind.GetAccessorDeclaration)
                            .WithSemicolonToken(SyntaxTokenFactory.Semicolon()))));

    private MemberDeclarationSyntax[] CreatePropertiesForStatusCodeContent()
    {
        var responseTypes = ApiOperation.Responses.GetResponseTypes(
            OperationSchemaMappings,
            FocusOnSegmentName,
            ApiProjectOptions.ProjectName,
            useProblemDetailsAsDefaultResponseBody: true,
            includeEmptyResponseTypes: false,
            HasParametersOrRequestBody,
            includeIfNotDefinedAuthorization: true,
            includeIfNotDefinedInternalServerError: true,
            isClient: true);

        return responseTypes
            .Select(x => CreatePropertyForStatusCodeContent(x.Item1, x.Item2))
            .ToArray();
    }

    private static MemberDeclarationSyntax CreatePropertyForStatusCodeContent(
        HttpStatusCode statusCode,
        string resultTypeName)
        => SyntaxFactory.PropertyDeclaration(
                SyntaxFactory.IdentifierName(resultTypeName),
                SyntaxFactory.Identifier(statusCode.ToNormalizedString() + "Content"))
            .WithAccessorList(
                SyntaxFactory.AccessorList(
                    SyntaxFactory.SingletonList(
                        SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                            .WithSemicolonToken(SyntaxTokenFactory.Semicolon()))));
}