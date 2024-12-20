﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace Monta.ApiClient.Generated.Endpoints.SponsoredChargePoints;

/// <summary>
/// Client Endpoint.
/// Description: Retrieve a list of sponsored charge points.
/// Operation: GetSponsoredChargePoints.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class GetSponsoredChargePointsEndpoint : IGetSponsoredChargePointsEndpoint
{
    private readonly IHttpClientFactory factory;
    private readonly IHttpMessageFactory httpMessageFactory;

    public GetSponsoredChargePointsEndpoint(
        IHttpClientFactory factory,
        IHttpMessageFactory httpMessageFactory)
    {
        this.factory = factory;
        this.httpMessageFactory = httpMessageFactory;
    }

    public async Task<GetSponsoredChargePointsEndpointResult> ExecuteAsync(
        GetSponsoredChargePointsParameters parameters,
        string httpClientName = "Monta-ApiClient",
        CancellationToken cancellationToken = default)
    {
        var client = factory.CreateClient(httpClientName);

        var requestBuilder = httpMessageFactory.FromTemplate("/api/v1/sponsored-charge-points");
        requestBuilder.WithQueryParameter("page", parameters.Page);
        requestBuilder.WithQueryParameter("perPage", parameters.PerPage);
        requestBuilder.WithQueryParameter("chargePointId", parameters.ChargePointId);
        requestBuilder.WithQueryParameter("teamId", parameters.TeamId);
        requestBuilder.WithQueryParameter("includeDeleted", parameters.IncludeDeleted);

        using var requestMessage = requestBuilder.Build(HttpMethod.Get);
        using var response = await client.SendAsync(requestMessage, cancellationToken);

        var responseBuilder = httpMessageFactory.FromResponse(response);
        responseBuilder.AddSuccessResponse<MontaPageSponsoredChargePointDto>(HttpStatusCode.OK);
        responseBuilder.AddErrorResponse<ErrorResponse>(HttpStatusCode.BadRequest);
        responseBuilder.AddErrorResponse<ErrorResponse>(HttpStatusCode.Unauthorized);
        responseBuilder.AddErrorResponse<ErrorResponse>(HttpStatusCode.Forbidden);
        responseBuilder.AddErrorResponse<ErrorResponse>(HttpStatusCode.NotFound);
        return await responseBuilder.BuildResponseAsync(x => new GetSponsoredChargePointsEndpointResult(x), cancellationToken);
    }
}