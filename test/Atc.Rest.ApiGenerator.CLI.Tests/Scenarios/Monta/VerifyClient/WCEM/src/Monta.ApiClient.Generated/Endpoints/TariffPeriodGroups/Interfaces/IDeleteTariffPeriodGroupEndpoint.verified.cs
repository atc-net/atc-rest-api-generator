﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace Monta.ApiClient.Generated.Endpoints.TariffPeriodGroups.Interfaces;

/// <summary>
/// Interface for Client Endpoint.
/// Description: Delete an existing Tariff Period Group, and all contained recurring periods and prices.
/// Operation: DeleteTariffPeriodGroup.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public interface IDeleteTariffPeriodGroupEndpoint
{
    /// <summary>
    /// Execute method.
    /// </summary>
    /// <param name="parameters">The parameters.</param>
    /// <param name="httpClientName">The http client name.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task<DeleteTariffPeriodGroupEndpointResult> ExecuteAsync(
        DeleteTariffPeriodGroupParameters parameters,
        string httpClientName = "Monta-ApiClient",
        CancellationToken cancellationToken = default);
}