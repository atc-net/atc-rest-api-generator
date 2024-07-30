namespace Atc.Rest.ApiGenerator.Framework.ContentGeneratorsParameters.Server;

public record ContentGeneratorServerEndpointMethodParameters(
    string OperationTypeRepresentation,
    string Name,
    CodeDocumentationTags DocumentationTags,
    string? Description,
    string? RouteSuffix,
    string InterfaceName,
    string? ParameterTypeName,
    long? MultipartBodyLengthLimit,
    string ResultName,
    ApiAuthorizeModel? Authorization,
    bool IsAuthorizationRequiredFromPath,
    IEnumerable<ApiOperationResponseModel> ResponseModels);