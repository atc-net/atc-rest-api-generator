namespace Atc.Rest.ApiGenerator.SyntaxGenerators.ApiClient;

public class SyntaxGeneratorClientEndpointResults
{
    private readonly ILogger logger;

    public SyntaxGeneratorClientEndpointResults(
        ILogger logger,
        ApiProjectOptions apiProjectOptions,
        List<ApiOperationSchemaMap> operationSchemaMappings,
        string focusOnSegmentName)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        ApiProjectOptions = apiProjectOptions ?? throw new ArgumentNullException(nameof(apiProjectOptions));
        OperationSchemaMappings = operationSchemaMappings ?? throw new ArgumentNullException(nameof(operationSchemaMappings));
        FocusOnSegmentName = focusOnSegmentName ?? throw new ArgumentNullException(nameof(focusOnSegmentName));
    }

    public ApiProjectOptions ApiProjectOptions { get; }

    public List<ApiOperationSchemaMap> OperationSchemaMappings { get; }

    public string FocusOnSegmentName { get; }

    public List<SyntaxGeneratorClientEndpointResult> GenerateSyntaxTrees()
    {
        var list = new List<SyntaxGeneratorClientEndpointResult>();
        foreach (var urlPath in ApiProjectOptions.Document.Paths)
        {
            if (!urlPath.IsPathStartingSegmentName(FocusOnSegmentName))
            {
                continue;
            }

            list.AddRange(
                urlPath.Value.Operations
                    .Select(x => new SyntaxGeneratorClientEndpointResult(
                        logger,
                        ApiProjectOptions,
                        OperationSchemaMappings,
                        urlPath.Value.Parameters,
                        x.Key,
                        x.Value,
                        FocusOnSegmentName,
                        urlPath.Key,
                        urlPath.Value.HasParameters() || x.Value.HasParametersOrRequestBody()))
                    .Where(item => item.GenerateCode()));
        }

        return list;
    }
}