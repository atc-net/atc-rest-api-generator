﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace MontaPartner.ApiClient.Generated.Endpoints.Charges.Interfaces;

/// <summary>
/// Interface for Client Endpoint.
/// Description: Retrieve a single charge.
/// Operation: GetCharge.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public interface IGetChargeEndpoint
{
    /// <summary>
    /// Execute method.
    /// </summary>
    /// <param name="parameters">The parameters.</param>
    /// <param name="httpClientName">The http client name.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task<GetChargeEndpointResult> ExecuteAsync(
        GetChargeParameters parameters,
        string httpClientName = "MontaPartner-ApiClient",
        CancellationToken cancellationToken = default);
}