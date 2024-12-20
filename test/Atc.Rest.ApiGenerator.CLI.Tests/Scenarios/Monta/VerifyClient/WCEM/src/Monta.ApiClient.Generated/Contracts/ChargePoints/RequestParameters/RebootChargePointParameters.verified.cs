﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace Monta.ApiClient.Generated.Contracts.ChargePoints;

/// <summary>
/// Parameters for operation request.
/// Description: Reboot an existing charge point.
/// Operation: RebootChargePoint.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class RebootChargePointParameters
{
    [Required]
    public long ChargePointId { get; set; }

    /// <inheritdoc />
    public override string ToString()
        => $"{nameof(ChargePointId)}: {ChargePointId}";
}