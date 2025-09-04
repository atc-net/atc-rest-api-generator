namespace Atc.Rest.ApiGenerator.OpenApi.Extensions;

public static class OpenApiAnyExtensions
{
    /// <summary>
    /// Returns a string representation of common OpenAPI primitives with predictable, culture-fixed formatting.
    /// Notes:
    ///  - Booleans -> "true"/"false" (lowercase JSON style).
    ///  - Date -> "yyyy-MM-dd" (e.g., 2025-09-03), culture-fixed to avoid month/day swaps.
    ///  - DateTime -> ISO 8601 round-trip "O" (e.g., 2025-09-03T12:34:56.7890123+00:00).
    ///  - Double/Float -> "N" using en-US (group separators + decimals; e.g., 1,234.00).
    ///  - Long/Integer -> default numeric ("G") with en-US (no group separators, no decimals; e.g., 1234).
    ///  - Null -> "null" literal.
    ///  - String/Password -> empty -> "string.Empty"; otherwise the raw value (unquoted).
    /// </summary>
    public static string? GetDefaultValueAsString(
        this IOpenApiAny? openApiAny)
    {
        if (openApiAny is null)
        {
            return null;
        }

        return openApiAny switch
        {
            OpenApiBoolean { Value: true } => "true",
            OpenApiBoolean { Value: false } => "false",
            OpenApiDate apiDate => apiDate.Value.ToString("yyyy-MM-dd", GlobalizationConstants.EnglishCultureInfo),
            OpenApiDateTime apiDateTime => apiDateTime.Value.ToString("O", GlobalizationConstants.EnglishCultureInfo),
            OpenApiDouble apiDouble => apiDouble.Value.ToString("N", GlobalizationConstants.EnglishCultureInfo),
            OpenApiFloat apiFloat => apiFloat.Value.ToString("N", GlobalizationConstants.EnglishCultureInfo),
            OpenApiLong apiLong => apiLong.Value.ToString(GlobalizationConstants.EnglishCultureInfo),
            OpenApiInteger apiInteger => apiInteger.Value.ToString(GlobalizationConstants.EnglishCultureInfo),
            OpenApiNull => "null",
            OpenApiPassword apiPassword => string.IsNullOrEmpty(apiPassword.Value) ? "string.Empty" : $"{apiPassword.Value}",
            OpenApiString apiString => string.IsNullOrEmpty(apiString.Value) ? "string.Empty" : $"{apiString.Value}",
            _ => throw new NotImplementedException("Property initializer: " + openApiAny.GetType()),
        };
    }
}