//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace Scenario1.Api.Tests.Endpoints.Addresses;

[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class GetAddressesByPostalCodesHandlerStub : IGetAddressesByPostalCodesHandler
{
    public Task<GetAddressesByPostalCodesResult> ExecuteAsync(
        GetAddressesByPostalCodesParameters parameters,
        CancellationToken cancellationToken = default)
    {
        var data = new Fixture().Create<List<Address>>();

        return Task.FromResult(GetAddressesByPostalCodesResult.Ok(data));
    }
}