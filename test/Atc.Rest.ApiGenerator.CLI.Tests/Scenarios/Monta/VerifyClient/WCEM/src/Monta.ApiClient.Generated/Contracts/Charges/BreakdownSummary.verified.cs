﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace Monta.ApiClient.Generated.Contracts.Charges;

/// <summary>
/// BreakdownSummary.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class BreakdownSummary
{
    /// <summary>
    /// Total master price for the given charge.
    /// </summary>
    [Required]
    public long TotalMasterPrice { get; set; }

    /// <summary>
    /// Total secondary price for the given charge.
    /// </summary>
    [Required]
    public long TotalSecondaryPrice { get; set; }

    /// <summary>
    /// Total fees for the given charge.
    /// </summary>
    [Required]
    public long TotalFees { get; set; }

    /// <summary>
    /// Total adjustments for a given charge, for instance, based on certain business rules, the final price may be adjusted to prevent it from exceeding 1000 EUR.
    /// </summary>
    [Required]
    public long TotalAdjustments { get; set; }

    /// <summary>
    /// Total price for the given charge.
    /// </summary>
    [Required]
    public long TotalPrice { get; set; }

    /// <inheritdoc />
    public override string ToString()
        => $"{nameof(TotalMasterPrice)}: {TotalMasterPrice}, {nameof(TotalSecondaryPrice)}: {TotalSecondaryPrice}, {nameof(TotalFees)}: {TotalFees}, {nameof(TotalAdjustments)}: {TotalAdjustments}, {nameof(TotalPrice)}: {TotalPrice}";
}