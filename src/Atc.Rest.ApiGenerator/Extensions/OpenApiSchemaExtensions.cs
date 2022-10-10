// ReSharper disable once CheckNamespace
namespace Microsoft.OpenApi.Models;

[ExcludeFromCodeCoverage]
public static class OpenApiSchemaExtensions
{
    public static bool HasAnySharedModel(
        this OpenApiSchema schema,
        List<ApiOperation> apiOperationSchemaMaps)
    {
        ArgumentNullException.ThrowIfNull(schema);

        if (!schema.IsObjectReferenceTypeDeclared())
        {
            return false;
        }

        var modelName = schema.GetModelName();
        if (apiOperationSchemaMaps.IsShared(modelName))
        {
            return true;
        }

        return schema.HasAnyProperties() &&
               schema.Properties.Any(x => HasAnySharedModel(x.Value, apiOperationSchemaMaps));
    }

    public static bool HasAnySharedModelOrEnum(
        this OpenApiSchema schema,
        IList<ApiOperation> apiOperationSchemaMaps,
        bool includeProperties = true)
    {
        ArgumentNullException.ThrowIfNull(schema);

        if (!schema.IsObjectReferenceTypeDeclared())
        {
            return false;
        }

        if (schema.IsSchemaEnumOrPropertyEnum())
        {
            return true;
        }

        var modelName = schema.GetModelName();
        if (apiOperationSchemaMaps.IsShared(modelName))
        {
            return true;
        }

        if (!includeProperties)
        {
            return false;
        }

        if (!schema.HasAnyProperties())
        {
            return false;
        }

        foreach (var (_, openApiSchema) in schema.Properties)
        {
            if (openApiSchema.HasAnySharedModelOrEnum(apiOperationSchemaMaps))
            {
                return true;
            }

            if (openApiSchema.OneOf is not null &&
                openApiSchema.OneOf.Count > 0 &&
                openApiSchema.OneOf.Any(x => x.HasAnySharedModelOrEnum(apiOperationSchemaMaps)))
            {
                return true;
            }

            if (openApiSchema.IsArrayReferenceTypeDeclared() &&
                openApiSchema.Items.HasAnySharedModelOrEnum(apiOperationSchemaMaps))
            {
                return true;
            }
        }

        return false;
    }
}