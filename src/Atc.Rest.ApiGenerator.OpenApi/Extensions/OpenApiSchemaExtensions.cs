namespace Atc.Rest.ApiGenerator.OpenApi.Extensions;

public static class OpenApiSchemaExtensions
{
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