using Atc.Rest.ApiGenerator.Helpers;
using Xunit;

namespace Atc.Rest.ApiGenerator.Tests.Helpers
{
    public class OpenApiDocumentSchemaModelNameHelperTests
    {
        [Theory]
        [InlineData("MyData", "MyData")]
        [InlineData("MyData", "List<MyData>")]
        [InlineData("MyData", "Hallo world List<MyData> HalloFoo")]
        [InlineData("MyData Foo", "MyData Foo")]
        [InlineData("MyDataListFoo", "MyDataListFoo")]
        public void GetRawModelName(string expected, string input)
            => Assert.Equal(expected, OpenApiDocumentSchemaModelNameHelper.GetRawModelName(input));
    }
}