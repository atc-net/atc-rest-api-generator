// ReSharper disable ReplaceSubstringWithRangeIndexer
// ReSharper disable InvertIf
namespace Atc.Rest.ApiGenerator.Contracts.Resolver;

public static class OpenApiDocumentSchemaModelNameResolver
{
    public static string GetRawModelName(
        string modelName)
    {
        if (string.IsNullOrEmpty(modelName))
        {
            return string.Empty;
        }

        var s = modelName;
        var indexEnd = s.IndexOf('>', StringComparison.Ordinal);
        if (indexEnd != -1)
        {
            s = s.Substring(0, indexEnd);
            s = s.Substring(s.IndexOf('<', StringComparison.Ordinal) + 1);
        }

        if (s.Contains('.', StringComparison.Ordinal))
        {
            s = s.Substring(s.LastIndexOf('.') + 1);
        }

        return s;
    }

    public static string EnsureModelNameWithNamespaceIfNeeded(
        string projectName,
        string apiGroupName,
        string modelName,
        bool isShared = false,
        bool isClient = false)
    {
        if (string.IsNullOrEmpty(modelName))
        {
            return string.Empty;
        }

        var isModelNameInNamespace = HasNamespaceRawModelName($"{projectName}.{apiGroupName}", modelName);

        if (isModelNameInNamespace)
        {
            return isClient
                ? $"{ContentGeneratorConstants.Contracts}.{apiGroupName}.{modelName}"
                : $"{projectName}.{ContentGeneratorConstants.Contracts}.{apiGroupName}.{modelName}";
        }

        if (!modelName.Contains('.', StringComparison.Ordinal) &&
            !Helpers.SimpleTypeHelper.IsSimpleType(modelName) &&
            IsReservedTypeName(modelName))
        {
            if (isShared)
            {
                return $"{projectName}.{ContentGeneratorConstants.Contracts}.{modelName}";
            }

            if (isClient)
            {
                return $"{ContentGeneratorConstants.Contracts}.{apiGroupName}.{modelName}";
            }

            return $"{projectName}.{ContentGeneratorConstants.Contracts}.{apiGroupName}.{modelName}";
        }

        return modelName;
    }

    private static bool HasNamespaceRawModelName(
        string namespacePart,
        string rawModelName)
        => namespacePart
            .Split('.', StringSplitOptions.RemoveEmptyEntries)
            .Any(s => s.Equals(rawModelName, StringComparison.Ordinal));

    private static bool IsReservedTypeName(
        string modelName)
    {
        if ("endpoint".Equals(modelName, StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        var exportedTypes = new List<Type>();
        var assemblies = AppDomain.CurrentDomain
            .GetAssemblies()
            .Where(x => x.FullName is not null &&
                        (x.FullName.StartsWith("System", StringComparison.Ordinal) ||
                         x.FullName.StartsWith("Microsoft", StringComparison.Ordinal)));

        foreach (var assembly in assemblies)
        {
            exportedTypes.AddRange(assembly.GetExportedTypes());
        }

        var rawModelName = GetRawModelName(modelName);
        return exportedTypes.Find(x => x.Name.Equals(rawModelName, StringComparison.Ordinal)) is not null;
    }
}