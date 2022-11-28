namespace Atc.Rest.ApiGenerator.Framework.Contracts.Options;

public class ApiOptionsValidation
{
    public bool StrictMode { get; set; }

    public bool OperationIdValidation { get; set; }

    public CasingStyle OperationIdCasingStyle { get; set; } = CasingStyle.CamelCase;

    public CasingStyle ModelNameCasingStyle { get; set; } = CasingStyle.PascalCase;

    public CasingStyle ModelPropertyNameCasingStyle { get; set; } = CasingStyle.CamelCase;
}