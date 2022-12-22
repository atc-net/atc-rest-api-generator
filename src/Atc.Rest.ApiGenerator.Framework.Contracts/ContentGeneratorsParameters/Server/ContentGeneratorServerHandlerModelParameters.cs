namespace Atc.Rest.ApiGenerator.Framework.Contracts.ContentGeneratorsParameters.Server;

public record ContentGeneratorServerHandlerModelParameters(
    string Namespace,
    string ModelName,
    string Description,
    IList<ContentGeneratorServerHandlerModelParametersProperty> PropertyParameters);

public record ContentGeneratorServerHandlerModelParametersProperty(
    string Name,
    string ParameterName,
    string Description,
    string DataType,
    bool IsSimpleType,
    bool UseListForDataType,
    bool IsNullable,
    bool IsRequired,
    IList<ValidationAttribute> AdditionalValidationAttributes,
    string? DefaultValueInitializer);