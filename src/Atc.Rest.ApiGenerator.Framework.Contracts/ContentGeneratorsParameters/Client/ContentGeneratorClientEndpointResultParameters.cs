namespace Atc.Rest.ApiGenerator.Framework.Contracts.ContentGeneratorsParameters.Client;

public record ContentGeneratorClientEndpointResultParameters(
    string Namespace,
    string OperationName,
    CodeDocumentationTags DocumentationTags,
    string EndpointResultName,
    string EndpointResultInterfaceName,
    string InheritClassName,
    bool HasParameterType,
    ApiAuthorizeModel? Authorization,
    IList<ApiOperationResponseModel> ResponseModels,
    IList<ContentGeneratorClientEndpointResultParametersParameters>? Parameters);

public record ContentGeneratorClientEndpointResultParametersParameters(
    string Name,
    string ParameterName,
    ParameterLocationType ParameterLocationType);