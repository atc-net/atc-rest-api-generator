namespace ExNsWithTask.Domain.Handlers.EventArgs;

/// <summary>
/// Handler for operation request.
/// Description: Get EventArgs By Id.
/// Operation: GetEventArgById.
/// </summary>
public class GetEventArgByIdHandler : IGetEventArgByIdHandler
{
    public Task<GetEventArgByIdResult> ExecuteAsync(
        GetEventArgByIdParameters parameters,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(parameters);

        throw new NotImplementedException("Add logic here for GetEventArgByIdHandler");
    }
}