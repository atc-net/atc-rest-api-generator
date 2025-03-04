namespace Atc.Rest.ApiGenerator.Contracts.ContentGeneratorsParameters.Client;

public record ContentGeneratorClientEndpointParameters(
    string Namespace,
    string HttpMethod,
    string OperationName,
    CodeDocumentationTags DocumentationTags,
    string HttpClientName,
    string UrlPath,
    DeclarationModifiers DeclarationModifier,
    string EndpointName,
    string InterfaceName,
    string ResultName,
    string? ParameterName,
    ApiAuthorizeModel? Authorization,
    bool IsAuthorizationRequiredFromPath,
    IList<ApiOperationResponseModel> ResponseModels,
    IList<ContentGeneratorClientEndpointParametersParameters>? Parameters);

public record ContentGeneratorClientEndpointParametersParameters(
    string Name,
    string ParameterName,
    ParameterLocationType ParameterLocationType,
    bool IsList);