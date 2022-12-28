namespace Atc.Rest.ApiGenerator.Framework.Contracts.ContentGeneratorsParameters.Server;

public record ContentGeneratorServerHandlerParameters(
    string Namespace,
    string HandlerName,
    CodeDocumentationTags DocumentationTags,
    string InterfaceName,
    string ResultName,
    string? ParameterName);