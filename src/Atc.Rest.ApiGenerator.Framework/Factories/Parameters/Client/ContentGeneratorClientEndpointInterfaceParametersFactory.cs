namespace Atc.Rest.ApiGenerator.Framework.Factories.Parameters.Client;

public static class ContentGeneratorClientEndpointInterfaceParametersFactory
{
    public static InterfaceParameters Create(
        string headerContent,
        string @namespace,
        AttributeParameters codeGeneratorAttribute,
        OpenApiPathItem openApiPath,
        OpenApiOperation openApiOperation,
        string httpClientName)
    {
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
                    IsReferenceType: true,
                    Name: "parameters",
                    DefaultValue: null));
        }

        methodParametersAttributes.Add("httpClientName", "The http client name.");
        methodParametersParameters.Add(
            new ParameterBaseParameters(
                Attributes: null,
                GenericTypeName: null,
                IsGenericListType: false,
                TypeName: "string",
                IsReferenceType: false,
                Name: "httpClientName",
                DefaultValue: httpClientName));

        methodParametersAttributes.Add("cancellationToken", "The cancellation token.");
        methodParametersParameters.Add(
            new ParameterBaseParameters(
                Attributes: null,
                GenericTypeName: null,
                IsGenericListType: false,
                TypeName: "CancellationToken",
                IsReferenceType: true,
                Name: "cancellationToken",
                DefaultValue: "default"));

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
                AccessModifier: AccessModifiers.None,
                ReturnTypeName: $"I{operationName}{ContentGeneratorConstants.EndpointResult}",
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
            DocumentationTags: openApiOperation.ExtractDocumentationTagsForEndpointInterface(),
            new List<AttributeParameters> { codeGeneratorAttribute },
            AccessModifiers.Public,
            InterfaceTypeName: $"I{operationName}{ContentGeneratorConstants.Endpoint}",
            InheritedInterfaceTypeName: null,
            Properties: null,
            Methods: methodParameters);
    }
}