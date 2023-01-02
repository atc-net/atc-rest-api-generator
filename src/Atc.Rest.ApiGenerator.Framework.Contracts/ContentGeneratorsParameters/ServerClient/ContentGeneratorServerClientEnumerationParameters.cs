namespace Atc.Rest.ApiGenerator.Framework.Contracts.ContentGeneratorsParameters.ServerClient;

public record ContentGeneratorServerClientEnumerationParameters(
    string Namespace,
    string EnumerationName,
    CodeDocumentationTags DocumentationTags,
    bool UseFlagsAttribute,
    IList<ContentGeneratorServerClientEnumerationParametersValue> ValueParameters);

public record ContentGeneratorServerClientEnumerationParametersValue(
    string Name,
    int? Value);