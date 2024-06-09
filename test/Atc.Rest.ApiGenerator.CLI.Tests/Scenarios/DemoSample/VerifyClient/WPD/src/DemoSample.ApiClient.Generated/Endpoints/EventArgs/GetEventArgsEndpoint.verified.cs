//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace DemoSample.ApiClient.Generated.Endpoints.EventArgs;

/// <summary>
/// Client Endpoint.
/// Description: Get EventArgs List.
/// Operation: GetEventArgs.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class GetEventArgsEndpoint : IGetEventArgsEndpoint
{
    private readonly IHttpClientFactory factory;
    private readonly IHttpMessageFactory httpMessageFactory;

    public GetEventArgsEndpoint(
        IHttpClientFactory factory,
        IHttpMessageFactory httpMessageFactory)
    {
        this.factory = factory;
        this.httpMessageFactory = httpMessageFactory;
    }

    public async Task<GetEventArgsEndpointResult> ExecuteAsync(
        string httpClientName = "DemoSample-ApiClient",
        CancellationToken cancellationToken = default)
    {
        var client = factory.CreateClient(httpClientName);

        var requestBuilder = httpMessageFactory.FromTemplate("/api/v1/eventArgs");

        using var requestMessage = requestBuilder.Build(HttpMethod.Get);
        using var response = await client.SendAsync(requestMessage, cancellationToken);

        var responseBuilder = httpMessageFactory.FromResponse(response);
        responseBuilder.AddSuccessResponse<IEnumerable<EventArgs>>(HttpStatusCode.OK);
        responseBuilder.AddErrorResponse<ProblemDetails>(HttpStatusCode.Unauthorized);
        return await responseBuilder.BuildResponseAsync(x => new GetEventArgsEndpointResult(x), cancellationToken);
    }
}
