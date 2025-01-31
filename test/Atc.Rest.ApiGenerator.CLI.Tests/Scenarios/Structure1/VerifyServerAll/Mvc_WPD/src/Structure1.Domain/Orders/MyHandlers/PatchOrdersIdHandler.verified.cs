﻿namespace Structure1.Domain.Orders.MyHandlers;

/// <summary>
/// Handler for operation request.
/// Description: Update part of order by id.
/// Operation: PatchOrdersId.
/// </summary>
public sealed class PatchOrdersIdHandler : IPatchOrdersIdHandler
{
    public Task<PatchOrdersIdResult> ExecuteAsync(
        PatchOrdersIdParameters parameters,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(parameters);

        throw new NotImplementedException("Add logic here for PatchOrdersIdHandler");
    }
}