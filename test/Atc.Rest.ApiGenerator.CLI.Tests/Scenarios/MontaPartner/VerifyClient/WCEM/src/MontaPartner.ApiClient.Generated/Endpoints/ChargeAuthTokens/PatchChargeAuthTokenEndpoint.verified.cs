﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace MontaPartner.ApiClient.Generated.Endpoints.ChargeAuthTokens;

/// <summary>
/// Client Endpoint.
/// Description: Patch an existing charge auth token.
/// Operation: PatchChargeAuthToken.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class PatchChargeAuthTokenEndpoint : IPatchChargeAuthTokenEndpoint
{
    private readonly IHttpClientFactory factory;
    private readonly IHttpMessageFactory httpMessageFactory;

    public PatchChargeAuthTokenEndpoint(
        IHttpClientFactory factory,
        IHttpMessageFactory httpMessageFactory)
    {
        this.factory = factory;
        this.httpMessageFactory = httpMessageFactory;
    }

    public async Task<PatchChargeAuthTokenEndpointResult> ExecuteAsync(
        PatchChargeAuthTokenParameters parameters,
        string httpClientName = "MontaPartner-ApiClient",
        CancellationToken cancellationToken = default)
    {
        var client = factory.CreateClient(httpClientName);

        var requestBuilder = httpMessageFactory.FromTemplate("/api/v1/charge-auth-tokens/{chargeAuthTokenId}");
        requestBuilder.WithPathParameter("chargeAuthTokenId", parameters.ChargeAuthTokenId);
        requestBuilder.WithBody(parameters.Request);

        using var requestMessage = requestBuilder.Build(HttpMethod.Patch);
        using var response = await client.SendAsync(requestMessage, cancellationToken);

        var responseBuilder = httpMessageFactory.FromResponse(response);
        responseBuilder.AddSuccessResponse<ChargeAuthToken>(HttpStatusCode.OK);
        responseBuilder.AddErrorResponse<ErrorResponse>(HttpStatusCode.BadRequest);
        responseBuilder.AddErrorResponse<ErrorResponse>(HttpStatusCode.Unauthorized);
        responseBuilder.AddErrorResponse<ErrorResponse>(HttpStatusCode.Forbidden);
        return await responseBuilder.BuildResponseAsync(x => new PatchChargeAuthTokenEndpointResult(x), cancellationToken);
    }
}