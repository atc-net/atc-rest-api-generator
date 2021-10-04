using System.Threading;
using System.Threading.Tasks;
using Scenario2.Api.Generated.Contracts.Users;

namespace Scenario2.Domain.Handlers.Users
{
    /// <summary>
    /// Handler for operation request.
    /// Description: Get user by id.
    /// Operation: GetUserById.
    /// Area: Users.
    /// </summary>
    public class GetUserByIdHandler : IGetUserByIdHandler
    {
        public Task<GetUserByIdResult> ExecuteAsync(GetUserByIdParameters parameters, CancellationToken cancellationToken = default)
        {
            if (parameters is null)
            {
                throw new System.ArgumentNullException(nameof(parameters));
            }

            return InvokeExecuteAsync(parameters, cancellationToken);
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        private async Task<GetUserByIdResult> InvokeExecuteAsync(GetUserByIdParameters parameters, CancellationToken cancellationToken)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            throw new System.NotImplementedException();
        }
    }
}