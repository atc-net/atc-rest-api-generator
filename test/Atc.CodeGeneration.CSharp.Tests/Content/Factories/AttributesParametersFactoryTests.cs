namespace Atc.CodeGeneration.CSharp.Tests.Content.Factories;

public class AttributesParametersFactoryTests
{
    [Theory]
    [InlineAutoNSubstituteData]
    public void Create(string name)
    {
        // Act
        var actual = AttributesParametersFactory.Create(name);

        // Assert
        Assert.NotNull(actual);
        Assert.Equal(1, actual.Count);
        Assert.Equal(name, actual[0].Name);
        Assert.Null(actual[0].Content);
    }

    [Theory]
    [InlineAutoNSubstituteData]
    public void CreateWithContent(string name, string content)
    {
        // Act
        var actual = AttributesParametersFactory.Create(name, content);

        // Assert
        Assert.NotNull(actual);
        Assert.Equal(1, actual.Count);
        Assert.Equal(name, actual[0].Name);
        Assert.Equal(content, actual[0].Content);
    }
}