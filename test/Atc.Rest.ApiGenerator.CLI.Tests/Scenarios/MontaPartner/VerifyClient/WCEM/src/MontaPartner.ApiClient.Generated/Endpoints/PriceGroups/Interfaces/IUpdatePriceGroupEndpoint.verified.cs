﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace MontaPartner.ApiClient.Generated.Endpoints.PriceGroups.Interfaces;

/// <summary>
/// Interface for Client Endpoint.
/// Description: Updates a price group.
/// Operation: UpdatePriceGroup.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public interface IUpdatePriceGroupEndpoint
{
    /// <summary>
    /// Execute method.
    /// </summary>
    /// <param name="parameters">The parameters.</param>
    /// <param name="httpClientName">The http client name.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task<UpdatePriceGroupEndpointResult> ExecuteAsync(
        UpdatePriceGroupParameters parameters,
        string httpClientName = "MontaPartner-ApiClient",
        CancellationToken cancellationToken = default);
}