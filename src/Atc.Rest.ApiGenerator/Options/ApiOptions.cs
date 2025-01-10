namespace Atc.Rest.ApiGenerator.Options;

public class ApiOptions
{
    public ApiOptionsGenerator Generator { get; set; } = new();

    public ApiOptionsValidation Validation { get; set; } = new();

    public bool IncludeDeprecatedOperations { get; set; }

    public override string ToString()
        => $"{nameof(Generator)}: {Generator}, {nameof(Validation)}: ({Validation}), {nameof(IncludeDeprecatedOperations)}: ({IncludeDeprecatedOperations})";
}