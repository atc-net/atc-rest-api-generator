// ReSharper disable InvertIf
namespace Atc.Rest.ApiGenerator.OpenApi.Extensions;

public static class OpenApiSchemaExtensions
{
    public static bool IsModelOfTypeArray(
        this OpenApiSchema apiSchema,
        IDictionary<string, OpenApiSchema> modelSchemas)
    {
        var modelType = apiSchema.GetModelType();
        if (modelType is null &&
            apiSchema.Reference?.Id is not null)
        {
            var (key, value) = modelSchemas.FirstOrDefault(x => x.Key.Equals(apiSchema.Reference.Id, StringComparison.OrdinalIgnoreCase));
            if (key is not null)
            {
                return value.Type is not null &&
                       value.Type.EndsWith(OpenApiDataTypeConstants.Array, StringComparison.OrdinalIgnoreCase);
            }
        }

        return modelType is not null &&
               modelType.EndsWith(OpenApiDataTypeConstants.Array, StringComparison.OrdinalIgnoreCase);
    }

    public static bool IsArray(
        this OpenApiSchema apiSchema)
        => apiSchema.IsTypeArray() &&
           apiSchema.Items?.Reference?.Id is not null;

    public static bool IsPaging(
        this OpenApiSchema apiSchema)
        => apiSchema.AllOf.Count == 2 &&
           (NameConstants.Pagination.Equals(apiSchema.AllOf[0].Reference?.Id, StringComparison.OrdinalIgnoreCase) ||
            NameConstants.Pagination.Equals(apiSchema.AllOf[1].Reference?.Id, StringComparison.OrdinalIgnoreCase));
}