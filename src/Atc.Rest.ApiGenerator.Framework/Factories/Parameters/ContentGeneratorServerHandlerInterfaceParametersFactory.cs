namespace Atc.Rest.ApiGenerator.Framework.Factories.Parameters;

public static class ContentGeneratorServerHandlerInterfaceParametersFactory
{
    public static ContentGeneratorServerHandlerInterfaceParameters Create(
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

            return new ContentGeneratorServerHandlerInterfaceParameters(
                @namespace,
                operationName,
                openApiOperation.ExtractDocumentationTagsForInterface(),
                InterfaceName: $"I{operationName}{ContentGeneratorConstants.Handler}",
                ResultName: $"{operationName}{ContentGeneratorConstants.Result}",
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

            return new ContentGeneratorServerHandlerInterfaceParameters(
                @namespace,
                operationName,
                openApiOperation.ExtractDocumentationTagsForInterface(),
                InterfaceName: $"I{operationName}{ContentGeneratorConstants.Handler}",
                ResultName: $"{operationName}{ContentGeneratorConstants.Result}",
                ParameterName: string.Empty,
                documentationTagsForMethod);
        }
    }
}