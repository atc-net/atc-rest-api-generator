using Atc.Rest.ApiGenerator.Models;

namespace Atc.Rest.ApiGenerator.SyntaxGenerators.Api
{
    public interface ISyntaxGeneratorContract
    {
        ApiProjectOptions ApiProjectOptions { get; }

        string FocusOnSegmentName { get; }
    }
}