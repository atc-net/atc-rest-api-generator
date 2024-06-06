//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace PetStore.ApiClient.Generated.Endpoints.Pets.Interfaces;

/// <summary>
/// Interface for Client Endpoint.
/// Description: List all pets.
/// Operation: ListPets.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public interface IListPetsEndpoint
{
    /// <summary>
    /// Execute method.
    /// </summary>
    /// <param name="parameters">The parameters.</param>
    /// <param name="httpClientName">The http client name.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task<IListPetsEndpointResult> ExecuteAsync(
        ListPetsParameters parameters,
        string httpClientName = "PetStore-ApiClient",
        CancellationToken cancellationToken = default);
}
