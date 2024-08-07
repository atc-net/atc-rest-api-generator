namespace Atc.Rest.ApiGenerator.Contracts.Models;

public sealed class ApiOperationModel
{
    public string Name { get; set; } = string.Empty;

    public bool IsEnum { get; set; }

    public bool IsShared { get; set; }

    public bool UsesIFormFile { get; set; }

    public ApiAuthorizeModel? Authorization { get; set; }

    public override string ToString()
        => $"{nameof(Name)}: {Name}, {nameof(IsEnum)}: {IsEnum}, {nameof(IsShared)}: {IsShared}, {nameof(UsesIFormFile)}: {UsesIFormFile}";
}