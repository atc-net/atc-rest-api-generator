namespace Atc.Rest.ApiGenerator.Framework.ContentGeneratorsParameters.Server;

public record ContentGeneratorServerEndpointParameters(
    string Namespace,
    string ApiGroupName,
    string RouteBase,
    CodeDocumentationTags DocumentationTags,
    string EndpointName,
    ApiAuthorizeModel? Authorization,
    IList<ContentGeneratorServerEndpointMethodParameters> MethodParameters);