namespace Atc.Rest.ApiGenerator.Framework.Mvc.Factories.Parameters.Server;

public static class ContentGeneratorServerHandlerParametersFactory
{
    public static ClassParameters Create(
        string @namespace,
        OpenApiPathItem openApiPath,
        OpenApiOperation openApiOperation)
    {
        var operationName = openApiOperation.GetOperationName();

        var hasParameters = openApiPath.HasParameters() ||
                            openApiOperation.HasParametersOrRequestBody();

        var methodParametersParameters = new List<ParameterBaseParameters>();
        if (hasParameters)
        {
            methodParametersParameters.Add(
                new ParameterBaseParameters(
                    Attributes: null,
                    GenericTypeName: null,
                    IsGenericListType: false,
                    TypeName: $"{operationName}{ContentGeneratorConstants.Parameters}",
                    IsNullableType: false,
                    IsReferenceType: true,
                    Name: "parameters",
                    DefaultValue: null));
        }

        methodParametersParameters.Add(
            new ParameterBaseParameters(
                Attributes: null,
                GenericTypeName: null,
                IsGenericListType: false,
                TypeName: "CancellationToken",
                IsNullableType: false,
                IsReferenceType: true,
                Name: "cancellationToken",
                DefaultValue: "default"));

        var returnTypeName = $"{operationName}{ContentGeneratorConstants.Result}";

        var methodParameters = new List<MethodParameters>
        {
            new(
                DocumentationTags: null,
                Attributes: null,
                AccessModifier: AccessModifiers.Public,
                ReturnTypeName: returnTypeName,
                ReturnGenericTypeName: "Task",
                Name: "ExecuteAsync",
                Parameters: methodParametersParameters,
                AlwaysBreakDownParameters: true,
                UseExpressionBody: false,
                Content: GenerateContentExecuteMethod(hasParameters, operationName)),
        };

        return new ClassParameters(
            HeaderContent: null,
            @namespace,
            openApiOperation.ExtractDocumentationTagsForHandler(),
            Attributes: null,
            AccessModifiers.PublicClass,
            ClassTypeName: $"{operationName}{ContentGeneratorConstants.Handler}",
            GenericTypeName: null,
            InheritedClassTypeName: $"I{operationName}{ContentGeneratorConstants.Handler}",
            InheritedGenericClassTypeName: null,
            InheritedInterfaceTypeName: null,
            Constructors: null,
            Properties: null,
            Methods: methodParameters,
            GenerateToStringMethod: false);
    }

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

    private static string GenerateContentExecuteMethod(
        bool hasParameters,
        string operationName)
    {
        var sb = new StringBuilder();
        if (hasParameters)
        {
            sb.AppendLine("ArgumentNullException.ThrowIfNull(parameters);");
            sb.AppendLine();
        }

        sb.Append($"throw new NotImplementedException(\"Add logic here for {operationName}{ContentGeneratorConstants.Handler}\");");
        return sb.ToString();
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