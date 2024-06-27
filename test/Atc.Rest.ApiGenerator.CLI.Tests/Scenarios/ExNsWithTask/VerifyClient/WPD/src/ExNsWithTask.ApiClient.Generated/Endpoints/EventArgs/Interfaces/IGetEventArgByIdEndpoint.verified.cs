//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace ExNsWithTask.ApiClient.Generated.Endpoints.EventArgs.Interfaces;

/// <summary>
/// Interface for Client Endpoint.
/// Description: Get EventArgs By Id.
/// Operation: GetEventArgById.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public interface IGetEventArgByIdEndpoint
{
    /// <summary>
    /// Execute method.
    /// </summary>
    /// <param name="parameters">The parameters.</param>
    /// <param name="httpClientName">The http client name.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task<GetEventArgByIdEndpointResult> ExecuteAsync(
        GetEventArgByIdParameters parameters,
        string httpClientName = "ExNsWithTask-ApiClient",
        CancellationToken cancellationToken = default);
}