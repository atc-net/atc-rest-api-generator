﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace MontaPartner.ApiClient.Generated.Contracts.TeamMembers;

/// <summary>
/// Parameters for operation request.
/// Description: Delete an existing team member.
/// Operation: DeleteTeamMember.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class DeleteTeamMemberParameters
{
    [Required]
    public long TeamMemberId { get; set; }

    /// <inheritdoc />
    public override string ToString()
        => $"{nameof(TeamMemberId)}: {TeamMemberId}";
}