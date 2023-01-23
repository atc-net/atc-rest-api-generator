namespace Atc.Rest.ApiGenerator.Framework.Contracts.Tests.Resolver;

public class OpenApiDocumentSchemaModelNameResolverTests
{
    [Theory]
    [InlineData("MyData", "MyData")]
    [InlineData("MyData", "List<MyData>")]
    [InlineData("MyData", "Hallo world List<MyData> HalloFoo")]
    [InlineData("MyData Foo", "MyData Foo")]
    [InlineData("MyDataListFoo", "MyDataListFoo")]
    [InlineData("MyData", "MtContract.MyData")]
    [InlineData("MyData", "List<MtContract.MyData>")]
    public void GetRawModelName(string expected, string input)
        => Assert.Equal(expected, OpenApiDocumentSchemaModelNameResolver.GetRawModelName(input));
}