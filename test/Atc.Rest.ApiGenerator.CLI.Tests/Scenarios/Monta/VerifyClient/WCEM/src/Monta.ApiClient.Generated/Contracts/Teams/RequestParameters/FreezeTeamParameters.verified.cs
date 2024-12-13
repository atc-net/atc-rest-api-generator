﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace Monta.ApiClient.Generated.Contracts.Teams;

/// <summary>
/// Parameters for operation request.
/// Description: Freeze a team.
/// Operation: FreezeTeam.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class FreezeTeamParameters
{
    [Required]
    public long Id { get; set; }

    [Required]
    public UpdateTeamStateDto Request { get; set; }

    /// <inheritdoc />
    public override string ToString()
        => $"{nameof(Id)}: {Id}, {nameof(Request)}: ({Request})";
}