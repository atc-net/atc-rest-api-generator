//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace DemoSample.Api.Generated.Contracts.Users;

/// <summary>
/// A list of users.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
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
