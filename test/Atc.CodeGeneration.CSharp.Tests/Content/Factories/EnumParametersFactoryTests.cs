namespace Atc.CodeGeneration.CSharp.Tests.Content.Factories;

public class EnumParametersFactoryTests
{
    [Theory]
    [InlineAutoNSubstituteData]
    public void Create(
        string headerContent,
        string @namespace,
        CodeDocumentationTags documentationTags,
        IList<AttributeParameters> attributes,
        string enumTypeName,
        string[] enumNames)
    {
        // Act
        var actual = EnumParametersFactory.Create(
            headerContent,
            @namespace,
            documentationTags,
            attributes,
            enumTypeName,
            enumNames);

        // Assert
        Assert.NotNull(actual);
    }

    [Theory]
    [InlineAutoNSubstituteData]
    public void CreateWithValues(
        string headerContent,
        string @namespace,
        CodeDocumentationTags documentationTags,
        IList<AttributeParameters> attributes,
        string enumTypeName,
        IDictionary<string, int> enumNameValues)
    {
        // Act
        var actual = EnumParametersFactory.Create(
            headerContent,
            @namespace,
            documentationTags,
            attributes,
            enumTypeName,
            enumNameValues);

        // Assert
        Assert.NotNull(actual);
    }
}