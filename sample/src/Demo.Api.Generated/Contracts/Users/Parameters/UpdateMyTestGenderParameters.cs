﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator 2.0.259.29354.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace Demo.Api.Generated.Contracts.Users;

/// <summary>
/// Parameters for operation request.
/// Description: Update gender on a user.
/// Operation: UpdateMyTestGender.
/// </summary>
[GeneratedCode("ApiGenerator", "2.0.259.29354")]
public class UpdateMyTestGenderParameters
{
    /// <summary>
    /// Id of the user.
    /// </summary>
    [FromRoute(Name = "id")]
    [Required]
    public Guid Id { get; set; }

    /// <summary>
    /// The gender to set on the user.
    /// </summary>
    [FromQuery(Name = "genderParam")]
    public GenderType? GenderParam { get; set; }

    /// <summary>
    /// Update test-gender Request.
    /// </summary>
    [FromBody]
    [Required]
    public UpdateTestGenderRequest Request { get; set; }

    /// <inheritdoc />
    public override string ToString()
        => $"{nameof(Id)}: {Id}, {nameof(GenderParam)}: {GenderParam}, {nameof(Request)}: ({Request})";
}