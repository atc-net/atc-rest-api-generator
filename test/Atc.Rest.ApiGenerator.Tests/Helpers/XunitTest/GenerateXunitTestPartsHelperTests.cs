namespace Atc.Rest.ApiGenerator.Tests.Helpers.XunitTest;

public class GenerateXunitTestPartsHelperTests
{
    // TODO: Add UT for => AppendModelSimpleProperty(int indentSpaces, StringBuilder sb, EndpointMethodMetadata endpointMethodMetadata, KeyValuePair<string, OpenApiSchema> schemaProperty, string dataType, string propertyValueGenerated, int countString, bool asJsonBody, int depthHierarchy, TrailingCharType trailingChar)
    // TODO: Add UT for => AppendModelSimplePropertyForString(int indentSpaces, StringBuilder sb, string propertyName, KeyValuePair<string, OpenApiSchema> schemaProperty, string propertyValueGenerated, int countString, bool asJsonBody, int depthHierarchy, TrailingCharType trailingChar)
    // TODO: Add UT for => AppendModelSimplePropertyForDateTimeOffset(int indentSpaces, StringBuilder sb, string propertyName, string propertyValueGenerated, bool asJsonBody, int depthHierarchy, TrailingCharType trailingChar)
    // TODO: Add UT for => AppendModelSimplePropertyForGuid(int indentSpaces, StringBuilder sb, string propertyName, string propertyValueGenerated, bool asJsonBody, int depthHierarchy, TrailingCharType trailingChar)
    // TODO: Add UT for => AppendModelSimplePropertyForUri(int indentSpaces, StringBuilder sb, string propertyName, string propertyValueGenerated, bool asJsonBody, int depthHierarchy, TrailingCharType trailingChar)
    // TODO: Add UT for => AppendModelSimplePropertyDefault(int indentSpaces, StringBuilder sb, string propertyName, string propertyValueGenerated, bool asJsonBody, int depthHierarchy, TrailingCharType trailingChar)
    [Theory]
    [InlineData(false, "Address")]
    [InlineData(false, "ListAddress")]
    [InlineData(true, "List<Address>")]
    [InlineData(false, "PaginationAddress")]
    [InlineData(true, "Pagination<Address>")]
    public void IsListKind(bool expected, string value)
        => Assert.Equal(
            expected,
            GenerateXunitTestPartsHelper.IsListKind(value));

    // TODO: Add UT for => AppendPartDataNew(int indentSpaces, StringBuilder sb)
    // TODO: Add UT for => AppendPartDataEqualNew(int indentSpaces, StringBuilder sb, string variableName)
    // TODO: Add UT for => AppendPartVarDataEqualNew(int indentSpaces, StringBuilder sb, string variableName = data)
    // TODO: Add UT for => AppendPartDataEqualNewListOf(int indentSpaces, StringBuilder sb, string variableName, string dataType)
    // TODO: Add UT for => AppendPartVarDataEqualNewListOf(int indentSpaces, StringBuilder sb, string variableName, string dataType)
    // TODO: Add UT for => GetDataTypeIfEnum(KeyValuePair<string, OpenApiSchema> schema, IDictionary<string, OpenApiSchema> componentsSchemas)
    [Theory]
    [InlineData(TrailingCharType.Comma, false, 0, 1)]
    [InlineData(TrailingCharType.Comma, false, 1, 1)]
    [InlineData(TrailingCharType.None, true, 0, 1)]
    [InlineData(TrailingCharType.Comma, true, 1, 1)]
    public void GetTrailingCharForProperty(TrailingCharType expected, bool asJsonBody, int currentItem, int totalItems)
        => Assert.Equal(
            expected,
            GenerateXunitTestPartsHelper.GetTrailingCharForProperty(asJsonBody, currentItem, totalItems));

