namespace ExampleWithAllRespTypes.Api.Tests.Endpoints.Example;

[Collection("Sequential-Endpoints")]
[Trait(Traits.Category, Traits.Categories.Integration)]
public class GetExampleTests : WebApiControllerBaseTest
{
    public GetExampleTests(WebApiStartupFactory fixture)
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