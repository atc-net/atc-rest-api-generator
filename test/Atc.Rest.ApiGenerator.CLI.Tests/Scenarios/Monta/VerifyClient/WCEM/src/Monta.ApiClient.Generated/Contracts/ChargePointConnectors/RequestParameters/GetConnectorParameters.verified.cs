﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace Monta.ApiClient.Generated.Contracts.ChargePointConnectors;

/// <summary>
/// Parameters for operation request.
/// Description: Retrieve a single charge point connector.
/// Operation: GetConnector.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class GetConnectorParameters
{
    [Required]
    public long ConnectorId { get; set; }

    /// <inheritdoc />
    public override string ToString()
        => $"{nameof(ConnectorId)}: {ConnectorId}";
}