namespace ExAsyncEnumerable.Domain.Handlers.Users;

/// <summary>
/// Handler for operation request.
/// Description: Get users.
/// Operation: GetUsers.
/// </summary>
public sealed class GetUsersHandler : IGetUsersHandler
{
    public Task<GetUsersResult> ExecuteAsync(
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("Add logic here for GetUsersHandler");
    }
}