using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Atc.Rest.ApiGenerator.Extensions;
using Atc.Rest.ApiGenerator.Models;
using Atc.Rest.ApiGenerator.SyntaxGenerators;
using Microsoft.OpenApi.Models;

// ReSharper disable ReplaceSubstringWithRangeIndexer
namespace Atc.Rest.ApiGenerator.Helpers
{
    public static class OpenApiDocumentSchemaModelNameHelper
    {
        public static bool ContainsModelNameTask(string modelName)
            => modelName.Equals("Task", StringComparison.Ordinal) ||
               modelName.EndsWith("Task>", StringComparison.Ordinal);

        public static string GetRawModelName(string modelName)
        {
            if (string.IsNullOrEmpty(modelName))
            {
                return string.Empty;
            }

            var s = modelName;
            var indexEnd = s.IndexOf(">", StringComparison.Ordinal);
            if (indexEnd != -1)
            {
                s = s.Substring(0, indexEnd);
                s = s.Substring(s.IndexOf("<", StringComparison.Ordinal) + 1);
            }

            if (s.Contains(".", StringComparison.Ordinal))
            {
                s = s.Substring(s.LastIndexOf(".", StringComparison.Ordinal) + 1);
            }

            return s;
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
                modelName,
                endpointMethodMetadata.IsSharedModel(modelName));
        }

        public static string EnsureModelNameWithNamespaceIfNeeded(
            string projectName,
            string segmentName,
            string modelName,
            bool isShared = false,
            bool isClient = false)
        {
            if (string.IsNullOrEmpty(modelName))
            {
                return string.Empty;
            }

            var isModelNameInNamespace = HasNamespaceRawModelName($"{projectName}.{segmentName}", modelName);

            if (isModelNameInNamespace)
            {
                return isClient
                    ? modelName
                    : $"{projectName}.{NameConstants.Contracts}.{segmentName}.{modelName}";
            }

            if (!modelName.Contains(".", StringComparison.Ordinal) && IsReservedTypeName(modelName))
            {
                if (isShared)
                {
                    return $"{projectName}.{NameConstants.Contracts}.{modelName}";
                }

                if (isClient)
                {
                    return $"{NameConstants.Contracts}.{modelName}";
                }

                return $"{projectName}.{NameConstants.Contracts}.{segmentName}.{modelName}";
            }

            return modelName;
        }

        public static string EnsureTaskNameWithNamespaceIfNeeded(ResponseTypeNameAndItemSchema contractReturnTypeNameAndSchema)
            => ContainsModelNameTask(contractReturnTypeNameAndSchema.FullModelName) ||
               (contractReturnTypeNameAndSchema.HasSchema &&
                contractReturnTypeNameAndSchema.Schema!.HasModelNameOrAnyPropertiesWithModelName("Task"))
                ? "System.Threading.Tasks.Task"
                : "Task";

        public static bool HasSharedResponseContract(
            OpenApiDocument document,
            List<ApiOperationSchemaMap> operationSchemaMappings,
            string focusOnSegmentName)
        {
            if (document == null)
            {
                throw new ArgumentNullException(nameof(document));
            }

            if (operationSchemaMappings == null)
            {
                throw new ArgumentNullException(nameof(operationSchemaMappings));
            }

            if (focusOnSegmentName == null)
            {
                throw new ArgumentNullException(nameof(focusOnSegmentName));
            }

            foreach (var (_, value) in document.GetPathsByBasePathSegmentName(focusOnSegmentName))
            {
                foreach (var apiOperation in value.Operations)
                {
                    if (apiOperation.Value.Responses == null)
                    {
                        continue;
                    }

                    var responseModelName = apiOperation.Value.Responses.GetModelNameForStatusCode(HttpStatusCode.OK);
                    var isSharedResponseModel = !string.IsNullOrEmpty(responseModelName) &&
                                                operationSchemaMappings.IsShared(responseModelName);
                    if (isSharedResponseModel)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static bool HasList(string typeName)
        {
            return !string.IsNullOrEmpty(typeName) &&
                   typeName.Contains(Microsoft.OpenApi.Models.NameConstants.List + "<", StringComparison.Ordinal);
        }

        private static bool HasNamespaceRawModelName(string namespacePart, string rawModelName)
            => namespacePart
                .Split('.', StringSplitOptions.RemoveEmptyEntries)
                .Any(s => s.Equals(rawModelName, StringComparison.Ordinal));

        private static bool IsReservedTypeName(string modelName)
        {
            if ("endpoint".Equals(modelName, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            var exportedTypes = new List<Type>();
            var assemblies = AppDomain.CurrentDomain
                .GetAssemblies()
                .Where(x => x.FullName.StartsWith("System", StringComparison.Ordinal) ||
                            x.FullName.StartsWith("Microsoft", StringComparison.Ordinal));

            foreach (var assembly in assemblies)
            {
                exportedTypes.AddRange(assembly.GetExportedTypes());
            }

            var rawModelName = GetRawModelName(modelName);
            return exportedTypes.Find(x => x.Name.Equals(rawModelName, StringComparison.Ordinal)) is not null;
        }
    }
}