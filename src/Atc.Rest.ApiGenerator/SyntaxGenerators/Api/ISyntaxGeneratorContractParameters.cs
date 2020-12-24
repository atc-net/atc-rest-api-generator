using System.Collections.Generic;

namespace Atc.Rest.ApiGenerator.SyntaxGenerators.Api
{
    public interface ISyntaxGeneratorContractParameters : ISyntaxGeneratorContract
    {
        List<SyntaxGeneratorContractParameter> GenerateSyntaxTrees();
    }
}