namespace Atc.Rest.ApiGenerator.Framework.Contracts.ContentGeneratorsParameters.Server;

public record ContentGeneratorServerEnumerationParameters(
    string Namespace,
    string EnumerationName,
    CodeDocumentationTags DocumentationTags,
    bool UseFlagAttribute,
    IList<ContentGeneratorServerEnumerationParametersValue> ValueParameters);

public record ContentGeneratorServerEnumerationParametersValue(
    string Name,
    int? Value);