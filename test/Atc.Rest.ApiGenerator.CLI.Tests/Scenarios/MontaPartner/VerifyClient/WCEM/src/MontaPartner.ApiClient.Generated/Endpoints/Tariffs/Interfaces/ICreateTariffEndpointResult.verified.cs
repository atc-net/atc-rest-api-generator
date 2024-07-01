﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace MontaPartner.ApiClient.Generated.Endpoints.Tariffs.Interfaces;

/// <summary>
/// Interface for Client Endpoint Result.
/// Description: Create a new Tariff.
/// Operation: CreateTariff.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public interface ICreateTariffEndpointResult : IEndpointResponse
{

    bool IsCreated { get; }

    bool IsBadRequest { get; }

    bool IsUnauthorized { get; }

    string? CreatedContent { get; }

    string? BadRequestContent { get; }

    string? UnauthorizedContent { get; }
}