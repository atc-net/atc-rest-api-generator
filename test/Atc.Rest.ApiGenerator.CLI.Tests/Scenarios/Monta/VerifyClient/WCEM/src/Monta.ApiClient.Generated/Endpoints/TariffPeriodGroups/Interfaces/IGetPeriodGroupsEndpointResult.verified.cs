﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace Monta.ApiClient.Generated.Endpoints.TariffPeriodGroups.Interfaces;

/// <summary>
/// Interface for Client Endpoint Result.
/// Description: Retrieve Tariff Period Groups by tariff id.
/// Operation: GetPeriodGroups.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public interface IGetPeriodGroupsEndpointResult : IEndpointResponse
{

    bool IsOk { get; }

    bool IsBadRequest { get; }

    bool IsUnauthorized { get; }

    bool IsForbidden { get; }

    bool IsNotFound { get; }

    IEnumerable<TariffPeriodGroup> OkContent { get; }

    string? BadRequestContent { get; }

    string? UnauthorizedContent { get; }

    string? ForbiddenContent { get; }

    string? NotFoundContent { get; }
}