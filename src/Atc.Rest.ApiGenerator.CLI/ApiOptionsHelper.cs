using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using Atc.Rest.ApiGenerator.Models.ApiOptions;
using McMaster.Extensions.CommandLineUtils;

namespace Atc.Rest.ApiGenerator.CLI
{
    public static class ApiOptionsHelper
    {
        public static ApiOptions CreateDefault(CommandLineApplication configCmd)
        {
            if (configCmd == null)
            {
                throw new ArgumentNullException(nameof(configCmd));
            }

            var cmdOptionOptionsPath = configCmd
                .GetOptions()
                .FirstOrDefault(x => x.LongName!.Equals("optionsPath", StringComparison.OrdinalIgnoreCase));

            var apiOptions = new ApiOptions();

            if (cmdOptionOptionsPath == null || string.IsNullOrEmpty(cmdOptionOptionsPath.Value()))
            {
                return apiOptions;
            }

            var optionsPath = cmdOptionOptionsPath.Value()!;

            var fileInfo = optionsPath.EndsWith(".json", StringComparison.Ordinal)
                ? new FileInfo(optionsPath)
                : new FileInfo(Path.Combine(optionsPath, "ApiGeneratorOptions.json"));

            if (!fileInfo.Exists)
            {
                return apiOptions;
            }

            var serializeOptions = new JsonSerializerOptions();
            serializeOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
            serializeOptions.WriteIndented = true;

            using var stream = new StreamReader(fileInfo.FullName);
            var json = stream.ReadToEnd();
            apiOptions = JsonSerializer.Deserialize<ApiOptions>(json, serializeOptions);

            return apiOptions;
        }

        public static void ApplyValidationOverrides(ApiOptions apiOptions, CommandLineApplication configCmd)
        {
            if (apiOptions == null)
            {
                throw new ArgumentNullException(nameof(apiOptions));
            }

            if (configCmd == null)
            {
                throw new ArgumentNullException(nameof(configCmd));
            }

            var cmdOptionStrictMode = configCmd
                .GetOptions()
                .FirstOrDefault(x => x.LongName!.EndsWith("strictMode", StringComparison.OrdinalIgnoreCase));
            if (cmdOptionStrictMode != null && !string.IsNullOrEmpty(cmdOptionStrictMode.Value()))
            {
                apiOptions.Validation.StrictMode = bool.Parse(cmdOptionStrictMode.Value()!);
            }

            var cmdOptionOperationIdCasingStyle = configCmd
                .GetOptions()
                .FirstOrDefault(x => x.LongName!.EndsWith("operationIdCasingStyle", StringComparison.OrdinalIgnoreCase));
            if (cmdOptionOperationIdCasingStyle != null && !string.IsNullOrEmpty(cmdOptionOperationIdCasingStyle.Value()))
            {
                apiOptions.Validation.OperationIdCasingStyle = Enum<CasingStyle>.Parse(cmdOptionOperationIdCasingStyle.Value()!);
            }

            var cmdOptionModelNameCasingStyle = configCmd
                .GetOptions()
                .FirstOrDefault(x => x.LongName!.EndsWith("modelNameCasingStyle", StringComparison.OrdinalIgnoreCase));
            if (cmdOptionModelNameCasingStyle != null && !string.IsNullOrEmpty(cmdOptionModelNameCasingStyle.Value()))
            {
                apiOptions.Validation.ModelNameCasingStyle = Enum<CasingStyle>.Parse(cmdOptionModelNameCasingStyle.Value()!);
            }

            var cmdOptionModelPropertyNameCasingStyle = configCmd
                .GetOptions()
                .FirstOrDefault(x => x.LongName!.EndsWith("modelPropertyNameCasingStyle", StringComparison.OrdinalIgnoreCase));
            if (cmdOptionModelPropertyNameCasingStyle != null && !string.IsNullOrEmpty(cmdOptionModelPropertyNameCasingStyle.Value()))
            {
                apiOptions.Validation.ModelPropertyNameCasingStyle = Enum<CasingStyle>.Parse(cmdOptionModelPropertyNameCasingStyle.Value()!);
            }
        }

        public static void ApplyGeneratorOverrides(ApiOptions apiOptions, CommandLineApplication configCmd)
        {
            if (apiOptions == null)
            {
                throw new ArgumentNullException(nameof(apiOptions));
            }

            if (configCmd == null)
            {
                throw new ArgumentNullException(nameof(configCmd));
            }

            var cmdOptionUseNullableReferenceTypes = configCmd
                .GetOptions()
                .FirstOrDefault(x => x.LongName!.EndsWith("useNullableReferenceTypes", StringComparison.OrdinalIgnoreCase));
            if (cmdOptionUseNullableReferenceTypes != null && !string.IsNullOrEmpty(cmdOptionUseNullableReferenceTypes.Value()))
            {
                apiOptions.Generator.UseNullableReferenceTypes = bool.Parse(cmdOptionUseNullableReferenceTypes.Value()!);
            }

            var cmdOptionUseAuthorization = configCmd
                .GetOptions()
                .FirstOrDefault(x => x.LongName!.EndsWith("useAuthorization", StringComparison.OrdinalIgnoreCase));
            if (cmdOptionUseAuthorization != null && !string.IsNullOrEmpty(cmdOptionUseAuthorization.Value()))
            {
                apiOptions.Generator.UseAuthorization = bool.Parse(cmdOptionUseAuthorization.Value()!);
            }
        }
    }
}