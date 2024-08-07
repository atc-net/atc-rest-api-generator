//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace ExAllResponseTypes.Api.Tests.Endpoints.Example;

[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class GetExampleHandlerStub : IGetExampleHandler
{
    public Task<GetExampleResult> ExecuteAsync(
        GetExampleParameters parameters,
        CancellationToken cancellationToken = default)
    {
        var data = new Fixture().Create<ExampleModel>();

        return Task.FromResult(GetExampleResult.Ok(data));
    }
}
