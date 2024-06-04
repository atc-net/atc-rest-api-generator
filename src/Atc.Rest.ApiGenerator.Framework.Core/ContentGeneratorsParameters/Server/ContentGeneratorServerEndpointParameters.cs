namespace Atc.Rest.ApiGenerator.Framework.Core.ContentGeneratorsParameters.Server;

public record ContentGeneratorServerEndpointParameters(
    string Namespace,
    string ApiGroupName,
    string RouteBase,
    CodeDocumentationTags DocumentationTags,
    string EndpointName,
    ApiAuthorizeModel? Authorization,
    IList<ContentGeneratorServerEndpointMethodParameters> MethodParameters);

public record ContentGeneratorServerEndpointMethodParameters(
    string OperationTypeRepresentation,
    string Name,
    CodeDocumentationTags DocumentationTags,
    string? RouteSuffix,
    string InterfaceName,
    string? ParameterTypeName,
    long? MultipartBodyLengthLimit,
    string ResultName,
    ApiAuthorizeModel? Authorization,
    IEnumerable<ApiOperationResponseModel> ResponseModels);