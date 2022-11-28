namespace Atc.Rest.ApiGenerator.OpenApi.Tests.Factories;

public class LogItemFactoryTests
{
    [Fact]
    public void Create()
    {
        // Arrange
        var sut = new LogItemFactory();

        // Act
        var actual = sut.Create(LogCategoryType.Error, ValidationRuleNameConstants.Operation04, "Hallo world");

        // Assert
        Assert.NotNull(actual);
        Assert.Equal("Key: CR0204, Value: Operation, LogCategory: Error, Description: Hallo world", actual.ToString());
    }
}