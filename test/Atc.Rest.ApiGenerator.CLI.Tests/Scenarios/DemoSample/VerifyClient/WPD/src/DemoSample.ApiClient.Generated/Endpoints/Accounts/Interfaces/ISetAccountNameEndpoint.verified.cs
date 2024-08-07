//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace DemoSample.ApiClient.Generated.Endpoints.Accounts.Interfaces;

/// <summary>
/// Interface for Client Endpoint.
/// Description: Set name of account.
/// Operation: SetAccountName.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public interface ISetAccountNameEndpoint
{
    /// <summary>
    /// Execute method.
    /// </summary>
    /// <param name="parameters">The parameters.</param>
    /// <param name="httpClientName">The http client name.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task<SetAccountNameEndpointResult> ExecuteAsync(
        SetAccountNameParameters parameters,
        string httpClientName = "DemoSample-ApiClient",
        CancellationToken cancellationToken = default);
}
