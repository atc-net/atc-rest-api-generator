﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator 2.0.265.35674.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace Demo.Api.Generated.Contracts.Users;

/// <summary>
/// Parameters for operation request.
/// Description: Get user by email.
/// Operation: GetUserByEmail.
/// </summary>
[GeneratedCode("ApiGenerator", "2.0.265.35674")]
public class GetUserByEmailParameters
{
    /// <summary>
    /// The email of the user to retrieve.
    /// </summary>
    /// <remarks>
    /// Email validation being enforced.
    /// </remarks>
    [FromQuery(Name = "email")]
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    /// <inheritdoc />
    public override string ToString()
        => $"{nameof(Email)}: {Email}";
}