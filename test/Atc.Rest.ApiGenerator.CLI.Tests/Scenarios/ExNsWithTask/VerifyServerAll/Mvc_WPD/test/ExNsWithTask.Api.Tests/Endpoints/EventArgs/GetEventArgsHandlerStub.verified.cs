﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace ExNsWithTask.Api.Tests.Endpoints.EventArgs;

[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class GetEventArgsHandlerStub : IGetEventArgsHandler
{
    public Task<GetEventArgsResult> ExecuteAsync(
        CancellationToken cancellationToken = default)
    {
        var data = new Fixture().Create<List<Generated.Contracts.EventArgs.EventArgs>>();

        return Task.FromResult(GetEventArgsResult.Ok(data));
    }
}