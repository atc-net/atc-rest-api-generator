// ReSharper disable ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
// ReSharper disable UseDeconstruction
namespace Atc.Rest.ApiGenerator.SyntaxGenerators.Api;

public class SyntaxGeneratorContractParameters
{
    public SyntaxGeneratorContractParameters(
        ApiProjectOptions apiProjectOptions,
        string focusOnSegmentName)
    {
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
                    globalPathParameters: urlPath.Value.Parameters,
                    apiOperation: apiOperation.Value);

                list.Add(generator);
            }
        }

        return list;
    }
}