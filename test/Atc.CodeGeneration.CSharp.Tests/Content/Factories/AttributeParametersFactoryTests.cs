namespace Atc.CodeGeneration.CSharp.Tests.Content.Factories;

public class AttributeParametersFactoryTests
{
    [Theory]
    [InlineAutoData]
    public void Create(string name)
    {
        // Act
        var actual = AttributeParametersFactory.Create(name);

        // Assert
        Assert.NotNull(actual);
        Assert.Equal(name, actual.Name);
        Assert.Null(actual.Content);
    }

    [Theory]
    [InlineAutoData]
    public void CreateWithContent(string name, string content)
    {
        // Act
        var actual = AttributeParametersFactory.Create(name, content);

        // Assert
        Assert.NotNull(actual);
        Assert.Equal(name, actual.Name);
        Assert.Equal(content, actual.Content);
    }
}