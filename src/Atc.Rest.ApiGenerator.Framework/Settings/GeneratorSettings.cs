namespace Atc.Rest.ApiGenerator.Framework.Settings;

public class GeneratorSettings
{
    public string EndpointsLocation { get; set; } = ContentGeneratorConstants.Endpoints;

    public string ContractsLocation { get; set; } = ContentGeneratorConstants.Contracts;

    public string HandlersLocation { get; set; } = ContentGeneratorConstants.Handlers;

    public bool UsePartialClassForContracts { get; set; }

    public bool UsePartialClassForEndpoints { get; set; }

    public bool UseProblemDetailsAsDefaultResponseBody { get; set; }

    public bool IncludeDeprecatedOperations { get; set; }
}