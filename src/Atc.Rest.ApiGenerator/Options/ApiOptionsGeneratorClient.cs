namespace Atc.Rest.ApiGenerator.Options;

public class ApiOptionsGeneratorClient
{
    public string ContractsLocation { get; set; } = ContentGeneratorConstants.Contracts;

    public string EndpointsLocation { get; set; } = ContentGeneratorConstants.Endpoints;

    public bool ExcludeEndpointGeneration { get; set; }

    public string HttpClientName { get; set; } = "ApiClient";

    public override string ToString()
        => $"{nameof(ContractsLocation)}: {ContractsLocation}, {nameof(EndpointsLocation)}: {EndpointsLocation}, {nameof(ExcludeEndpointGeneration)}: {ExcludeEndpointGeneration}, {nameof(HttpClientName)}: {HttpClientName}";
}