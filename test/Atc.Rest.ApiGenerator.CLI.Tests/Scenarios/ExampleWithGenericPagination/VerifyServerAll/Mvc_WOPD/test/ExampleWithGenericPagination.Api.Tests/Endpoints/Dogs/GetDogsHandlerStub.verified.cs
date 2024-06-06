//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace ExampleWithGenericPagination.Api.Tests.Endpoints.Dogs;

[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class GetDogsHandlerStub : IGetDogsHandler
{
    public Task<GetDogsResult> ExecuteAsync(
        GetDogsParameters parameters,
        CancellationToken cancellationToken = default)
    {
        var data = new Fixture().Create<List<Dog>>();

        var paginationData = new PaginatedResult<Dog>();

        return Task.FromResult(GetDogsResult.Ok(paginationData));
    }
}
