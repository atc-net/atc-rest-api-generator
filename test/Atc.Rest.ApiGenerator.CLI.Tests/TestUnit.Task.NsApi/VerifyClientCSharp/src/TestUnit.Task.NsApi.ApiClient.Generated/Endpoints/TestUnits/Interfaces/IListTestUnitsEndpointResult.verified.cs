﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace TestUnit.Task.NsApi.ApiClient.Generated.Endpoints;

/// <summary>
/// Interface for Client Endpoint Result.
/// Description: List test units.
/// Operation: ListTestUnits.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public interface IListTestUnitsEndpointResult : IEndpointResponse
{
    bool IsOk { get; }

    bool IsBadRequest { get; }

    bool IsUnauthorized { get; }

    bool IsForbidden { get; }

    bool IsInternalServerError { get; }

    PaginationResult<Contracts.TestUnit> OkContent { get; }

    ValidationProblemDetails BadRequestContent { get; }

    string InternalServerErrorContent { get; }
}