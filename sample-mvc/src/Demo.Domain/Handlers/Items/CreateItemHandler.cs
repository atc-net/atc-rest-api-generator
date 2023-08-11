namespace Demo.Domain.Handlers.Items;

/// <summary>
/// Handler for operation request.
/// Description: Create a new item.
/// Operation: CreateItem.
/// </summary>
public class CreateItemHandler : ICreateItemHandler
{
    public Task<CreateItemResult> ExecuteAsync(
        CreateItemParameters parameters,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(parameters);

        throw new NotImplementedException("Add logic here for CreateItemHandler");
    }
}