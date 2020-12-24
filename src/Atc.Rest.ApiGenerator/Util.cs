using System;
using System.IO;

namespace Atc.Rest.ApiGenerator
{
    public static class Util
    {
        public static DirectoryInfo GetProjectPath()
        {
            var currentDomainBaseDirectory = AppDomain.CurrentDomain.BaseDirectory;

            var projectPath = currentDomainBaseDirectory!
                .Replace("\\Bin", string.Empty, StringComparison.OrdinalIgnoreCase)
                .Replace("\\netcoreapp3.1", string.Empty, StringComparison.OrdinalIgnoreCase)
                .Replace("\\Debug", string.Empty, StringComparison.OrdinalIgnoreCase)
                .Replace("\\ApiGenerator", string.Empty, StringComparison.OrdinalIgnoreCase);

            return new DirectoryInfo(projectPath!);
        }

        public static string GetCsFileNameForEndpoints(
            DirectoryInfo pathForEndpoints,
            string modelName)
        {
            if (pathForEndpoints == null)
            {
                throw new ArgumentNullException(nameof(pathForEndpoints));
            }

            if (modelName == null)
            {
                throw new ArgumentNullException(nameof(modelName));
            }

            return Path.Combine(pathForEndpoints.FullName, $"{modelName}.cs");
        }

        public static string GetCsFileNameForContract(
            DirectoryInfo pathForContracts,
            string area,
            string subArea,
            string modelName)
        {
            if (pathForContracts == null)
            {
                throw new ArgumentNullException(nameof(pathForContracts));
            }

            if (area == null)
            {
                throw new ArgumentNullException(nameof(area));
            }

            if (subArea == null)
            {
                throw new ArgumentNullException(nameof(subArea));
            }

            if (modelName == null)
            {
                throw new ArgumentNullException(nameof(modelName));
            }

            var a = Path.Combine(pathForContracts.FullName, area);
            var b = Path.Combine(a, subArea);
            var c = Path.Combine(b, $"{modelName}.cs");
            return c;
        }

        public static string GetCsFileNameForContractEnumTypes(
            DirectoryInfo pathForContracts,
            string modelName)
        {
            if (pathForContracts == null)
            {
                throw new ArgumentNullException(nameof(pathForContracts));
            }

            if (modelName == null)
            {
                throw new ArgumentNullException(nameof(modelName));
            }

            var a = Path.Combine(pathForContracts.FullName, NameConstants.ContractsEnumerationTypes);
            var b = Path.Combine(a, $"{modelName}.cs");
            return b;
        }

        public static string GetCsFileNameForContractShared(
            DirectoryInfo pathForContracts,
            string modelName)
        {
            if (pathForContracts == null)
            {
                throw new ArgumentNullException(nameof(pathForContracts));
            }

            if (modelName == null)
            {
                throw new ArgumentNullException(nameof(modelName));
            }

            return Path.Combine(pathForContracts.FullName, $"{modelName}.cs");
        }

        public static string GetCsFileNameForHandler(
            DirectoryInfo pathForHandlers,
            string area,
            string handlerName)
        {
            if (pathForHandlers == null)
            {
                throw new ArgumentNullException(nameof(pathForHandlers));
            }

            if (area == null)
            {
                throw new ArgumentNullException(nameof(area));
            }

            if (handlerName == null)
            {
                throw new ArgumentNullException(nameof(handlerName));
            }

            var a = Path.Combine(pathForHandlers.FullName, area);
            var b = Path.Combine(a, $"{handlerName}.cs");
            return b;
        }
    }
}