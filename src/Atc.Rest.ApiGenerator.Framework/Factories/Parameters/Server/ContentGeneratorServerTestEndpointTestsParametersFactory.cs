// ReSharper disable ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
namespace Atc.Rest.ApiGenerator.Framework.Factories.Parameters.Server;

public static class ContentGeneratorServerTestEndpointTestsParametersFactory
{
    public static ClassParameters Create(
        string @namespace,
        OpenApiOperation openApiOperation)
    {
        var operationName = openApiOperation.GetOperationName();

        var constructorParameters = new List<ConstructorParameters>
        {
            new(
                DocumentationTags: null,
                AccessModifiers.Public,
                GenericTypeName: null,
                TypeName: $"{operationName}{ContentGeneratorConstants.Tests}",
                InheritedClassTypeName: "base",
                new List<ConstructorParameterBaseParameters>
                {
                    new(
                        GenericTypeName: null,
                        TypeName: "WebApiStartupFactory",
                        Name: "fixture",
                        DefaultValue: null,
                        PassToInheritedClass: true,
                        CreateAsPrivateReadonlyMember: false,
                        CreateAaOneLiner: false),
                }),
        };

        var methodParameters = new List<MethodParameters>
        {
            new(
                DocumentationTags: null,
                Attributes: AttributesParametersFactory.Create("Fact", "Skip = \"Change this to a real integration-test\""),
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
            new List<AttributeParameters>
            {
                AttributeParametersFactory.Create("Collection", "\"Sequential-Endpoints\""),
                AttributeParametersFactory.Create("Trait", "Traits.Category, Traits.Categories.Integration"),
            },
            AccessModifiers.Public,
            ClassTypeName: $"{operationName}{ContentGeneratorConstants.Tests}",
            GenericTypeName: null,
            InheritedClassTypeName: "WebApiControllerBaseTest",
            InheritedGenericClassTypeName: null,
            InheritedInterfaceTypeName: null,
            Constructors: constructorParameters,
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