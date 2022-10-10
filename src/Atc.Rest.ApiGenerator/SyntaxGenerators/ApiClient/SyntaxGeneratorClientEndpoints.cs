// ReSharper disable ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
// ReSharper disable UseDeconstruction
namespace Atc.Rest.ApiGenerator.SyntaxGenerators.ApiClient;

public class SyntaxGeneratorClientEndpoints : ISyntaxGeneratorClientEndpoints
{
    private readonly ILogger logger;

    public SyntaxGeneratorClientEndpoints(
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

    public ApiProjectOptions ApiProjectOptions { get; }

    public IList<ApiOperation> OperationSchemaMappings { get; }

    public string FocusOnSegmentName { get; }

    public List<SyntaxGeneratorClientEndpoint> GenerateSyntaxTrees()
    {
        var list = new List<SyntaxGeneratorClientEndpoint>();
        foreach (var urlPath in ApiProjectOptions.Document.Paths)
        {
            if (!urlPath.IsPathStartingSegmentName(FocusOnSegmentName))
            {
                continue;
            }

            list.AddRange(
                urlPath.Value.Operations
                    .Select(x => new SyntaxGeneratorClientEndpoint(
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