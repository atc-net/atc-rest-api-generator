namespace CodeStructure1.Domain.RouteWithDash.MyHandlers;

/// <summary>
/// Handler for operation request.
/// Description: Your GET endpoint.
/// Operation: GetRouteWithDash.
/// </summary>
public sealed class GetRouteWithDashHandler : IGetRouteWithDashHandler
{
    public Task<GetRouteWithDashResult> ExecuteAsync(
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("Add logic here for GetRouteWithDashHandler");
    }
}