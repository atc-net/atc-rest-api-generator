namespace Atc.Rest.ApiGenerator.OpenApi.Extensions;

public static class OpenApiAnyExtensions
{
    /// <summary>
    /// Returns a string representation of common OpenAPI primitives with predictable, culture-fixed formatting.
    /// </summary>
    /// <remarks>
    /// <para>Formatting rules:</para>
    /// <list type="bullet">
    ///   <item><description><b>Booleans</b> → <c>"true"</c>/<c>"false"</c> (lowercase JSON style).</description></item>
    ///   <item><description><b>Date</b> → <c>"yyyy-MM-dd"</c> (e.g., <c>2025-09-03</c>), culture-fixed to avoid month/day swaps.</description></item>
    ///   <item><description><b>DateTime</b> → ISO 8601 round-trip <c>"O"</c> (e.g., <c>2025-09-03T12:34:56.7890123+00:00</c>).</description></item>
    ///   <item><description><b>Double/Float</b> → standard numeric format <c>"N"</c> using en-US (group separators + decimals; e.g., <c>1,234.00</c>).</description></item>
    ///   <item><description><b>Long/Integer</b> → general numeric format <c>"G"</c> using en-US (no group separators, no decimals; e.g., <c>1234</c>).</description></item>
    ///   <item><description><b>Null</b> → the literal <c>"null"</c>.</description></item>
    ///   <item><description><b>String/Password</b> → empty → <c>"string.Empty"</c>; otherwise the raw value (unquoted).</description></item>
    /// </list>
    /// <para>
    /// Culture is fixed to <see cref="GlobalizationConstants.EnglishCultureInfo"/> (en-US) to ensure consistent output across environments.
    /// </para>
    /// </remarks>
    /// <param name="openApiAny">The <see cref="IOpenApiAny"/> value to format (e.g., <see cref="OpenApiBoolean"/>, <see cref="OpenApiDate"/>, <see cref="OpenApiDateTime"/>, <see cref="OpenApiDouble"/>, <see cref="OpenApiFloat"/>, <see cref="OpenApiLong"/>, <see cref="OpenApiInteger"/>, <see cref="OpenApiNull"/>, <see cref="OpenApiPassword"/>, <see cref="OpenApiString"/>).</param>
    /// <returns>
    /// The formatted string representation of <paramref name="openApiAny"/>; <see langword="null"/> if <paramref name="openApiAny"/> is <see langword="null"/>.
    /// </returns>
    /// <exception cref="NotImplementedException">
    /// Thrown when <paramref name="openApiAny"/> is an <see cref="IOpenApiAny"/> subtype not handled by this method.
    /// </exception>
    /// <example>
    /// <code>
    /// IOpenApiAny v1 = new OpenApiDouble(1234);
    /// string? s1 = v1.GetDefaultValueAsString(); // "1,234.00"
    ///
    /// IOpenApiAny v2 = new OpenApiDate(new DateOnly(2025, 9, 3));
    /// string? s2 = v2.GetDefaultValueAsString(); // "2025-09-03"
    ///
    /// IOpenApiAny v3 = new OpenApiString(string.Empty);
    /// string? s3 = v3.GetDefaultValueAsString(); // "string.Empty"
    /// </code>
    /// </example>
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