//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace Scenario2.Api.Tests.Endpoints.Accounts;

[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class SetAccountNameHandlerStub : ISetAccountNameHandler
{
    public Task<SetAccountNameResult> ExecuteAsync(
        SetAccountNameParameters parameters,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(SetAccountNameResult.Ok("Hallo world"));
    }
}