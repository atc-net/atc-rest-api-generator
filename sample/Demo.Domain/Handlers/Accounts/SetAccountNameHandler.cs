using System.Threading;
using System.Threading.Tasks;
using Demo.Api.Generated.Contracts.Accounts;

namespace Demo.Domain.Handlers.Accounts
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

        public async Task<SetAccountNameResult> InvokeExecuteAsync(SetAccountNameParameters parameters, CancellationToken cancellationToken = default)
        {
            return await Task.FromResult(SetAccountNameResult.Ok("Now the account name is set"));
        }
    }
}