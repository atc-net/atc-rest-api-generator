﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace MontaPartner.ApiClient.Generated.Contracts.PaymentTerminals;

/// <summary>
/// PaymentTerminal.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class PaymentTerminal
{
    /// <summary>
    /// Id of this terminal. **NOTE: This is a String**.
    /// </summary>
    [Required]
    public string Id { get; set; }

    [Required]
    public PaymentTerminalType Type { get; set; }

    /// <summary>
    /// The team this terminal belongs to.
    /// </summary>
    public long TeamId { get; set; }

    /// <summary>
    /// The charge points ids this terminal is assigned to.
    /// </summary>
    [Required]
    public List<long> ChargePointIds { get; set; }

    /// <summary>
    /// Serial number of the Payment Terminal.
    /// </summary>
    [Required]
    public string Serial { get; set; }

    /// <summary>
    /// Name given to the Payment Terminal, defaults to "No Name" if not set by the merchant.
    /// </summary>
    [Required]
    public string Name { get; set; }

    /// <summary>
    /// Indicates if the terminal is connected or disconnected.
    /// </summary>
    public bool IsConnected { get; set; }

    /// <summary>
    /// Date this payment terminal did connect.
    /// </summary>
    public DateTimeOffset? ConnectedAt { get; set; }

    /// <summary>
    /// Date this entity was created.
    /// </summary>
    [Required]
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// Date this entity was updated.
    /// </summary>
    public DateTimeOffset? UpdatedAt { get; set; }

    /// <summary>
    /// Date this entity was deleted.
    /// </summary>
    public DateTimeOffset? DeletedAt { get; set; }

    /// <inheritdoc />
    public override string ToString()
        => $"{nameof(Id)}: {Id}, {nameof(Type)}: ({Type}), {nameof(TeamId)}: {TeamId}, {nameof(ChargePointIds)}.Count: {ChargePointIds?.Count ?? 0}, {nameof(Serial)}: {Serial}, {nameof(Name)}: {Name}, {nameof(IsConnected)}: {IsConnected}, {nameof(ConnectedAt)}: ({ConnectedAt}), {nameof(CreatedAt)}: ({CreatedAt}), {nameof(UpdatedAt)}: ({UpdatedAt}), {nameof(DeletedAt)}: ({DeletedAt})";
}