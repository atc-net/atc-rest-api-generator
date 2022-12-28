namespace Scenario2.Domain.Handlers.Pagination;

/// <summary>
/// Handler for operation request.
/// Description: Your GET endpoint.
/// Operation: GetPaginatedListOfStrings.
/// </summary>
public class GetPaginatedListOfStringsHandler : IGetPaginatedListOfStringsHandler
{
    public Task<GetPaginatedListOfStringsResult> ExecuteAsync(
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("Add logic here for GetPaginatedListOfStringsHandler");
    }
}