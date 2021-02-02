using Atc.Rest.ApiGenerator.Models;

namespace Atc.Rest.ApiGenerator.SyntaxGenerators.Api.Interfaces
{
    public interface ISyntaxGeneratorContract
    {
        ApiProjectOptions ApiProjectOptions { get; }

        string FocusOnSegmentName { get; }
    }
}