//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace DemoSample.Api.Generated.Contracts.RouteWithDash;

/// <summary>
/// Results for operation request.
/// Description: Your GET endpoint.
/// Operation: GetRouteWithDash.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class GetRouteWithDashResult : ResultBase
{
    private GetRouteWithDashResult(ActionResult result) : base(result) { }

    /// <summary>
    /// 200 - Ok response.
    /// </summary>
    public static GetRouteWithDashResult Ok(string? message = null)
        => new GetRouteWithDashResult(new OkObjectResult(message));

    /// <summary>
    /// Performs an implicit conversion from GetRouteWithDashResult to ActionResult.
    /// </summary>
    public static implicit operator GetRouteWithDashResult(string response)
        => Ok(response);
}
