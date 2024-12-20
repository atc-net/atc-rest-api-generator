﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace Monta.ApiClient.Generated.Endpoints.ChargePointBrands;

/// <summary>
/// Client Endpoint.
/// Description: Retrieve list of charge point brands.
/// Operation: GetChargePointBrands.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class GetChargePointBrandsEndpoint : IGetChargePointBrandsEndpoint
{
    private readonly IHttpClientFactory factory;
    private readonly IHttpMessageFactory httpMessageFactory;

    public GetChargePointBrandsEndpoint(
        IHttpClientFactory factory,
        IHttpMessageFactory httpMessageFactory)
    {
        this.factory = factory;
        this.httpMessageFactory = httpMessageFactory;
    }

    public async Task<GetChargePointBrandsEndpointResult> ExecuteAsync(
        GetChargePointBrandsParameters parameters,
        string httpClientName = "Monta-ApiClient",
        CancellationToken cancellationToken = default)
    {
        var client = factory.CreateClient(httpClientName);

        var requestBuilder = httpMessageFactory.FromTemplate("/api/v1/charge-point-brands");
        requestBuilder.WithQueryParameter("search", parameters.Search);
        requestBuilder.WithQueryParameter("page", parameters.Page);
        requestBuilder.WithQueryParameter("perPage", parameters.PerPage);

        using var requestMessage = requestBuilder.Build(HttpMethod.Get);
        using var response = await client.SendAsync(requestMessage, cancellationToken);

        var responseBuilder = httpMessageFactory.FromResponse(response);
        responseBuilder.AddSuccessResponse<MontaPageChargePointBrandDto>(HttpStatusCode.OK);
        responseBuilder.AddErrorResponse<ErrorResponse>(HttpStatusCode.BadRequest);
        responseBuilder.AddErrorResponse<ErrorResponse>(HttpStatusCode.Unauthorized);
        responseBuilder.AddErrorResponse<ErrorResponse>(HttpStatusCode.Forbidden);
        responseBuilder.AddErrorResponse<ErrorResponse>(HttpStatusCode.NotFound);
        return await responseBuilder.BuildResponseAsync(x => new GetChargePointBrandsEndpointResult(x), cancellationToken);
    }
}