namespace Atc.Rest.ApiGenerator.SyntaxGenerators.Api.Interfaces;

public interface ISyntaxGeneratorContractModels : ISyntaxGeneratorContract
{
    IList<ApiOperation> OperationSchemaMappings { get; }

    List<SyntaxGeneratorContractModel> GenerateSyntaxTrees();
}