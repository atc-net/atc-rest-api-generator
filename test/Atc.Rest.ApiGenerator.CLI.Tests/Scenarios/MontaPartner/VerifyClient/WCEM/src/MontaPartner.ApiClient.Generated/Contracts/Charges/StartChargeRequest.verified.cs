﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace MontaPartner.ApiClient.Generated.Contracts.Charges;

/// <summary>
/// StartChargeRequest.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class StartChargeRequest
{
    /// <summary>
    /// The `id` of the team that will be paying for the charge.
    /// </summary>
    [Required]
    public long PayingTeamId { get; set; }

    /// <summary>
    /// Id of the charge point used for this charge.
    /// </summary>
    [Required]
    public long ChargePointId { get; set; }

    /// <summary>
    /// If true, a charge point will be reserved and a charge object with state reserved will be returned.
    /// Use `restart` endpoint to start the charge.
    /// .
    /// </summary>
    public bool ReserveCharge { get; set; }

    /// <summary>
    /// The charge kWh limit, meaning the charge will stop once the limit kWh is reached. &lt;br /&gt;*Note*: `kwhLimit` can be set in parallel with `socLimit`, in this case the first limit to hit will stop the charge.
    /// </summary>
    public double? KwhLimit { get; set; }

    /// <summary>
    /// The charge SoC limit, meaning the charge will stop once the limit SoC % is reached. &lt;br /&gt;*Note*: `socLimit` can be set in parallel with `kwhLimit`, in this case the first limit to hit will stop the charge.
    /// </summary>
    public double? SocLimit { get; set; }

    /// <summary>
    /// Allows you to enforce a specific price group for this charge. &lt;br /&gt;*Note*: The price group must be of type `team` or `charge-point`.
    /// </summary>
    [Range(0, 2147483647)]
    public long? PriceGroupId { get; set; }

    public GenericPaymentSession? GenericPaymentSession { get; set; }

    /// <summary>
    /// External Id of this entity, managed by you.
    /// </summary>
    public string? PartnerExternalId { get; set; }

    /// <summary>
    /// Custom JSON payload for this entity, managed by you.
    /// </summary>
    public List<Object>? PartnerCustomPayload { get; set; } = new List<Object>();

    /// <inheritdoc />
    public override string ToString()
        => $"{nameof(PayingTeamId)}: {PayingTeamId}, {nameof(ChargePointId)}: {ChargePointId}, {nameof(ReserveCharge)}: {ReserveCharge}, {nameof(KwhLimit)}: {KwhLimit}, {nameof(SocLimit)}: {SocLimit}, {nameof(PriceGroupId)}: {PriceGroupId}, {nameof(GenericPaymentSession)}: ({GenericPaymentSession}), {nameof(PartnerExternalId)}: {PartnerExternalId}, {nameof(PartnerCustomPayload)}.Count: {PartnerCustomPayload?.Count ?? 0}";
}