﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace Monta.ApiClient.Generated.Contracts.Prices;

/// <summary>
/// PricesForecast.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class PricesForecast
{
    [Required]
    public Operator Operator { get; set; }

    [Required]
    public Currency Currency { get; set; }

    [Required]
    public PriceGroup PriceGroup { get; set; }

    /// <summary>
    /// The compilation of entries composing this prices forecast.
    /// </summary>
    [Required]
    public List<PricesForecastEntry> Forecasts { get; set; }

    /// <inheritdoc />
    public override string ToString()
        => $"{nameof(Operator)}: ({Operator}), {nameof(Currency)}: ({Currency}), {nameof(PriceGroup)}: ({PriceGroup}), {nameof(Forecasts)}.Count: {Forecasts?.Count ?? 0}";
}