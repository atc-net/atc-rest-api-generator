// ReSharper disable ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
// ReSharper disable UseDeconstruction
namespace Atc.Rest.ApiGenerator.SyntaxGenerators.Api;

public class SyntaxGeneratorContractResults : ISyntaxGeneratorContractResults
{
    private readonly ILogger logger;

    public SyntaxGeneratorContractResults(
        ILogger logger,
        ApiProjectOptions apiProjectOptions,
        string focusOnSegmentName)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.ApiProjectOptions = apiProjectOptions ?? throw new ArgumentNullException(nameof(apiProjectOptions));
        this.FocusOnSegmentName = focusOnSegmentName ?? throw new ArgumentNullException(nameof(focusOnSegmentName));
    }

    public ApiProjectOptions ApiProjectOptions { get; }

    public string FocusOnSegmentName { get; }

    public List<SyntaxGeneratorContractResult> GenerateSyntaxTrees()
    {
        var list = new List<SyntaxGeneratorContractResult>();
        foreach (var urlPath in ApiProjectOptions.Document.Paths)
        {
            if (!urlPath.IsPathStartingSegmentName(FocusOnSegmentName))
            {
                continue;
            }

            list.AddRange(
                urlPath.Value.Operations
                    .Select(x => new SyntaxGeneratorContractResult(logger, ApiProjectOptions, x.Key, x.Value, FocusOnSegmentName))
                    .Where(item => item.GenerateCode()));
        }

        return list;
    }
}