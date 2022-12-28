namespace Atc.Rest.ApiGenerator.Framework.Contracts.ContentGeneratorsParameters.ServerClient;

public record ContentGeneratorServerClientModelParameters(
    string Namespace,
    string ModelName,
    CodeDocumentationTags DocumentationTags,
    IList<ContentGeneratorServerClientModelParametersProperty> PropertyParameters);

public record ContentGeneratorServerClientModelParametersProperty(
    string Name,
    string ParameterName,
    CodeDocumentationTags DocumentationTags,
    string DataType,
    bool IsSimpleType,
    bool UseListForDataType,
    bool IsNullable,
    bool IsRequired,
    IList<ValidationAttribute> AdditionalValidationAttributes,
    string? DefaultValueInitializer);