﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace CodeStructure1.Api.Tests.Users.MyEndpoints;

[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class DeleteUserByIdHandlerStub : IDeleteUserByIdHandler
{
    public Task<DeleteUserByIdResult> ExecuteAsync(
        DeleteUserByIdParameters parameters,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(DeleteUserByIdResult.Ok("Hallo world"));
    }
}