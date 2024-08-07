﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace Monta.ApiClient.Generated.Endpoints.Tariffs.Interfaces;

/// <summary>
/// Interface for Client Endpoint.
/// Description: Create the prices for a given tariff within a given period of time.
/// Operation: CreateTariffPrices.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public interface ICreateTariffPricesEndpoint
{
    /// <summary>
    /// Execute method.
    /// </summary>
    /// <param name="parameters">The parameters.</param>
    /// <param name="httpClientName">The http client name.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task<CreateTariffPricesEndpointResult> ExecuteAsync(
        CreateTariffPricesParameters parameters,
        string httpClientName = "Monta-ApiClient",
        CancellationToken cancellationToken = default);
}