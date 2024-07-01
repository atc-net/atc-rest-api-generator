﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace MontaPartner.ApiClient.Generated.Endpoints.SponsoredChargePoints.Interfaces;

/// <summary>
/// Interface for Client Endpoint.
/// Description: Create a sponsored charge point.
/// Operation: PostSponsoredChargePoint.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public interface IPostSponsoredChargePointEndpoint
{
    /// <summary>
    /// Execute method.
    /// </summary>
    /// <param name="parameters">The parameters.</param>
    /// <param name="httpClientName">The http client name.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task<PostSponsoredChargePointEndpointResult> ExecuteAsync(
        PostSponsoredChargePointParameters parameters,
        string httpClientName = "MontaPartner-ApiClient",
        CancellationToken cancellationToken = default);
}