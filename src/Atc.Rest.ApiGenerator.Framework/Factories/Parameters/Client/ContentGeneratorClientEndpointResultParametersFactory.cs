namespace Atc.Rest.ApiGenerator.Framework.Factories.Parameters.Client;

public static class ContentGeneratorClientEndpointResultParametersFactory
{
    public static ContentGeneratorClientEndpointResultParameters Create(
        string projectName,
        string apiGroupName,
        string @namespace,
        string contractsLocation,
        OpenApiPathItem openApiPath,
        OpenApiOperation openApiOperation,
        bool usePartialClass)
    {
        ArgumentNullException.ThrowIfNull(openApiPath);
        ArgumentNullException.ThrowIfNull(openApiOperation);

        var parameters = new List<ContentGeneratorClientEndpointResultParametersParameters>();

        AppendParameters(parameters, openApiPath.Parameters);
        AppendParameters(parameters, openApiOperation.Parameters);
        AppendParametersFromBody(parameters, openApiOperation.RequestBody);

        var modelNamespace = NamespaceFactory.CreateFull(projectName, contractsLocation, apiGroupName);
        var operationName = openApiOperation.GetOperationName();
        var controllerAuthorization = openApiPath.ExtractApiPathAuthorization();
        var endpointAuthorization = openApiOperation.ExtractApiOperationAuthorization(openApiPath);
        var responseModels = openApiOperation.ExtractApiOperationResponseModels(modelNamespace).ToList();
        var hasParameterType = openApiPath.HasParameters() || openApiOperation.HasParametersOrRequestBody();

        if (hasParameterType)
        {
            return new ContentGeneratorClientEndpointResultParameters(
                Namespace: @namespace,
                OperationName: operationName,
                DocumentationTags: openApiOperation.ExtractDocumentationTagsForEndpointResult(),
                usePartialClass ? DeclarationModifiers.PublicPartialClass : DeclarationModifiers.PublicClass,
                EndpointResultName: $"{operationName}{ContentGeneratorConstants.EndpointResult}",
                EndpointResultInterfaceName: $"I{operationName}{ContentGeneratorConstants.EndpointResult}",
                InheritClassName: ContentGeneratorConstants.EndpointResponse,
                HasParameterType: hasParameterType,
                Authorization: endpointAuthorization,
                IsAuthorizationRequiredFromPath: controllerAuthorization is not null,
                ResponseModels: responseModels,
                parameters);
        }

        return new ContentGeneratorClientEndpointResultParameters(
            Namespace: @namespace,
            OperationName: operationName,
            DocumentationTags: openApiOperation.ExtractDocumentationTagsForEndpointResult(),
            usePartialClass ? DeclarationModifiers.PublicPartialClass : DeclarationModifiers.PublicClass,
            EndpointResultName: $"{operationName}{ContentGeneratorConstants.EndpointResult}",
            EndpointResultInterfaceName: $"I{operationName}{ContentGeneratorConstants.EndpointResult}",
            InheritClassName: ContentGeneratorConstants.EndpointResponse,
            HasParameterType: hasParameterType,
            Authorization: endpointAuthorization,
            IsAuthorizationRequiredFromPath: controllerAuthorization is not null,
            ResponseModels: responseModels,
            Parameters: null);
    }

    private static void AppendParameters(
        ICollection<ContentGeneratorClientEndpointResultParametersParameters> parameters,
        IEnumerable<OpenApiParameter> openApiParameters)
    {
        foreach (var openApiParameter in openApiParameters)
        {
            if (parameters.FirstOrDefault(x => x.Name == openApiParameter.Name) is null)
            {
                parameters.Add(new ContentGeneratorClientEndpointResultParametersParameters(
                    openApiParameter.Name,
                    openApiParameter.Name.EnsureFirstCharacterToUpper(),
                    ConvertToParameterLocationType(openApiParameter.In)));
            }
        }
    }

    private static void AppendParametersFromBody(
        ICollection<ContentGeneratorClientEndpointResultParametersParameters> parameters,
        OpenApiRequestBody? requestBody)
    {
        var requestSchema = requestBody?.Content?.GetSchemaByFirstMediaType();

        if (requestSchema is null)
        {
            return;
        }

        parameters.Add(new ContentGeneratorClientEndpointResultParametersParameters(
            string.Empty,
            ContentGeneratorConstants.Request,
            requestSchema.GetParameterLocationType()));
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