﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace Monta.ApiClient.Generated.Endpoints.Reports;

/// <summary>
/// Client Endpoint.
/// Description: Retrieve a report related to a charge point.
/// Operation: GetReport.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class GetReportEndpoint : IGetReportEndpoint
{
    private readonly IHttpClientFactory factory;
    private readonly IHttpMessageFactory httpMessageFactory;

    public GetReportEndpoint(
        IHttpClientFactory factory,
        IHttpMessageFactory httpMessageFactory)
    {
        this.factory = factory;
        this.httpMessageFactory = httpMessageFactory;
    }

    public async Task<GetReportEndpointResult> ExecuteAsync(
        GetReportParameters parameters,
        string httpClientName = "Monta-ApiClient",
        CancellationToken cancellationToken = default)
    {
        var client = factory.CreateClient(httpClientName);

        var requestBuilder = httpMessageFactory.FromTemplate("/api/v1/reports/{reportId}");
        requestBuilder.WithPathParameter("reportId", parameters.ReportId);

        using var requestMessage = requestBuilder.Build(HttpMethod.Get);
        using var response = await client.SendAsync(requestMessage, cancellationToken);

        var responseBuilder = httpMessageFactory.FromResponse(response);
        responseBuilder.AddSuccessResponse<ReportDto>(HttpStatusCode.OK);
        responseBuilder.AddErrorResponse<ErrorResponse>(HttpStatusCode.BadRequest);
        responseBuilder.AddErrorResponse<ErrorResponse>(HttpStatusCode.Unauthorized);
        responseBuilder.AddErrorResponse<ErrorResponse>(HttpStatusCode.Forbidden);
        responseBuilder.AddErrorResponse<ErrorResponse>(HttpStatusCode.NotFound);
        return await responseBuilder.BuildResponseAsync(x => new GetReportEndpointResult(x), cancellationToken);
    }
}