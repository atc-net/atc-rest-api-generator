﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator 2.0.259.29354.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace Demo.Api.Tests.Endpoints.Users.Generated
{
    [GeneratedCode("ApiGenerator", "2.0.259.29354")]
    public class UpdateMyTestGenderHandlerStub : IUpdateMyTestGenderHandler
    {
        public Task<UpdateMyTestGenderResult> ExecuteAsync(UpdateMyTestGenderParameters parameters, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(UpdateMyTestGenderResult.Ok("Hallo world"));
        }
    }
}