﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace MontaPartner.ApiClient.Generated.Contracts.Teams;

/// <summary>
/// Parameters for operation request.
/// Description: Patch an existing team.
/// Operation: PatchTeam.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class PatchTeamParameters
{
    [Required]
    public long TeamId { get; set; }

    [Required]
    public PatchTeam Request { get; set; }

    /// <inheritdoc />
    public override string ToString()
        => $"{nameof(TeamId)}: {TeamId}, {nameof(Request)}: ({Request})";
}