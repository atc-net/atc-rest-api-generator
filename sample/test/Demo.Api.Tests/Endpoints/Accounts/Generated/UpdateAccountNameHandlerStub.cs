﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator 2.0.259.29354.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace Demo.Api.Tests.Endpoints.Accounts.Generated
{
    [GeneratedCode("ApiGenerator", "2.0.259.29354")]
    public class UpdateAccountNameHandlerStub : IUpdateAccountNameHandler
    {
        public Task<UpdateAccountNameResult> ExecuteAsync(UpdateAccountNameParameters parameters, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(UpdateAccountNameResult.Ok("Hallo world"));
        }
    }
}