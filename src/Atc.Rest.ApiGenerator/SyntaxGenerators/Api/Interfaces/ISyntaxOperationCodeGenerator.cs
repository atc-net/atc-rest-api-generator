namespace Atc.Rest.ApiGenerator.SyntaxGenerators.Api.Interfaces;

public interface ISyntaxOperationCodeGenerator : ISyntaxCodeGenerator
{
    ApiProjectOptions ApiProjectOptions { get; }

    OperationType ApiOperationType { get; }

    OpenApiOperation ApiOperation { get; }
}