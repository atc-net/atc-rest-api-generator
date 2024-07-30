namespace Atc.Rest.ApiGenerator.Contracts.ContentGeneratorsParameters.Client;

public record ContentGeneratorClientEndpointResultInterfaceParameters(
    string Namespace,
    string OperationName,
    CodeDocumentationTags DocumentationTags,
    string InterfaceName,
    string InheritInterfaceName,
    bool HasParameterType,
    ApiAuthorizeModel? Authorization,
    bool IsAuthorizationRequiredFromPath,
    IList<ApiOperationResponseModel> ResponseModels);

public record ContentGeneratorClientEndpointResultInterfaceParametersParameters(
    string Name,
    string ParameterName,
    ParameterLocationType ParameterLocationType);