﻿using System.CodeDom.Compiler;
using System.Threading;
using System.Threading.Tasks;
using Demo.Api.Generated.Contracts.Items;

//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator 1.1.124.0.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace Demo.Api.Tests.Endpoints.Items.Generated
{
    [GeneratedCode("ApiGenerator", "1.1.124.0")]
    public class CreateItemHandlerStub : ICreateItemHandler
    {
        public Task<CreateItemResult> ExecuteAsync(CreateItemParameters parameters, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(CreateItemResult.Ok("Hallo world"));
        }
    }
}