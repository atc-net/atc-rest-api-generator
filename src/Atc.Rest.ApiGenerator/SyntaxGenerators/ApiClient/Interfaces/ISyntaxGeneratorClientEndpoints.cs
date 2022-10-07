namespace Atc.Rest.ApiGenerator.SyntaxGenerators.ApiClient.Interfaces;

public interface ISyntaxGeneratorClientEndpoints
{
    ApiProjectOptions ApiProjectOptions { get; }

    IList<ApiOperationSchemaMap> OperationSchemaMappings { get; }

    string FocusOnSegmentName { get; }

    List<SyntaxGeneratorClientEndpoint> GenerateSyntaxTrees();
}