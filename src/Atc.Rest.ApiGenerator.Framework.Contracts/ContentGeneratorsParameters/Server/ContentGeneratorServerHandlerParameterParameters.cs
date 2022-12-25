namespace Atc.Rest.ApiGenerator.Framework.Contracts.ContentGeneratorsParameters.Server;

public record ContentGeneratorServerHandlerParameterParameters(
    string Namespace,
    string OperationName,
    CodeDocumentationTags DocumentationTags,
    string ParameterName,
    IList<ContentGeneratorServerParameterParametersProperty> PropertyParameters);

public record ContentGeneratorServerParameterParametersProperty(
    string Name,
    string ParameterName,
    CodeDocumentationTags DocumentationTags,
    ParameterLocationType ParameterLocationType,
    string DataType,
    bool IsSimpleType,
    bool UseListForDataType,
    bool IsNullable,
    bool IsRequired,
    IList<ValidationAttribute> AdditionalValidationAttributes,
    string? DefaultValueInitializer);