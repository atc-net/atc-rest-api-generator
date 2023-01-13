namespace Atc.CodeGeneration.CSharp.Tests.Content.Factories;

public class ClassParametersFactoryTests
{
    [Theory]
    [InlineAutoData]
    public void Create(
        string headerContent,
        string @namespace,
        AttributeParameters attribute,
        string classTypeName)
    {
        // Act
        var actual = ClassParametersFactory.Create(
            headerContent,
            @namespace,
            attribute,
            classTypeName);

        // Assert
        Assert.NotNull(actual);
    }

}