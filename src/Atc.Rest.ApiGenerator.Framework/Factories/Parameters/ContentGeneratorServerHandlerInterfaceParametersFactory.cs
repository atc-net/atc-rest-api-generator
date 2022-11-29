namespace Atc.Rest.ApiGenerator.Framework.Factories.Parameters;

public static class ContentGeneratorServerHandlerInterfaceParametersFactory
{
    public static ContentGeneratorServerHandlerInterfaceParameters Create(
        string @namespace,
        string apiGroupName,
        OpenApiPathItem openApiPath,
        OpenApiOperation openApiOperation)
    {
        var operationName = openApiOperation.GetOperationName();

        return openApiPath.HasParameters() || openApiOperation.HasParametersOrRequestBody()
            ? new ContentGeneratorServerHandlerInterfaceParameters(
                @namespace,
                apiGroupName,
                operationName,
                openApiOperation.GetOperationSummaryDescription(),
                InterfaceName: $"I{operationName}{ContentGeneratorConstants.Handler}",
                ResultName: $"{operationName}{ContentGeneratorConstants.Result}",
                ParameterName: $"{operationName}{ContentGeneratorConstants.Parameters}")
            : new ContentGeneratorServerHandlerInterfaceParameters(
                @namespace,
                apiGroupName,
                operationName,
                openApiOperation.GetOperationSummaryDescription(),
                InterfaceName: $"I{operationName}{ContentGeneratorConstants.Handler}",
                ResultName: $"{operationName}{ContentGeneratorConstants.Result}",
                ParameterName: string.Empty);
    }
}