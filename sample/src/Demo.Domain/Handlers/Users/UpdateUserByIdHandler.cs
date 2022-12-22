﻿namespace Demo.Domain.Handlers.Users;

/// <summary>
/// Handler for operation request.
/// Description: Update user by id.
/// Operation: UpdateUserById.
/// Area: Users.
/// </summary>
public class UpdateUserByIdHandler : IUpdateUserByIdHandler
{
    public Task<UpdateUserByIdResult> ExecuteAsync(UpdateUserByIdParameters parameters, CancellationToken cancellationToken = default)
    {
        if (parameters is null)
        {
            throw new System.ArgumentNullException(nameof(parameters));
        }

        return InvokeExecuteAsync(parameters, cancellationToken);
    }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
    private async Task<UpdateUserByIdResult> InvokeExecuteAsync(UpdateUserByIdParameters parameters, CancellationToken cancellationToken)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
    {
        throw new System.NotImplementedException();
    }
}