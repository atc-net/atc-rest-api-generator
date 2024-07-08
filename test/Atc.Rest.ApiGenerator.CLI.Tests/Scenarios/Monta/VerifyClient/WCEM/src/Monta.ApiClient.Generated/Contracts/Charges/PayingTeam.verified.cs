﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace Monta.ApiClient.Generated.Contracts.Charges;

/// <summary>
/// PayingTeam.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class PayingTeam
{
    /// <summary>
    /// Id of the paying team.
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Public name of the team.
    /// </summary>
    [Required]
    public string PublicName { get; set; }

    /// <summary>
    /// Id of the operator the paying team belongs to.
    /// </summary>
    public long OperatorId { get; set; }

    /// <inheritdoc />
    public override string ToString()
        => $"{nameof(Id)}: {Id}, {nameof(PublicName)}: {PublicName}, {nameof(OperatorId)}: {OperatorId}";
}