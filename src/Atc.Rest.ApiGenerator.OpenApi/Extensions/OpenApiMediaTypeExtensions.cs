namespace Atc.Rest.ApiGenerator.OpenApi.Extensions;

public static class OpenApiMediaTypeExtensions
{
    public static bool IsSchemaEnumAndUseJsonString(this OpenApiMediaType apiMediaType)
    {
        if (apiMediaType.Schema.IsSchemaEnum())
        {
            foreach (var apiAny in apiMediaType.Schema.Enum)
            {
                if (apiAny is not OpenApiString openApiString)
                {
                    continue;
                }

                if ((!apiMediaType.Schema.Type.Equals("string", StringComparison.Ordinal) && openApiString.Value.IsFirstCharacterLowerCase()) ||
                    openApiString.Value.Contains('-', StringComparison.Ordinal))
                {
                    return true;
                }
            }
        }

        return false;
    }
}