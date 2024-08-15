﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace DemoSample.Api.Generated.Contracts.Users;

/// <summary>
/// Parameters for operation request.
/// Description: Update user by id.
/// Operation: UpdateUserById.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class UpdateUserByIdParameters
{
    /// <summary>
    /// Id of the user.
    /// </summary>
    [FromRoute]
    [Required]
    public Guid Id { get; set; }

    /// <summary>
    /// Request to update a user.
    /// </summary>
    [FromBody]
    [Required]
    public UpdateUserRequest Request { get; set; }

    /// <inheritdoc />
    public override string ToString()
        => $"{nameof(Id)}: {Id}, {nameof(Request)}: ({Request})";
}