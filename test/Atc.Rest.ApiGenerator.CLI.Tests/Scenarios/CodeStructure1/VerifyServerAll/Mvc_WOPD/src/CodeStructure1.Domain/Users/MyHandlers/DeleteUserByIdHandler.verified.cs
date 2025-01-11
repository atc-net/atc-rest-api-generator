namespace CodeStructure1.Domain.Users.MyHandlers;

/// <summary>
/// Handler for operation request.
/// Description: Delete user by id.
/// Operation: DeleteUserById.
/// </summary>
public sealed class DeleteUserByIdHandler : IDeleteUserByIdHandler
{
    public Task<DeleteUserByIdResult> ExecuteAsync(
        DeleteUserByIdParameters parameters,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(parameters);

        throw new NotImplementedException("Add logic here for DeleteUserByIdHandler");
    }
}