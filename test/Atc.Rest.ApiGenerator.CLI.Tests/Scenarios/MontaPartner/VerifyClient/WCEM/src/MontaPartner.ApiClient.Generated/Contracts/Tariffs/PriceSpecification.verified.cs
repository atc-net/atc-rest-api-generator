﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace MontaPartner.ApiClient.Generated.Contracts.Tariffs;

/// <summary>
/// PriceSpecification.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class PriceSpecification
{
    /// <summary>
    /// price specifications.
    /// </summary>
    [Required]
    public List<PairStringBigDecimal> Specifications { get; set; }

    /// <inheritdoc />
    public override string ToString()
        => $"{nameof(Specifications)}.Count: {Specifications?.Count ?? 0}";
}