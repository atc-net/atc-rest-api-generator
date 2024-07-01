﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace MontaPartner.ApiClient.Generated.Endpoints.InstallerJobs.Interfaces;

/// <summary>
/// Interface for Client Endpoint Result.
/// Description: Retrieve a list of installer jobs.
/// Operation: GetInstallerJobs.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public interface IGetInstallerJobsEndpointResult : IEndpointResponse
{

    bool IsOk { get; }

    bool IsBadRequest { get; }

    bool IsUnauthorized { get; }

    MontaPageInstallerJobDto OkContent { get; }

    string? BadRequestContent { get; }

    string? UnauthorizedContent { get; }
}