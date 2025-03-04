﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace ExNsWithTask.Api.Tests.Endpoints.TestUnits;

[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class ListTestUnitsHandlerStub : IListTestUnitsHandler
{
    public Task<ListTestUnitsResult> ExecuteAsync(
        ListTestUnitsParameters parameters,
        CancellationToken cancellationToken = default)
    {
        var data = new Fixture().Create<List<TestUnit>>();

        var paginationData = new PaginationResult<TestUnit>
        {
            PageSize = 10,
            Results = data,
        };

        return Task.FromResult(ListTestUnitsResult.Ok(paginationData));
    }
}