namespace PetStoreApi.Domain.Handlers.Pets;

/// <summary>
/// Handler for operation request.
/// Description: Create a pet.
/// Operation: CreatePets.
/// </summary>
public class CreatePetsHandler : ICreatePetsHandler
{
    public Task<CreatePetsResult> ExecuteAsync(
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("Add logic here for CreatePetsHandler");
    }
}