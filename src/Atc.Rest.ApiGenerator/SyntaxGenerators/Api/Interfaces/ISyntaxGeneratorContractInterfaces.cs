namespace Atc.Rest.ApiGenerator.SyntaxGenerators.Api.Interfaces;

public interface ISyntaxGeneratorContractInterfaces : ISyntaxGeneratorContract
{
    List<SyntaxGeneratorContractInterface> GenerateSyntaxTrees();
}