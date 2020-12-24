using System;
using System.IO;
using System.Linq;
using McMaster.Extensions.CommandLineUtils;

namespace Atc.Rest.ApiGenerator.CLI
{
    public static class CommandLineApplicationHelper
    {
        public static bool GetVerboseMode(CommandLineApplication configCmd)
        {
            return TryGetValueForParameter(configCmd, "verboseMode", "v", out string parameterValueResult) &&
                   bool.TryParse(parameterValueResult, out bool result) &&
                   result;
        }

        public static string GetSpecificationPath(CommandLineApplication configCmd)
        {
            return GetValueForParameter(configCmd, "specificationPath");
        }

        public static string GetProjectPrefixName(CommandLineApplication configCmd)
        {
            return GetValueForParameter(configCmd, "projectPrefixName", "p");
        }

        public static DirectoryInfo GetOutputPath(CommandLineApplication configCmd)
        {
            return new DirectoryInfo(GetValueForParameter(configCmd, "outputPath", "o"));
        }

        public static DirectoryInfo GetApiPath(CommandLineApplication configCmd)
        {
            return new DirectoryInfo(GetValueForParameter(configCmd, "apiPath"));
        }

        public static DirectoryInfo GetDomainPath(CommandLineApplication configCmd)
        {
            return new DirectoryInfo(GetValueForParameter(configCmd, "domainPath"));
        }

        public static string GetOutputSlnPath(CommandLineApplication configCmd)
        {
            return GetValueForParameter(configCmd, "outputSlnPath");
        }

        public static DirectoryInfo GetOutputSrcPath(CommandLineApplication configCmd)
        {
            return new DirectoryInfo(GetValueForParameter(configCmd, "outputSrcPath"));
        }

        public static DirectoryInfo? GetOutputTestPath(CommandLineApplication configCmd)
        {
            return TryGetValueForParameter(configCmd, "outputTestPath", null, out string value)
                ? new DirectoryInfo(value)
                : null;
        }

        private static string GetValueForParameter(CommandLineApplication configCmd, string parameterName, string? shortParameterName = null)
        {
            if (TryGetValueForParameter(configCmd, parameterName, shortParameterName, out string value))
            {
                return value;
            }

            throw new ArgumentNullOrDefaultException(parameterName, $"Argument {parameterName} is not specified.");
        }

        private static bool TryGetValueForParameter(CommandLineApplication configCmd, string parameterName, string? shortParameterName, out string value)
        {
            if (configCmd == null)
            {
                throw new ArgumentNullException(nameof(configCmd));
            }

            if (parameterName == null)
            {
                throw new ArgumentNullException(nameof(parameterName));
            }

            var cmdOptionParameter = configCmd
                .GetOptions()
                .FirstOrDefault(x => x.LongName!.Equals(parameterName, StringComparison.OrdinalIgnoreCase));

            if (cmdOptionParameter == null && shortParameterName != null)
            {
                cmdOptionParameter = configCmd
                    .GetOptions()
                    .FirstOrDefault(x => x.ShortName!.Equals(shortParameterName, StringComparison.OrdinalIgnoreCase));
            }

            if (cmdOptionParameter == null || string.IsNullOrEmpty(cmdOptionParameter.Value()))
            {
                value = string.Empty;
                return false;
            }

            value = cmdOptionParameter.Value()!;
            return true;
        }
    }
}