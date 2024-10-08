﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace ExAsyncEnumerable.ApiClient.Generated.Endpoints.Customers.Interfaces;

/// <summary>
/// Interface for Client Endpoint Result.
/// Description: Get customers.
/// Operation: GetCustomers.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public interface IGetCustomersEndpointResult : IEndpointResponse
{

    bool IsOk { get; }

    bool IsBadRequest { get; }

    IAsyncEnumerable<PaginationResult<Customer>> OkContent { get; }

    string? BadRequestContent { get; }
}