﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace MontaPartner.ApiClient.Generated.Contracts.Charges;

/// <summary>
/// Parameters for operation request.
/// Description: Stop a charge.
/// Operation: StopCharge.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class StopChargeParameters
{
    [Required]
    public long ChargeId { get; set; }

    /// <inheritdoc />
    public override string ToString()
        => $"{nameof(ChargeId)}: {ChargeId}";
}