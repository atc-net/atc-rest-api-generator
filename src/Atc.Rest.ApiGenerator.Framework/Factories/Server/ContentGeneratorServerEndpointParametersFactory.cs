namespace Atc.Rest.ApiGenerator.Framework.Factories.Server;

public static class ContentGeneratorServerEndpointParametersFactory
{
    public static ContentGeneratorServerEndpointParameters Create(
        IList<ApiOperation> operationSchemaMappings,
        string projectName,
        string @namespace,
        string apiGroupName,
        string route,
        string endpointSuffixName,
        OpenApiDocument openApiDocument)
    {
        var modelNamespace = $"{projectName}.{ContentGeneratorConstants.Contracts}.{apiGroupName}";
        var methodParameters = new List<ContentGeneratorServerEndpointMethodParameters>();

        ApiAuthorizeModel? controllerAuthorization = null;
        foreach (var (apiPath, apiPathData) in openApiDocument.GetPathsByBasePathSegmentName(apiGroupName))
        {
            controllerAuthorization ??= apiPathData.ExtractApiPathAuthorization();

            foreach (var apiOperation in apiPathData.Operations)
            {
                var operationName = apiOperation.Value.GetOperationName();
                var endpointAuthorization = apiOperation.Value.ExtractApiOperationAuthorization(apiPathData);
                var responseModels = apiOperation.Value
                    .ExtractApiOperationResponseModels(modelNamespace)
                    .AdjustNamespacesIfNeeded(operationSchemaMappings);

                methodParameters.Add(new ContentGeneratorServerEndpointMethodParameters(
                    OperationTypeRepresentation: apiOperation.Key.ToString(),
                    Name: operationName,
                    DocumentationTags: apiOperation.Value.ExtractDocumentationTags(),
                    Description: apiOperation.Value.Description,
                    RouteSuffix: GetRouteSuffix(apiPath),
                    InterfaceName: $"I{operationName}{ContentGeneratorConstants.Handler}",
                    ParameterTypeName: GetParameterTypeName(operationName, apiPathData, apiOperation.Value),
                    MultipartBodyLengthLimit: GetMultipartBodyLengthLimit(apiOperation.Value),
                    ResultName: $"{operationName}{ContentGeneratorConstants.Result}",
                    ResponseModels: responseModels,
                    Authorization: endpointAuthorization,
                    IsAuthorizationRequiredFromPath: controllerAuthorization is not null));
            }
        }

        var documentationTags = new CodeDocumentationTags("Endpoint definitions.");

        return new ContentGeneratorServerEndpointParameters(
            Namespace: @namespace,
            ApiGroupName: apiGroupName,
            route,
            documentationTags,
            EndpointName: $"{apiGroupName}{endpointSuffixName}",
            controllerAuthorization,
            methodParameters);
    }

    private static string? GetRouteSuffix(
        string apiPath)
    {
        var apiPathParts = apiPath.Split('/', StringSplitOptions.RemoveEmptyEntries);
        if (apiPathParts.Length <= 1)
        {
            return null;
        }

        var sb = new StringBuilder();
        for (var i = 1; i < apiPathParts.Length; i++)
        {
            if (i != 1)
            {
                sb.Append('/');
            }

            sb.Append(apiPathParts[i]);
        }

        return sb.ToString();
    }

    private static string? GetParameterTypeName(
        string operationName,
        OpenApiPathItem apiPathData,
        OpenApiOperation apiOperation)
        => apiPathData.HasParameters() ||
           apiOperation.HasParametersOrRequestBody()
            ? $"{operationName}{ContentGeneratorConstants.Parameters}"
            : null;

    private static long? GetMultipartBodyLengthLimit(
        OpenApiOperation apiOperation)
    {
        if (apiOperation.HasRequestBodyWithAnythingAsFormatTypeBinary())
        {
            // TODO: Use project settings
            return long.MaxValue;
        }

        return null;
    }
}