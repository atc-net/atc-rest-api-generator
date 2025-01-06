namespace Atc.Rest.ApiGenerator.Options;

public class ApiOptionsGeneratorClient
{
    public string ContractsLocation { get; set; } = ContentGeneratorConstants.Contracts;

    public string EndpointsLocation { get; set; } = ContentGeneratorConstants.Endpoints;

    public string HttpClientName { get; set; } = ContentGeneratorConstants.DefaultHttpClientName;

    public bool ExcludeEndpointGeneration { get; set; }

    public bool UsePartialClassForContracts { get; set; }

    public bool UsePartialClassForEndpoints { get; set; }

    public override string ToString()
        => $"{nameof(ContractsLocation)}: {ContractsLocation}, {nameof(EndpointsLocation)}: {EndpointsLocation}, {nameof(HttpClientName)}: {HttpClientName}, {nameof(ExcludeEndpointGeneration)}: {ExcludeEndpointGeneration}, {nameof(UsePartialClassForContracts)}: {UsePartialClassForContracts}, {nameof(UsePartialClassForEndpoints)}: {UsePartialClassForEndpoints}";
}