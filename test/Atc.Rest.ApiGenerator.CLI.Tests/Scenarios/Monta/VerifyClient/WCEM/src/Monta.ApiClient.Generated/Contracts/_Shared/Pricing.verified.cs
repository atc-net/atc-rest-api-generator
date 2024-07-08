﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace Monta.ApiClient.Generated.Contracts;

/// <summary>
/// Pricing.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class Pricing
{
    /// <summary>
    /// Id of the pricing.
    /// </summary>
    [Required]
    [Range(0, 2147483647)]
    public long Id { get; set; }

    /// <summary>
    /// Name of the pricing. It will be null when it's the master price.
    /// </summary>
    public string? Description { get; set; }

    [Required]
    public PricingType Type { get; set; }

    /// <summary>
    /// If this is the master price (not a fee).
    /// </summary>
    [Required]
    public bool Master { get; set; }

    /// <summary>
    /// If it's a dynamic price. It will be true if a `tariffId` is present.
    /// </summary>
    [Required]
    public bool DynamicPricing { get; set; }

    /// <summary>
    /// Used by the Minute fee. True means it will stop charging the fee when the charge is complete. False means it will stop charging the fee when the cable is unplugged.
    /// </summary>
    [Required]
    public bool EndAtFullyCharged { get; set; }

    /// <summary>
    /// Used by Spot Price. True means it will add % of VAT on top the price calculations&lt;br /&gt;*Note*: `vat` rates differ from country to country.
    /// </summary>
    [Required]
    public bool Vat { get; set; }

    /// <summary>
    /// Used by Spot Price. It will multiply the fallback price by this percentage.
    /// </summary>
    public double? Percentage { get; set; } = 100.000;

    /// <summary>
    /// The id of the selected Tariff.
    /// </summary>
    public long? TariffId { get; set; }

    /// <summary>
    /// When the pricing was last updated.
    /// </summary>
    [Required]
    public DateTimeOffset UpdatedAt { get; set; }

    /// <summary>
    /// Used by Charging, Minute and Idle Fees. After how many minutes the fee should start being applied.
    /// </summary>
    public int? ApplyAfterMinutes { get; set; }

    [Required]
    public Money Price { get; set; }

    public Money? PriceMin { get; set; }

    public Money? PriceMax { get; set; }

    public Money? FeePriceMax { get; set; }

    /// <summary>
    /// Used by spot price. Additional absolute money or percentages values to be added on top of the previous calculations.
    /// </summary>
    public List<AdditionalPricing>? Additional { get; set; } = new List<AdditionalPricing>();

    /// <summary>
    /// DateTime "from" time to which this pricing should apply from.
    /// </summary>
    public DateTimeOffset? From { get; set; }

    /// <summary>
    /// DateTime "to" time to which this pricing should apply to.
    /// </summary>
    public DateTimeOffset? To { get; set; }

    /// <summary>
    /// The id of the charge pricing tag for this pricing.
    /// </summary>
    public long? TagId { get; set; }

    /// <inheritdoc />
    public override string ToString()
        => $"{nameof(Id)}: {Id}, {nameof(Description)}: {Description}, {nameof(Type)}: ({Type}), {nameof(Master)}: {Master}, {nameof(DynamicPricing)}: {DynamicPricing}, {nameof(EndAtFullyCharged)}: {EndAtFullyCharged}, {nameof(Vat)}: {Vat}, {nameof(Percentage)}: {Percentage}, {nameof(TariffId)}: {TariffId}, {nameof(UpdatedAt)}: ({UpdatedAt}), {nameof(ApplyAfterMinutes)}: {ApplyAfterMinutes}, {nameof(Price)}: ({Price}), {nameof(PriceMin)}: ({PriceMin}), {nameof(PriceMax)}: ({PriceMax}), {nameof(FeePriceMax)}: ({FeePriceMax}), {nameof(Additional)}.Count: {Additional?.Count ?? 0}, {nameof(From)}: ({From}), {nameof(To)}: ({To}), {nameof(TagId)}: {TagId}";
}