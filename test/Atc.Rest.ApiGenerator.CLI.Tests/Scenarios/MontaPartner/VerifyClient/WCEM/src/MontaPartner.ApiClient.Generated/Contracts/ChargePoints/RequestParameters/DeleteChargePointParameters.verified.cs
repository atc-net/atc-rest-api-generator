﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace MontaPartner.ApiClient.Generated.Contracts.ChargePoints;

/// <summary>
/// Parameters for operation request.
/// Description: Delete an existing charge point.
/// Operation: DeleteChargePoint.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class DeleteChargePointParameters
{
    [Required]
    public long ChargePointId { get; set; }

    /// <inheritdoc />
    public override string ToString()
        => $"{nameof(ChargePointId)}: {ChargePointId}";
}