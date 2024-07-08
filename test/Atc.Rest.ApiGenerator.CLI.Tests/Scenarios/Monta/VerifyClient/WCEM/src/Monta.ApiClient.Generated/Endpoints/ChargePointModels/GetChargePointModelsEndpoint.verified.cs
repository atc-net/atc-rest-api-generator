﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace Monta.ApiClient.Generated.Endpoints.ChargePointModels;

/// <summary>
/// Client Endpoint.
/// Description: Retrieve list of charge point models.
/// Operation: GetChargePointModels.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class GetChargePointModelsEndpoint : IGetChargePointModelsEndpoint
{
    private readonly IHttpClientFactory factory;
    private readonly IHttpMessageFactory httpMessageFactory;

    public GetChargePointModelsEndpoint(
        IHttpClientFactory factory,
        IHttpMessageFactory httpMessageFactory)
    {
        this.factory = factory;
        this.httpMessageFactory = httpMessageFactory;
    }

    public async Task<GetChargePointModelsEndpointResult> ExecuteAsync(
        GetChargePointModelsParameters parameters,
        string httpClientName = "Monta-ApiClient",
        CancellationToken cancellationToken = default)
    {
        var client = factory.CreateClient(httpClientName);

        var requestBuilder = httpMessageFactory.FromTemplate("/api/v1/charge-point-models");
        requestBuilder.WithQueryParameter("search", parameters.Search);
        requestBuilder.WithQueryParameter("brandId", parameters.BrandId);
        requestBuilder.WithQueryParameter("page", parameters.Page);
        requestBuilder.WithQueryParameter("perPage", parameters.PerPage);

        using var requestMessage = requestBuilder.Build(HttpMethod.Get);
        using var response = await client.SendAsync(requestMessage, cancellationToken);

        var responseBuilder = httpMessageFactory.FromResponse(response);
        responseBuilder.AddSuccessResponse<MontaPageChargePointModelDto>(HttpStatusCode.OK);
        responseBuilder.AddErrorResponse<ErrorResponse>(HttpStatusCode.BadRequest);
        responseBuilder.AddErrorResponse<ErrorResponse>(HttpStatusCode.Unauthorized);
        return await responseBuilder.BuildResponseAsync(x => new GetChargePointModelsEndpointResult(x), cancellationToken);
    }
}