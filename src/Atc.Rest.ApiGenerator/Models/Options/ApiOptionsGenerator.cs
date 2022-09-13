// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace Atc.Rest.ApiGenerator.Models.Options;

public class ApiOptionsGenerator
{
    public bool UseGlobalUsings { get; set; } = true;

    public bool UseAuthorization { get; set; }

    public bool UseRestExtended { get; set; } = true;

    public ApiOptionsGeneratorRequest Request { get; set; } = new ();

    public ApiOptionsGeneratorResponse Response { get; set; } = new ();
}