namespace Atc.CodeGeneration.CSharp.Tests.Extensions;

[SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "OK.")]
public class StringBuilderExtensionsTests
{
    [Theory]
    [InlineData("", AccessModifiers.None)]
    [InlineData("public ", AccessModifiers.Public)]
    [InlineData("public async ", AccessModifiers.PublicAsync)]
    [InlineData("public static ", AccessModifiers.PublicStatic)]
    [InlineData("public static implicit operator ", AccessModifiers.PublicStaticImplicitOperator)]
    [InlineData("public record ", AccessModifiers.PublicRecord)]
    [InlineData("public record struct ", AccessModifiers.PublicRecordStruct)]
    [InlineData("private ", AccessModifiers.Private)]
    [InlineData("protected ", AccessModifiers.Protected)]
    [InlineData("internal ", AccessModifiers.Internal)]
    public void AppendAccessModifier(
        string expected,
        AccessModifiers accessModifier)
    {
        // Arrange
        var sb = new StringBuilder();

        // Act
        sb.AppendAccessModifier(accessModifier);
        var actual = sb.ToString();

        // Assert
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("", 3, AccessModifiers.None)]
    [InlineData("   public ", 3, AccessModifiers.Public)]
    [InlineData("   public async ", 3, AccessModifiers.PublicAsync)]
    [InlineData("   public static ", 3, AccessModifiers.PublicStatic)]
    [InlineData("   public static implicit operator ", 3, AccessModifiers.PublicStaticImplicitOperator)]
    [InlineData("   public record ", 3, AccessModifiers.PublicRecord)]
    [InlineData("   public record struct ", 3, AccessModifiers.PublicRecordStruct)]
    [InlineData("   private ", 3, AccessModifiers.Private)]
    [InlineData("   protected ", 3, AccessModifiers.Protected)]
    [InlineData("   internal ", 3, AccessModifiers.Internal)]
    public void AppendAccessModifier_WithIndentSpaces(
        string expected,
        int indentSpaces,
        AccessModifiers accessModifier)
    {
        // Arrange
        var sb = new StringBuilder();

        // Act
        sb.AppendAccessModifier(indentSpaces, accessModifier);
        var actual = sb.ToString();

        // Assert
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("int age", null, "int", "age")]
    [InlineData("List<int> ages", "List", "int", "ages")]
    public void AppendTypeAndName(
        string expected,
        string? genericTypeName,
        string typeName,
        string name)
    {
        // Arrange
        var sb = new StringBuilder();

        // Act
        sb.AppendTypeAndName(genericTypeName, typeName, name);
        var actual = sb.ToString();

        // Assert
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("   int age", 3, null, "int", "age")]
    [InlineData("   List<int> ages", 3, "List", "int", "ages")]
    public void AppendTypeAndName_WithIndentSpaces(
        string expected,
        int indentSpaces,
        string? genericTypeName,
        string typeName,
        string name)
    {
        // Arrange
        var sb = new StringBuilder();

        // Act
        sb.AppendTypeAndName(indentSpaces, genericTypeName, typeName, name);
        var actual = sb.ToString();

        // Assert
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("   int age", 3, null, "int", "age", null)]
    [InlineData("   List<int> ages", 3, "List", "int", "ages", null)]
    [InlineData("   int age = 5", 3, null, "int", "age", "5")]
    [InlineData("   int? age = null", 3, null, "int?", "age", "null")]
    [InlineData("   List<int> ages = new List<int>", 3, "List", "int", "ages", "new List<int>")]
    public void AppendTypeAndName_WithIndentSpaces_AndDefaultValue(
        string expected,
        int indentSpaces,
        string? genericTypeName,
        string typeName,
        string name,
        string? defaultValue)
    {
        // Arrange
        var sb = new StringBuilder();

        // Act
        sb.AppendTypeAndName(indentSpaces, genericTypeName, typeName, name, defaultValue);
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
        sb.AppendInputParameter(indentSpaces, usePropertyPrefix: false, attributes, genericTypeName, typeName, name, defaultValue, useCommaForEndChar);
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