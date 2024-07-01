﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace MontaPartner.ApiClient.Generated.Endpoints.InstallerJobs.Interfaces;

/// <summary>
/// Interface for Client Endpoint Result.
/// Description: Delete an existing installer job.
/// Operation: DeleteInstallerJob.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public interface IDeleteInstallerJobEndpointResult : IEndpointResponse
{

    bool IsNoContent { get; }

    bool IsBadRequest { get; }

    bool IsUnauthorized { get; }

    bool IsForbidden { get; }

    string? NoContentContent { get; }

    string? BadRequestContent { get; }

    string? UnauthorizedContent { get; }

    string? ForbiddenContent { get; }
}