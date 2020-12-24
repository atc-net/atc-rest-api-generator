using System;
using System.Threading;
using System.Threading.Tasks;
using Demo.Api.Generated.Contracts.Users;

namespace Demo.Domain.Handlers.Users
{
    public class UpdateMyTestGenderHandler : IUpdateMyTestGenderHandler
    {
        public Task<UpdateMyTestGenderResult> ExecuteAsync(UpdateMyTestGenderParameters parameters, CancellationToken cancellationToken = default)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            return InvokeExecuteAsync(parameters, cancellationToken);
        }

        private async Task<UpdateMyTestGenderResult> InvokeExecuteAsync(UpdateMyTestGenderParameters parameters, CancellationToken cancellationToken)
        {
            return await Task.FromResult("We are now updated.");
        }
    }
}