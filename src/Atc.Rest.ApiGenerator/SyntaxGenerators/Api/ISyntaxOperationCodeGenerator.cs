using Atc.Rest.ApiGenerator.Models;
using Microsoft.OpenApi.Models;

namespace Atc.Rest.ApiGenerator.SyntaxGenerators.Api
{
    public interface ISyntaxOperationCodeGenerator : ISyntaxCodeGenerator
    {
        ApiProjectOptions ApiProjectOptions { get; }

        OperationType ApiOperationType { get; }

        OpenApiOperation ApiOperation { get; }
    }
}