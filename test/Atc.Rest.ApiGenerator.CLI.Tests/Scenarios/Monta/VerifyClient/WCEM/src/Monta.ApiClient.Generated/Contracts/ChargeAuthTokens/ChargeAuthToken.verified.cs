﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace Monta.ApiClient.Generated.Contracts.ChargeAuthTokens;

/// <summary>
/// ChargeAuthToken.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class ChargeAuthToken
{
    /// <summary>
    /// The id of the charge auth token.
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// The identifier of the charge auth token, Note: without prefix e.g `VID:`.
    /// </summary>
    [Required]
    public string Identifier { get; set; }

    /// <summary>
    /// The method type used for this charge auth token.
    /// </summary>
    [Required]
    public string Type { get; set; }

    /// <summary>
    /// Id of the team that the charge auth token belongs to.
    /// </summary>
    public long TeamId { get; set; }

    /// <summary>
    /// Id of the user that the charge auth token is associated to.
    /// </summary>
    public long? UserId { get; set; }

    /// <summary>
    /// External Id of this entity, managed by you.
    /// </summary>
    public string? PartnerExternalId { get; set; }

    /// <summary>
    /// Custom JSON payload for this entity, managed by you.
    /// </summary>
    public List<object>? PartnerCustomPayload { get; set; } = new List<object>();

    /// <summary>
    /// Name of the charge auth token.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Operator of this charge auth token.
    /// </summary>
    public Operator? Operator { get; set; }

    /// <summary>
    /// If the charge auth token is active in the Monta network.
    /// </summary>
    public bool MontaNetwork { get; set; }

    /// <summary>
    /// If the charge auth token is active in the Roaming network.
    /// </summary>
    public bool RoamingNetwork { get; set; }

    /// <summary>
    /// Allow this charge auth token to be used on other teams charge points under the same operator.
    /// </summary>
    public bool OperatorNetwork { get; set; }

    /// <summary>
    /// Indicates until when this charge auth token is active, null means indefinitely.
    /// </summary>
    public DateTimeOffset? ActiveUntil { get; set; }

    /// <summary>
    /// If the charge auth token is blocked, it will not be able to charge.
    /// * `null` = not blocked
    /// * date-time = blocked since date-time.
    /// </summary>
    public DateTimeOffset? BlockedAt { get; set; }

    /// <summary>
    /// Creation date of this charge auth token.
    /// </summary>
    [Required]
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// Update date of this charge auth token.
    /// </summary>
    [Required]
    public DateTimeOffset UpdatedAt { get; set; }

    /// <inheritdoc />
    public override string ToString()
        => $"{nameof(Id)}: {Id}, {nameof(Identifier)}: {Identifier}, {nameof(Type)}: {Type}, {nameof(TeamId)}: {TeamId}, {nameof(UserId)}: {UserId}, {nameof(PartnerExternalId)}: {PartnerExternalId}, {nameof(PartnerCustomPayload)}.Count: {PartnerCustomPayload?.Count ?? 0}, {nameof(Name)}: {Name}, {nameof(Operator)}: ({Operator}), {nameof(MontaNetwork)}: {MontaNetwork}, {nameof(RoamingNetwork)}: {RoamingNetwork}, {nameof(OperatorNetwork)}: {OperatorNetwork}, {nameof(ActiveUntil)}: ({ActiveUntil}), {nameof(BlockedAt)}: ({BlockedAt}), {nameof(CreatedAt)}: ({CreatedAt}), {nameof(UpdatedAt)}: ({UpdatedAt})";
}