﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace Monta.ApiClient.Generated.Endpoints.Auth.Interfaces;

/// <summary>
/// Interface for Client Endpoint.
/// Description: Obtain your `accessToken` with a `refreshToken`.
/// Operation: GetAccessTokenWithRefreshToken.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public interface IGetAccessTokenWithRefreshTokenEndpoint
{
    /// <summary>
    /// Execute method.
    /// </summary>
    /// <param name="parameters">The parameters.</param>
    /// <param name="httpClientName">The http client name.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task<GetAccessTokenWithRefreshTokenEndpointResult> ExecuteAsync(
        GetAccessTokenWithRefreshTokenParameters parameters,
        string httpClientName = "Monta-ApiClient",
        CancellationToken cancellationToken = default);
}