namespace ExampleWithNsWithTask.Api.Tests.Endpoints.Orders;

[Collection("Sequential-Endpoints")]
[Trait(Traits.Category, Traits.Categories.Integration)]
public class GetOrdersTests : WebApiControllerBaseTest
{
    public GetOrdersTests(WebApiStartupFactory fixture)
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