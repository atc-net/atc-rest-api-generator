namespace ExampleWithNsWithTask.Domain.Handlers.Orders;

/// <summary>
/// Handler for operation request.
/// Description: Get orders.
/// Operation: GetOrders.
/// </summary>
public class GetOrdersHandler : IGetOrdersHandler
{
    public Task<GetOrdersResult> ExecuteAsync(
        GetOrdersParameters parameters,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(parameters);

        throw new NotImplementedException("Add logic here for GetOrdersHandler");
    }
}