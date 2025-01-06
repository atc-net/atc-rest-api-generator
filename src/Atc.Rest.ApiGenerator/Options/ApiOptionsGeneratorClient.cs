namespace Atc.Rest.ApiGenerator.Options;

public class ApiOptionsGeneratorClient
{
    public string HttpClientName { get; set; } = ContentGeneratorConstants.DefaultHttpClientName;

    public bool ExcludeEndpointGeneration { get; set; }

    public override string ToString()
        => $"{nameof(HttpClientName)}: {HttpClientName}, {nameof(ExcludeEndpointGeneration)}: {ExcludeEndpointGeneration}";
}