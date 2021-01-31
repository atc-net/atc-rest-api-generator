using System.Collections.Generic;

namespace Atc.Rest.ApiGenerator.SyntaxGenerators.Api.Interfaces
{
    public interface ISyntaxGeneratorContractResults : ISyntaxGeneratorContract
    {
        List<SyntaxGeneratorContractResult> GenerateSyntaxTrees();
    }
}