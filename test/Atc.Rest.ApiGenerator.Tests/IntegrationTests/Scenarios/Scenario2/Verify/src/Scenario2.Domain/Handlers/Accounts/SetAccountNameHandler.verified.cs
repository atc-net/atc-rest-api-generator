namespace Scenario2.Domain.Handlers.Accounts
{
    /// <summary>
    /// Handler for operation request.
    /// Description: Set name of account.
    /// Operation: SetAccountName.
    /// Area: Accounts.
    /// </summary>
    public class SetAccountNameHandler : ISetAccountNameHandler
    {
        public Task<SetAccountNameResult> ExecuteAsync(SetAccountNameParameters parameters, CancellationToken cancellationToken = default)
        {
            if (parameters is null)
            {
                throw new System.ArgumentNullException(nameof(parameters));
            }

            return InvokeExecuteAsync(parameters, cancellationToken);
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        private async Task<SetAccountNameResult> InvokeExecuteAsync(SetAccountNameParameters parameters, CancellationToken cancellationToken)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            throw new System.NotImplementedException();
        }
    }
}