﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace MontaPartner.ApiClient.Generated.Contracts.ChargePoints;

/// <summary>
/// Parameters for operation request.
/// Description: Patch an existing charge point.
/// Operation: PatchChargePoint.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class PatchChargePointParameters
{
    [Required]
    public long ChargePointId { get; set; }

    [Required]
    public PatchChargePoint Request { get; set; }

    /// <inheritdoc />
    public override string ToString()
        => $"{nameof(ChargePointId)}: {ChargePointId}, {nameof(Request)}: ({Request})";
}