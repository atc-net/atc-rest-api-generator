namespace Atc.Rest.ApiGenerator.Framework.Minimal.ContentGeneratorsParameters.Server;

public record ContentGeneratorServerEndpointParameters(
    string Namespace,
    string ApiGroupName,
    string RouteBase,
    CodeDocumentationTags DocumentationTags,
    string EndpointName,
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
    bool? ApiPathUseAuthorization,
    IEnumerable<string> ApiPathAuthorizationRoles,
    IEnumerable<string> ApiPathAuthenticationSchemes,
    bool? ApiOperationUseAuthorization,
    IEnumerable<string> ApiOperationAuthorizationRoles,
    IEnumerable<string> ApiOperationAuthenticationSchemes);