﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace Monta.ApiClient.Generated.Contracts.NestedTeams;

/// <summary>
/// Parameters for operation request.
/// Description: Accept a nested team invitation.
/// Operation: AcceptInvite.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class AcceptInviteParameters
{
    [Required]
    [Range(0, 2147483647)]
    public long RelationId { get; set; }

    /// <inheritdoc />
    public override string ToString()
        => $"{nameof(RelationId)}: {RelationId}";
}