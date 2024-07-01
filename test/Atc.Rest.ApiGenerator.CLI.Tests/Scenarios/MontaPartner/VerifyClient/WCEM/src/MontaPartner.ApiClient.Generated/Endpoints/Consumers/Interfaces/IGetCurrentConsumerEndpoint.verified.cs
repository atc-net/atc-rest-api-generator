﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace MontaPartner.ApiClient.Generated.Endpoints.Consumers.Interfaces;

/// <summary>
/// Interface for Client Endpoint.
/// Description: Obtain information about current API Consumer.
/// Operation: GetCurrentConsumer.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public interface IGetCurrentConsumerEndpoint
{
    /// <summary>
    /// Execute method.
    /// </summary>
    /// <param name="httpClientName">The http client name.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task<GetCurrentConsumerEndpointResult> ExecuteAsync(
        string httpClientName = "MontaPartner-ApiClient",
        CancellationToken cancellationToken = default);
}