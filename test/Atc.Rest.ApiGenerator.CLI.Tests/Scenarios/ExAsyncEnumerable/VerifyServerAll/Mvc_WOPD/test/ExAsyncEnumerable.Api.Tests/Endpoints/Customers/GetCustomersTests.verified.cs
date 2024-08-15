namespace ExAsyncEnumerable.Api.Tests.Endpoints.Customers;

[Collection("Sequential-Endpoints")]
[Trait(Traits.Category, Traits.Categories.Integration)]
public class GetCustomersTests : WebApiControllerBaseTest
{
    public GetCustomersTests(WebApiStartupFactory fixture)
        : base(fixture)
    {
    }

    [Fact(Skip = "Change this to a real integration-test")]
    public void Sample()
    {
        // Arrange

        // Act

        // Assert
    }
}