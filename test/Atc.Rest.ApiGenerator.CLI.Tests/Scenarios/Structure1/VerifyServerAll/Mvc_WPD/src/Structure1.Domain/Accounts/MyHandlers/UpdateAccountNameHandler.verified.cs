﻿namespace Structure1.Domain.Accounts.MyHandlers;

/// <summary>
/// Handler for operation request.
/// Description: Update name of account.
/// Operation: UpdateAccountName.
/// </summary>
public sealed class UpdateAccountNameHandler : IUpdateAccountNameHandler
{
    public Task<UpdateAccountNameResult> ExecuteAsync(
        UpdateAccountNameParameters parameters,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(parameters);

        throw new NotImplementedException("Add logic here for UpdateAccountNameHandler");
    }
}