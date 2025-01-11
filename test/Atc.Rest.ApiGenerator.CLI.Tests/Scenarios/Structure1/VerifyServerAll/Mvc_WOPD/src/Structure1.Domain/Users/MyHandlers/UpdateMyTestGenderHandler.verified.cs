namespace Structure1.Domain.Users.MyHandlers;

/// <summary>
/// Handler for operation request.
/// Description: Update gender on a user.
/// Operation: UpdateMyTestGender.
/// </summary>
public sealed class UpdateMyTestGenderHandler : IUpdateMyTestGenderHandler
{
    public Task<UpdateMyTestGenderResult> ExecuteAsync(
        UpdateMyTestGenderParameters parameters,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(parameters);

        throw new NotImplementedException("Add logic here for UpdateMyTestGenderHandler");
    }
}