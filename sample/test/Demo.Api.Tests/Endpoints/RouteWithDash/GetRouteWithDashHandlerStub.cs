﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator 2.0.324.48157.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace Demo.Api.Tests.Endpoints.RouteWithDash;

[GeneratedCode("ApiGenerator", "2.0.324.48157")]
public class GetRouteWithDashHandlerStub : IGetRouteWithDashHandler
{
    public Task<GetRouteWithDashResult> ExecuteAsync(
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(GetRouteWithDashResult.Ok("Hallo world"));
    }
}