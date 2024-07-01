namespace Atc.Rest.ApiGenerator.Contracts.Models;

public class CustomErrorResponseModelSchema
{
    public string DataType { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public override string ToString()
        => $"{nameof(DataType)}: {DataType}, {nameof(Description)}: {Description}";
}