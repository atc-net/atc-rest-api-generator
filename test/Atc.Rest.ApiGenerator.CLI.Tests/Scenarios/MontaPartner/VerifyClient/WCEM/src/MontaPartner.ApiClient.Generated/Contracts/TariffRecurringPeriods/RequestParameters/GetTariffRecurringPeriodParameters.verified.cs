﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace MontaPartner.ApiClient.Generated.Contracts.TariffRecurringPeriods;

/// <summary>
/// Parameters for operation request.
/// Description: Retrieve a tariff recurring period.
/// Operation: GetTariffRecurringPeriod.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class GetTariffRecurringPeriodParameters
{
    [Required]
    public long PeriodId { get; set; }

    /// <inheritdoc />
    public override string ToString()
        => $"{nameof(PeriodId)}: {PeriodId}";
}