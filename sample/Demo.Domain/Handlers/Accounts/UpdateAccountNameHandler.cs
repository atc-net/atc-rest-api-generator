using System.Threading;
using System.Threading.Tasks;
using Demo.Api.Generated.Contracts.Accounts;

namespace Demo.Domain.Handlers.Accounts
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

        public async Task<UpdateAccountNameResult> InvokeExecuteAsync(UpdateAccountNameParameters parameters, CancellationToken cancellationToken = default)
        {
            return await Task.FromResult(UpdateAccountNameResult.Ok("Now the account name is updated"));
        }
    }
}