// ReSharper disable InvertIf
namespace Atc.Rest.ApiGenerator.OpenApi.Extensions;

public static class OpenApiKeyValuePairExtensions
{
    public static bool IsTypeArray(
        this KeyValuePair<string, OpenApiSchema> value)
        => value.Value.IsTypeArray();

    public static bool IsSchemaEnum(
        this KeyValuePair<string, OpenApiSchema> value)
        => value.Value.IsSchemaEnum();

    public static bool IsSchemaEnumAndUseJsonString(
        this KeyValuePair<string, OpenApiSchema> value)
    {
        if (value.Value.IsSchemaEnum())
        {
            foreach (var apiAny in value.Value.Enum)
            {
                if (apiAny is not OpenApiString openApiString)
                {
                    continue;
                }

                if ((!value.Value.Type.Equals("string", StringComparison.Ordinal) && openApiString.Value.IsFirstCharacterLowerCase()) ||
                    openApiString.Value.Contains('-', StringComparison.Ordinal))
                {
                    return true;
                }
            }
        }

        return false;
    }

    public static string GetFormattedKey(
        this KeyValuePair<string, OpenApiSchema> value)
        => value.Key.PascalCase(ApiOperationExtractor.ModelNameSeparators, removeSeparators: true);

    public static string GetFormattedKey(
        this KeyValuePair<string, OpenApiResponse> value)
        => value.Key.PascalCase(ApiOperationExtractor.ModelNameSeparators, removeSeparators: true);
}