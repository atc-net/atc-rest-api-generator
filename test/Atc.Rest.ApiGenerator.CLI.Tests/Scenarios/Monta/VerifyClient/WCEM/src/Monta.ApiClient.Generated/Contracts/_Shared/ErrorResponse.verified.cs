namespace Monta.ApiClient.Generated.Contracts;

/// <summary>
/// Represents an error response.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class ErrorResponse
{
    public string? Status { get; set; }

    public string? Message { get; set; }

    public string? ReadableMessage { get; set; }

    public string? ErrorCode { get; set; }

    public object? Context { get; set; }

    /// <inheritdoc />
    public override string ToString()
        => $"{nameof(Status)}: {Status}, {nameof(Message)}: {Message}, {nameof(ReadableMessage)}: {ReadableMessage}, {nameof(ErrorCode)}: {ErrorCode}, {nameof(Context)}: {Context}";
}