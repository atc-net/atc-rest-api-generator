//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace Scenario2.Api.Generated.Contracts.Accounts;

/// <summary>
/// Parameters for operation request.
/// Description: Update name of account.
/// Operation: UpdateAccountName.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class UpdateAccountNameParameters
{
    /// <summary>
    /// The accountId.
    /// </summary>
    [FromRoute(Name = "accountId")]
    [Required]
    public Guid AccountId { get; set; }

    /// <summary>
    /// The account name.
    /// </summary>
    [FromHeader(Name = "name")]
    public string Name { get; set; }

    /// <inheritdoc />
    public override string ToString()
        => $"{nameof(AccountId)}: {AccountId}, {nameof(Name)}: {Name}";
}