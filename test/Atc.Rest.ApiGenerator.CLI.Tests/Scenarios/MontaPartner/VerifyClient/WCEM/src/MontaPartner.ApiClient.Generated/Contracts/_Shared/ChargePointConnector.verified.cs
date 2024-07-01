﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace MontaPartner.ApiClient.Generated.Contracts;

/// <summary>
/// ChargePointConnector.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class ChargePointConnector
{
    /// <summary>
    /// Id of this connector.
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Identifier of connector.
    /// </summary>
    [Required]
    public string Identifier { get; set; }

    /// <summary>
    /// Readable name of connector.
    /// </summary>
    [Required]
    public string Name { get; set; }

    /// <inheritdoc />
    public override string ToString()
        => $"{nameof(Id)}: {Id}, {nameof(Identifier)}: {Identifier}, {nameof(Name)}: {Name}";
}