﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace MontaPartner.ApiClient.Generated.Contracts.SponsoredChargePoints;

/// <summary>
/// Parameters for operation request.
/// Description: Delete an existing sponsored charge point.
/// Operation: DeleteSponsoredChargePoint.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class DeleteSponsoredChargePointParameters
{
    [Required]
    public long SponsoredChargePointId { get; set; }

    /// <inheritdoc />
    public override string ToString()
        => $"{nameof(SponsoredChargePointId)}: {SponsoredChargePointId}";
}