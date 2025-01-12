namespace Structure1.Domain.EventArgs.MyHandlers;

/// <summary>
/// Handler for operation request.
/// Description: Get EventArgs List.
/// Operation: GetEventArgs.
/// </summary>
public sealed class GetEventArgsHandler : IGetEventArgsHandler
{
    public Task<GetEventArgsResult> ExecuteAsync(
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("Add logic here for GetEventArgsHandler");
    }
}