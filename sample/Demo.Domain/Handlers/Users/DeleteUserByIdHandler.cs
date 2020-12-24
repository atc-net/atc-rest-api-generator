using System;
using System.Threading;
using System.Threading.Tasks;
using Demo.Api.Generated.Contracts.Users;

// ReSharper disable ConvertIfStatementToReturnStatement
namespace Demo.Domain.Handlers.Users
{
    public class DeleteUserByIdHandler : IDeleteUserByIdHandler
    {
        public Task<DeleteUserByIdResult> ExecuteAsync(DeleteUserByIdParameters parameters, CancellationToken cancellationToken = default)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            if (parameters.Id == Guid.Parse("77a33260-0007-441f-ba60-b0a833803fab"))
            {
                return Task.FromResult(DeleteUserByIdResult.NotFound($"Can't find user with id={parameters.Id}"));
            }

            return InvokeExecuteAsync(parameters, cancellationToken);
        }

        private async Task<DeleteUserByIdResult> InvokeExecuteAsync(DeleteUserByIdParameters parameters, CancellationToken cancellationToken)
        {
            return await Task.FromResult("User deleted.");
        }
    }
}