﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace ExAsyncEnumerable.Api.Generated.Endpoints;

/// <summary>
/// Endpoint definitions.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public sealed class UsersEndpointDefinition : IEndpointDefinition
{
    internal const string ApiRouteBase = "/api/v1/users";

    public void DefineEndpoints(
        WebApplication app)
    {
        var users = app
            .NewVersionedApi("Users")
            .MapGroup(ApiRouteBase);

        users
            .MapGet("/", GetUsers)
            .WithName("GetUsers")
            .WithSummary("Get users.")
            .WithDescription("Get users.")
            .Produces<IAsyncEnumerable<User>>();
    }

    internal async Task<IResult> GetUsers(
        [FromServices] IGetUsersHandler handler,
        CancellationToken cancellationToken)
        => GetUsersResult.ToIResult(
            await handler.ExecuteAsync(
                cancellationToken));
}