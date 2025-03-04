﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace Monta.ApiClient.Generated.Endpoints.SponsoredChargePoints.Interfaces;

/// <summary>
/// Interface for Client Endpoint Result.
/// Description: Update a sponsored charge point.
/// Operation: PatchSponsoredChargePoint.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public interface IPatchSponsoredChargePointEndpointResult : IEndpointResponse
{

    bool IsOk { get; }

    bool IsBadRequest { get; }

    bool IsUnauthorized { get; }

    bool IsForbidden { get; }

    bool IsNotFound { get; }

    SponsoredChargePoint OkContent { get; }

    string? BadRequestContent { get; }

    string? UnauthorizedContent { get; }

    string? ForbiddenContent { get; }

    string? NotFoundContent { get; }
}