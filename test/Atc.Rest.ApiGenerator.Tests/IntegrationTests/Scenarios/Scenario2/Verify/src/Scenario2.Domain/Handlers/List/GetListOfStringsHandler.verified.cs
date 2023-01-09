namespace Scenario2.Domain.Handlers.List;

/// <summary>
/// Handler for operation request.
/// Description: Your GET endpoint.
/// Operation: GetListOfStrings.
/// </summary>
public class GetListOfStringsHandler : IGetListOfStringsHandler
{
    public Task<GetListOfStringsResult> ExecuteAsync(
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("Add logic here for GetListOfStringsHandler");
    }
}