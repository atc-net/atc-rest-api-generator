namespace ExAsyncEnumerable.Domain.Handlers.Customers;

/// <summary>
/// Handler for operation request.
/// Description: Get customers.
/// Operation: GetCustomers.
/// </summary>
public sealed class GetCustomersHandler : IGetCustomersHandler
{
    public Task<GetCustomersResult> ExecuteAsync(
        GetCustomersParameters parameters,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(parameters);

        throw new NotImplementedException("Add logic here for GetCustomersHandler");
    }
}