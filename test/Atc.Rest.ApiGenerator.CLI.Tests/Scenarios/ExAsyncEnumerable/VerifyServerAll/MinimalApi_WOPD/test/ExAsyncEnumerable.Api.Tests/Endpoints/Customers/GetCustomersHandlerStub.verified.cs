﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace ExAsyncEnumerable.Api.Tests.Endpoints.Customers;

[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class GetCustomersHandlerStub : IGetCustomersHandler
{
    public Task<GetCustomersResult> ExecuteAsync(
        GetCustomersParameters parameters,
        CancellationToken cancellationToken = default)
    {
        var data = new Fixture().Create<List<Customer>>();

        var paginationData = new PaginationResult<Customer>();

        return Task.FromResult(GetCustomersResult.Ok(paginationData));
    }
}