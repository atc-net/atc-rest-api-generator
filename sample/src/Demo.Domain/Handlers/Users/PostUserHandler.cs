using System.Threading;
using System.Threading.Tasks;
using Demo.Api.Generated.Contracts.Users;

namespace Demo.Domain.Handlers.Users
{
    /// <summary>
    /// Handler for operation request.
    /// Description: Create a new user.
    /// Operation: PostUser.
    /// Area: Users.
    /// </summary>
    public class PostUserHandler : IPostUserHandler
    {
        public Task<PostUserResult> ExecuteAsync(PostUserParameters parameters, CancellationToken cancellationToken = default)
        {
            if (parameters is null)
            {
                throw new System.ArgumentNullException(nameof(parameters));
            }

            return InvokeExecuteAsync(parameters, cancellationToken);
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        private async Task<PostUserResult> InvokeExecuteAsync(PostUserParameters parameters, CancellationToken cancellationToken)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            throw new System.NotImplementedException();
        }
    }
}