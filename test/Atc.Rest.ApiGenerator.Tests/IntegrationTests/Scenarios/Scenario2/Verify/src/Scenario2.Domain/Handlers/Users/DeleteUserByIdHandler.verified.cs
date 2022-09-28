namespace Scenario2.Domain.Handlers.Users;

/// <summary>
/// Handler for operation request.
/// Description: Delete user by id.
/// Operation: DeleteUserById.
/// Area: Users.
/// </summary>
public class DeleteUserByIdHandler : IDeleteUserByIdHandler
{
    public Task<DeleteUserByIdResult> ExecuteAsync(DeleteUserByIdParameters parameters, CancellationToken cancellationToken = default)
    {
        if (parameters is null)
        {
            throw new System.ArgumentNullException(nameof(parameters));
        }

        return InvokeExecuteAsync(parameters, cancellationToken);
    }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
    private async Task<DeleteUserByIdResult> InvokeExecuteAsync(DeleteUserByIdParameters parameters, CancellationToken cancellationToken)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
    {
        throw new System.NotImplementedException();
    }
}