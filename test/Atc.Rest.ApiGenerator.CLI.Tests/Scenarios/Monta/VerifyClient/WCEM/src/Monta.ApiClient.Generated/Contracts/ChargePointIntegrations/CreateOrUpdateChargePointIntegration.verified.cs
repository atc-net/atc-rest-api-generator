﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace Monta.ApiClient.Generated.Contracts.ChargePointIntegrations;

/// <summary>
/// CreateOrUpdateChargePointIntegration.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class CreateOrUpdateChargePointIntegration
{
    /// <summary>
    /// Id of the charge point to setup the integration.
    /// </summary>
    [Required]
    [Range(0, int.MaxValue)]
    public long ChargePointId { get; set; }

    /// <summary>
    /// Id of the charge point model to be integrated.
    /// </summary>
    [Required]
    [Range(0, int.MaxValue)]
    public long ChargePointModelId { get; set; }

    /// <summary>
    /// The serial number for the charge point integration.
    /// </summary>
    [Required]
    [MinLength(1)]
    public string SerialNumber { get; set; }

    /// <summary>
    /// Connector id for the charge point integration. when `null` the `connectorId` will default to `1`.
    /// </summary>
    [Range(0, int.MaxValue)]
    public int? ConnectorId { get; set; }

    /// <summary>
    /// Indicates that an attempt to connect the charge point should happen, when `autoConnect` is set to true the Charge Point needs to point to Monta (via OCPP websocket)&lt;br /&gt;&lt;br /&gt;Note: when false the integration will be create but the state will be to `pending`.
    /// </summary>
    public bool? AutoConnect { get; set; } = true;

    /// <inheritdoc />
    public override string ToString()
        => $"{nameof(ChargePointId)}: {ChargePointId}, {nameof(ChargePointModelId)}: {ChargePointModelId}, {nameof(SerialNumber)}: {SerialNumber}, {nameof(ConnectorId)}: {ConnectorId}, {nameof(AutoConnect)}: {AutoConnect}";
}