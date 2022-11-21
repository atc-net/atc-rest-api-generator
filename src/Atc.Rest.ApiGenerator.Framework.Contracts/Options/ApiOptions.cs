namespace Atc.Rest.ApiGenerator.Framework.Contracts.Options;

public class ApiOptions
{
    public ApiOptionsGenerator Generator { get; set; } = new();

    public ApiOptionsValidation Validation { get; set; } = new();
}