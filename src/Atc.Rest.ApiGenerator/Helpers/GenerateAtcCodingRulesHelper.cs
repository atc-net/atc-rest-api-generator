// ReSharper disable SuggestBaseTypeForParameter
// ReSharper disable InvertIf
namespace Atc.Rest.ApiGenerator.Helpers;

public static class GenerateAtcCodingRulesHelper
{
    private const string RawCodingRulesDistribution = "https://raw.githubusercontent.com/atc-net/atc-coding-rules/main/distribution/dotnetcore";
    public const string FileNameEditorConfig = ".editorconfig";
    public const string FileNameDirectoryBuildProps = "Directory.Build.props";

    public static IEnumerable<LogKeyValueItem> Generate(
        string outputSlnPath,
        DirectoryInfo outputSrcPath,
        DirectoryInfo? outputTestPath)
    {
        ArgumentNullException.ThrowIfNull(outputSlnPath);
        ArgumentNullException.ThrowIfNull(outputSrcPath);

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

            // -> root
            logItems.Add(HandleFileEditorConfig(rootPath, "root", string.Empty));
            logItems.Add(HandleFileDirectoryBuildProps(rootPath, "root", string.Empty));

            // -> src
            if (rootPath.FullName != outputSrcPath.FullName)
            {
                logItems.Add(HandleFileEditorConfig(outputSrcPath, "src", "src"));
                logItems.Add(HandleFileDirectoryBuildProps(outputSrcPath, "src", "src"));
            }

            // -> test
            if (outputTestPath != null && rootPath.FullName != outputTestPath.FullName)
            {
                logItems.Add(HandleFileEditorConfig(outputTestPath, "test", "test"));
                logItems.Add(HandleFileDirectoryBuildProps(outputTestPath, "test", "test"));
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
            sb.AppendLine("  \"projectTarget\": \"DotNet5\",");
            sb.AppendLine("  \"mappings\": {");
            sb.Append("    \"src\": { \"paths\": [ \"");
            if (outputTestPath == null)
            {
                sb.Append(outputSrcPath.FullName.Replace("\\", "\\\\", StringComparison.Ordinal));
                sb.AppendLine("\" ] }");
            }
            else
            {
                sb.Append(outputSrcPath.FullName.Replace("\\", "\\\\", StringComparison.Ordinal));
                sb.AppendLine("\" ] },");
                sb.Append("    \"test\": { \"paths\": [ \"");
                sb.Append(outputTestPath.FullName.Replace("\\", "\\\\", StringComparison.Ordinal));
            }

            sb.AppendLine("\" ] }");
            sb.AppendLine("  }");
            sb.AppendLine("}");
            File.WriteAllText(filePath, sb.ToString());
            return LogItemFactory.CreateDebug("FileCreate", $"{file} created");
        }
        catch (Exception ex)
        {
            return LogItemFactory.CreateError("FileSkip", $"atc-coding-rules-updater.json - {ex.Message}");
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
            return LogItemFactory.CreateDebug("FileCreate", $"{file} created");
        }
        catch (Exception ex)
        {
            return LogItemFactory.CreateError("FileSkip", $"atc-coding-rules-updater.json - {ex.Message}");
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
            rawEditorConfig += $"{Environment.NewLine}dotnet_diagnostic.IDE0058.severity = none           # Have to override this for now - to get smoke-test to run";
            File.WriteAllText(file.FullName, rawEditorConfig);
            return LogItemFactory.CreateDebug("FileCreate", $"{area} - {descriptionPart} created");
        }
        catch (Exception ex)
        {
            return LogItemFactory.CreateError("FileSkip", $"{area} - {descriptionPart} skipped - {ex.Message}");
        }
    }

    private static LogKeyValueItem HandleFileDirectoryBuildProps(
        DirectoryInfo path,
        string area,
        string urlPart)
    {
        var descriptionPart = string.IsNullOrEmpty(urlPart)
            ? FileNameDirectoryBuildProps
            : $"{urlPart}/{FileNameDirectoryBuildProps}";

        var file = new FileInfo(Path.Combine(path.FullName, FileNameDirectoryBuildProps));

        var rawGitUrl = string.IsNullOrEmpty(urlPart)
            ? $"{RawCodingRulesDistribution}/{FileNameDirectoryBuildProps}"
            : $"{RawCodingRulesDistribution}/{urlPart}/{FileNameDirectoryBuildProps}";

        try
        {
            if (!file.Directory!.Exists)
            {
                Directory.CreateDirectory(file.Directory.FullName);
            }

            var rawDirectoryBuildProps = HttpClientHelper.GetRawFile(rawGitUrl);
            File.WriteAllText(file.FullName, rawDirectoryBuildProps);
            return LogItemFactory.CreateDebug("FileCreate", $"{area} - {descriptionPart} created");
        }
        catch (Exception ex)
        {
            return LogItemFactory.CreateError("FileSkip", $"{area} - {descriptionPart} skipped - {ex.Message}");
        }
    }
}