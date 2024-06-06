namespace DemoSample.Api.Tests.Endpoints.Users;

[Collection("Sequential-Endpoints")]
[Trait(Traits.Category, Traits.Categories.Integration)]
public class GetUsersTests : WebApiControllerBaseTest
{
    public GetUsersTests(WebApiStartupFactory fixture)
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