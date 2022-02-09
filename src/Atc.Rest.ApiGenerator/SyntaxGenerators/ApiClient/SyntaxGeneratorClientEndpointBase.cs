namespace Atc.Rest.ApiGenerator.SyntaxGenerators.ApiClient;

public abstract class SyntaxGeneratorClientEndpointBase
{
    protected SyntaxGeneratorClientEndpointBase(
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
        this.OperationSchemaMappings = operationSchemaMappings ?? throw new ArgumentNullException(nameof(operationSchemaMappings));
        this.GlobalPathParameters = globalPathParameters ?? throw new ArgumentNullException(nameof(globalPathParameters));
        this.ApiOperationType = apiOperationType;
        this.ApiOperation = apiOperation ?? throw new ArgumentNullException(nameof(apiOperation));
        this.FocusOnSegmentName = focusOnSegmentName ?? throw new ArgumentNullException(nameof(focusOnSegmentName));
        this.ApiUrlPath = urlPath ?? throw new ArgumentNullException(nameof(urlPath));
        this.HasParametersOrRequestBody = hasParametersOrRequestBody;

        this.ResponseTypes = ApiOperation.Responses.GetResponseTypes(
            OperationSchemaMappings,
            FocusOnSegmentName,
            ApiProjectOptions.ProjectName,
            useProblemDetailsAsDefaultResponseBody: false,
            includeEmptyResponseTypes: false,
            HasParametersOrRequestBody,
            ApiProjectOptions.ApiOptions.Generator.UseAuthorization,
            includeIfNotDefinedInternalServerError: true,
            isClient: true);

        this.ResultTypeName = ResponseTypes
            .FirstOrDefault(x => x.Item1 == HttpStatusCode.OK)?.Item2 ?? ResponseTypes
            .FirstOrDefault(x => x.Item1 == HttpStatusCode.Created)?.Item2 ?? "string";
    }

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