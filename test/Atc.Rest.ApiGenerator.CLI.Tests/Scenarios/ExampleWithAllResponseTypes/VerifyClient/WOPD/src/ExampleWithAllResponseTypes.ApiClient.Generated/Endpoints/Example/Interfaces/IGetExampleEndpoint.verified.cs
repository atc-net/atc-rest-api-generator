//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace ExampleWithAllResponseTypes.ApiClient.Generated.Endpoints.Example.Interfaces;

/// <summary>
/// Interface for Client Endpoint.
/// Description: Example endpoint.
/// Operation: GetExample.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public interface IGetExampleEndpoint
{
    /// <summary>
    /// Execute method.
    /// </summary>
    /// <param name="parameters">The parameters.</param>
    /// <param name="httpClientName">The http client name.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task<IGetExampleEndpointResult> ExecuteAsync(
        GetExampleParameters parameters,
        string httpClientName = "ExampleWithAllResponseTypes-ApiClient",
        CancellationToken cancellationToken = default);
}
