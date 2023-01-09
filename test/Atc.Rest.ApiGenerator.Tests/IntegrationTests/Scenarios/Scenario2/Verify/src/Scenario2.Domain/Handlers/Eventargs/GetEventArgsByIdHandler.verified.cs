namespace Scenario2.Domain.Handlers.Eventargs;

/// <summary>
/// Handler for operation request.
/// Description: Get EventArgs By Id.
/// Operation: GetEventArgsById.
/// </summary>
public class GetEventArgsByIdHandler : IGetEventArgsByIdHandler
{
    public Task<GetEventArgsByIdResult> ExecuteAsync(
        GetEventArgsByIdParameters parameters,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(parameters);

        throw new NotImplementedException("Add logic here for GetEventArgsByIdHandler");
    }
}