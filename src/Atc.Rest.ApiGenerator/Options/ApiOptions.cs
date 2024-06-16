namespace Atc.Rest.ApiGenerator.Options;

public class ApiOptions
{
    public ApiOptionsGenerator Generator { get; set; } = new();

    public ApiOptionsValidation Validation { get; set; } = new();
}