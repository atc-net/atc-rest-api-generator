﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace CodeStructure1.ApiClient.Generated.Users.MyContracts;

/// <summary>
/// Parameters for operation request.
/// Description: Create a new user.
/// Operation: PostUser.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class PostUserParameters
{
    /// <summary>
    /// Request to create a user.
    /// </summary>
    [Required]
    public CreateUserRequest Request { get; set; }

    /// <inheritdoc />
    public override string ToString()
        => $"{nameof(Request)}: ({Request})";
}