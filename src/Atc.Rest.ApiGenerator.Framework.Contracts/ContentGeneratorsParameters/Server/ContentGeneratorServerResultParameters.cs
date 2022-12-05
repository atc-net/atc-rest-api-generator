namespace Atc.Rest.ApiGenerator.Framework.Contracts.ContentGeneratorsParameters.Server;

public record ContentGeneratorServerResultParameters(
    string Namespace,
    string OperationName,
    string Description,
    string ResultName,
    List<ContentGeneratorServerResultMethodParameters> MethodParameters,
    List<ContentGeneratorServerResultImplicitOperatorParameters> ImplicitOperatorParameters);

public record ContentGeneratorServerResultMethodParameters(
    HttpStatusCode HttpStatusCode,
    SchemaType SchemaType,
    bool UsesProblemDetails,
    string? ModelName,
    bool? UsesBinaryResponse);

public record ContentGeneratorServerResultImplicitOperatorParameters();