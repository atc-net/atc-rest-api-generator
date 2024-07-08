﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace Monta.ApiClient.Generated.Endpoints.TariffRecurringPeriods;

/// <summary>
/// Client Endpoint.
/// Description: Update existing recurring period.
/// Operation: UpdateTariffRecurringPeriods.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class UpdateTariffRecurringPeriodsEndpoint : IUpdateTariffRecurringPeriodsEndpoint
{
    private readonly IHttpClientFactory factory;
    private readonly IHttpMessageFactory httpMessageFactory;

    public UpdateTariffRecurringPeriodsEndpoint(
        IHttpClientFactory factory,
        IHttpMessageFactory httpMessageFactory)
    {
        this.factory = factory;
        this.httpMessageFactory = httpMessageFactory;
    }

    public async Task<UpdateTariffRecurringPeriodsEndpointResult> ExecuteAsync(
        UpdateTariffRecurringPeriodsParameters parameters,
        string httpClientName = "Monta-ApiClient",
        CancellationToken cancellationToken = default)
    {
        var client = factory.CreateClient(httpClientName);

        var requestBuilder = httpMessageFactory.FromTemplate("/api/v1/tariff-recurring-periods/{periodId}");
        requestBuilder.WithPathParameter("periodId", parameters.PeriodId);
        requestBuilder.WithBody(parameters.Request);

        using var requestMessage = requestBuilder.Build(HttpMethod.Patch);
        using var response = await client.SendAsync(requestMessage, cancellationToken);

        var responseBuilder = httpMessageFactory.FromResponse(response);
        responseBuilder.AddSuccessResponse<TariffRecurringPeriod>(HttpStatusCode.OK);
        responseBuilder.AddErrorResponse<ErrorResponse>(HttpStatusCode.BadRequest);
        responseBuilder.AddErrorResponse<ErrorResponse>(HttpStatusCode.Unauthorized);
        return await responseBuilder.BuildResponseAsync(x => new UpdateTariffRecurringPeriodsEndpointResult(x), cancellationToken);
    }
}