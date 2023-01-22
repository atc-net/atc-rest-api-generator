namespace DemoUsersApi.Domain.Handlers.Users;

/// <summary>
/// Handler for operation request.
/// Description: Get all users.
/// Operation: GetUsers.
/// </summary>
public class GetUsersHandler : IGetUsersHandler
{
    public Task<GetUsersResult> ExecuteAsync(
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("Add logic here for GetUsersHandler");
    }
}