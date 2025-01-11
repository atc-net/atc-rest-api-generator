namespace Atc.CodeGeneration.CSharp.Tests.Extensions;

[SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "OK.")]
public class StringBuilderExtensionsTests
{
    [Theory]
    [InlineData("", DeclarationModifiers.None)]
    [InlineData("public ", DeclarationModifiers.Public)]
    [InlineData("public async ", DeclarationModifiers.PublicAsync)]
    [InlineData("public static ", DeclarationModifiers.PublicStatic)]
    [InlineData("public static implicit operator ", DeclarationModifiers.PublicStaticImplicitOperator)]
    [InlineData("public record ", DeclarationModifiers.PublicRecord)]
    [InlineData("public record struct ", DeclarationModifiers.PublicRecordStruct)]
    [InlineData("private ", DeclarationModifiers.Private)]
    [InlineData("protected ", DeclarationModifiers.Protected)]
    [InlineData("internal ", DeclarationModifiers.Internal)]
    public void AppendDeclarationModifier(
        string expected,
        DeclarationModifiers declarationModifier)
    {
        // Arrange
        var sb = new StringBuilder();

        // Act
        sb.AppendDeclarationModifier(declarationModifier);
        var actual = sb.ToString();

        // Assert
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("", 3, DeclarationModifiers.None)]
    [InlineData("   public ", 3, DeclarationModifiers.Public)]
    [InlineData("   public async ", 3, DeclarationModifiers.PublicAsync)]
    [InlineData("   public static ", 3, DeclarationModifiers.PublicStatic)]
    [InlineData("   public static implicit operator ", 3, DeclarationModifiers.PublicStaticImplicitOperator)]
    [InlineData("   public record ", 3, DeclarationModifiers.PublicRecord)]
    [InlineData("   public record struct ", 3, DeclarationModifiers.PublicRecordStruct)]
    [InlineData("   private ", 3, DeclarationModifiers.Private)]
    [InlineData("   protected ", 3, DeclarationModifiers.Protected)]
    [InlineData("   internal ", 3, DeclarationModifiers.Internal)]
    public void AppendDeclarationModifier_WithIndentSpaces(
        string expected,
        int indentSpaces,
        DeclarationModifiers declarationModifier)
    {
        // Arrange
        var sb = new StringBuilder();

        // Act
        sb.AppendDeclarationModifier(indentSpaces, declarationModifier);
        var actual = sb.ToString();

        // Assert
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("int age", null, "int", false, "age")]
    [InlineData("List<int> ages", "List", "int", false, "ages")]
    public void AppendTypeAndName(
        string expected,
        string? genericTypeName,
        string typeName,
        bool isNullableType,
        string name)
    {
        // Arrange
        var sb = new StringBuilder();

        // Act
        sb.AppendTypeAndName(genericTypeName, typeName, isNullableType, name);
        var actual = sb.ToString();

        // Assert
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("   int age", 3, null, "int", false, "age")]
    [InlineData("   List<int> ages", 3, "List", "int", false, "ages")]
    public void AppendTypeAndName_WithIndentSpaces(
        string expected,
        int indentSpaces,
        string? genericTypeName,
        string typeName,
        bool isNullableType,
        string name)
    {
        // Arrange
        var sb = new StringBuilder();

        // Act
        sb.AppendTypeAndName(indentSpaces, genericTypeName, typeName, isNullableType, name);
        var actual = sb.ToString();

        // Assert
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("   int age", 3, null, "int", false, "age", null)]
    [InlineData("   List<int> ages", 3, "List", "int", false, "ages", null)]
    [InlineData("   int age = 5", 3, null, "int", false, "age", "5")]
    [InlineData("   int? age = null", 3, null, "int", true, "age", "null")]
    [InlineData("   List<int> ages = new List<int>", 3, "List", "int", false, "ages", "new List<int>")]
    public void AppendTypeAndName_WithIndentSpaces_AndDefaultValue(
        string expected,
        int indentSpaces,
        string? genericTypeName,
        string typeName,
        bool isNullableType,
        string name,
        string? defaultValue)
    {
        // Arrange
        var sb = new StringBuilder();

        // Act
        sb.AppendTypeAndName(indentSpaces, genericTypeName, typeName, isNullableType, name, defaultValue);
        var actual = sb.ToString();

        // Assert
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineAutoData(3)]
    public void AppendAttributesAsLines_WithIndentSpaces(
        int indentSpaces,
        IList<AttributeParameters> attributes)
    {
        // Arrange
        var sb = new StringBuilder();

        // Act
        sb.AppendAttributesAsLines(indentSpaces, usePropertyPrefix: false, attributes);
        var actual = sb.ToString();

        // Assert
        Assert.NotEmpty(actual);

        var lines = actual.Split(Environment.NewLine);

        Assert.Equal(attributes.Count + 1, lines.Length);
    }

