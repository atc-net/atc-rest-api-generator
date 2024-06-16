namespace Atc.Rest.ApiGenerator.Contracts.ContentGeneratorsParameters.Client;

public record ContentGeneratorClientParameterParameters(
    string Namespace,
    string OperationName,
    CodeDocumentationTags DocumentationTags,
    string ParameterName,
    IList<ContentGeneratorClientParameterParametersProperty> PropertyParameters);

public record ContentGeneratorClientParameterParametersProperty(
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