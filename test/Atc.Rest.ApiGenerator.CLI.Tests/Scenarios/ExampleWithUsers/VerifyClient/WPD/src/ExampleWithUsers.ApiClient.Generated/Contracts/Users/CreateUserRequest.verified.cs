//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace ExampleWithUsers.ApiClient.Generated.Contracts.Users;

/// <summary>
/// Request to create a user.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class CreateUserRequest
{
    [Required]
    public string FirstName { get; set; }

    [Required]
    public string LastName { get; set; }

    public DateTimeOffset? MyNullableDateTime { get; set; }

    [Required]
    public DateTimeOffset MyDateTime { get; set; }

    /// <summary>
    /// Undefined description.
    /// </summary>
    /// <remarks>
    /// Email validation being enforced.
    /// </remarks>
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    /// <summary>
    /// Undefined description.
    /// </summary>
    /// <remarks>
    /// Url validation being enforced.
    /// </remarks>
    [Uri]
    public Uri Homepage { get; set; }

    /// <summary>
    /// GenderType.
    /// </summary>
    [Required]
    public GenderType Gender { get; set; }

    public Address? MyNullableAddress { get; set; }

    /// <inheritdoc />
    public override string ToString()
        => $"{nameof(FirstName)}: {FirstName}, {nameof(LastName)}: {LastName}, {nameof(MyNullableDateTime)}: ({MyNullableDateTime}), {nameof(MyDateTime)}: ({MyDateTime}), {nameof(Email)}: {Email}, {nameof(Homepage)}: ({Homepage}), {nameof(Gender)}: {Gender}, {nameof(MyNullableAddress)}: ({MyNullableAddress})";
}
