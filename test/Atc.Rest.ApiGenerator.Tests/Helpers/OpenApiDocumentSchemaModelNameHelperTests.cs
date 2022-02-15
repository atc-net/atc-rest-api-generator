namespace Atc.Rest.ApiGenerator.Tests.Helpers;

public class OpenApiDocumentSchemaModelNameHelperTests
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
        => Assert.Equal(expected, OpenApiDocumentSchemaModelNameHelper.GetRawModelName(input));
}