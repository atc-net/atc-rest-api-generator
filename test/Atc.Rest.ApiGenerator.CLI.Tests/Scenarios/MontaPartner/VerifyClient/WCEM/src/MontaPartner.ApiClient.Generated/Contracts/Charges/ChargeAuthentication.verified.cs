﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace MontaPartner.ApiClient.Generated.Contracts.Charges;

/// <summary>
/// ChargeAuthentication.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class ChargeAuthentication
{
    /// <summary>
    /// The method type used to authenticate a charge.
    /// </summary>
    [Required]
    public string Type { get; set; }

    /// <summary>
    /// The id of the chosen authentication method.
    /// </summary>
    [Required]
    public string Id { get; set; }

    /// <summary>
    /// External Id of this entity, managed by you.
    /// </summary>
    public string? PartnerExternalId { get; set; }

    /// <inheritdoc />
    public override string ToString()
        => $"{nameof(Type)}: {Type}, {nameof(Id)}: {Id}, {nameof(PartnerExternalId)}: {PartnerExternalId}";
}