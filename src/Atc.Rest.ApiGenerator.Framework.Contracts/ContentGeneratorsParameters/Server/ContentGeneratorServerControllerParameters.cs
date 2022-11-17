namespace Atc.Rest.ApiGenerator.Framework.Contracts.ContentGeneratorsParameters.Server;

public record ContentGeneratorServerControllerParameters(
    string Namespace,
    string Area,
    string RouteBase,
    List<ContentGeneratorServerControllerMethodParameters> MethodParameters);

public record ContentGeneratorServerControllerMethodParameters(
    string OperationTypeRepresentation,
    string Name,
    string Description,
    string? RouteSuffix,
    string InterfaceName,
    string? ParameterTypeName,
    long? MultipartBodyLengthLimit,
    List<string> ProducesResponseTypeRepresentations);