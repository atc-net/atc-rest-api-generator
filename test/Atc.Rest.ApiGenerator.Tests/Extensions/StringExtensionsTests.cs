namespace Atc.Rest.ApiGenerator.Tests.Extensions;

public class StringExtensionsTests
{
    [Fact]
    public void Should_Add_Newline_After_Method()
    {
        Assert.Equal(
            $"class partial Test {{ partial void Method();{Environment.NewLine}}}",
            "class partial Test { partial void Method();}".EnsureNewlineAfterMethod("partial void Method();"));
    }
}