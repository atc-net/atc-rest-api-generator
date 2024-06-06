//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace DemoSample.ApiClient.Generated.Endpoints.Orders;

/// <summary>
/// Client Endpoint.
/// Description: Update part of order by id.
/// Operation: PatchOrdersId.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class PatchOrdersIdEndpoint : IPatchOrdersIdEndpoint
{
    private readonly IHttpClientFactory factory;
    private readonly IHttpMessageFactory httpMessageFactory;

    public PatchOrdersIdEndpoint(
        IHttpClientFactory factory,
        IHttpMessageFactory httpMessageFactory)
    {
        this.factory = factory;
        this.httpMessageFactory = httpMessageFactory;
    }

    public async Task<IPatchOrdersIdEndpointResult> ExecuteAsync(
        PatchOrdersIdParameters parameters,
        string httpClientName = "DemoSample-ApiClient",
        CancellationToken cancellationToken = default)
    {
        var client = factory.CreateClient(httpClientName);

        var requestBuilder = httpMessageFactory.FromTemplate("/api/v1/orders/{id}");
        requestBuilder.WithPathParameter("id", parameters.Id);
        requestBuilder.WithHeaderParameter("myTestHeader", parameters.MyTestHeader);
        requestBuilder.WithHeaderParameter("myTestHeaderBool", parameters.MyTestHeaderBool);
        requestBuilder.WithHeaderParameter("myTestHeaderInt", parameters.MyTestHeaderInt);
        requestBuilder.WithHeaderParameter("x-correlation-id", parameters.CorrelationId);
        requestBuilder.WithBody(parameters.Request);

        using var requestMessage = requestBuilder.Build(HttpMethod.Patch);
        using var response = await client.SendAsync(requestMessage, cancellationToken);

        var responseBuilder = httpMessageFactory.FromResponse(response);
        responseBuilder.AddSuccessResponse<string>(HttpStatusCode.OK);
        responseBuilder.AddErrorResponse<ValidationProblemDetails>(HttpStatusCode.BadRequest);
        responseBuilder.AddErrorResponse(HttpStatusCode.Unauthorized);
        responseBuilder.AddErrorResponse(HttpStatusCode.Forbidden);
        responseBuilder.AddErrorResponse(HttpStatusCode.NotFound);
        responseBuilder.AddErrorResponse(HttpStatusCode.Conflict);
        responseBuilder.AddErrorResponse<string>(HttpStatusCode.InternalServerError);
        responseBuilder.AddErrorResponse(HttpStatusCode.BadGateway);

        return await responseBuilder.BuildResponseAsync(x => new PatchOrdersIdEndpointResult(x), cancellationToken);
    }
}
