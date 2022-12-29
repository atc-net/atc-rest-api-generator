namespace Atc.Rest.ApiGenerator.Framework.Contracts.ContentGeneratorsParameters.Server;

public record ContentGeneratorServerWebApiStartupFactoryParameters(
    string Namespace,
    CodeDocumentationTags DocumentationTags);