﻿namespace PetStoreApi.Domain.Handlers.Pets;

/// <summary>
/// Handler for operation request.
/// Description: Info for a specific pet.
/// Operation: ShowPetById.
/// </summary>
public class ShowPetByIdHandler : IShowPetByIdHandler
{
    public Task<ShowPetByIdResult> ExecuteAsync(
        ShowPetByIdParameters parameters,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(parameters);

        throw new NotImplementedException("Add logic here for ShowPetByIdHandler");
    }
}