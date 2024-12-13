﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace Monta.ApiClient.Generated.Endpoints.TeamMemberProfiles;

/// <summary>
/// Client Endpoint.
/// Description: Delete an existing team member profile.
/// Operation: DeleteTeamMemberProfile.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class DeleteTeamMemberProfileEndpoint : IDeleteTeamMemberProfileEndpoint
{
    private readonly IHttpClientFactory factory;
    private readonly IHttpMessageFactory httpMessageFactory;

    public DeleteTeamMemberProfileEndpoint(
        IHttpClientFactory factory,
        IHttpMessageFactory httpMessageFactory)
    {
        this.factory = factory;
        this.httpMessageFactory = httpMessageFactory;
    }

    public async Task<DeleteTeamMemberProfileEndpointResult> ExecuteAsync(
        DeleteTeamMemberProfileParameters parameters,
        string httpClientName = "Monta-ApiClient",
        CancellationToken cancellationToken = default)
    {
        var client = factory.CreateClient(httpClientName);

        var requestBuilder = httpMessageFactory.FromTemplate("/api/v1/team-member-profiles/{id}");
        requestBuilder.WithPathParameter("id", parameters.Id);

        using var requestMessage = requestBuilder.Build(HttpMethod.Delete);
        using var response = await client.SendAsync(requestMessage, cancellationToken);

        var responseBuilder = httpMessageFactory.FromResponse(response);
        responseBuilder.AddErrorResponse<ErrorResponse>(HttpStatusCode.NoContent);
        responseBuilder.AddErrorResponse<ErrorResponse>(HttpStatusCode.BadRequest);
        responseBuilder.AddErrorResponse<ErrorResponse>(HttpStatusCode.Unauthorized);
        responseBuilder.AddErrorResponse<ErrorResponse>(HttpStatusCode.Forbidden);
        responseBuilder.AddErrorResponse<ErrorResponse>(HttpStatusCode.NotFound);
        return await responseBuilder.BuildResponseAsync(x => new DeleteTeamMemberProfileEndpointResult(x), cancellationToken);
    }
}