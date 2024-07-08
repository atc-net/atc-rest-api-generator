namespace Atc.Rest.ApiGenerator.Options;

public class ApiOptionsGeneratorClient
{
    public string FolderName { get; set; } = string.Empty;

    public bool ExcludeEndpointGeneration { get; set; }

    public string HttpClientName { get; set; } = "ApiClient";

    public override string ToString()
        => $"{nameof(FolderName)}: {FolderName}, {nameof(ExcludeEndpointGeneration)}: {ExcludeEndpointGeneration}, {nameof(HttpClientName)}: {HttpClientName}";
}