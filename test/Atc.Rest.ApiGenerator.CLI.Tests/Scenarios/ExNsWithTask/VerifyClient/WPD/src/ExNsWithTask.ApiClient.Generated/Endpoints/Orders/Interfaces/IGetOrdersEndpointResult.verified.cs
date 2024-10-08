﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace ExNsWithTask.ApiClient.Generated.Endpoints.Orders.Interfaces;

/// <summary>
/// Interface for Client Endpoint Result.
/// Description: Get orders.
/// Operation: GetOrders.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public interface IGetOrdersEndpointResult : IEndpointResponse
{

    bool IsOk { get; }

    bool IsBadRequest { get; }

    bool IsNotFound { get; }

    PaginationResult<Order> OkContent { get; }

    ValidationProblemDetails BadRequestContent { get; }

    string? NotFoundContent { get; }
}