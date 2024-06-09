//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace ExampleWithGenericPagination.ApiClient.Generated.Endpoints.Cats;

/// <summary>
/// Client Endpoint.
/// Description: Find all cats.
/// Operation: GetCats.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class GetCatsEndpoint : IGetCatsEndpoint
{
    private readonly IHttpClientFactory factory;
    private readonly IHttpMessageFactory httpMessageFactory;

    public GetCatsEndpoint(
        IHttpClientFactory factory,
        IHttpMessageFactory httpMessageFactory)
    {
        this.factory = factory;
        this.httpMessageFactory = httpMessageFactory;
    }

    public async Task<GetCatsEndpointResult> ExecuteAsync(
        GetCatsParameters parameters,
        string httpClientName = "ExampleWithGenericPagination-ApiClient",
        CancellationToken cancellationToken = default)
    {
        var client = factory.CreateClient(httpClientName);

        var requestBuilder = httpMessageFactory.FromTemplate("/api/v1/cats");
        requestBuilder.WithQueryParameter("pageSize", parameters.PageSize);
        requestBuilder.WithQueryParameter("pageIndex", parameters.PageIndex);
        requestBuilder.WithQueryParameter("queryString", parameters.QueryString);

        using var requestMessage = requestBuilder.Build(HttpMethod.Get);
        using var response = await client.SendAsync(requestMessage, cancellationToken);

        var responseBuilder = httpMessageFactory.FromResponse(response);
        responseBuilder.AddSuccessResponse<PaginatedResult<Cat>>(HttpStatusCode.OK);
        responseBuilder.AddErrorResponse<ValidationProblemDetails>(HttpStatusCode.BadRequest);
        responseBuilder.AddErrorResponse<string>(HttpStatusCode.Unauthorized);
        return await responseBuilder.BuildResponseAsync(x => new GetCatsEndpointResult(x), cancellationToken);
    }
}
