﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace MontaPartner.ApiClient.Generated.Contracts.TeamMembers;

/// <summary>
/// Parameters for operation request.
/// Description: Resend an invite to a team member and reset expiry date.
/// Operation: ResendTeamMemberInvite.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class ResendTeamMemberInviteParameters
{
    [Required]
    public long Id { get; set; }

    /// <inheritdoc />
    public override string ToString()
        => $"{nameof(Id)}: {Id}";
}