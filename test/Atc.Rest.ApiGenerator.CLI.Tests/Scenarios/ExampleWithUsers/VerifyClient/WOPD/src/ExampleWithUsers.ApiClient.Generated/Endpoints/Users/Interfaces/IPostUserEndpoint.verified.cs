//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace ExampleWithUsers.ApiClient.Generated.Endpoints.Users.Interfaces;

/// <summary>
/// Interface for Client Endpoint.
/// Description: Create a new user.
/// Operation: PostUser.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public interface IPostUserEndpoint
{
    /// <summary>
    /// Execute method.
    /// </summary>
    /// <param name="parameters">The parameters.</param>
    /// <param name="httpClientName">The http client name.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task<PostUserEndpointResult> ExecuteAsync(
        PostUserParameters parameters,
        string httpClientName = "ExampleWithUsers-ApiClient",
        CancellationToken cancellationToken = default);
}
