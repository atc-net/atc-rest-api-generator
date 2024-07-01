﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace MontaPartner.ApiClient.Generated.Endpoints.CountryAreas.Interfaces;

/// <summary>
/// Interface for Client Endpoint Result.
/// Description: Retrieve a country area.
/// Operation: GetCountryArea.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public interface IGetCountryAreaEndpointResult : IEndpointResponse
{

    bool IsOk { get; }

    bool IsBadRequest { get; }

    bool IsUnauthorized { get; }

    bool IsNotFound { get; }

    CountryArea OkContent { get; }

    string? BadRequestContent { get; }

    string? UnauthorizedContent { get; }

    string? NotFoundContent { get; }
}