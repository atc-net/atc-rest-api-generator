// ReSharper disable ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
// ReSharper disable UseDeconstruction
namespace Atc.Rest.ApiGenerator.SyntaxGenerators.Api;

public class SyntaxGeneratorContractInterfaces : ISyntaxGeneratorContractInterfaces
{
    public SyntaxGeneratorContractInterfaces(
        ApiProjectOptions apiProjectOptions,
        string focusOnSegmentName)
    {
        this.ApiProjectOptions = apiProjectOptions ?? throw new ArgumentNullException(nameof(apiProjectOptions));
        this.FocusOnSegmentName = focusOnSegmentName ?? throw new ArgumentNullException(nameof(focusOnSegmentName));
    }

    public ApiProjectOptions ApiProjectOptions { get; }

    public string FocusOnSegmentName { get; }

    public List<SyntaxGeneratorContractInterface> GenerateSyntaxTrees()
    {
        var list = new List<SyntaxGeneratorContractInterface>();
        foreach (var urlPath in ApiProjectOptions.Document.Paths)
        {
            if (!urlPath.IsPathStartingSegmentName(FocusOnSegmentName))
            {
                continue;
            }

            list.AddRange(
                urlPath.Value.Operations
                    .Select(x => new SyntaxGeneratorContractInterface(
                        ApiProjectOptions,
                        urlPath.Value.Parameters,
                        x.Key,
                        x.Value,
                        FocusOnSegmentName,
                        urlPath.Value.HasParameters() || x.Value.HasParametersOrRequestBody()))
                    .Where(item => item.GenerateCode()));
        }

        return list;
    }
}