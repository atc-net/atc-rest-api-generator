using System.Threading;
using System.Threading.Tasks;
using Scenario2.Api.Generated.Contracts.List;

namespace Scenario2.Domain.Handlers.List
{
    /// <summary>
    /// Handler for operation request.
    /// Description: Your GET endpoint.
    /// Operation: GetListOfInts.
    /// Area: List.
    /// </summary>
    public class GetListOfIntsHandler : IGetListOfIntsHandler
    {
        public Task<GetListOfIntsResult> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }
    }
}