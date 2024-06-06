namespace PetStore.Domain.Handlers.Pets;

/// <summary>
/// Handler for operation request.
/// Description: List all pets.
/// Operation: ListPets.
/// </summary>
public class ListPetsHandler : IListPetsHandler
{
    public Task<ListPetsResult> ExecuteAsync(
        ListPetsParameters parameters,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(parameters);

        throw new NotImplementedException("Add logic here for ListPetsHandler");
    }
}