namespace Atc.Rest.ApiGenerator.Framework.Contracts.ContentGeneratorsParameters.Server;

public record ContentGeneratorServerHandlerEnumerationParameters(
    string Namespace,
    string EnumerationName,
    bool UseFlagAttribute,
    IList<ContentGeneratorServerEnumerationParametersValue> ValueParameters);

public record ContentGeneratorServerEnumerationParametersValue(
    string Name,
    int? Value);