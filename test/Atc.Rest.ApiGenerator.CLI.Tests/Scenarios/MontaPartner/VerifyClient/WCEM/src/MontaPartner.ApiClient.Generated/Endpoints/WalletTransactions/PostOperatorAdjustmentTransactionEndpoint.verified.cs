﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace MontaPartner.ApiClient.Generated.Endpoints.WalletTransactions;

/// <summary>
/// Client Endpoint.
/// Description: Post an operator adjustment transaction.
/// Operation: PostOperatorAdjustmentTransaction.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class PostOperatorAdjustmentTransactionEndpoint : IPostOperatorAdjustmentTransactionEndpoint
{
    private readonly IHttpClientFactory factory;
    private readonly IHttpMessageFactory httpMessageFactory;

    public PostOperatorAdjustmentTransactionEndpoint(
        IHttpClientFactory factory,
        IHttpMessageFactory httpMessageFactory)
    {
        this.factory = factory;
        this.httpMessageFactory = httpMessageFactory;
    }

    public async Task<PostOperatorAdjustmentTransactionEndpointResult> ExecuteAsync(
        PostOperatorAdjustmentTransactionParameters parameters,
        string httpClientName = "MontaPartner-ApiClient",
        CancellationToken cancellationToken = default)
    {
        var client = factory.CreateClient(httpClientName);

        var requestBuilder = httpMessageFactory.FromTemplate("/api/v1/wallet-transactions/operator-adjustment-transaction");
        requestBuilder.WithBody(parameters.Request);

        using var requestMessage = requestBuilder.Build(HttpMethod.Post);
        using var response = await client.SendAsync(requestMessage, cancellationToken);

        var responseBuilder = httpMessageFactory.FromResponse(response);
        responseBuilder.AddErrorResponse<ErrorResponse>(HttpStatusCode.Created);
        responseBuilder.AddErrorResponse<ErrorResponse>(HttpStatusCode.BadRequest);
        responseBuilder.AddErrorResponse<ErrorResponse>(HttpStatusCode.Unauthorized);
        responseBuilder.AddErrorResponse<ErrorResponse>(HttpStatusCode.Forbidden);
        return await responseBuilder.BuildResponseAsync(x => new PostOperatorAdjustmentTransactionEndpointResult(x), cancellationToken);
    }
}