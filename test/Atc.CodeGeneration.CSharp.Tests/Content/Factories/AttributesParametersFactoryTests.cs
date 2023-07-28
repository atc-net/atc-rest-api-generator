namespace Atc.CodeGeneration.CSharp.Tests.Content.Factories;

public class AttributesParametersFactoryTests
{
    [Theory]
    [InlineAutoData]
    public void Create(string name)
    {
        // Act
        var actual = AttributesParametersFactory.Create(name);

        // Assert
        Assert.NotNull(actual);
        Assert.Single(actual);
        Assert.Equal(name, actual[0].Name);
        Assert.Null(actual[0].Content);
    }

    [Theory]
    [InlineAutoData]
    public void CreateWithContent(string name, string content)
    {
        // Act
        var actual = AttributesParametersFactory.Create(name, content);

        // Assert
        Assert.NotNull(actual);
        Assert.Single(actual);
        Assert.Equal(name, actual[0].Name);
        Assert.Equal(content, actual[0].Content);
    }
}