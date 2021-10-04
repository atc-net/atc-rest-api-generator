using System.Threading;
using System.Threading.Tasks;
using Scenario2.Api.Generated.Contracts.RouteWithDash;

namespace Scenario2.Domain.Handlers.RouteWithDash
{
    /// <summary>
    /// Handler for operation request.
    /// Description: Your GET endpoint.
    /// Operation: GetRouteWithDash.
    /// Area: RouteWithDash.
    /// </summary>
    public class GetRouteWithDashHandler : IGetRouteWithDashHandler
    {
        public Task<GetRouteWithDashResult> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }
    }
}