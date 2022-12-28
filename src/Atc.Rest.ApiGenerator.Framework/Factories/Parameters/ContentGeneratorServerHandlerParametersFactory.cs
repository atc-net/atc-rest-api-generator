namespace Atc.Rest.ApiGenerator.Framework.Factories.Parameters;

public static class ContentGeneratorServerHandlerParametersFactory
{
    public static ContentGeneratorServerHandlerParameters Create(
        string @namespace,
        OpenApiPathItem openApiPath,
        OpenApiOperation openApiOperation)
    {
        var operationName = openApiOperation.GetOperationName();

        if (openApiPath.HasParameters() ||
            openApiOperation.HasParametersOrRequestBody())
        {
            return new ContentGeneratorServerHandlerParameters(
                @namespace,
                HandlerName: $"{operationName}{ContentGeneratorConstants.Handler}",
                openApiOperation.ExtractDocumentationTagsForHandler(),
                InterfaceName: $"I{operationName}{ContentGeneratorConstants.Handler}",
                ResultName: $"{operationName}{ContentGeneratorConstants.Result}",
                ParameterName: $"{operationName}{ContentGeneratorConstants.Parameters}");
        }

        return new ContentGeneratorServerHandlerParameters(
            @namespace,
            HandlerName: $"{operationName}{ContentGeneratorConstants.Handler}",
            openApiOperation.ExtractDocumentationTagsForHandler(),
            InterfaceName: $"I{operationName}{ContentGeneratorConstants.Handler}",
            ResultName: $"{operationName}{ContentGeneratorConstants.Result}",
            ParameterName: null);
    }
}