namespace ExUsers.Domain.Handlers.Users;

/// <summary>
/// Handler for operation request.
/// Description: Get user by id.
/// Operation: GetUserById.
/// </summary>
public class GetUserByIdHandler : IGetUserByIdHandler
{
    public Task<GetUserByIdResult> ExecuteAsync(
        GetUserByIdParameters parameters,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(parameters);

        throw new NotImplementedException("Add logic here for GetUserByIdHandler");
    }
}