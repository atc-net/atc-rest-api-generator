namespace Structure1.Api.Tests.RouteWithDash.MyEndpoints;

[Collection("Sequential-Endpoints")]
[Trait(Traits.Category, Traits.Categories.Integration)]
public class GetRouteWithDashTests : WebApiControllerBaseTest
{
    public GetRouteWithDashTests(WebApiStartupFactory fixture)
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