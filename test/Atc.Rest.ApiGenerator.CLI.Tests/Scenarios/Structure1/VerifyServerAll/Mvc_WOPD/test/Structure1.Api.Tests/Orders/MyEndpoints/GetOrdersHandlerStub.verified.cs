﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace Structure1.Api.Tests.Orders.MyEndpoints;

[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class GetOrdersHandlerStub : IGetOrdersHandler
{
    public Task<GetOrdersResult> ExecuteAsync(
        GetOrdersParameters parameters,
        CancellationToken cancellationToken = default)
    {
        var data = new Fixture().Create<List<Order>>();

        var paginationData = new Pagination<Order>(
            data,
            parameters.PageSize,
            parameters.QueryString,
            parameters.ContinuationToken);

        return Task.FromResult(GetOrdersResult.Ok(paginationData));
    }
}