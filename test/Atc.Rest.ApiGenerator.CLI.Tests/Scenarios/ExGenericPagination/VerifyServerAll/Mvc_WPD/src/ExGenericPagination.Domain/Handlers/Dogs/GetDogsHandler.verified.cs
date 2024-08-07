﻿namespace ExGenericPagination.Domain.Handlers.Dogs;

/// <summary>
/// Handler for operation request.
/// Description: Find all dogs.
/// Operation: GetDogs.
/// </summary>
public sealed class GetDogsHandler : IGetDogsHandler
{
    public Task<GetDogsResult> ExecuteAsync(
        GetDogsParameters parameters,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(parameters);

        throw new NotImplementedException("Add logic here for GetDogsHandler");
    }
}