    [Theory]
    [InlineAutoData]
    public void AppendAttribute(
        AttributeParameters attribute)
    {
        // Arrange
        var sb = new StringBuilder();

        // Act
        sb.AppendAttribute(usePropertyPrefix: false, attribute);
        var actual = sb.ToString();

        // Assert
        Assert.NotEmpty(actual);
        Assert.StartsWith($"[{attribute.Name}", actual, StringComparison.Ordinal);
        Assert.EndsWith("]", actual, StringComparison.Ordinal);
    }

    [Theory]
    [InlineAutoData]
    public void AppendAttribute_WithIndentSpaces(
        int indentSpaces,
        AttributeParameters attribute)
    {
        // Arrange
        var sb = new StringBuilder();

        // Act
        sb.AppendAttribute(indentSpaces, usePropertyPrefix: false, attribute);
        var actual = sb.ToString();

        // Assert
        var leftPad = 1 + attribute.Name.Length + indentSpaces;
        Assert.NotEmpty(actual);
        Assert.StartsWith($"[{attribute.Name}".PadLeft(leftPad), actual, StringComparison.Ordinal);
        Assert.EndsWith("]", actual, StringComparison.Ordinal);
    }

    [Theory]
    [InlineAutoData(3)]
    public void AppendAttribute_AsSimpleValues_WithIndentSpaces(
        int indentSpaces,
        string name,
        string? content)
    {
        // Arrange
        var sb = new StringBuilder();

        // Act
        sb.AppendAttribute(indentSpaces, usePropertyPrefix: false, name, content);
        var actual = sb.ToString();

        // Assert
        var leftPad = 1 + name.Length + indentSpaces;
        Assert.NotEmpty(actual);
        Assert.StartsWith($"[{name}".PadLeft(leftPad), actual, StringComparison.Ordinal);
        Assert.EndsWith("]", actual, StringComparison.Ordinal);
    }

    [Theory]
    [InlineData("int age,\r\n", 0, null, null, "int", "age", null, true)]
    [InlineData("int age)", 0, null, null, "int", "age", null, false)]
    public void AppendInputParameterLine(
        string expected,
        int indentSpaces,
        IList<AttributeParameters>? attributes,
        string? genericTypeName,
        string typeName,
        string name,
        string? defaultValue,
        bool useCommaForEndChar)
    {
        // Arrange
        expected = expected.EnsureEnvironmentNewLines();
        var sb = new StringBuilder();

        // Act
        sb.AppendInputParameter(indentSpaces, usePropertyPrefix: false, attributes, genericTypeName, typeName, isNullableType: false, name, defaultValue, useCommaForEndChar);
        var actual = sb.ToString();

        // Assert
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineAutoData]
    public void AppendContent(
        string content)
    {
        // Arrange
        var sb = new StringBuilder();

        // Act
        sb.AppendContent(content);
        var actual = sb.ToString();

        // Assert
        var leftPad = content.Length;
        Assert.NotEmpty(actual);
        Assert.Equal(content.PadLeft(leftPad), actual);
    }

    [Theory]
    [InlineAutoData(3)]
    public void AppendContent_WithIndentSpaces(
        int indentSpaces,
        string content)
    {
        // Arrange
        var sb = new StringBuilder();

        // Act
        sb.AppendContent(indentSpaces, content);
        var actual = sb.ToString();

        // Assert
        var leftPad = content.Length + indentSpaces;
        Assert.NotEmpty(actual);
        Assert.Equal(content.PadLeft(leftPad), actual);
    }

    [Theory]
    [InlineData("   => Hallo\r\n   World;", 3, "Hallo\r\nWorld")]
    public void AppendContentAsExpressionBody(
        string expected,
        int indentSpaces,
        string content)
    {
        // Arrange
        expected = expected.EnsureEnvironmentNewLines();
        var sb = new StringBuilder();

        // Act
        sb.AppendContentAsExpressionBody(indentSpaces, content);
        var actual = sb.ToString();

        // Assert
        Assert.Equal(expected, actual);
    }
}