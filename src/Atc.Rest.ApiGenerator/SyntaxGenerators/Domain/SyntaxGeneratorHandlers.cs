// ReSharper disable ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
// ReSharper disable UseDeconstruction
namespace Atc.Rest.ApiGenerator.SyntaxGenerators.Domain;

public class SyntaxGeneratorHandlers : ISyntaxGeneratorHandlers
{
    private readonly ILogger logger;

    public SyntaxGeneratorHandlers(
        ILogger logger,
        DomainProjectOptions domainProjectOptions,
        string focusOnSegmentName)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        DomainProjectOptions = domainProjectOptions ?? throw new ArgumentNullException(nameof(domainProjectOptions));
        FocusOnSegmentName = focusOnSegmentName ?? throw new ArgumentNullException(nameof(focusOnSegmentName));
    }

    public DomainProjectOptions DomainProjectOptions { get; }

    public string FocusOnSegmentName { get; }

    public List<SyntaxGeneratorHandler> GenerateSyntaxTrees()
    {
        var list = new List<SyntaxGeneratorHandler>();
        foreach (var urlPath in DomainProjectOptions.Document.Paths)
        {
            if (!urlPath.IsPathStartingSegmentName(FocusOnSegmentName))
            {
                continue;
            }

            list.AddRange(
                urlPath.Value.Operations
                    .Select(x => new SyntaxGeneratorHandler(
                        logger,
                        DomainProjectOptions,
                        x.Key,
                        x.Value,
                        FocusOnSegmentName,
                        urlPath.Value.HasParameters() || x.Value.HasParametersOrRequestBody()))
                    .Where(item => item.GenerateCode()));
        }

        return list;
    }
}