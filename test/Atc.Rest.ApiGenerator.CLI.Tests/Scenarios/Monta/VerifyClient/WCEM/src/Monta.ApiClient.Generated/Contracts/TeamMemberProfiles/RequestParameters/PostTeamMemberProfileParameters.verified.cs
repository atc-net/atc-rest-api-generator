﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace Monta.ApiClient.Generated.Contracts.TeamMemberProfiles;

/// <summary>
/// Parameters for operation request.
/// Description: Create a team member profile.
/// Operation: PostTeamMemberProfile.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class PostTeamMemberProfileParameters
{
    [Required]
    public CreateTeamMemberProfile Request { get; set; }

    /// <inheritdoc />
    public override string ToString()
        => $"{nameof(Request)}: ({Request})";
}