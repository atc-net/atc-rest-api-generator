﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace CodeStructure1.ApiClient.Generated.Users.MyEndpoints.Interfaces;

/// <summary>
/// Interface for Client Endpoint Result.
/// Description: Delete user by id.
/// Operation: DeleteUserById.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public interface IDeleteUserByIdEndpointResult : IEndpointResponse
{

    bool IsOk { get; }

    bool IsBadRequest { get; }

    bool IsNotFound { get; }

    bool IsConflict { get; }

    string? OkContent { get; }

    string? BadRequestContent { get; }

    string? NotFoundContent { get; }

    string? ConflictContent { get; }
}