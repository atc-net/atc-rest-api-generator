﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace Monta.ApiClient.Generated.Endpoints.NestedTeams.Interfaces;

/// <summary>
/// Interface for Client Endpoint.
/// Description: Get a nested team by nested team relation id.
/// Operation: GetNestedTeam.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public interface IGetNestedTeamEndpoint
{
    /// <summary>
    /// Execute method.
    /// </summary>
    /// <param name="parameters">The parameters.</param>
    /// <param name="httpClientName">The http client name.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task<GetNestedTeamEndpointResult> ExecuteAsync(
        GetNestedTeamParameters parameters,
        string httpClientName = "Monta-ApiClient",
        CancellationToken cancellationToken = default);
}