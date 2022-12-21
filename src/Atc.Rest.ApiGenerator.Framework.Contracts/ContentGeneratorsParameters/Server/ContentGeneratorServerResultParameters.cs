namespace Atc.Rest.ApiGenerator.Framework.Contracts.ContentGeneratorsParameters.Server;

public record ContentGeneratorServerResultParameters(
    string Namespace,
    string OperationName,
    string Description,
    string ResultName,
    IList<ContentGeneratorServerResultMethodParameters> MethodParameters,
    ContentGeneratorServerResultImplicitOperatorParameters? ImplicitOperatorParameters);

public record ContentGeneratorServerResultMethodParameters(
    HttpStatusCode HttpStatusCode,
    SchemaType SchemaType,
    bool UsesProblemDetails,
    string? ModelName,
    bool? UsesBinaryResponse,
    string? SimpleDataTypeName);

public record ContentGeneratorServerResultImplicitOperatorParameters(
    SchemaType SchemaType,
    string? ModelName,
    string? SimpleDataTypeName);