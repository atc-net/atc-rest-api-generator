namespace ExUsers.Domain.Handlers.Users;

/// <summary>
/// Handler for operation request.
/// Description: Delete user by id.
/// Operation: DeleteUserById.
/// </summary>
public class DeleteUserByIdHandler : IDeleteUserByIdHandler
{
    public Task<DeleteUserByIdResult> ExecuteAsync(
        DeleteUserByIdParameters parameters,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(parameters);

        throw new NotImplementedException("Add logic here for DeleteUserByIdHandler");
    }
}