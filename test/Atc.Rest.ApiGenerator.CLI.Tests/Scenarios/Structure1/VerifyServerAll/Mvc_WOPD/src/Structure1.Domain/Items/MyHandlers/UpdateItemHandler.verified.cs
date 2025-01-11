namespace Structure1.Domain.Items.MyHandlers;

/// <summary>
/// Handler for operation request.
/// Description: Updates an item.
/// Operation: UpdateItem.
/// </summary>
public sealed class UpdateItemHandler : IUpdateItemHandler
{
    public Task<UpdateItemResult> ExecuteAsync(
        UpdateItemParameters parameters,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(parameters);

        throw new NotImplementedException("Add logic here for UpdateItemHandler");
    }
}