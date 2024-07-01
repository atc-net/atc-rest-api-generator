﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace MontaPartner.ApiClient.Generated.Endpoints.TariffPeriodGroups.Interfaces;

/// <summary>
/// Interface for Client Endpoint.
/// Description: Create a new Tariff Period Group.
/// Operation: CreateTariffPeriodGroup.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public interface ICreateTariffPeriodGroupEndpoint
{
    /// <summary>
    /// Execute method.
    /// </summary>
    /// <param name="parameters">The parameters.</param>
    /// <param name="httpClientName">The http client name.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task<CreateTariffPeriodGroupEndpointResult> ExecuteAsync(
        CreateTariffPeriodGroupParameters parameters,
        string httpClientName = "MontaPartner-ApiClient",
        CancellationToken cancellationToken = default);
}