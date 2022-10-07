namespace Atc.Rest.ApiGenerator.SyntaxGenerators.Api.Interfaces;

public interface ISyntaxGeneratorContractModels : ISyntaxGeneratorContract
{
    IList<ApiOperationSchemaMap> OperationSchemaMappings { get; }

    List<SyntaxGeneratorContractModel> GenerateSyntaxTrees();
}