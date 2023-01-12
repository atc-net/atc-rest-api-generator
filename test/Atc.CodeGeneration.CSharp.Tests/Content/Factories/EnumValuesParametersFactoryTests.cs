namespace Atc.CodeGeneration.CSharp.Tests.Content.Factories;

public class EnumValuesParametersFactoryTests
{
    [Theory]
    [InlineAutoData]
    public void Create(string[] names)
    {
        ArgumentNullException.ThrowIfNull(names);

        // Act
        var actual = EnumValuesParametersFactory.Create(names);

        // Assert
        Assert.NotNull(actual);
        Assert.Equal(names.Length, actual.Count);
    }

    [Theory]
    [InlineAutoData]
    public void CreateWithValues(IDictionary<string, int> nameValues)
    {
        ArgumentNullException.ThrowIfNull(nameValues);

        // Act
        var actual = EnumValuesParametersFactory.Create(nameValues);

        // Assert
        Assert.NotNull(actual);
        Assert.Equal(nameValues.Keys.Count, actual.Count);
    }
}