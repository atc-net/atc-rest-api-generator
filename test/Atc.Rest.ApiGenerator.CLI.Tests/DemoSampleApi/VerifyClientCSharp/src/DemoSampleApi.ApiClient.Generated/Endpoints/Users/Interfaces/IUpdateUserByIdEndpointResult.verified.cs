﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace DemoSampleApi.ApiClient.Generated.Endpoints;

/// <summary>
/// Interface for Client Endpoint Result.
/// Description: Update user by id.
/// Operation: UpdateUserById.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public interface IUpdateUserByIdEndpointResult : IEndpointResponse
{
    bool IsOk { get; }

    bool IsBadRequest { get; }

    bool IsUnauthorized { get; }

    bool IsForbidden { get; }

    bool IsNotFound { get; }

    bool IsConflict { get; }

    bool IsInternalServerError { get; }

    string OkContent { get; }

    ValidationProblemDetails BadRequestContent { get; }

    string NotFoundContent { get; }

    string ConflictContent { get; }

    string InternalServerErrorContent { get; }
}