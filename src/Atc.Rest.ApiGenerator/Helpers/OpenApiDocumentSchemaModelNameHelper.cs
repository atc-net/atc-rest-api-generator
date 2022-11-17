// ReSharper disable ReplaceSubstringWithRangeIndexer
namespace Atc.Rest.ApiGenerator.Helpers;

//// TODO: Die after moved to OpenApiDocumentSchemaModelNameResolver
public static class OpenApiDocumentSchemaModelNameHelper
{
    public static bool ContainsModelNameTask(string modelName)
    {
        ArgumentNullException.ThrowIfNull(modelName);

        return modelName.Equals("Task", StringComparison.Ordinal) ||
               modelName.EndsWith("Task>", StringComparison.Ordinal);
    }

    public static string EnsureModelNameWithNamespaceIfNeeded(
        EndpointMethodMetadata endpointMethodMetadata,
        string modelName)
    {
        ArgumentNullException.ThrowIfNull(endpointMethodMetadata);

        return OpenApiDocumentSchemaModelNameResolver.EnsureModelNameWithNamespaceIfNeeded(
            endpointMethodMetadata.ProjectName,
            endpointMethodMetadata.SegmentName,
            modelName,
            endpointMethodMetadata.IsSharedModel(modelName));
    }

    public static string EnsureTaskNameWithNamespaceIfNeeded(
        ResponseTypeNameAndItemSchema contractReturnTypeNameAndSchema)
    {
        ArgumentNullException.ThrowIfNull(contractReturnTypeNameAndSchema);

        return ContainsModelNameTask(contractReturnTypeNameAndSchema.FullModelName) ||
               (contractReturnTypeNameAndSchema.HasSchema &&
                contractReturnTypeNameAndSchema.Schema!.HasModelNameOrAnyPropertiesWithModelName("Task"))
            ? "System.Threading.Tasks.Task"
            : "Task";
    }
}