    // TODO: Add UT for => GetTrailingCharForProperty(bool asJsonBody, KeyValuePair<string, OpenApiSchema> currentSchema, IDictionary<string, OpenApiSchema> totalSchema)
    // TODO: Add UT for => PropertyValueGenerator(KeyValuePair<string, OpenApiSchema> schema, IDictionary<string, OpenApiSchema> componentsSchemas, bool useForBadRequest, int itemNumber, string customValue)
    // TODO: Add UT for => PropertyValueGeneratorTypeResolver(KeyValuePair<string, OpenApiSchema> schema, IDictionary<string, OpenApiSchema> componentsSchemas, bool useForBadRequest)
    [Theory]
    [InlineData("\\\"Hallo World\\\"", "Hallo World")]
    public void WrapInQuotes(string expected, string value)
        => Assert.Equal(
            expected,
            GenerateXunitTestPartsHelper.WrapInQuotes(value));

    [Theory]
    [InlineData("sb.AppendLine(\"Hallo World\");", "Hallo World")]
    public void WrapInStringBuilderAppendLine(string expected, string value)
        => Assert.Equal(
            expected,
            GenerateXunitTestPartsHelper.WrapInStringBuilderAppendLine(value));

    [Theory]
    [InlineData("sb.AppendLine(\"  \\\"Hallo\\\": World\");", 0, "Hallo", "World", TrailingCharType.None)]
    [InlineData("sb.AppendLine(\"  \\\"Hallo\\\": World,\");", 0, "Hallo", "World", TrailingCharType.Comma)]
    [InlineData("sb.AppendLine(\"  \\\"Hallo\\\": World;\");", 0, "Hallo", "World", TrailingCharType.SemiColon)]
    [InlineData("sb.AppendLine(\"  \\\"Hallo\\\": World:\");", 0, "Hallo", "World", TrailingCharType.Colon)]
    [InlineData("sb.AppendLine(\"    \\\"Hallo\\\": World\");", 1, "Hallo", "World", TrailingCharType.None)]
    [InlineData("sb.AppendLine(\"    \\\"Hallo\\\": World,\");", 1, "Hallo", "World", TrailingCharType.Comma)]
    [InlineData("sb.AppendLine(\"    \\\"Hallo\\\": World;\");", 1, "Hallo", "World", TrailingCharType.SemiColon)]
    [InlineData("sb.AppendLine(\"    \\\"Hallo\\\": World:\");", 1, "Hallo", "World", TrailingCharType.Colon)]
    public void WrapInStringBuilderAppendLineWithKeyQuotes(string expected, int depthHierarchy, string key, string value, TrailingCharType trailingChar)
        => Assert.Equal(
            expected,
            GenerateXunitTestPartsHelper.WrapInStringBuilderAppendLineWithKeyQuotes(depthHierarchy, key, value, trailingChar));

    [Theory]
    [InlineData("sb.AppendLine(\"  \\\"Hallo\\\": \\\"World\\\"\");", 0, "Hallo", "World", TrailingCharType.None)]
    [InlineData("sb.AppendLine(\"  \\\"Hallo\\\": \\\"World\\\",\");", 0, "Hallo", "World", TrailingCharType.Comma)]
    [InlineData("sb.AppendLine(\"  \\\"Hallo\\\": \\\"World\\\";\");", 0, "Hallo", "World", TrailingCharType.SemiColon)]
    [InlineData("sb.AppendLine(\"  \\\"Hallo\\\": \\\"World\\\":\");", 0, "Hallo", "World", TrailingCharType.Colon)]
    [InlineData("sb.AppendLine(\"    \\\"Hallo\\\": \\\"World\\\"\");", 1, "Hallo", "World", TrailingCharType.None)]
    [InlineData("sb.AppendLine(\"    \\\"Hallo\\\": \\\"World\\\",\");", 1, "Hallo", "World", TrailingCharType.Comma)]
    [InlineData("sb.AppendLine(\"    \\\"Hallo\\\": \\\"World\\\";\");", 1, "Hallo", "World", TrailingCharType.SemiColon)]
    [InlineData("sb.AppendLine(\"    \\\"Hallo\\\": \\\"World\\\":\");", 1, "Hallo", "World", TrailingCharType.Colon)]
    public void WrapInStringBuilderAppendLineWithKeyAndValueQuotes(string expected, int depthHierarchy, string key, string value, TrailingCharType trailingChar)
        => Assert.Equal(
            expected,
            GenerateXunitTestPartsHelper.WrapInStringBuilderAppendLineWithKeyAndValueQuotes(depthHierarchy, key, value, trailingChar));
}