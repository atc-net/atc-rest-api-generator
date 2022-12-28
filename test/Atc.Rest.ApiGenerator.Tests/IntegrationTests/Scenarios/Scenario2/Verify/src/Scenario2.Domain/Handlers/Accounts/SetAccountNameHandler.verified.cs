namespace Scenario2.Domain.Handlers.Accounts;

/// <summary>
/// Handler for operation request.
/// Description: Set name of account.
/// Operation: SetAccountName.
/// </summary>
public class SetAccountNameHandler : ISetAccountNameHandler
{
    public Task<SetAccountNameResult> ExecuteAsync(
        SetAccountNameParameters parameters,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(parameters);

        throw new NotImplementedException("Add logic here for SetAccountNameHandler");
    }
}