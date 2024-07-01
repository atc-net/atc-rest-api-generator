﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace MontaPartner.ApiClient.Generated.Endpoints.PaymentTerminals;

/// <summary>
/// Client Endpoint.
/// Description: Retrieve a single payment terminal.
/// Operation: GetPaymentTerminal.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class GetPaymentTerminalEndpoint : IGetPaymentTerminalEndpoint
{
    private readonly IHttpClientFactory factory;
    private readonly IHttpMessageFactory httpMessageFactory;

    public GetPaymentTerminalEndpoint(
        IHttpClientFactory factory,
        IHttpMessageFactory httpMessageFactory)
    {
        this.factory = factory;
        this.httpMessageFactory = httpMessageFactory;
    }

    public async Task<GetPaymentTerminalEndpointResult> ExecuteAsync(
        GetPaymentTerminalParameters parameters,
        string httpClientName = "MontaPartner-ApiClient",
        CancellationToken cancellationToken = default)
    {
        var client = factory.CreateClient(httpClientName);

        var requestBuilder = httpMessageFactory.FromTemplate("/api/v1/payment-terminals/{paymentTerminalId}");
        requestBuilder.WithPathParameter("paymentTerminalId", parameters.PaymentTerminalId);

        using var requestMessage = requestBuilder.Build(HttpMethod.Get);
        using var response = await client.SendAsync(requestMessage, cancellationToken);

        var responseBuilder = httpMessageFactory.FromResponse(response);
        responseBuilder.AddSuccessResponse<PaymentTerminal>(HttpStatusCode.OK);
        responseBuilder.AddErrorResponse<ErrorResponse>(HttpStatusCode.BadRequest);
        responseBuilder.AddErrorResponse<ErrorResponse>(HttpStatusCode.Unauthorized);
        responseBuilder.AddErrorResponse<ErrorResponse>(HttpStatusCode.Forbidden);
        responseBuilder.AddErrorResponse<ErrorResponse>(HttpStatusCode.NotFound);
        return await responseBuilder.BuildResponseAsync(x => new GetPaymentTerminalEndpointResult(x), cancellationToken);
    }
}