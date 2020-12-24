using System.Collections.Generic;

namespace Atc.Rest.ApiGenerator.SyntaxGenerators.Api
{
    public interface ISyntaxGeneratorContractInterfaces : ISyntaxGeneratorContract
    {
        List<SyntaxGeneratorContractInterface> GenerateSyntaxTrees();
    }
}