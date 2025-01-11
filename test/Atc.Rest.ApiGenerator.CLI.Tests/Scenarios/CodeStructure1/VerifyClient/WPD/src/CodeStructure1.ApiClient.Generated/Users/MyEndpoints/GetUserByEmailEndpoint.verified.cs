﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace CodeStructure1.ApiClient.Generated.Users.MyEndpoints;

/// <summary>
/// Client Endpoint.
/// Description: Get user by email.
/// Operation: GetUserByEmail.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class GetUserByEmailEndpoint : IGetUserByEmailEndpoint
{
    private readonly IHttpClientFactory factory;
    private readonly IHttpMessageFactory httpMessageFactory;

    public GetUserByEmailEndpoint(
        IHttpClientFactory factory,
        IHttpMessageFactory httpMessageFactory)
    {
        this.factory = factory;
        this.httpMessageFactory = httpMessageFactory;
    }

    public async Task<GetUserByEmailEndpointResult> ExecuteAsync(
        GetUserByEmailParameters parameters,
        string httpClientName = "CodeStructure1-ApiClient",
        CancellationToken cancellationToken = default)
    {
        var client = factory.CreateClient(httpClientName);

        var requestBuilder = httpMessageFactory.FromTemplate("/api/v1/users/email");
        requestBuilder.WithQueryParameter("email", parameters.Email);

        using var requestMessage = requestBuilder.Build(HttpMethod.Get);
        using var response = await client.SendAsync(requestMessage, cancellationToken);

        var responseBuilder = httpMessageFactory.FromResponse(response);
        responseBuilder.AddSuccessResponse<User>(HttpStatusCode.OK);
        responseBuilder.AddErrorResponse<ValidationProblemDetails>(HttpStatusCode.BadRequest);
        responseBuilder.AddErrorResponse<string?>(HttpStatusCode.NotFound);
        responseBuilder.AddErrorResponse<ProblemDetails>(HttpStatusCode.Conflict);
        return await responseBuilder.BuildResponseAsync(x => new GetUserByEmailEndpointResult(x), cancellationToken);
    }
}