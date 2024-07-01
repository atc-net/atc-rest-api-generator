﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace MontaPartner.ApiClient.Generated.Contracts.SponsoredChargePoints;

/// <summary>
/// Parameters for operation request.
/// Description: Update a sponsored charge point.
/// Operation: PatchSponsoredChargePoint.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class PatchSponsoredChargePointParameters
{
    [Required]
    public long SponsoredChargePointId { get; set; }

    /// <summary>
    /// Note: Only use Optional for fields that can bet set null. Optional will insure jackson can differentiate between a field that was set to NULL X Field that was never present on the request.
    /// </summary>
    [Required]
    public PatchSponsoredChargePoint Request { get; set; }

    /// <inheritdoc />
    public override string ToString()
        => $"{nameof(SponsoredChargePointId)}: {SponsoredChargePointId}, {nameof(Request)}: ({Request})";
}