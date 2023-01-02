namespace Atc.Rest.ApiGenerator.Framework.Factories.Parameters.Client;

public static class ContentGeneratorClientEndpointInterfaceParametersFactory
{
    public static ContentGeneratorClientEndpointInterfaceParameters Create(
        string @namespace,
        OpenApiPathItem openApiPath,
        OpenApiOperation openApiOperation)
    {
        var operationName = openApiOperation.GetOperationName();

        if (openApiPath.HasParameters() ||
            openApiOperation.HasParametersOrRequestBody())
        {
            var docParameters = new Dictionary<string, string>(StringComparer.Ordinal)
            {
                { "parameters", "The parameters." },
                { "cancellationToken", "The cancellation token." },
            };

            var documentationTagsForMethod = new CodeDocumentationTags(
                summary: "Execute method.",
                parameters: docParameters,
                remark: null,
                code: null,
                example: null,
                exceptions: null,
                @return: null);

            return new ContentGeneratorClientEndpointInterfaceParameters(
                @namespace,
                operationName,
                openApiOperation.ExtractDocumentationTagsForEndpointInterface(),
                InterfaceName: $"I{operationName}{ContentGeneratorConstants.Endpoint}",
                ResultName: $"{operationName}{ContentGeneratorConstants.EndpointResult}",
                ParameterName: $"{operationName}{ContentGeneratorConstants.Parameters}",
                documentationTagsForMethod);
        }
        else
        {
            var docParameters = new Dictionary<string, string>(StringComparer.Ordinal)
            {
                { "cancellationToken", "The cancellation token." },
            };

            var documentationTagsForMethod = new CodeDocumentationTags(
                summary: "Execute method.",
                parameters: docParameters,
                remark: null,
                code: null,
                example: null,
                exceptions: null,
                @return: null);

            return new ContentGeneratorClientEndpointInterfaceParameters(
                @namespace,
                operationName,
                openApiOperation.ExtractDocumentationTagsForEndpointInterface(),
                InterfaceName: $"I{operationName}{ContentGeneratorConstants.Endpoint}",
                ResultName: $"{operationName}{ContentGeneratorConstants.EndpointResult}",
                ParameterName: string.Empty,
                documentationTagsForMethod);
        }
    }
}