﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace MontaPartner.ApiClient.Generated.Endpoints.Insights.Interfaces;

/// <summary>
/// Interface for Client Endpoint.
/// Description: Retrieve insights about charges broken down by team members of a team.
/// Operation: GetInsightsChargesDriverReport.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public interface IGetInsightsChargesDriverReportEndpoint
{
    /// <summary>
    /// Execute method.
    /// </summary>
    /// <param name="parameters">The parameters.</param>
    /// <param name="httpClientName">The http client name.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task<GetInsightsChargesDriverReportEndpointResult> ExecuteAsync(
        GetInsightsChargesDriverReportParameters parameters,
        string httpClientName = "MontaPartner-ApiClient",
        CancellationToken cancellationToken = default);
}