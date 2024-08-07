﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace Monta.ApiClient.Generated.Endpoints.Webhooks.Interfaces;

/// <summary>
/// Interface for Client Endpoint.
/// Description: Get your webhook config.
/// Operation: GetWebhookConfig.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public interface IGetWebhookConfigEndpoint
{
    /// <summary>
    /// Execute method.
    /// </summary>
    /// <param name="httpClientName">The http client name.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task<GetWebhookConfigEndpointResult> ExecuteAsync(
        string httpClientName = "Monta-ApiClient",
        CancellationToken cancellationToken = default);
}