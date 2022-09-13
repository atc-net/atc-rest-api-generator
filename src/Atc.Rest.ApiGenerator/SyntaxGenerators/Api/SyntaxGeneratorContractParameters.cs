// ReSharper disable ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
// ReSharper disable UseDeconstruction
namespace Atc.Rest.ApiGenerator.SyntaxGenerators.Api;

public class SyntaxGeneratorContractParameters : ISyntaxGeneratorContractParameters
{
    private readonly ILogger logger;

    public SyntaxGeneratorContractParameters(
        ILogger logger,
        ApiProjectOptions apiProjectOptions,
        string focusOnSegmentName)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        ApiProjectOptions = apiProjectOptions ?? throw new ArgumentNullException(nameof(apiProjectOptions));
        FocusOnSegmentName = focusOnSegmentName ?? throw new ArgumentNullException(nameof(focusOnSegmentName));
    }

    public ApiProjectOptions ApiProjectOptions { get; }

    public string FocusOnSegmentName { get; }

    public List<SyntaxGeneratorContractParameter> GenerateSyntaxTrees()
    {
        var list = new List<SyntaxGeneratorContractParameter>();
        foreach (var urlPath in ApiProjectOptions.Document.Paths)
        {
            if (!urlPath.IsPathStartingSegmentName(FocusOnSegmentName))
            {
                continue;
            }

            foreach (var apiOperation in urlPath.Value.Operations)
            {
                if (!apiOperation.Value.HasParametersOrRequestBody() &&
                    !urlPath.Value.HasParameters())
                {
                    continue;
                }

                var generator = new SyntaxGeneratorContractParameter(
                    logger,
                    ApiProjectOptions,
                    urlPath.Value.Parameters,
                    apiOperation.Key,
                    apiOperation.Value,
                    FocusOnSegmentName);

                generator.GenerateCode();
                list.Add(generator);
            }
        }

        return list;
    }
}