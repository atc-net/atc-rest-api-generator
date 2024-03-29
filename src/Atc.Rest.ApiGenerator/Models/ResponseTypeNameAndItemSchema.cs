namespace Atc.Rest.ApiGenerator.Models;

public class ResponseTypeNameAndItemSchema
{
    public ResponseTypeNameAndItemSchema(
        HttpStatusCode statusCode,
        string fullModelName,
        OpenApiSchema? schema)
    {
        StatusCode = statusCode;
        FullModelName = fullModelName;
        Schema = schema;
    }

    public HttpStatusCode StatusCode { get; }

    public string FullModelName { get; }

    public OpenApiSchema? Schema { get; }

    public bool HasModelName
        => !string.IsNullOrEmpty(FullModelName);

    public bool HasSchema
        => Schema is not null;

    public override string ToString()
        => $"{nameof(StatusCode)}: {StatusCode}, {nameof(FullModelName)}: {FullModelName}, {nameof(Schema)}: {Schema?.GetModelType()}";
}