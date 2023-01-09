namespace Atc.Rest.ApiGenerator.Framework.Contracts.ContentGeneratorsParameters.Client;

public record ContentGeneratorClientEndpointInterfaceParameters(
    string Namespace,
    string OperationName,
    CodeDocumentationTags DocumentationTagsForClass,
    string InterfaceName,
    string ResultName,
    string? ParameterName,
    CodeDocumentationTags DocumentationTagsForMethod);