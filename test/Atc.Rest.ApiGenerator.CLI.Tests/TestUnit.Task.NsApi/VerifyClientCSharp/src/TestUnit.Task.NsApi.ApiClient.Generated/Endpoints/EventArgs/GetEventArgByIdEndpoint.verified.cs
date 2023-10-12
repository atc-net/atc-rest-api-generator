﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace TestUnit.Task.NsApi.ApiClient.Generated.Endpoints;

/// <summary>
/// Client Endpoint.
/// Description: Get EventArgs By Id.
/// Operation: GetEventArgById.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class GetEventArgByIdEndpoint : IGetEventArgByIdEndpoint
{
    private readonly IHttpClientFactory factory;
    private readonly IHttpMessageFactory httpMessageFactory;

    public GetEventArgByIdEndpoint(
        IHttpClientFactory factory,
        IHttpMessageFactory httpMessageFactory)
    {
        this.factory = factory;
        this.httpMessageFactory = httpMessageFactory;
    }

    public async Task<IGetEventArgByIdEndpointResult> ExecuteAsync(
        GetEventArgByIdParameters parameters,
        CancellationToken cancellationToken = default)
    {
        var client = factory.CreateClient("TestUnit.Task.NsApi-ApiClient");

        var requestBuilder = httpMessageFactory.FromTemplate("/api/v1/eventArgs/{id}");
        requestBuilder.WithPathParameter("id", parameters.Id);

        using var requestMessage = requestBuilder.Build(HttpMethod.Get);
        using var response = await client.SendAsync(requestMessage, cancellationToken);

        var responseBuilder = httpMessageFactory.FromResponse(response);
        responseBuilder.AddSuccessResponse<Contracts.EventArgs>(HttpStatusCode.OK);
        responseBuilder.AddErrorResponse<ValidationProblemDetails>(HttpStatusCode.BadRequest);
        responseBuilder.AddErrorResponse(HttpStatusCode.Unauthorized);
        responseBuilder.AddErrorResponse(HttpStatusCode.Forbidden);
        responseBuilder.AddErrorResponse(HttpStatusCode.NotFound);
        responseBuilder.AddErrorResponse<string>(HttpStatusCode.InternalServerError);

        return await responseBuilder.BuildResponseAsync(x => new GetEventArgByIdEndpointResult(x), cancellationToken);
    }
}