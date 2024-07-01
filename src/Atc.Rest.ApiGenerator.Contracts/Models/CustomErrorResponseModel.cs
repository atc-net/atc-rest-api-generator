namespace Atc.Rest.ApiGenerator.Contracts.Models;

public class CustomErrorResponseModel
{
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    [SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "OK - for binding.")]
    public IDictionary<string, CustomErrorResponseModelSchema> Schema { get; set; } = new Dictionary<string, CustomErrorResponseModelSchema>(StringComparer.OrdinalIgnoreCase);

    public override string ToString()
        => $"{nameof(Name)}: {Name}, {nameof(Description)}: {Description}, {nameof(Schema)}: {Schema}";
}