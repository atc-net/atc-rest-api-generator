// ReSharper disable LoopCanBeConvertedToQuery
// ReSharper disable ReplaceWithSingleAssignment.True
namespace Atc.Rest.ApiGenerator.Framework.Minimal.Factories.Parameters.Server;

public static class ContentGeneratorServerEndpointParametersFactory
{
    public static ContentGeneratorServerEndpointParameters Create(
        IList<ApiOperation> operationSchemaMappings,
        string projectName,
        string @namespace,
        string apiGroupName,
        string route,
        OpenApiDocument openApiDocument)
    {
        var methodParameters = new List<ContentGeneratorServerEndpointMethodParameters>();

        foreach (var (apiPath, apiPathData) in openApiDocument.GetPathsByBasePathSegmentName(apiGroupName))
        {
            var apiPathAuthenticationRequired = apiPathData.Extensions.ExtractAuthenticationRequired();
            var apiPathAuthorizationRoles = apiPathData.Extensions.ExtractAuthorizationRoles();
            var apiPathAuthenticationSchemes = apiPathData.Extensions.ExtractAuthenticationSchemes();

            foreach (var apiOperation in apiPathData.Operations)
            {
                var apiOperationAuthenticationRequired = apiOperation.Value.Extensions.ExtractAuthenticationRequired();
                var apiOperationAuthorizationRoles = apiOperation.Value.Extensions.ExtractAuthorizationRoles();
                var apiOperationAuthenticationSchemes = apiOperation.Value.Extensions.ExtractAuthenticationSchemes();

                var operationName = apiOperation.Value.GetOperationName();

                methodParameters.Add(new ContentGeneratorServerEndpointMethodParameters(
                    OperationTypeRepresentation: apiOperation.Key.ToString(),
                    Name: operationName,
                    DocumentationTags: apiOperation.Value.ExtractDocumentationTags(),
                    RouteSuffix: GetRouteSuffix(apiPath),
                    InterfaceName: $"I{operationName}{ContentGeneratorConstants.Handler}",
                    ParameterTypeName: GetParameterTypeName(operationName, apiPathData, apiOperation.Value),
                    MultipartBodyLengthLimit: GetMultipartBodyLengthLimit(apiOperation.Value),
                    ResultName: $"{operationName}{ContentGeneratorConstants.Result}",
                    apiPathAuthenticationRequired,
                    apiPathAuthorizationRoles,
                    apiPathAuthenticationSchemes,
                    apiOperationAuthenticationRequired,
                    apiOperationAuthorizationRoles,
                    apiOperationAuthenticationSchemes));
            }
        }

        var documentationTags = new CodeDocumentationTags("Endpoint definitions.");

        return new ContentGeneratorServerEndpointParameters(
            Namespace: @namespace,
            ApiGroupName: apiGroupName,
            route,
            documentationTags,
            EndpointName: $"{apiGroupName}{ContentGeneratorConstants.EndpointDefinition}",
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