namespace Atc.CodeGeneration.CSharp.Tests.Content.Factories;

public class EnumValueParametersFactoryTests
{
    [Theory]
    [InlineAutoData]
    public void Create(string name)
    {
        // Act
        var actual = EnumValueParametersFactory.Create(name);

        // Assert
        Assert.NotNull(actual);
        Assert.Null(actual.DocumentationTags);
        Assert.Null(actual.DescriptionAttribute);
        Assert.Equal(name, actual.Name);
        Assert.Null(actual.Value);
    }

    [Theory]
    [InlineAutoData]
    public void CreateWithValue(string name, int value)
    {
        // Act
        var actual = EnumValueParametersFactory.Create(name, value);

        // Assert
        Assert.NotNull(actual);
        Assert.Null(actual.DocumentationTags);
        Assert.Null(actual.DescriptionAttribute);
        Assert.Equal(name, actual.Name);
        Assert.Equal(value, actual.Value);
    }
}