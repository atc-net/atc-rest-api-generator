namespace Atc.Rest.ApiGenerator.Contracts.Options;

public class ApiOptionsValidation
{
    public bool StrictMode { get; set; }

    public bool OperationIdValidation { get; set; }

    public CasingStyle OperationIdCasingStyle { get; set; } = CasingStyle.CamelCase;

    public CasingStyle ModelNameCasingStyle { get; set; } = CasingStyle.PascalCase;

    public CasingStyle ModelPropertyNameCasingStyle { get; set; } = CasingStyle.CamelCase;

    public override string ToString()
        => $"{nameof(StrictMode)}: {StrictMode}, {nameof(OperationIdValidation)}: {OperationIdValidation}, {nameof(OperationIdCasingStyle)}: {OperationIdCasingStyle}, {nameof(ModelNameCasingStyle)}: {ModelNameCasingStyle}, {nameof(ModelPropertyNameCasingStyle)}: {ModelPropertyNameCasingStyle}";
}