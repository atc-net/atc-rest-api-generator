namespace GenericPaginationApi.Domain.Handlers.Cats;

/// <summary>
/// Handler for operation request.
/// Description: Find all cats.
/// Operation: GetCats.
/// </summary>
public class GetCatsHandler : IGetCatsHandler
{
    public Task<GetCatsResult> ExecuteAsync(
        GetCatsParameters parameters,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(parameters);

        throw new NotImplementedException("Add logic here for GetCatsHandler");
    }
}