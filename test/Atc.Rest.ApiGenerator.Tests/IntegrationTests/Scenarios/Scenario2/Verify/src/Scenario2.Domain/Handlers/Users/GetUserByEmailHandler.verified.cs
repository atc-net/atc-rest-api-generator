using System.Threading;
using System.Threading.Tasks;
using Scenario2.Api.Generated.Contracts.Users;

namespace Scenario2.Domain.Handlers.Users
{
    /// <summary>
    /// Handler for operation request.
    /// Description: Get user by email.
    /// Operation: GetUserByEmail.
    /// Area: Users.
    /// </summary>
    public class GetUserByEmailHandler : IGetUserByEmailHandler
    {
        public Task<GetUserByEmailResult> ExecuteAsync(GetUserByEmailParameters parameters, CancellationToken cancellationToken = default)
        {
            if (parameters is null)
            {
                throw new System.ArgumentNullException(nameof(parameters));
            }

            return InvokeExecuteAsync(parameters, cancellationToken);
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        private async Task<GetUserByEmailResult> InvokeExecuteAsync(GetUserByEmailParameters parameters, CancellationToken cancellationToken)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            throw new System.NotImplementedException();
        }
    }
}