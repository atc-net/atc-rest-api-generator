﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator 2.0.323.55388.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace Demo.Api.Tests.Endpoints.Files;

[GeneratedCode("ApiGenerator", "2.0.323.55388")]
public class GetFileByIdHandlerStub : IGetFileByIdHandler
{
    public Task<GetFileByIdResult> ExecuteAsync(
        GetFileByIdParameters parameters,
        CancellationToken cancellationToken = default)
    {
        var bytes = Encoding.UTF8.GetBytes("Hello World");
        return Task.FromResult(GetFileByIdResult.Ok(bytes, "dummy.txt"));
    }
}