namespace Atc.Rest.ApiGenerator.Framework.Factories.Server;

public static class ContentGeneratorServerHandlerParametersTestFactory
{
    public static ClassParameters CreateForCustomTest(
        string @namespace,
        OpenApiOperation openApiOperation)
    {
        var operationName = openApiOperation.GetOperationName();

        var methodParameters = new List<MethodParameters>
        {
            new(
                DocumentationTags: null,
                Attributes: AttributesParametersFactory.Create("Fact", "Skip = \"Change this to a real test\""),
                AccessModifier: AccessModifiers.Public,
                ReturnTypeName: "void",
                ReturnGenericTypeName: null,
                Name: "Sample",
                Parameters: null,
                AlwaysBreakDownParameters: false,
                UseExpressionBody: false,
                Content: GenerateContentTestSample()),
        };

        return new ClassParameters(
            HeaderContent: null,
            @namespace,
            DocumentationTags: null,
            Attributes: null,
            AccessModifiers.PublicClass,
            ClassTypeName: $"{operationName}{ContentGeneratorConstants.HandlerTests}",
            GenericTypeName: null,
            InheritedClassTypeName: null,
            InheritedGenericClassTypeName: null,
            InheritedInterfaceTypeName: null,
            Constructors: null,
            Properties: null,
            Methods: methodParameters,
            GenerateToStringMethod: false);
    }

    private static string GenerateContentTestSample()
    {
        var sb = new StringBuilder();
        sb.AppendLine(4, "{");
        sb.AppendLine(8, "// Arrange");
        sb.AppendLine();
        sb.AppendLine(8, "// Act");
        sb.AppendLine();
        sb.AppendLine(8, "// Assert");
        sb.Append(4, "}");
        return sb.ToString();
    }
}