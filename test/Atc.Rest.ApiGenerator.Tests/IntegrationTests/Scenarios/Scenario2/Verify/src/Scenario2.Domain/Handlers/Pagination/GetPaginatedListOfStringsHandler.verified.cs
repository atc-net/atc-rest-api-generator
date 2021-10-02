using System.Threading;
using System.Threading.Tasks;
using Scenario2.Api.Generated.Contracts.Pagination;

namespace Scenario2.Domain.Handlers.Pagination
{
    /// <summary>
    /// Handler for operation request.
    /// Description: Your GET endpoint.
    /// Operation: GetPaginatedListOfStrings.
    /// Area: Pagination.
    /// </summary>
    public class GetPaginatedListOfStringsHandler : IGetPaginatedListOfStringsHandler
    {
        public Task<GetPaginatedListOfStringsResult> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }
    }
}