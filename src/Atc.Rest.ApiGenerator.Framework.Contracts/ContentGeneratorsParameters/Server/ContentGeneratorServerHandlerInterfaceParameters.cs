namespace Atc.Rest.ApiGenerator.Framework.Contracts.ContentGeneratorsParameters.Server;

public record ContentGeneratorServerHandlerInterfaceParameters(
    string Namespace,
    string OperationName,
    CodeDocumentationTags DocumentationTags,
    string InterfaceName,
    string ResultName,
    string? ParameterName);