namespace Scenario2.Domain.Handlers.Eventargs;

/// <summary>
/// Handler for operation request.
/// Description: Get EventArgs By Id.
/// Operation: GetEventArgsById.
/// Area: Eventargs.
/// </summary>
public class GetEventArgsByIdHandler : IGetEventArgsByIdHandler
{
    public Task<GetEventArgsByIdResult> ExecuteAsync(GetEventArgsByIdParameters parameters, CancellationToken cancellationToken = default)
    {
        if (parameters is null)
        {
            throw new System.ArgumentNullException(nameof(parameters));
        }

        return InvokeExecuteAsync(parameters, cancellationToken);
    }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
    private async Task<GetEventArgsByIdResult> InvokeExecuteAsync(GetEventArgsByIdParameters parameters, CancellationToken cancellationToken)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
    {
        throw new System.NotImplementedException();
    }
}