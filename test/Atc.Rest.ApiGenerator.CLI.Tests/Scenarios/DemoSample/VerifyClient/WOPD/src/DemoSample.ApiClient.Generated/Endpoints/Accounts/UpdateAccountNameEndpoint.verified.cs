//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace DemoSample.ApiClient.Generated.Endpoints.Accounts;

/// <summary>
/// Client Endpoint.
/// Description: Update name of account.
/// Operation: UpdateAccountName.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class UpdateAccountNameEndpoint : IUpdateAccountNameEndpoint
{
    private readonly IHttpClientFactory factory;
    private readonly IHttpMessageFactory httpMessageFactory;

    public UpdateAccountNameEndpoint(
        IHttpClientFactory factory,
        IHttpMessageFactory httpMessageFactory)
    {
        this.factory = factory;
        this.httpMessageFactory = httpMessageFactory;
    }

    public async Task<IUpdateAccountNameEndpointResult> ExecuteAsync(
        UpdateAccountNameParameters parameters,
        string httpClientName = "DemoSample-ApiClient",
        CancellationToken cancellationToken = default)
    {
        var client = factory.CreateClient(httpClientName);

        var requestBuilder = httpMessageFactory.FromTemplate("/api/v1/accounts/{accountId}/name");
        requestBuilder.WithPathParameter("accountId", parameters.AccountId);
        requestBuilder.WithHeaderParameter("name", parameters.Name);

        using var requestMessage = requestBuilder.Build(HttpMethod.Put);
        using var response = await client.SendAsync(requestMessage, cancellationToken);

        var responseBuilder = httpMessageFactory.FromResponse(response);
        responseBuilder.AddSuccessResponse<string>(HttpStatusCode.OK);
        responseBuilder.AddErrorResponse<ValidationProblemDetails>(HttpStatusCode.BadRequest);
        responseBuilder.AddErrorResponse(HttpStatusCode.Unauthorized);
        responseBuilder.AddErrorResponse(HttpStatusCode.Forbidden);
        responseBuilder.AddErrorResponse<string>(HttpStatusCode.InternalServerError);

        return await responseBuilder.BuildResponseAsync(x => new UpdateAccountNameEndpointResult(x), cancellationToken);
    }
}
