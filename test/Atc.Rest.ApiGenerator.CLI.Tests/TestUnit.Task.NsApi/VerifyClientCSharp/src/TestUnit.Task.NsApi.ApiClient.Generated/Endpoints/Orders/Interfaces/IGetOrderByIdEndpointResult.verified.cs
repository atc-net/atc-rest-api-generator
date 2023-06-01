﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace TestUnit.Task.NsApi.ApiClient.Generated.Endpoints;

/// <summary>
/// Interface for Client Endpoint Result.
/// Description: Get order by id.
/// Operation: GetOrderById.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public interface IGetOrderByIdEndpointResult : IEndpointResponse
{
    bool IsOk { get; }

    bool IsBadRequest { get; }

    bool IsUnauthorized { get; }

    bool IsForbidden { get; }

    bool IsNotFound { get; }

    bool IsInternalServerError { get; }

    Order OkContent { get; }

    ValidationProblemDetails BadRequestContent { get; }

    string NotFoundContent { get; }

    string InternalServerErrorContent { get; }
}