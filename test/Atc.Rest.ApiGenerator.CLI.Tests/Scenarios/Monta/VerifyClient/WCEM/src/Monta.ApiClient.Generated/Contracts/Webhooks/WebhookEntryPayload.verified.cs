﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace Monta.ApiClient.Generated.Contracts.Webhooks;

/// <summary>
/// WebhookEntryPayload.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class WebhookEntryPayload
{
    /// <summary>
    /// Type of the entity.
    /// </summary>
    [Required]
    public WebhookEntryPayloadEntityType EntityType { get; set; }

    /// <summary>
    /// Id of the entity.
    /// </summary>
    [Required]
    public string EntityId { get; set; }

    /// <summary>
    /// Type of the event.
    /// </summary>
    [Required]
    public KafkaEventType EventType { get; set; }

    /// <summary>
    /// The timestamp indicating when the event was generated.
    /// </summary>
    public DateTimeOffset? EventTime { get; set; }

    /// <summary>
    /// payload of this entity, e.g. a full Charge object.
    /// </summary>
    public List<object>? Payload { get; set; } = new List<object>();

    /// <inheritdoc />
    public override string ToString()
        => $"{nameof(EntityType)}: ({EntityType}), {nameof(EntityId)}: {EntityId}, {nameof(EventType)}: ({EventType}), {nameof(EventTime)}: ({EventTime}), {nameof(Payload)}.Count: {Payload?.Count ?? 0}";
}