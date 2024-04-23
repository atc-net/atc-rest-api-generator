namespace Atc.Rest.ApiGenerator.Framework.Contracts.Options;

public class ApiOptionsGenerator
{
    public bool UseRestExtended { get; set; } = true;

    public SwaggerThemeMode SwaggerThemeMode { get; set; } = SwaggerThemeMode.Dark;

    public ApiOptionsGeneratorRequest Request { get; set; } = new();

    public ApiOptionsGeneratorResponse Response { get; set; } = new();
}