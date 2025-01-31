﻿namespace Structure1.Domain.Users.MyHandlers;

/// <summary>
/// Handler for operation request.
/// Description: Update user by id.
/// Operation: UpdateUserById.
/// </summary>
public sealed class UpdateUserByIdHandler : IUpdateUserByIdHandler
{
    public Task<UpdateUserByIdResult> ExecuteAsync(
        UpdateUserByIdParameters parameters,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(parameters);

        throw new NotImplementedException("Add logic here for UpdateUserByIdHandler");
    }
}