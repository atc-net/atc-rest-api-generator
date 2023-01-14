namespace Demo.Domain.Handlers.EventArgs;

/// <summary>
/// Handler for operation request.
/// Description: Get EventArgs List.
/// Operation: GetEventArgs.
/// Area: EventArgs.
/// </summary>
public class GetEventArgsHandler : IGetEventArgsHandler
{
    public Task<GetEventArgsResult> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        throw new System.NotImplementedException();
    }
}