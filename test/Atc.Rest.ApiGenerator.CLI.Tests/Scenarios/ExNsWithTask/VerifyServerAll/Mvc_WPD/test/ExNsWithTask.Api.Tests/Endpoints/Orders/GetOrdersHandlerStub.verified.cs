﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace ExNsWithTask.Api.Tests.Endpoints.Orders;

[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class GetOrdersHandlerStub : IGetOrdersHandler
{
    public Task<GetOrdersResult> ExecuteAsync(
        GetOrdersParameters parameters,
        CancellationToken cancellationToken = default)
    {
        var data = new Fixture().Create<List<Order>>();

        var paginationData = new PaginationResult<Order>
        {
            PageSize = 10,
            Results = data,
        };

        return Task.FromResult(GetOrdersResult.Ok(paginationData));
    }
}