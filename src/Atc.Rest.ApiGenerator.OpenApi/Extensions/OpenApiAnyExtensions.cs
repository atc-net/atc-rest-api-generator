namespace Atc.Rest.ApiGenerator.OpenApi.Extensions;

public static class OpenApiAnyExtensions
{
    public static string? GetDefaultValueAsString(
        this IOpenApiAny? openApiAny)
    {
        if (openApiAny is null)
        {
            return null;
        }

        return openApiAny switch
        {
            OpenApiInteger apiInteger => apiInteger.Value.ToString(),
            OpenApiString apiString => string.IsNullOrEmpty(apiString.Value) ? "string.Empty" : $"\"{apiString.Value}\"",
            OpenApiBoolean { Value: true } => "true",
            OpenApiBoolean { Value: false } => "false",
            _ => throw new NotImplementedException("Property initializer: " + openApiAny.GetType()),
        };
    }
}