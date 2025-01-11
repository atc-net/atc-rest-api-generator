namespace CodeStructure1.Domain.Orders.MyHandlers;

/// <summary>
/// Handler for operation request.
/// Description: Get order by id.
/// Operation: GetOrderById.
/// </summary>
public sealed class GetOrderByIdHandler : IGetOrderByIdHandler
{
    public Task<GetOrderByIdResult> ExecuteAsync(
        GetOrderByIdParameters parameters,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(parameters);

        throw new NotImplementedException("Add logic here for GetOrderByIdHandler");
    }
}