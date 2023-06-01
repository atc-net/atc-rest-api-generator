namespace TestUnit.Task.NsApi.Domain.Handlers.EventArgs;

/// <summary>
/// Handler for operation request.
/// Description: Get EventArgs List.
/// Operation: GetEventArgs.
/// </summary>
public class GetEventArgsHandler : IGetEventArgsHandler
{
    public Task<GetEventArgsResult> ExecuteAsync(
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("Add logic here for GetEventArgsHandler");
    }
}