﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace MontaPartner.ApiClient.Generated.Contracts.ChargePointBrands;

/// <summary>
/// MontaPageChargePointBrandDto.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class MontaPageChargePointBrandDto
{
    [Required]
    public List<ChargePointBrand> Data { get; set; }

    [Required]
    public MontaPageMeta Meta { get; set; }

    /// <inheritdoc />
    public override string ToString()
        => $"{nameof(Data)}.Count: {Data?.Count ?? 0}, {nameof(Meta)}: ({Meta})";
}