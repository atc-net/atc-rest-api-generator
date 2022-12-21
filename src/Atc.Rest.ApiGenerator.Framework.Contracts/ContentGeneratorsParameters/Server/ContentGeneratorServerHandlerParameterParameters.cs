namespace Atc.Rest.ApiGenerator.Framework.Contracts.ContentGeneratorsParameters.Server;

public record ContentGeneratorServerHandlerParameterParameters(
    string Namespace,
    string OperationName,
    string Description,
    string ParameterName,
    List<ContentGeneratorServerParameterParametersProperty> Parameters);

public record ContentGeneratorServerParameterParametersProperty(
    string Name,
    string ParameterName,
    string Description,
    ParameterLocationType ParameterLocationType,
    string DataType,
    bool IsNullable,
    bool IsRequired);