namespace Atc.Rest.ApiGenerator.SyntaxGenerators.Api.Interfaces;

public interface ISyntaxGeneratorContractParameters : ISyntaxGeneratorContract
{
    List<SyntaxGeneratorContractParameter> GenerateSyntaxTrees();
}