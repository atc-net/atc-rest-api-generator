﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace MontaPartner.ApiClient.Generated.Contracts.Insights;

/// <summary>
/// ChargesInsightDriverReportDtoConsumption.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class ChargesInsightDriverReportDtoConsumption
{
    /// <summary>
    /// Defines the type of charge, ie. if it was a sponsored or public charge.
    /// definitions:
    /// * `sponsored` - Charges that have been sponsored.
    /// * `team-operator` - Charges that belong to Charge Points of the same operator as paying team operator.
    /// * `public` - Any charge that was paid for by this team that does not match the other cases.
    /// Note that more chargeTypes might be added in the future. Make sure to handle this gracefully.
    /// .
    /// </summary>
    [Required]
    public string ChargeType { get; set; }

    /// <summary>
    /// Number of charging sessions.
    /// </summary>
    [Required]
    public long TotalSessions { get; set; }

    /// <summary>
    /// Sum of all Kwh consumed.
    /// </summary>
    [Required]
    public double TotalKwh { get; set; }

    /// <summary>
    /// Sum of all charge net prices.
    /// </summary>
    [Required]
    public double TotalNetPrice { get; set; }

    /// <summary>
    /// Sum of all charge vat amounts.
    /// </summary>
    [Required]
    public double TotalVat { get; set; }

    /// <summary>
    /// Sum of all charge gross prices.
    /// </summary>
    [Required]
    public double TotalGrossPrice { get; set; }

    /// <inheritdoc />
    public override string ToString()
        => $"{nameof(ChargeType)}: {ChargeType}, {nameof(TotalSessions)}: {TotalSessions}, {nameof(TotalKwh)}: {TotalKwh}, {nameof(TotalNetPrice)}: {TotalNetPrice}, {nameof(TotalVat)}: {TotalVat}, {nameof(TotalGrossPrice)}: {TotalGrossPrice}";
}