﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace Monta.ApiClient.Generated.Contracts.TeamMembers;

/// <summary>
/// Parameters for operation request.
/// Description: Create (invite) a team member.
/// Operation: PostTeamMember.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class PostTeamMemberParameters
{
    [Required]
    public CreateTeamMember Request { get; set; }

    /// <inheritdoc />
    public override string ToString()
        => $"{nameof(Request)}: ({Request})";
}