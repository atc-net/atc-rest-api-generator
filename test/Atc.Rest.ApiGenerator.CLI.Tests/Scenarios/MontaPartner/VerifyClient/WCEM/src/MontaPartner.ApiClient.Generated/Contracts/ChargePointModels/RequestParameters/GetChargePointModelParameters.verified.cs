﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace MontaPartner.ApiClient.Generated.Contracts.ChargePointModels;

/// <summary>
/// Parameters for operation request.
/// Description: Retrieve a charge point model.
/// Operation: GetChargePointModel.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class GetChargePointModelParameters
{
    [Required]
    public long ChargePointModelId { get; set; }

    /// <inheritdoc />
    public override string ToString()
        => $"{nameof(ChargePointModelId)}: {ChargePointModelId}";
}