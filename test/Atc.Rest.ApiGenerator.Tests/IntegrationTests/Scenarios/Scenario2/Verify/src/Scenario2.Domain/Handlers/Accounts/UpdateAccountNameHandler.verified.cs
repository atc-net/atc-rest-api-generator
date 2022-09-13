namespace Scenario2.Domain.Handlers.Accounts
{
    /// <summary>
    /// Handler for operation request.
    /// Description: Update name of account.
    /// Operation: UpdateAccountName.
    /// Area: Accounts.
    /// </summary>
    public class UpdateAccountNameHandler : IUpdateAccountNameHandler
    {
        public Task<UpdateAccountNameResult> ExecuteAsync(UpdateAccountNameParameters parameters, CancellationToken cancellationToken = default)
        {
            if (parameters is null)
            {
                throw new System.ArgumentNullException(nameof(parameters));
            }

            return InvokeExecuteAsync(parameters, cancellationToken);
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        private async Task<UpdateAccountNameResult> InvokeExecuteAsync(UpdateAccountNameParameters parameters, CancellationToken cancellationToken)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            throw new System.NotImplementedException();
        }
    }
}