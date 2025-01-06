namespace Atc.Rest.ApiGenerator.Contracts.ContentGeneratorsParameters.Client;

public record ContentGeneratorClientEndpointResultParameters(
    string Namespace,
    string OperationName,
    CodeDocumentationTags DocumentationTags,
    AccessModifiers AccessModifiers,
    string EndpointResultName,
    string EndpointResultInterfaceName,
    string InheritClassName,
    bool HasParameterType,
    ApiAuthorizeModel? Authorization,
    bool IsAuthorizationRequiredFromPath,
    IList<ApiOperationResponseModel> ResponseModels,
    IList<ContentGeneratorClientEndpointResultParametersParameters>? Parameters);

public record ContentGeneratorClientEndpointResultParametersParameters(
    string Name,
    string ParameterName,
    ParameterLocationType ParameterLocationType);