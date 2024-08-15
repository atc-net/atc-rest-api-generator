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
[Authorize]
[ApiController]
[Route("/api/v1/users")]
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public sealed class UsersController : ControllerBase
{
    /// <summary>
    /// Description: Get users.
    /// Operation: GetUsers.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IAsyncEnumerable<User>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult> GetUsers(
        [FromServices] IGetUsersHandler handler,
        CancellationToken cancellationToken)
        => await handler.ExecuteAsync(cancellationToken);
}