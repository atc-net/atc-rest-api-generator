﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace DemoSampleApi.Api.Tests.Endpoints.Users;

[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class GetUserByIdHandlerStub : IGetUserByIdHandler
{
    public Task<GetUserByIdResult> ExecuteAsync(
        GetUserByIdParameters parameters,
        CancellationToken cancellationToken = default)
    {
        var data = new Fixture().Create<User>();

        return Task.FromResult(GetUserByIdResult.Ok(data));
    }
}