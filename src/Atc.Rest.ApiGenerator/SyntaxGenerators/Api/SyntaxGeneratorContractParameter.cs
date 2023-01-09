// ReSharper disable LoopCanBeConvertedToQuery
namespace Atc.Rest.ApiGenerator.SyntaxGenerators.Api;

public class SyntaxGeneratorContractParameter
{
    public SyntaxGeneratorContractParameter(
        IList<OpenApiParameter> globalPathParameters,
        OpenApiOperation apiOperation)
    {
        GlobalPathParameters = globalPathParameters ?? throw new ArgumentNullException(nameof(globalPathParameters));
        ApiOperation = apiOperation ?? throw new ArgumentNullException(nameof(apiOperation));
    }

    public IList<OpenApiParameter> GlobalPathParameters { get; }

    public OpenApiOperation ApiOperation { get; }
}