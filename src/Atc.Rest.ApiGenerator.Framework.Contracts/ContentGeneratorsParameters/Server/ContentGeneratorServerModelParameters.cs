namespace Atc.Rest.ApiGenerator.Framework.Contracts.ContentGeneratorsParameters.Server;

public record ContentGeneratorServerModelParameters(
    string Namespace,
    string ModelName,
    CodeDocumentationTags DocumentationTags,
    IList<ContentGeneratorServerModelParametersProperty> PropertyParameters);

public record ContentGeneratorServerModelParametersProperty(
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