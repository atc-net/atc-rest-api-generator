﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace MontaPartner.ApiClient.Generated.Endpoints.Plans.Interfaces;

/// <summary>
/// Interface for Client Endpoint Result.
/// Description: Retrieve a plan.
/// Operation: GetPlan.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public interface IGetPlanEndpointResult : IEndpointResponse
{

    bool IsOk { get; }

    bool IsBadRequest { get; }

    bool IsUnauthorized { get; }

    Plan OkContent { get; }

    string? BadRequestContent { get; }

    string? UnauthorizedContent { get; }
}