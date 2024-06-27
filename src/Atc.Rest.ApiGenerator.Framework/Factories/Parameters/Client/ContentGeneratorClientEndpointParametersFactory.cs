namespace Atc.Rest.ApiGenerator.Framework.Factories.Parameters.Client;

public static class ContentGeneratorClientEndpointParametersFactory
{
    public static ContentGeneratorClientEndpointParameters Create(
        string projectName,
        string apiGroupName,
        string @namespace,
        OpenApiPathItem openApiPath,
        OperationType httpMethod,
        OpenApiOperation openApiOperation,
        string httpClientName,
        string urlPath)
    {
        ArgumentNullException.ThrowIfNull(openApiPath);
        ArgumentNullException.ThrowIfNull(openApiOperation);

        var parameters = new List<ContentGeneratorClientEndpointParametersParameters>();

        AppendParameters(parameters, openApiPath.Parameters);
        AppendParameters(parameters, openApiOperation.Parameters);
        AppendParametersFromBody(parameters, openApiOperation.RequestBody);

        var operationName = openApiOperation.GetOperationName();
        var endpointAuthorization = openApiOperation.ExtractApiOperationAuthorization(openApiPath);
        var responseModels = openApiOperation.ExtractApiOperationResponseModels().ToList();

        if (openApiPath.HasParameters() ||
            openApiOperation.HasParametersOrRequestBody())
        {
            return new ContentGeneratorClientEndpointParameters(
                Namespace: @namespace,
                HttpMethod: httpMethod.ToString(),
                OperationName: operationName,
                DocumentationTags: openApiOperation.ExtractDocumentationTagsForEndpoint(),
                HttpClientName: httpClientName,
                UrlPath: urlPath,
                EndpointName: $"{operationName}{ContentGeneratorConstants.Endpoint}",
                InterfaceName: $"I{operationName}{ContentGeneratorConstants.Endpoint}",
                ResultName: $"{operationName}{ContentGeneratorConstants.EndpointResult}",
                ParameterName: $"{operationName}{ContentGeneratorConstants.Parameters}",
                Authorization: endpointAuthorization,
                ResponseModels: responseModels,
                parameters);
        }

        return new ContentGeneratorClientEndpointParameters(
            Namespace: @namespace,
            HttpMethod: httpMethod.ToString(),
            OperationName: operationName,
            DocumentationTags: openApiOperation.ExtractDocumentationTagsForEndpoint(),
            HttpClientName: httpClientName,
            UrlPath: urlPath,
            EndpointName: $"{operationName}{ContentGeneratorConstants.Endpoint}",
            InterfaceName: $"I{operationName}{ContentGeneratorConstants.Endpoint}",
            ResultName: $"{operationName}{ContentGeneratorConstants.EndpointResult}",
            ParameterName: null,
            Authorization: endpointAuthorization,
            ResponseModels: responseModels,
            Parameters: null);
    }

    private static void AppendParameters(
        ICollection<ContentGeneratorClientEndpointParametersParameters> parameters,
        IEnumerable<OpenApiParameter> openApiParameters)
    {
        foreach (var openApiParameter in openApiParameters)
        {
            if (parameters.FirstOrDefault(x => x.Name == openApiParameter.Name) is null)
            {
                parameters.Add(new ContentGeneratorClientEndpointParametersParameters(
                    openApiParameter.Name,
                    openApiParameter.Name.EnsureValidFormattedPropertyName(),
                    ConvertToParameterLocationType(openApiParameter.In),
                    openApiParameter.Schema.IsTypeArray()));
            }
        }
    }

    private static void AppendParametersFromBody(
        ICollection<ContentGeneratorClientEndpointParametersParameters> parameters,
        OpenApiRequestBody? requestBody)
    {
        var requestSchema = requestBody?.Content?.GetSchemaByFirstMediaType();

        if (requestSchema is null)
        {
            return;
        }

        parameters.Add(new ContentGeneratorClientEndpointParametersParameters(
            string.Empty,
            ContentGeneratorConstants.Request,
            requestSchema.GetParameterLocationType(),
            requestSchema.IsTypeArray()));
    }

    private static ParameterLocationType ConvertToParameterLocationType(
        ParameterLocation? openApiParameterLocation)
        => openApiParameterLocation switch
        {
            ParameterLocation.Query => ParameterLocationType.Query,
            ParameterLocation.Header => ParameterLocationType.Header,
            ParameterLocation.Path => ParameterLocationType.Route,
            ParameterLocation.Cookie => ParameterLocationType.Cookie,
            null => ParameterLocationType.None,
            _ => throw new SwitchCaseDefaultException(openApiParameterLocation),
        };
}