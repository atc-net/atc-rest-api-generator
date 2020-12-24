using System.Collections.Generic;

namespace Atc.Rest.ApiGenerator.SyntaxGenerators.Api
{
    public interface ISyntaxGeneratorContractResults : ISyntaxGeneratorContract
    {
        List<SyntaxGeneratorContractResult> GenerateSyntaxTrees();
    }
}