// ReSharper disable ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
// ReSharper disable LoopCanBeConvertedToQuery
// ReSharper disable ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
// ReSharper disable UseDeconstruction
using Atc.Rest.ApiGenerator.Framework.Interfaces;

namespace Atc.Rest.ApiGenerator.SyntaxGenerators.Api;

public class SyntaxGeneratorEndpointControllers : ISyntaxGeneratorEndpointControllers
{
    private readonly ILogger logger;

    public SyntaxGeneratorEndpointControllers(
        ILogger logger,
        ApiProjectOptions apiProjectOptions,
        IList<ApiOperation> operationSchemaMappings,
        string focusOnSegmentName)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        ApiProjectOptions = apiProjectOptions ?? throw new ArgumentNullException(nameof(apiProjectOptions));
        OperationSchemaMappings = operationSchemaMappings ?? throw new ArgumentNullException(nameof(operationSchemaMappings));
        FocusOnSegmentName = focusOnSegmentName ?? throw new ArgumentNullException(nameof(focusOnSegmentName));
    }

    private ApiProjectOptions ApiProjectOptions { get; }

    private IList<ApiOperation> OperationSchemaMappings { get; }

    public string FocusOnSegmentName { get; }

    public CompilationUnitSyntax? Code { get; private set; }

    public bool GenerateCode()
    {
        var controllerTypeName = FocusOnSegmentName.EnsureFirstCharacterToUpper() + "Controller";

        // Create compilationUnit
        var compilationUnit = SyntaxFactory.CompilationUnit();

        // Create a namespace
        var @namespace = SyntaxProjectFactory.CreateNamespace(
            ApiProjectOptions,
            NameConstants.Endpoints);

        // Create class
        var classDeclaration = SyntaxClassDeclarationFactory.Create(controllerTypeName);
        if (ApiProjectOptions.ApiOptions.Generator.UseAuthorization)
        {
            classDeclaration =
                classDeclaration.AddAttributeLists(
                    SyntaxAttributeListFactory.Create(nameof(AuthorizeAttribute)));
        }

        classDeclaration = classDeclaration.AddAttributeLists(
                SyntaxAttributeListFactory.Create(nameof(ApiControllerAttribute)),
                SyntaxAttributeListFactory.CreateWithOneItemWithOneArgument(nameof(RouteAttribute), $"{ApiProjectOptions.RouteBase}/{GetRouteSegment()}"))
            .AddBaseListTypes(SyntaxFactory.SimpleBaseType(SyntaxFactory.ParseTypeName(nameof(ControllerBase))))
            .AddGeneratedCodeAttribute(ApiProjectOptions.ApiGeneratorName, ApiProjectOptions.ApiGeneratorVersion.ToString())
            .WithLeadingTrivia(SyntaxDocumentationFactory.CreateForEndpoints(FocusOnSegmentName));

        // Create Methods
        var usedApiOperations = new List<OpenApiOperation>();
        foreach (var (key, value) in ApiProjectOptions.Document.GetPathsByBasePathSegmentName(FocusOnSegmentName))
        {
            var hasRouteParameters = value.HasParameters();
            foreach (var apiOperation in value.Operations)
            {
                var methodDeclaration = CreateMembersForEndpoints(apiOperation, key, FocusOnSegmentName, hasRouteParameters)
                    .WithLeadingTrivia(SyntaxDocumentationFactory.CreateForEndpointMethods(apiOperation, FocusOnSegmentName));
                classDeclaration = classDeclaration.AddMembers(methodDeclaration);

                usedApiOperations.Add(apiOperation.Value);
            }
        }

        // Create private part for methods
        foreach (var (_, value) in ApiProjectOptions.Document.GetPathsByBasePathSegmentName(FocusOnSegmentName))
        {
            var hasRouteParameters = value.HasParameters();
            foreach (var apiOperation in value.Operations)
            {
                var methodDeclaration = CreateMembersForEndpointsPrivateHelper(apiOperation, hasRouteParameters);
                classDeclaration = classDeclaration.AddMembers(methodDeclaration);

                usedApiOperations.Add(apiOperation.Value);
            }
        }

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
            return $"Syntax generate problem for endpoints-controller for: {FocusOnSegmentName}";
        }

        return Code
            .NormalizeWhitespace()
            .ToFullString()
            .EnsureEnvironmentNewLines()
            .EnsureFileScopedNamespace();
    }

    public void ToFile()
    {
        var controllerName = FocusOnSegmentName.EnsureFirstCharacterToUpper() + "Controller";
        var file = DirectoryInfoHelper.GetCsFileNameForEndpoints(ApiProjectOptions.PathForEndpoints, controllerName);
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

    public List<EndpointMethodMetadata> GetMetadataForMethods()
    {
        var list = new List<EndpointMethodMetadata>();
        foreach (var (key, value) in ApiProjectOptions.Document.GetPathsByBasePathSegmentName(FocusOnSegmentName))
        {
            var generatorParameters = new SyntaxGeneratorContractParameters(logger, ApiProjectOptions, FocusOnSegmentName);
            var generatedParameters = generatorParameters.GenerateSyntaxTrees();
            var hasGlobalParameters = value.HasParameters();

            foreach (var apiOperation in value.Operations)
            {
                var httpAttributeRoutePart = GetHttpAttributeRoutePart(key);
                var routePart = string.IsNullOrEmpty(httpAttributeRoutePart)
                    ? $"/{GetRouteSegment()}"
                    : $"/{GetRouteSegment()}/{httpAttributeRoutePart}";
                var operationName = apiOperation.Value.GetOperationName();

                string? contractParameterTypeName = null;
                if (apiOperation.Value.HasParametersOrRequestBody() ||
                    value.HasParameters())
                {
                    contractParameterTypeName = operationName + NameConstants.ContractParameters;
                }

                var sgContractParameter = generatedParameters.FirstOrDefault(x => x.ApiOperation.GetOperationName() == operationName);

                var responseTypes = apiOperation.Value.Responses.GetResponseTypes(
                    OperationSchemaMappings,
                    FocusOnSegmentName,
                    ApiProjectOptions.ProjectName,
                    useProblemDetailsAsDefaultResponseBody: false,
                    includeEmptyResponseTypes: false,
                    hasGlobalParameters || apiOperation.Value.HasParametersOrRequestBody(),
                    includeIfNotDefinedAuthorization: false,
                    includeIfNotDefinedInternalServerError: false,
                    isClient: false);

                var responseTypeNamesAndItemSchema = GetResponseTypeNamesAndItemSchema(responseTypes);

                var endpointMethodMetadata = new EndpointMethodMetadata(
                    ApiProjectOptions.UseNullableReferenceTypes,
                    ApiProjectOptions.ProjectName,
                    FocusOnSegmentName,
                    $"{ApiProjectOptions.RouteBase}{routePart}",
                    apiOperation.Key,
                    operationName,
                    "I" + operationName + NameConstants.ContractHandler,
                    contractParameterTypeName,
                    operationName + NameConstants.ContractResult,
                    responseTypeNamesAndItemSchema,
                    sgContractParameter,
                    ApiProjectOptions.Document.Components.Schemas,
                    OperationSchemaMappings);

                list.Add(endpointMethodMetadata);
            }
        }

        return list;
    }

    private string GetRouteSegment()
    {
        var (key, _) = ApiProjectOptions.Document.Paths
            .FirstOrDefault(x => x.IsPathStartingSegmentName(FocusOnSegmentName));

        return key?
                   .Split('/', StringSplitOptions.RemoveEmptyEntries)?
                   .FirstOrDefault()
               ?? throw new NotSupportedException("SegmentName was not found in any route.");
    }

    private List<ResponseTypeNameAndItemSchema> GetResponseTypeNamesAndItemSchema(
        List<Tuple<HttpStatusCode, string>> responseTypeNames)
    {
        var list = new List<ResponseTypeNameAndItemSchema>();
        foreach (var responseTypeName in responseTypeNames)
        {
            if (string.IsNullOrEmpty(responseTypeName.Item2))
            {
                list.Add(new ResponseTypeNameAndItemSchema(responseTypeName.Item1, responseTypeName.Item2, null!));
            }
            else
            {
                var rawModelName = OpenApiDocumentSchemaModelNameHelper.GetRawModelName(responseTypeName.Item2);

                var isShared = OperationSchemaMappings.IsShared(rawModelName);
                var fullModelName = OpenApiDocumentSchemaModelNameHelper.EnsureModelNameWithNamespaceIfNeeded(
                    ApiProjectOptions.ProjectName,
                    FocusOnSegmentName,
                    responseTypeName.Item2,
                    isShared,
                    isClient: false);

                var schema = ApiProjectOptions.Document.Components.Schemas.FirstOrDefault(x => x.Key.Equals(rawModelName, StringComparison.OrdinalIgnoreCase));
                list.Add(new ResponseTypeNameAndItemSchema(responseTypeName.Item1, fullModelName, schema.Value));
            }
        }

        return list;
    }

    private MethodDeclarationSyntax CreateMembersForEndpoints(
        KeyValuePair<OperationType, OpenApiOperation> apiOperation,
        string urlPath,
        string area,
        bool hasRouteParameters)
    {
        var operationName = apiOperation.Value.GetOperationName();
        var interfaceName = "I" + operationName + NameConstants.ContractHandler;
        var methodName = operationName + "Async";
        var helperMethodName = $"Invoke{operationName}Async";
        var parameterTypeName = operationName + NameConstants.ContractParameters;

        // Create method # use CreateParameterList & CreateCodeBlockReturnStatement
        var methodDeclaration = SyntaxFactory.MethodDeclaration(
                SyntaxFactory.GenericName(SyntaxFactory.Identifier(nameof(Task)))
                    .WithTypeArgumentList(SyntaxTypeArgumentListFactory.CreateWithOneItem(nameof(ActionResult))),
                SyntaxFactory.Identifier(methodName))
            .AddModifiers(SyntaxTokenFactory.PublicKeyword())
            .WithParameterList(CreateParameterList(apiOperation.Value.HasParametersOrRequestBody() || hasRouteParameters, parameterTypeName, interfaceName, true))
            .WithBody(
                SyntaxFactory.Block(
                    SyntaxIfStatementFactory.CreateParameterArgumentNullCheck("handler", false),
                    CreateCodeBlockReturnStatement(helperMethodName, apiOperation.Value.HasParametersOrRequestBody() || hasRouteParameters)));

        // Create and add Http-method-attribute
        var httpAttributeRoutePart = GetHttpAttributeRoutePart(urlPath);
        methodDeclaration = string.IsNullOrEmpty(httpAttributeRoutePart)
            ? methodDeclaration.AddAttributeLists(
                SyntaxAttributeListFactory.Create($"Http{apiOperation.Key}"))
            : methodDeclaration.AddAttributeLists(
                SyntaxAttributeListFactory.CreateWithOneItemWithOneArgument(
                    $"Http{apiOperation.Key}",
                    httpAttributeRoutePart));

        // Create and add RequestFormLimits-attribute
        if (apiOperation.Value.HasRequestBodyWithAnythingAsFormatTypeBinary())
        {
            methodDeclaration = methodDeclaration.AddAttributeLists(
                SyntaxAttributeListFactory.Create(
                    "RequestFormLimits(MultipartBodyLengthLimit = long.MaxValue)"));
        }

        // Create and add producesResponseTypes-attributes
        var producesResponseAttributeParts = apiOperation.Value.Responses.GetProducesResponseAttributeParts(
            OperationSchemaMappings,
            area,
            ApiProjectOptions.ProjectName,
            ApiProjectOptions.ApiOptions.Generator.Response.UseProblemDetailsAsDefaultBody,
            apiOperation.Value.HasParametersOrRequestBody(),
            includeIfNotDefinedAuthorization: false,
            includeIfNotDefinedInternalServerError: false);

        return producesResponseAttributeParts
            .Aggregate(
                methodDeclaration,
                (current, producesResponseAttributePart) => current.AddAttributeLists(
                    SyntaxAttributeListFactory.Create(producesResponseAttributePart)));
    }

    private static MethodDeclarationSyntax CreateMembersForEndpointsPrivateHelper(
        KeyValuePair<OperationType, OpenApiOperation> apiOperation,
        bool hasRouteParameters)
    {
        var operationName = apiOperation.Value.GetOperationName();
        var interfaceName = "I" + operationName + NameConstants.ContractHandler;
        var methodName = $"Invoke{operationName}Async";
        var parameterTypeName = operationName + NameConstants.ContractParameters;
        var hasParametersOrRequestBody = apiOperation.Value.HasParametersOrRequestBody() || hasRouteParameters;

        // Create method # use CreateParameterList & CreateCodeBlockReturnStatement
        var methodDeclaration = SyntaxFactory.MethodDeclaration(
                SyntaxFactory.GenericName(SyntaxFactory.Identifier(nameof(Task)))
                    .WithTypeArgumentList(SyntaxTypeArgumentListFactory.CreateWithOneItem(nameof(ActionResult))),
                SyntaxFactory.Identifier(methodName))
            .AddModifiers(SyntaxTokenFactory.PrivateKeyword())
            .AddModifiers(SyntaxTokenFactory.StaticKeyword())
            .AddModifiers(SyntaxTokenFactory.AsyncKeyword())
            .WithParameterList(CreateParameterList(hasParametersOrRequestBody, parameterTypeName, interfaceName, false))
            .WithBody(
                SyntaxFactory.Block(
                    CreateCodeBlockReturnStatementForHelper(hasParametersOrRequestBody)));

        return methodDeclaration;
    }

    private static string GetHttpAttributeRoutePart(
        string urlPath)
    {
        var urlPathParts = urlPath.Split('/', StringSplitOptions.RemoveEmptyEntries);
        if (urlPathParts.Length <= 1)
        {
            return string.Empty;
        }

        var sb = new StringBuilder();
        for (var i = 1; i < urlPathParts.Length; i++)
        {
            if (i != 1)
            {
                sb.Append('/');
            }

            sb.Append(urlPathParts[i]);
        }

        return sb.ToString();
    }

    private static ParameterListSyntax CreateParameterList(
        bool hasParametersOrRequestBody,
        string parameterTypeName,
        string interfaceName,
        bool useFromServicesAttributeOnInterface)
    {
        ParameterListSyntax parameterList;
        if (hasParametersOrRequestBody)
        {
            if (useFromServicesAttributeOnInterface)
            {
                parameterList = SyntaxFactory.ParameterList(
                    SyntaxFactory.SeparatedList<ParameterSyntax>(
                        new SyntaxNodeOrToken[]
                        {
                            SyntaxParameterFactory.Create(parameterTypeName, "parameters"),
                            SyntaxTokenFactory.Comma(),
                            SyntaxParameterFactory.CreateWithAttribute(nameof(FromServicesAttribute), interfaceName, "handler"),
                            SyntaxTokenFactory.Comma(),
                            SyntaxParameterFactory.Create(nameof(CancellationToken), nameof(CancellationToken).EnsureFirstCharacterToLower()),
                        }));
            }
            else
            {
                parameterList = SyntaxFactory.ParameterList(
                    SyntaxFactory.SeparatedList<ParameterSyntax>(
                        new SyntaxNodeOrToken[]
                        {
                            SyntaxParameterFactory.Create(parameterTypeName, "parameters"),
                            SyntaxTokenFactory.Comma(),
                            SyntaxParameterFactory.Create(interfaceName, "handler"),
                            SyntaxTokenFactory.Comma(),
                            SyntaxParameterFactory.Create(nameof(CancellationToken), nameof(CancellationToken).EnsureFirstCharacterToLower()),
                        }));
            }
        }
        else
        {
            parameterList = SyntaxFactory.ParameterList(
                SyntaxFactory.SeparatedList<ParameterSyntax>(
                    new SyntaxNodeOrToken[]
                    {
                        SyntaxParameterFactory.CreateWithAttribute(nameof(FromServicesAttribute), interfaceName, "handler"),
                        SyntaxTokenFactory.Comma(),
                        SyntaxParameterFactory.Create(nameof(CancellationToken), nameof(CancellationToken).EnsureFirstCharacterToLower()),
                    }));
        }

        return parameterList;
    }

    private static ReturnStatementSyntax CreateCodeBlockReturnStatement(
        string helperMethodName,
        bool hasParameters)
    {
        var arguments = hasParameters
            ? new SyntaxNodeOrToken[]
            {
                SyntaxArgumentFactory.Create("parameters"),
                SyntaxTokenFactory.Comma(),
                SyntaxArgumentFactory.Create("handler"),
                SyntaxTokenFactory.Comma(),
                SyntaxArgumentFactory.Create(nameof(CancellationToken).EnsureFirstCharacterToLower()),
            }
            : new SyntaxNodeOrToken[]
            {
                SyntaxArgumentFactory.Create("handler"),
                SyntaxTokenFactory.Comma(),
                SyntaxArgumentFactory.Create(nameof(CancellationToken).EnsureFirstCharacterToLower()),
            };

        return SyntaxFactory.ReturnStatement(
            SyntaxFactory.InvocationExpression(SyntaxFactory.IdentifierName(helperMethodName))
                .WithArgumentList(
                    SyntaxFactory.ArgumentList(
                        SyntaxFactory.SeparatedList<ArgumentSyntax>(arguments))));
    }

    private static ReturnStatementSyntax CreateCodeBlockReturnStatementForHelper(
        bool hasParameters)
    {
        var arguments = hasParameters
            ? new SyntaxNodeOrToken[]
            {
                SyntaxArgumentFactory.Create("parameters"),
                SyntaxTokenFactory.Comma(),
                SyntaxArgumentFactory.Create(nameof(CancellationToken).EnsureFirstCharacterToLower()),
            }
            : new SyntaxNodeOrToken[]
            {
                SyntaxArgumentFactory.Create(nameof(CancellationToken).EnsureFirstCharacterToLower()),
            };

        return SyntaxFactory.ReturnStatement(
            SyntaxFactory.AwaitExpression(
                SyntaxFactory.InvocationExpression(
                        SyntaxMemberAccessExpressionFactory.Create("ExecuteAsync", "handler"))
                    .WithArgumentList(
                        SyntaxFactory.ArgumentList(
                            SyntaxFactory.SeparatedList<ArgumentSyntax>(arguments)))));
    }
}