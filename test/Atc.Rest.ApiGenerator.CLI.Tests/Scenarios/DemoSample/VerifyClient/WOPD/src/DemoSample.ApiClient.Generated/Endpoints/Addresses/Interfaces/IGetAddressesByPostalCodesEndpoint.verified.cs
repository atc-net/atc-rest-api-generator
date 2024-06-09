//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace DemoSample.ApiClient.Generated.Endpoints.Addresses.Interfaces;

/// <summary>
/// Interface for Client Endpoint.
/// Description: Get addresses by postal code.
/// Operation: GetAddressesByPostalCodes.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public interface IGetAddressesByPostalCodesEndpoint
{
    /// <summary>
    /// Execute method.
    /// </summary>
    /// <param name="parameters">The parameters.</param>
    /// <param name="httpClientName">The http client name.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task<GetAddressesByPostalCodesEndpointResult> ExecuteAsync(
        GetAddressesByPostalCodesParameters parameters,
        string httpClientName = "DemoSample-ApiClient",
        CancellationToken cancellationToken = default);
}
