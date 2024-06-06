namespace DemoSample.Domain.Handlers.RouteWithDash;

/// <summary>
/// Handler for operation request.
/// Description: Your GET endpoint.
/// Operation: GetRouteWithDash.
/// </summary>
public class GetRouteWithDashHandler : IGetRouteWithDashHandler
{
    public Task<GetRouteWithDashResult> ExecuteAsync(
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("Add logic here for GetRouteWithDashHandler");
    }
}