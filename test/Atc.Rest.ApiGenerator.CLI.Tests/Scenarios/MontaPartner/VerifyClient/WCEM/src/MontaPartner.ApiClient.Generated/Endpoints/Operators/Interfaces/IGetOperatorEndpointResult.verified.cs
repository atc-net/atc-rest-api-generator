﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace MontaPartner.ApiClient.Generated.Endpoints.Operators.Interfaces;

/// <summary>
/// Interface for Client Endpoint Result.
/// Description: Retrieve an operator.
/// Operation: GetOperator.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public interface IGetOperatorEndpointResult : IEndpointResponse
{

    bool IsOk { get; }

    bool IsBadRequest { get; }

    bool IsUnauthorized { get; }

    bool IsForbidden { get; }

    Operator OkContent { get; }

    string? BadRequestContent { get; }

    string? UnauthorizedContent { get; }

    string? ForbiddenContent { get; }
}