namespace Atc.Rest.ApiGenerator.SyntaxGenerators.ApiClient;

// ReSharper disable ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
// ReSharper disable UseDeconstruction
public class SyntaxGeneratorClientEndpointInterfaces
{
    private readonly ILogger logger;

    public SyntaxGeneratorClientEndpointInterfaces(
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

    public List<SyntaxGeneratorClientEndpointInterface> GenerateSyntaxTrees()
    {
        var list = new List<SyntaxGeneratorClientEndpointInterface>();
        foreach (var urlPath in ApiProjectOptions.Document.Paths)
        {
            if (!urlPath.IsPathStartingSegmentName(FocusOnSegmentName))
            {
                continue;
            }

            list.AddRange(
                urlPath.Value.Operations
                    .Select(x => new SyntaxGeneratorClientEndpointInterface(
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