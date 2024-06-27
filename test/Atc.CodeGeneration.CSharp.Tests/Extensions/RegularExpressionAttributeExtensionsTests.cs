namespace Atc.CodeGeneration.CSharp.Tests.Extensions;

public class RegularExpressionAttributeExtensionsTests
{
    [Theory]
    [InlineData("\"^[a-zA-Z0-9]*$\"", "^[a-zA-Z0-9]*$")]
    [InlineData(@"""^\\d+\\.\\d+\\.\\d+\\.\\d+$""", @"^\d+\.\d+\.\d+\.\d+$")]
    [InlineData(@"""^(-?\\d*\\.\\d+|\\d+\\.\\d*)(,)(-?\\d*\\.\\d+|\\d+\\.\\d*)$""", @"^(-?\\d*\\.\\d+|\\d+\\.\\d*)(,)(-?\\d*\\.\\d+|\\d+\\.\\d*)$")]
    public void GetEscapedPattern(string expected, string input)
    {
        // Arrange
        var regularExpressionAttribute = new RegularExpressionAttribute(input);

        // Act
        var actual = regularExpressionAttribute.GetEscapedPattern();

        // Assert
        Assert.Equal(expected, actual);
    }
}