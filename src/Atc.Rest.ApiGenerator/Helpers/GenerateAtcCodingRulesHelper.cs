using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Atc.Data.Models;

// ReSharper disable SuggestBaseTypeForParameter
// ReSharper disable InvertIf
namespace Atc.Rest.ApiGenerator.Helpers
{
    public static class GenerateAtcCodingRulesHelper
    {
        private const string RawCodingRulesDistribution = "https://raw.githubusercontent.com/atc-net/atc-coding-rules/main/distribution";
        public const string FileNameEditorConfig = ".editorconfig";

        public static IEnumerable<LogKeyValueItem> Generate(
            string outputSlnPath,
            DirectoryInfo outputSrcPath,
            DirectoryInfo? outputTestPath)
        {
            if (outputSlnPath == null)
            {
                throw new ArgumentNullException(nameof(outputSlnPath));
            }

            if (outputSrcPath == null)
            {
                throw new ArgumentNullException(nameof(outputSrcPath));
            }

            var logItems = new List<LogKeyValueItem>();

            var rootPath = outputSlnPath.EndsWith(".sln", StringComparison.OrdinalIgnoreCase)
                ? new FileInfo(outputSlnPath).Directory
                : new DirectoryInfo(outputSlnPath);

            if (rootPath == null)
            {
                throw new IOException("Invalid outputSlnPath");
            }

            if (IsFirstTime(rootPath))
            {
                // atc-coding-rules
                logItems.Add(HandleAtcCodingRulesJson(rootPath, outputSrcPath, outputTestPath));
                logItems.Add(HandleAtcCodingRulesPowerShell(rootPath));

                // -> build folder
                logItems.AddRange(HandleBuildPropsFiles(new DirectoryInfo(Path.Combine(rootPath.FullName, "build"))));

                // -> root .editorConfig
                logItems.Add(HandleFileEditorConfig(rootPath, "root", string.Empty));

                // -> src folder -> 3 files
                logItems.Add(HandleFileEditorConfig(outputSrcPath, "src", "src"));
                logItems.Add(HandleDirectoryBuildPropsFile(outputSrcPath, "src", "src"));
                logItems.Add(HandleDirectoryBuildTargetsFile(outputSrcPath, "src", "src"));

                if (outputTestPath != null)
                {
                    // -> test folder -> 3 files
                    logItems.Add(HandleFileEditorConfig(outputTestPath, "test", "test"));
                    logItems.Add(HandleDirectoryBuildPropsFile(outputTestPath, "test", "test"));
                    logItems.Add(HandleDirectoryBuildTargetsFile(outputTestPath, "test", "test"));
                }
            }

            return logItems;
        }

        private static bool IsFirstTime(DirectoryInfo rootPath)
        {
            var file = new FileInfo(Path.Combine(rootPath.FullName, FileNameEditorConfig));
            return !file.Exists;
        }

        private static LogKeyValueItem HandleAtcCodingRulesJson(
            DirectoryInfo path,
            DirectoryInfo outputSrcPath,
            DirectoryInfo? outputTestPath)
        {
            const string file = "atc-coding-rules-updater.json";
            var filePath = Path.Combine(path.FullName, file);
            try
            {
                var sb = new StringBuilder();
                sb.AppendLine("{");
                sb.AppendLine("  \"Mappings\": {");
                sb.Append("    \"Src\": { \"Paths\": [ \"");
                if (outputTestPath == null)
                {
                    sb.Append(outputSrcPath.FullName.Replace("\\", "\\\\", StringComparison.Ordinal));
                    sb.AppendLine("\" ] }");
                }
                else
                {
                    sb.Append(outputSrcPath.FullName.Replace("\\", "\\\\", StringComparison.Ordinal));
                    sb.AppendLine("\" ] },");
                    sb.Append("    \"Test\": { \"Paths\": [ \"");
                    sb.Append(outputTestPath.FullName.Replace("\\", "\\\\", StringComparison.Ordinal));
                }

                sb.AppendLine("\" ] }");
                sb.AppendLine("  }");
                sb.AppendLine("}");
                File.WriteAllText(filePath, sb.ToString());
                return new LogKeyValueItem(LogCategoryType.Debug, "FileCreate", $"{file} created");
            }
            catch (Exception ex)
            {
                return new LogKeyValueItem(LogCategoryType.Error, "FileSkip", $"atc-coding-rules-updater.json - {ex.Message}");
            }
        }

        private static LogKeyValueItem HandleAtcCodingRulesPowerShell(DirectoryInfo path)
        {
            const string file = "atc-coding-rules-updater.ps1";
            var filePath = Path.Combine(path.FullName, file);
            try
            {
                var sb = new StringBuilder();
                sb.AppendLine("Clear-Host");
                sb.AppendLine("Write-Host \"Update atc-coding-rules\"");
                sb.AppendLine("dotnet tool update -g atc-coding-rules-updater");
                sb.AppendLine();
                sb.AppendLine("$currentPath = Get-Location");
                sb.AppendLine();
                sb.AppendLine("atc-coding-rules-updater `");
                sb.AppendLine("-r $currentPath `");
                sb.AppendLine("--optionsPath $currentPath'\\atc-coding-rules-updater.json' `");
                sb.AppendLine("-v true");
                File.WriteAllText(file, sb.ToString());
                File.WriteAllText(filePath, sb.ToString());
                return new LogKeyValueItem(LogCategoryType.Debug, "FileCreate", $"{file} created");
            }
            catch (Exception ex)
            {
                return new LogKeyValueItem(LogCategoryType.Error, "FileSkip", $"atc-coding-rules-updater.json - {ex.Message}");
            }
        }

