namespace Scenario2.Domain.Handlers.Users
{
    /// <summary>
    /// Handler for operation request.
    /// Description: Update gender on a user.
    /// Operation: UpdateMyTestGender.
    /// Area: Users.
    /// </summary>
    public class UpdateMyTestGenderHandler : IUpdateMyTestGenderHandler
    {
        public Task<UpdateMyTestGenderResult> ExecuteAsync(UpdateMyTestGenderParameters parameters, CancellationToken cancellationToken = default)
        {
            if (parameters is null)
            {
                throw new System.ArgumentNullException(nameof(parameters));
            }

            return InvokeExecuteAsync(parameters, cancellationToken);
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        private async Task<UpdateMyTestGenderResult> InvokeExecuteAsync(UpdateMyTestGenderParameters parameters, CancellationToken cancellationToken)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            throw new System.NotImplementedException();
        }
    }
}