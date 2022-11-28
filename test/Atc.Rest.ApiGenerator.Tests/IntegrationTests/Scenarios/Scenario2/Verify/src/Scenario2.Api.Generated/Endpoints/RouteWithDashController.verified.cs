//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace Scenario2.Api.Generated.Endpoints;

/// <summary>
/// Endpoint definitions.
/// </summary>
[Authorize]
[ApiController]
[Route("/api/v1/route-with-dash")]
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class RouteWithDashController : ControllerBase
{
    /// <summary>
    /// Description: Your GET endpoint.
    /// Operation: GetRouteWithDash.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult> GetRouteWithDash(
        [FromServices] IGetRouteWithDashHandler handler,
        CancellationToken cancellationToken)
        => await handler.ExecuteAsync(cancellationToken);
}