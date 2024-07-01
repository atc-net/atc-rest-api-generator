﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace MontaPartner.ApiClient.Generated.Contracts;

/// <summary>
/// Consumer.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class Consumer
{
    /// <summary>
    /// A name for this credential (e.g. Monta Team A).
    /// </summary>
    [Required]
    public string Name { get; set; }

    /// <summary>
    /// Your operator id.
    /// </summary>
    public long OperatorId { get; set; }

    /// <summary>
    /// List of team ids that are unlocked for API operations. If empty, all teams of this operator are unlocked.
    /// </summary>
    [Required]
    public List<long> TeamIds { get; set; }

    /// <summary>
    /// Your client id.
    /// </summary>
    [Required]
    public string ClientId { get; set; }

    /// <summary>
    /// Rate-limit for your account; requests per `rateLimitIntervalInSeconds`.
    /// </summary>
    public long RateLimit { get; set; }

    /// <summary>
    /// Rate-limit interval for your `rateLimit`.
    /// </summary>
    public long RateLimitIntervalInSeconds { get; set; }

    /// <summary>
    /// List of scopes for this account.
    /// </summary>
    [Required]
    public List<string> Scopes { get; set; }

    /// <summary>
    /// Creation date of this consumer.
    /// </summary>
    [Required]
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// Update date of this consumer.
    /// </summary>
    public DateTimeOffset? UpdatedAt { get; set; }

    /// <inheritdoc />
    public override string ToString()
        => $"{nameof(Name)}: {Name}, {nameof(OperatorId)}: {OperatorId}, {nameof(TeamIds)}.Count: {TeamIds?.Count ?? 0}, {nameof(ClientId)}: {ClientId}, {nameof(RateLimit)}: {RateLimit}, {nameof(RateLimitIntervalInSeconds)}: {RateLimitIntervalInSeconds}, {nameof(Scopes)}.Count: {Scopes?.Count ?? 0}, {nameof(CreatedAt)}: ({CreatedAt}), {nameof(UpdatedAt)}: ({UpdatedAt})";
}