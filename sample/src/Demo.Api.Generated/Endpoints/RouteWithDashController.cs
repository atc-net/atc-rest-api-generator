﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator 2.0.259.29354.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace Demo.Api.Generated.Endpoints;

/// <summary>
/// Endpoint definitions.
/// </summary>
[Authorize]
[ApiController]
[Route("/api/v1/route-with-dash")]
[GeneratedCode("ApiGenerator", "2.0.259.29354")]
public class RouteWithDashController : ControllerBase
{
    /// <summary>
    /// Description: Your GET endpoint.
    /// Operation: GetRouteWithDash.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult> GetRouteWithDash(
        [FromServices] IGetRouteWithDashHandler handler,
        CancellationToken cancellationToken)
        => await handler.ExecuteAsync(cancellationToken);
}