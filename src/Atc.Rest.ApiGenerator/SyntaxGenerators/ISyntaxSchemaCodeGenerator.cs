namespace Atc.Rest.ApiGenerator.SyntaxGenerators;

public interface ISyntaxSchemaCodeGenerator : ISyntaxCodeGenerator
{
    string ApiSchemaKey { get; }

    OpenApiSchema ApiSchema { get; }
}