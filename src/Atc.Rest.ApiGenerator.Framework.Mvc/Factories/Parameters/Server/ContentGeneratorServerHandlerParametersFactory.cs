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

        var methodParameters = new List<MethodParameters>
        {
            new(
                DocumentationTags: null,
                Attributes: null,
                AccessModifier: AccessModifiers.Public,
                ReturnTypeName: $"{operationName}{ContentGeneratorConstants.Result}",
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
}