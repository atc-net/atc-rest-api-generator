﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace MontaPartner.ApiClient.Generated.Contracts.ChargePointModels;

/// <summary>
/// ChargePointModelDtoChargePointBrandDto.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class ChargePointModelDtoChargePointBrandDto
{
    /// <summary>
    /// The id of the charge point brand.
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// The name of the charge point brand.
    /// </summary>
    [Required]
    public string Name { get; set; }

    /// <inheritdoc />
    public override string ToString()
        => $"{nameof(Id)}: {Id}, {nameof(Name)}: {Name}";
}