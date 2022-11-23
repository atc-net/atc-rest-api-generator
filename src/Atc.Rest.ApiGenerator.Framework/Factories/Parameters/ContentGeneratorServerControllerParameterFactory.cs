// ReSharper disable LoopCanBeConvertedToQuery
namespace Atc.Rest.ApiGenerator.Framework.Factories.Parameters;

public static class ContentGeneratorServerControllerParameterFactory
{
    public static ContentGeneratorServerControllerParameters Create(
        IList<ApiOperation> operationSchemaMappings,
        string projectName,
        bool useProblemDetailsAsDefaultBody,
        string @namespace,
        string area,
        string route,
        OpenApiDocument openApiDocument,
        bool apiOptionsUseAuthorization)
    {
        var methodParameters = new List<ContentGeneratorServerControllerMethodParameters>();

        foreach (var (apiPath, apiPathData) in openApiDocument.GetPathsByBasePathSegmentName(area))
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

                methodParameters.Add(new ContentGeneratorServerControllerMethodParameters(
                    OperationTypeRepresentation: apiOperation.Key.ToString(),
                    Name: operationName,
                    Description: GetOperationSummaryDescription(apiOperation.Value),
                    RouteSuffix: GetRouteSuffix(apiPath),
                    InterfaceName: $"I{operationName}{ContentGeneratorConstants.Handler}",
                    ParameterTypeName: GetParameterTypeName(operationName, apiPathData, apiOperation.Value),
                    MultipartBodyLengthLimit: GetMultipartBodyLengthLimit(apiOperation.Value),
                    ProducesResponseTypeRepresentations: GetProducesResponseTypeRepresentations(
                        operationSchemaMappings,
                        apiOperation.Value,
                        area,
                        projectName,
                        useProblemDetailsAsDefaultBody),
                    apiPathAuthenticationRequired,
                    apiPathAuthorizationRoles,
                    apiPathAuthenticationSchemes,
                    apiOperationAuthenticationRequired,
                    apiOperationAuthorizationRoles,
                    apiOperationAuthenticationSchemes));
            }
        }

        return new ContentGeneratorServerControllerParameters(
            @namespace,
            area,
            route,
            apiOptionsUseAuthorization || openApiDocument.HasAllPathsAuthenticationRequiredSet(area),
            methodParameters);
    }

    private static string GetOperationSummaryDescription(
        OpenApiOperation apiOperation)
    {
        var result = apiOperation.Summary;

        if (string.IsNullOrEmpty(result))
        {
            result = apiOperation.Description;
        }

        if (string.IsNullOrEmpty(result))
        {
            return "Undefined description.";
        }

        if (!result.EndsWith('.'))
        {
            result += ".";
        }

        return result;
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

    private static List<string> GetProducesResponseTypeRepresentations(
        IList<ApiOperation> operationSchemaMappings,
        OpenApiOperation apiOperation,
        string area,
        string projectName,
        bool useProblemDetailsAsDefaultBody)
        => apiOperation.Responses.GetProducesResponseAttributeParts(
            operationSchemaMappings,
            area,
            projectName,
            useProblemDetailsAsDefaultBody,
            apiOperation.HasParametersOrRequestBody(),
            includeIfNotDefinedAuthorization: false,
            includeIfNotDefinedInternalServerError: false);
}