namespace Atc.Rest.ApiGenerator.Framework.Minimal.Factories;

public static class ContentGeneratorServerHandlerInterfaceParametersFactory
{
    public static InterfaceParameters Create(
        string headerContent,
        string @namespace,
        AttributeParameters codeGeneratorAttribute,
        OpenApiPathItem openApiPath,
        OpenApiOperation openApiOperation)
    {
        ArgumentNullException.ThrowIfNull(openApiOperation);

        var operationName = openApiOperation.GetOperationName();

        var methodParametersAttributes = new Dictionary<string, string>(StringComparer.Ordinal);
        var methodParametersParameters = new List<ParameterBaseParameters>();
        if (openApiPath.HasParameters() ||
            openApiOperation.HasParametersOrRequestBody())
        {
            methodParametersAttributes.Add("parameters", "The parameters.");
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

        methodParametersAttributes.Add("cancellationToken", "The cancellation token.");
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
                DocumentationTags: new CodeDocumentationTags(
                    "Execute method",
                    parameters: methodParametersAttributes,
                    remark: null,
                    code: null,
                    example: null,
                    exceptions: null,
                    @return: null),
                Attributes: null,
                DeclarationModifier: DeclarationModifiers.None,
                ReturnTypeName: returnTypeName,
                ReturnGenericTypeName: "Task",
                Name: "ExecuteAsync",
                Parameters: methodParametersParameters,
                AlwaysBreakDownParameters: true,
                UseExpressionBody: false,
                Content: null),
        };

        return new InterfaceParameters(
            headerContent,
            @namespace,
            DocumentationTags: openApiOperation.ExtractDocumentationTagsForHandlerInterface(),
            new List<AttributeParameters> { codeGeneratorAttribute },
            DeclarationModifiers.PublicInterface,
            InterfaceTypeName: $"I{operationName}{ContentGeneratorConstants.Handler}",
            InheritedInterfaceTypeName: null,
            Properties: null,
            Methods: methodParameters);
    }
}