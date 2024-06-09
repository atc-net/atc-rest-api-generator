//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace DemoSample.ApiClient.Generated.Endpoints.Users;

/// <summary>
/// Client Endpoint.
/// Description: Delete user by id.
/// Operation: DeleteUserById.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class DeleteUserByIdEndpoint : IDeleteUserByIdEndpoint
{
    private readonly IHttpClientFactory factory;
    private readonly IHttpMessageFactory httpMessageFactory;

    public DeleteUserByIdEndpoint(
        IHttpClientFactory factory,
        IHttpMessageFactory httpMessageFactory)
    {
        this.factory = factory;
        this.httpMessageFactory = httpMessageFactory;
    }

    public async Task<DeleteUserByIdEndpointResult> ExecuteAsync(
        DeleteUserByIdParameters parameters,
        string httpClientName = "DemoSample-ApiClient",
        CancellationToken cancellationToken = default)
    {
        var client = factory.CreateClient(httpClientName);

        var requestBuilder = httpMessageFactory.FromTemplate("/api/v1/users/{id}");
        requestBuilder.WithPathParameter("id", parameters.Id);

        using var requestMessage = requestBuilder.Build(HttpMethod.Delete);
        using var response = await client.SendAsync(requestMessage, cancellationToken);

        var responseBuilder = httpMessageFactory.FromResponse(response);
        responseBuilder.AddSuccessResponse<string?>(HttpStatusCode.OK);
        responseBuilder.AddErrorResponse<ValidationProblemDetails>(HttpStatusCode.BadRequest);
        responseBuilder.AddErrorResponse<ProblemDetails>(HttpStatusCode.Unauthorized);
        responseBuilder.AddErrorResponse<ProblemDetails>(HttpStatusCode.NotFound);
        responseBuilder.AddErrorResponse<ProblemDetails>(HttpStatusCode.Conflict);
        return await responseBuilder.BuildResponseAsync(x => new DeleteUserByIdEndpointResult(x), cancellationToken);
    }
}
