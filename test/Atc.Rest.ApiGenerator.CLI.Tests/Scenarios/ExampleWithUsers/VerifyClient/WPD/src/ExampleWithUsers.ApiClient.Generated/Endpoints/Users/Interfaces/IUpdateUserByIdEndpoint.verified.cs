//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace ExampleWithUsers.ApiClient.Generated.Endpoints.Users.Interfaces;

/// <summary>
/// Interface for Client Endpoint.
/// Description: Update user by id.
/// Operation: UpdateUserById.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public interface IUpdateUserByIdEndpoint
{
    /// <summary>
    /// Execute method.
    /// </summary>
    /// <param name="parameters">The parameters.</param>
    /// <param name="httpClientName">The http client name.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task<UpdateUserByIdEndpointResult> ExecuteAsync(
        UpdateUserByIdParameters parameters,
        string httpClientName = "ExampleWithUsers-ApiClient",
        CancellationToken cancellationToken = default);
}
