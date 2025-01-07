namespace Atc.Rest.ApiGenerator.Framework.ContentGeneratorsParameters.Server;

public record ContentGeneratorServerEndpointParameters(
    string Namespace,
    string ApiGroupName,
    string RouteBase,
    CodeDocumentationTags DocumentationTags,
    DeclarationModifiers DeclarationModifier,
    string EndpointName,
    ApiAuthorizeModel? Authorization,
    IList<ContentGeneratorServerEndpointMethodParameters> MethodParameters);