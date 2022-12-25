namespace Atc.Rest.ApiGenerator.Framework.Factories.Parameters;

public static class ContentGeneratorServerHandlerInterfaceParametersFactory
{
    public static ContentGeneratorServerHandlerInterfaceParameters Create(
        string @namespace,
        OpenApiPathItem openApiPath,
        OpenApiOperation openApiOperation)
    {
        var operationName = openApiOperation.GetOperationName();

        return openApiPath.HasParameters() || openApiOperation.HasParametersOrRequestBody()
            ? new ContentGeneratorServerHandlerInterfaceParameters(
                @namespace,
                operationName,
                openApiOperation.ExtractDocumentationTagsForInterface(),
                InterfaceName: $"I{operationName}{ContentGeneratorConstants.Handler}",
                ResultName: $"{operationName}{ContentGeneratorConstants.Result}",
                ParameterName: $"{operationName}{ContentGeneratorConstants.Parameters}")
            : new ContentGeneratorServerHandlerInterfaceParameters(
                @namespace,
                operationName,
                openApiOperation.ExtractDocumentationTagsForInterface(),
                InterfaceName: $"I{operationName}{ContentGeneratorConstants.Handler}",
                ResultName: $"{operationName}{ContentGeneratorConstants.Result}",
                ParameterName: string.Empty);
    }
}