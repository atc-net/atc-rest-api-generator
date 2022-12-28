namespace Scenario2.Domain.Handlers.Orders;

/// <summary>
/// Handler for operation request.
/// Description: Get order by id.
/// Operation: GetOrderById.
/// </summary>
public class GetOrderByIdHandler : IGetOrderByIdHandler
{
    public Task<GetOrderByIdResult> ExecuteAsync(
        GetOrderByIdParameters parameters,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(parameters);

        throw new NotImplementedException("Add logic here for GetOrderByIdHandler");
    }
}