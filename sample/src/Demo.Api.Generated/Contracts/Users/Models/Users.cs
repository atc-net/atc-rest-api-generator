﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator 2.0.259.29354.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace Demo.Api.Generated.Contracts.Users;

/// <summary>
/// A list of users.
/// </summary>
[GeneratedCode("ApiGenerator", "2.0.259.29354")]
public class Users
{
    /// <summary>
    /// A list of User.
    /// </summary>
    public List<User> UserList { get; set; } = new List<User>();

    /// <inheritdoc />
    public override string ToString()
        => $"{nameof(UserList)}.Count: {UserList?.Count ?? 0}";
}