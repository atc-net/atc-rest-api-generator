﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace MontaPartner.ApiClient.Generated.Endpoints.ChargePointConnectors.Interfaces;

/// <summary>
/// Interface for Client Endpoint Result.
/// Description: Retrieve a list of charge point connectors.
/// Operation: GetConnectors.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public interface IGetConnectorsEndpointResult : IEndpointResponse
{

    bool IsOk { get; }

    bool IsBadRequest { get; }

    bool IsUnauthorized { get; }

    MontaPageChargePointConnectorDto OkContent { get; }

    string? BadRequestContent { get; }

    string? UnauthorizedContent { get; }
}