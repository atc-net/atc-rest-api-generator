﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace ExGenericPagination.ApiClient.Generated.Endpoints.Dogs.Interfaces;

/// <summary>
/// Interface for Client Endpoint Result.
/// Description: Find all dogs.
/// Operation: GetDogs.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public interface IGetDogsEndpointResult : IEndpointResponse
{

    bool IsOk { get; }

    bool IsBadRequest { get; }

    PaginatedResult<Dog> OkContent { get; }

    ValidationProblemDetails BadRequestContent { get; }
}