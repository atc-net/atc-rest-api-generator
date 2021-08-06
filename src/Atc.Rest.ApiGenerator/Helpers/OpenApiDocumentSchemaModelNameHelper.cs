using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Atc.Rest.ApiGenerator.Models;
using Microsoft.OpenApi.Models;

// ReSharper disable ReplaceSubstringWithRangeIndexer
namespace Atc.Rest.ApiGenerator.Helpers
{
    public static class OpenApiDocumentSchemaModelNameHelper
    {
        public static string GetRawModelName(string modelName)
        {
            if (string.IsNullOrEmpty(modelName))
            {
                return string.Empty;
            }

            var strippedModelName = modelName;
            if (strippedModelName.Contains(Microsoft.OpenApi.Models.NameConstants.Pagination + "<", StringComparison.Ordinal) ||
                strippedModelName.Contains(Microsoft.OpenApi.Models.NameConstants.List + "<", StringComparison.Ordinal))
            {
                strippedModelName = strippedModelName.GetValueBetweenLessAndGreaterThanCharsIfExist();
            }

            if (strippedModelName.Contains('.', StringComparison.Ordinal))
            {
                strippedModelName = strippedModelName.Split('.', StringSplitOptions.RemoveEmptyEntries).Last();
            }

            return strippedModelName;
        }

        public static string EnsureModelNameWithNamespaceIfNeeded(EndpointMethodMetadata endpointMethodMetadata, string modelName)
        {
            if (endpointMethodMetadata == null)
            {
                throw new ArgumentNullException(nameof(endpointMethodMetadata));
            }

            return EnsureModelNameWithNamespaceIfNeeded(
                endpointMethodMetadata.ProjectName,
                endpointMethodMetadata.SegmentName,
                modelName);
        }

        public static string EnsureModelNameWithNamespaceIfNeeded(string projectName, string segmentName, string modelName, bool isShared = false)
        {
            if (string.IsNullOrEmpty(modelName))
            {
                return string.Empty;
            }

            var isModelNameInNamespace = HasNamespaceRawModelName($"{projectName}.{segmentName}", modelName);

            if (isModelNameInNamespace)
            {
                return $"{projectName}.{NameConstants.Contracts}.{segmentName}.{modelName}";
            }

            if (!modelName.Contains(".", StringComparison.Ordinal) && IsReservedSystemTypeName(modelName))
            {
                return $"{projectName}.{NameConstants.Contracts}.{segmentName}.{modelName}";
            }

            if (isShared)
            {
                // TO-DO: Maybe use it?..
            }

            return modelName;
        }

        public static bool ContainsModelNameTask(string modelName)
            => modelName.Equals("Task", StringComparison.Ordinal) ||
               modelName.EndsWith("Task>", StringComparison.Ordinal);

        public static string EnsureTaskNameWithNamespaceIfNeeded(string contractReturnTypeName)
            => ContainsModelNameTask(contractReturnTypeName)
                ? "System.Threading.Tasks.Task"
                : "Task";

        public static string EnsureTaskNameWithNameWithNeeded(List<Tuple<HttpStatusCode, string, OpenApiSchema?>> contractReturnTypeNames)
        {
            var useFullNamespace = contractReturnTypeNames.Any(x => ContainsModelNameTask(x.Item2));
            return useFullNamespace
                ? EnsureTaskNameWithNamespaceIfNeeded("Task")
                : "Task";
        }

        public static bool HasReservedSystemNameInContractReturnTypes(List<Tuple<HttpStatusCode, string, OpenApiSchema?>> contractReturnTypeNames)
            => contractReturnTypeNames.Any(x => IsReservedSystemTypeName(x.Item2));

        private static bool IsReservedSystemTypeName(string modelName)
        {
            // TODO: Optimize//Cache exportedTypes
            var exportedTypes = new List<Type>();
            var assemblies = AppDomain.CurrentDomain
                .GetAssemblies()
                .Where(x => x.FullName.StartsWith("System", StringComparison.Ordinal));

            foreach (var assembly in assemblies)
            {
                exportedTypes.AddRange(assembly.GetExportedTypes());
            }

            var rawModelName = modelName;
            if (ContractHelper.HasList(modelName))
            {
                rawModelName = modelName
                    .Replace(Microsoft.OpenApi.Models.NameConstants.List + "<", string.Empty, StringComparison.Ordinal)
                    .Replace(">", string.Empty, StringComparison.Ordinal);
            }

            if (rawModelName.Contains(".", StringComparison.Ordinal))
            {
                rawModelName = rawModelName.Substring(rawModelName.LastIndexOf('.') + 1);
            }

            return exportedTypes.Find(x => x.Name.Equals(rawModelName, StringComparison.Ordinal)) is not null;
        }

        private static bool HasNamespaceRawModelName(string namespacePart, string rawModelName)
            => namespacePart
                .Split('.', StringSplitOptions.RemoveEmptyEntries)
                .Any(s => s.Equals(rawModelName, StringComparison.Ordinal));
    }
}