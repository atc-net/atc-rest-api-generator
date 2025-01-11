namespace Atc.Rest.ApiGenerator.Framework.Factories.Parameters.Client;

public static class ContentGeneratorClientEndpointResultInterfaceParametersFactory
{
    public static ContentGeneratorClientEndpointResultInterfaceParameters Create(
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

        var parameters = new List<ContentGeneratorClientEndpointResultInterfaceParametersParameters>();

        AppendParameters(parameters, openApiPath.Parameters);
        AppendParameters(parameters, openApiOperation.Parameters);
        AppendParametersFromBody(parameters, openApiOperation.RequestBody);

        var modelNamespace = NamespaceFactory.Create(projectName, contractsLocation);
        var operationName = openApiOperation.GetOperationName();
        var controllerAuthorization = openApiPath.ExtractApiPathAuthorization();
        var endpointAuthorization = openApiOperation.ExtractApiOperationAuthorization(openApiPath);
        var responseModels = openApiOperation.ExtractApiOperationResponseModels(modelNamespace).ToList();
        var hasParameterType = openApiPath.HasParameters() || openApiOperation.HasParametersOrRequestBody();

        return new ContentGeneratorClientEndpointResultInterfaceParameters(
            Namespace: @namespace,
            OperationName: operationName,
            DocumentationTags: openApiOperation.ExtractDocumentationTagsForEndpointResultInterface(),
            usePartialClass ? DeclarationModifiers.PublicPartialInterface : DeclarationModifiers.PublicInterface,
            InterfaceName: $"I{operationName}{ContentGeneratorConstants.EndpointResult}",
            InheritInterfaceName: "IEndpointResponse",
            HasParameterType: hasParameterType,
            Authorization: endpointAuthorization,
            IsAuthorizationRequiredFromPath: controllerAuthorization is not null,
            ResponseModels: responseModels);
    }

    private static void AppendParameters(
        ICollection<ContentGeneratorClientEndpointResultInterfaceParametersParameters> parameters,
        IEnumerable<OpenApiParameter> openApiParameters)
    {
        foreach (var openApiParameter in openApiParameters)
        {
            if (parameters.FirstOrDefault(x => x.Name == openApiParameter.Name) is null)
            {
                parameters.Add(new ContentGeneratorClientEndpointResultInterfaceParametersParameters(
                    openApiParameter.Name,
                    openApiParameter.Name.EnsureFirstCharacterToUpper(),
                    ConvertToParameterLocationType(openApiParameter.In)));
            }
        }
    }

    private static void AppendParametersFromBody(
        ICollection<ContentGeneratorClientEndpointResultInterfaceParametersParameters> parameters,
        OpenApiRequestBody? requestBody)
    {
        var requestSchema = requestBody?.Content?.GetSchemaByFirstMediaType();

        if (requestSchema is null)
        {
            return;
        }

        parameters.Add(new ContentGeneratorClientEndpointResultInterfaceParametersParameters(
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