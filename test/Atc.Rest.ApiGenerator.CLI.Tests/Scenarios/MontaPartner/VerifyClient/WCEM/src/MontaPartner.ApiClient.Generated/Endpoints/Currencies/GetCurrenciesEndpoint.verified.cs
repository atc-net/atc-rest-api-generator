﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace MontaPartner.ApiClient.Generated.Endpoints.Currencies;

/// <summary>
/// Client Endpoint.
/// Description: Retrieve a list of currencies.
/// Operation: GetCurrencies.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class GetCurrenciesEndpoint : IGetCurrenciesEndpoint
{
    private readonly IHttpClientFactory factory;
    private readonly IHttpMessageFactory httpMessageFactory;

    public GetCurrenciesEndpoint(
        IHttpClientFactory factory,
        IHttpMessageFactory httpMessageFactory)
    {
        this.factory = factory;
        this.httpMessageFactory = httpMessageFactory;
    }

    public async Task<GetCurrenciesEndpointResult> ExecuteAsync(
        GetCurrenciesParameters parameters,
        string httpClientName = "MontaPartner-ApiClient",
        CancellationToken cancellationToken = default)
    {
        var client = factory.CreateClient(httpClientName);

        var requestBuilder = httpMessageFactory.FromTemplate("/api/v1/currencies");
        requestBuilder.WithQueryParameter("page", parameters.Page);
        requestBuilder.WithQueryParameter("perPage", parameters.PerPage);
        requestBuilder.WithQueryParameter("identifier", parameters.Identifier);

        using var requestMessage = requestBuilder.Build(HttpMethod.Get);
        using var response = await client.SendAsync(requestMessage, cancellationToken);

        var responseBuilder = httpMessageFactory.FromResponse(response);
        responseBuilder.AddSuccessResponse<MontaPageCurrencyDto>(HttpStatusCode.OK);
        responseBuilder.AddErrorResponse<ErrorResponse>(HttpStatusCode.BadRequest);
        responseBuilder.AddErrorResponse<ErrorResponse>(HttpStatusCode.Unauthorized);
        return await responseBuilder.BuildResponseAsync(x => new GetCurrenciesEndpointResult(x), cancellationToken);
    }
}