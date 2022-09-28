namespace Atc.Rest.ApiGenerator.SyntaxGenerators.ApiClient;

public abstract class SyntaxGeneratorClientEndpointBase
{
    protected SyntaxGeneratorClientEndpointBase(
        ILogger logger,
        ApiProjectOptions apiProjectOptions,
        List<ApiOperationSchemaMap> operationSchemaMappings,
        IList<OpenApiParameter> globalPathParameters,
        OperationType apiOperationType,
        OpenApiOperation apiOperation,
        string focusOnSegmentName,
        string urlPath,
        bool hasParametersOrRequestBody)
    {
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        ApiProjectOptions = apiProjectOptions ?? throw new ArgumentNullException(nameof(apiProjectOptions));
        OperationSchemaMappings = operationSchemaMappings ?? throw new ArgumentNullException(nameof(operationSchemaMappings));
        GlobalPathParameters = globalPathParameters ?? throw new ArgumentNullException(nameof(globalPathParameters));
        ApiOperationType = apiOperationType;
        ApiOperation = apiOperation ?? throw new ArgumentNullException(nameof(apiOperation));
        FocusOnSegmentName = focusOnSegmentName ?? throw new ArgumentNullException(nameof(focusOnSegmentName));
        ApiUrlPath = urlPath ?? throw new ArgumentNullException(nameof(urlPath));
        HasParametersOrRequestBody = hasParametersOrRequestBody;

        ResponseTypes = ApiOperation.Responses.GetResponseTypes(
            OperationSchemaMappings,
            FocusOnSegmentName,
            ApiProjectOptions.ProjectName,
            useProblemDetailsAsDefaultResponseBody: false,
            includeEmptyResponseTypes: false,
            HasParametersOrRequestBody,
            ApiProjectOptions.ApiOptions.Generator.UseAuthorization,
            includeIfNotDefinedInternalServerError: true,
            isClient: true);

        ResultTypeName = ResponseTypes
            .FirstOrDefault(x => x.Item1 == HttpStatusCode.OK)?.Item2 ?? ResponseTypes
            .FirstOrDefault(x => x.Item1 == HttpStatusCode.Created)?.Item2 ?? "string";
    }

    public ILogger Logger { get; }

    public ApiProjectOptions ApiProjectOptions { get; }

    public List<ApiOperationSchemaMap> OperationSchemaMappings { get; }

    public IList<OpenApiParameter> GlobalPathParameters { get; }

    public OperationType ApiOperationType { get; }

    public OpenApiOperation ApiOperation { get; }

    public string ApiUrlPath { get; }

    public string FocusOnSegmentName { get; }

    public bool HasParametersOrRequestBody { get; }

    public List<Tuple<HttpStatusCode, string>> ResponseTypes { get; }

    public string ResultTypeName { get; set; }
}