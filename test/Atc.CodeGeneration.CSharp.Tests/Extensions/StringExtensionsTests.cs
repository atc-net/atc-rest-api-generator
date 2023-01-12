// ReSharper disable StringLiteralTypo
namespace Atc.CodeGeneration.CSharp.Tests.Extensions;

public class StringExtensionsTests
{
    [Theory]
    [InlineData("A", "A")]
    [InlineData("A", "a")]
    [InlineData("PostalCode", "PostalCode")]
    [InlineData("PostalCode", "postalCode")]
    [InlineData("PostalCode", "Postal-Code")]
    [InlineData("PostalCode", "postal-Code")]
    [InlineData("CorrelationId", "X-correlation-id")]
    [InlineData("CorrelationId", "x-correlation-id")]
    [InlineData("Correlationid", "x-correlationid")]
    public void EnsureValidFormattedPropertyName(
        string expected,
        string name)
    {
        // Atc
        var actual = name.EnsureValidFormattedPropertyName();

        // Assert
        Assert.Equal(expected, actual);
    }
}