        private static IEnumerable<LogKeyValueItem> HandleBuildPropsFiles(DirectoryInfo path)
        {
            try
            {
                Directory.CreateDirectory(path.FullName);

                var rawCommonPropsData = HttpClientHelper.GetRawFile($"{RawCodingRulesDistribution}/build/common.props");
                File.WriteAllText(Path.Combine(path.FullName, "common.props"), rawCommonPropsData);

                var rawCodeAnalysisPropsData = HttpClientHelper.GetRawFile($"{RawCodingRulesDistribution}/build/code-analysis.props");
                File.WriteAllText(Path.Combine(path.FullName, "code-analysis.props"), rawCodeAnalysisPropsData);

                return new List<LogKeyValueItem>
                    {
                        new LogKeyValueItem(LogCategoryType.Information, "FileUpdate", "common.props updated - Remember to change the CompanyName in the file"),
                        new LogKeyValueItem(LogCategoryType.Debug, "FileUpdate", "code-analysis.props updated"),
                    };
            }
            catch (Exception ex)
            {
                return new List<LogKeyValueItem>
                {
                    new LogKeyValueItem(LogCategoryType.Error, "FileSkip", $"build folder skipped - {ex.Message}"),
                };
            }
        }

        private static LogKeyValueItem HandleFileEditorConfig(
            DirectoryInfo path,
            string area,
            string urlPart)
        {
            var descriptionPart = string.IsNullOrEmpty(urlPart)
                ? FileNameEditorConfig
                : $"{urlPart}/{FileNameEditorConfig}";

            var file = new FileInfo(Path.Combine(path.FullName, FileNameEditorConfig));

            var rawGitUrl = string.IsNullOrEmpty(urlPart)
                ? $"{RawCodingRulesDistribution}/{FileNameEditorConfig}"
                : $"{RawCodingRulesDistribution}/{urlPart}/{FileNameEditorConfig}";

            try
            {
                if (!file.Directory!.Exists)
                {
                    Directory.CreateDirectory(file.Directory.FullName);
                }

                var rawEditorConfig = HttpClientHelper.GetRawFile(rawGitUrl);
                File.WriteAllText(file.FullName, rawEditorConfig);
                return new LogKeyValueItem(LogCategoryType.Debug, "FileCreate", $"{area} - {descriptionPart} created");
            }
            catch (Exception ex)
            {
                return new LogKeyValueItem(LogCategoryType.Error, "FileSkip", $"{area} - {descriptionPart} skipped - {ex.Message}");
            }
        }

        private static LogKeyValueItem HandleDirectoryBuildPropsFile(
            DirectoryInfo path,
            string area,
            string urlPart)
        {
            var descriptionPart = string.IsNullOrEmpty(urlPart)
                ? "directory.build.props"
                : $"{urlPart}/directory.build.props";

            var file = new FileInfo(Path.Combine(path.FullName, "directory.build.props"));

            var rawGitUrl = string.IsNullOrEmpty(urlPart)
                ? $"{RawCodingRulesDistribution}/directory.build.props"
                : $"{RawCodingRulesDistribution}/{urlPart}/directory.build.props";

            return HandleDirectoryBuildFile(area, file, descriptionPart, rawGitUrl);
        }

        private static LogKeyValueItem HandleDirectoryBuildTargetsFile(
            DirectoryInfo path,
            string area,
            string urlPart)
        {
            var descriptionPart = string.IsNullOrEmpty(urlPart)
                ? "directory.build.targets"
                : $"{urlPart}/directory.build.targets";

            var file = new FileInfo(Path.Combine(path.FullName, "directory.build.targets"));

            var rawGitUrl = string.IsNullOrEmpty(urlPart)
                ? $"{RawCodingRulesDistribution}/directory.build.targets"
                : $"{RawCodingRulesDistribution}/{urlPart}/directory.build.targets";

            return HandleDirectoryBuildFile(area, file, descriptionPart, rawGitUrl);
        }

        private static LogKeyValueItem HandleDirectoryBuildFile(
            string area,
            FileInfo file,
            string descriptionPart,
            string rawGitUrl)
        {
            try
            {
                if (!file.Directory!.Exists)
                {
                    Directory.CreateDirectory(file.Directory.FullName);
                }

                var rawGitData = HttpClientHelper.GetRawFile(rawGitUrl);
                File.WriteAllText(file.FullName, rawGitData);
                return new LogKeyValueItem(LogCategoryType.Debug, "FileCreate", $"{area} - {descriptionPart} created");
            }
            catch (Exception ex)
            {
                return new LogKeyValueItem(LogCategoryType.Error, "FileSkip", $"{area} folder skipped - {ex.Message}");
            }
        }
    }
}