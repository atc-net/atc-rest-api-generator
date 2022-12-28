namespace Scenario2.Domain.Handlers.Pagination;

/// <summary>
/// Handler for operation request.
/// Description: Your GET endpoint.
/// Operation: GetPaginatedListOfInts.
/// </summary>
public class GetPaginatedListOfIntsHandler : IGetPaginatedListOfIntsHandler
{
    public Task<GetPaginatedListOfIntsResult> ExecuteAsync(
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("Add logic here for GetPaginatedListOfIntsHandler");
    }
}