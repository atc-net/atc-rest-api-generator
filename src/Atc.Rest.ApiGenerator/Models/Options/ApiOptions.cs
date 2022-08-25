namespace Atc.Rest.ApiGenerator.Models.Options;

public class ApiOptions
{
    public ApiOptionsGenerator Generator { get; set; } = new();

    public ApiOptionsValidation Validation { get; set; } = new();
}