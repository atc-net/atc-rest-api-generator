using System;
using System.Threading;
using System.Threading.Tasks;
using Demo.Api.Generated.Contracts.Users;

// ReSharper disable ConvertIfStatementToReturnStatement
namespace Demo.Domain.Handlers.Users
{
    public class GetUserByEmailHandler : IGetUserByEmailHandler
    {
        public Task<GetUserByEmailResult> ExecuteAsync(GetUserByEmailParameters parameters, CancellationToken cancellationToken = default)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            if (parameters.Email == "a@a.com")
            {
                return Task.FromResult(GetUserByEmailResult.NotFound($"Could not find user with email={parameters.Email}"));
            }

            return InvokeExecuteAsync(parameters, cancellationToken);
        }

        private async Task<GetUserByEmailResult> InvokeExecuteAsync(GetUserByEmailParameters parameters, CancellationToken cancellationToken)
        {
            var data = new User
            {
                Id = Guid.NewGuid(),
                FirstName = "John",
                LastName = "Doe",
                Email = parameters.Email,
            };

            return await Task.FromResult(data);
        }
    }
}