﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace MontaPartner.ApiClient.Generated.Endpoints.Subscriptions;

/// <summary>
/// Client Endpoint.
/// Description: Retrieve a subscription.
/// Operation: GetSubscription.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class GetSubscriptionEndpoint : IGetSubscriptionEndpoint
{
    private readonly IHttpClientFactory factory;
    private readonly IHttpMessageFactory httpMessageFactory;

    public GetSubscriptionEndpoint(
        IHttpClientFactory factory,
        IHttpMessageFactory httpMessageFactory)
    {
        this.factory = factory;
        this.httpMessageFactory = httpMessageFactory;
    }

    public async Task<GetSubscriptionEndpointResult> ExecuteAsync(
        GetSubscriptionParameters parameters,
        string httpClientName = "MontaPartner-ApiClient",
        CancellationToken cancellationToken = default)
    {
        var client = factory.CreateClient(httpClientName);

        var requestBuilder = httpMessageFactory.FromTemplate("/api/v1/subscriptions/{subscriptionId}");
        requestBuilder.WithPathParameter("subscriptionId", parameters.SubscriptionId);

        using var requestMessage = requestBuilder.Build(HttpMethod.Get);
        using var response = await client.SendAsync(requestMessage, cancellationToken);

        var responseBuilder = httpMessageFactory.FromResponse(response);
        responseBuilder.AddSuccessResponse<Subscription>(HttpStatusCode.OK);
        responseBuilder.AddErrorResponse<ErrorResponse>(HttpStatusCode.BadRequest);
        responseBuilder.AddErrorResponse<ErrorResponse>(HttpStatusCode.Unauthorized);
        return await responseBuilder.BuildResponseAsync(x => new GetSubscriptionEndpointResult(x), cancellationToken);
    }
}