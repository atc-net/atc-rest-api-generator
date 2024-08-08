namespace ExNsWithTask.Domain.Handlers.TestUnits;

/// <summary>
/// Handler for operation request.
/// Description: List test units.
/// Operation: ListTestUnits.
/// </summary>
public sealed class ListTestUnitsHandler : IListTestUnitsHandler
{
    public Task<ListTestUnitsResult> ExecuteAsync(
        ListTestUnitsParameters parameters,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(parameters);

        throw new NotImplementedException("Add logic here for ListTestUnitsHandler");
    }
}