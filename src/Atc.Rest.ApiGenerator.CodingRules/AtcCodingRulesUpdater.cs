// ReSharper disable SuggestBaseTypeForParameter
namespace Atc.Rest.ApiGenerator.CodingRules;

/// <summary>
/// The main AtcCodingRulesUpdater - Handles call execution.
/// </summary>
public sealed partial class AtcCodingRulesUpdater : IAtcCodingRulesUpdater
{
    public const string GitRawContentUrl = "https://raw.githubusercontent.com";
    public const string GitHubPrefix = "[silver][[GitHub]][/] ";

    private const string RawCodingRulesDistributionUrl = "https://raw.githubusercontent.com/atc-net/atc-coding-rules/main/distribution/dotnet9";
    public const string FileNameEditorConfig = ".editorconfig";
    public const string FileNameDirectoryBuildProps = "Directory.Build.props";

    public AtcCodingRulesUpdater(
        ILogger<AtcCodingRulesUpdater> logger)
    {
        this.logger = logger;
    }

    public bool Scaffold(
        string slnPath,
        DirectoryInfo srcPath,
        DirectoryInfo? testPath)
    {
        var rootPath = slnPath.EndsWith(".sln", StringComparison.OrdinalIgnoreCase)
            ? new FileInfo(slnPath).Directory
            : new DirectoryInfo(slnPath);

        if (rootPath is null)
        {
            throw new IOException("Invalid outputSlnPath");
        }

        if (!IsFirstTime(rootPath))
        {
            return true;
        }

        HandleCodingRulesFiles(srcPath, testPath, rootPath);
        HandleEditorConfigFiles(srcPath, testPath, rootPath);
        HandleDirectoryBuildPropsFiles(srcPath, testPath, rootPath);

        return true;
    }

    private static bool IsFirstTime(
        DirectoryInfo rootPath)
        => !rootPath.CombineFileInfo(FileNameEditorConfig).Exists;

    private void HandleCodingRulesFiles(
        DirectoryInfo outputSrcPath,
        DirectoryInfo? outputTestPath,
        DirectoryInfo rootPath)
    {
        LogWorkingOnCodingRules($"{CodingRulesConstants.AreaCodingRules} ");
        HandleAtcCodingRulesJson(rootPath, outputSrcPath, outputTestPath);
        HandleAtcCodingRulesPowerShell(rootPath);
    }

    private void HandleEditorConfigFiles(
        DirectoryInfo outputSrcPath,
        DirectoryInfo? outputTestPath,
        DirectoryInfo rootPath)
    {
        LogWorkingOnEditorConfigFiles($"{CodingRulesConstants.AreaEditorConfig} ");
        HandleFileEditorConfig(rootPath, "root", string.Empty);

        if (rootPath.FullName != outputSrcPath.FullName)
        {
            HandleFileEditorConfig(outputSrcPath, "src", "src");
        }

        if (outputTestPath is not null &&
            rootPath.FullName != outputTestPath.FullName)
        {
            HandleFileEditorConfig(outputTestPath, "test", "test");
        }
    }

    private void HandleDirectoryBuildPropsFiles(
        DirectoryInfo outputSrcPath,
        DirectoryInfo? outputTestPath,
        DirectoryInfo rootPath)
    {
        LogWorkingOnDirectoryBuildPropsFiles($"{CodingRulesConstants.AreaDirectoryBuildProps} ");
        HandleFileDirectoryBuildProps(rootPath, "root", string.Empty);

        if (rootPath.FullName != outputSrcPath.FullName)
        {
            HandleFileDirectoryBuildProps(outputSrcPath, "src", "src");
        }

        if (outputTestPath is not null &&
            rootPath.FullName != outputTestPath.FullName)
        {
            HandleFileDirectoryBuildProps(outputTestPath, "test", "test");
        }
    }

    private void HandleAtcCodingRulesJson(
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
            sb.AppendLine("  \"projectTarget\": \"Dotnet9\",");
            sb.AppendLine("  \"mappings\": {");
            sb.Append("    \"src\": { \"paths\": [ \"");

            if (outputTestPath is null)
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
            LogFileCreated($"{CodingRulesConstants.LogFileCreated}   ", "root", file);
        }
        catch (IOException ex)
        {
            LogFileSkipped($"{CodingRulesConstants.LogError}   ", "root", file, ex.GetLastInnerMessage());
        }
    }

    private void HandleAtcCodingRulesPowerShell(
        DirectoryInfo path)
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
            sb.AppendLine("atc-coding-rules-updater run `");
            sb.AppendLine("  -p $currentPath `");
            sb.AppendLine("  --optionsPath $currentPath'\\atc-coding-rules-updater.json' `");
            sb.AppendLine("  --verbose");
            File.WriteAllText(file, sb.ToString());
            File.WriteAllText(filePath, sb.ToString());
            logger.LogDebug($"{CodingRulesConstants.LogFileCreated}   root: {file} created");
        }
        catch (IOException ex)
        {
            logger.LogError($"{CodingRulesConstants.LogError}   root: {file} skipped - {ex.Message}");
        }
    }

    private void HandleFileEditorConfig(
        DirectoryInfo path,
        string area,
        string urlPart)
    {
        var file = path.CombineFileInfo(FileNameEditorConfig);

        var rawGitUrl = string.IsNullOrEmpty(urlPart)
            ? $"{RawCodingRulesDistributionUrl}/{FileNameEditorConfig}"
            : $"{RawCodingRulesDistributionUrl}/{urlPart}/{FileNameEditorConfig}";

        try
        {
            if (!file.Directory!.Exists)
            {
                Directory.CreateDirectory(file.Directory.FullName);
            }

            var displayName = rawGitUrl.Replace(GitRawContentUrl, GitHubPrefix, StringComparison.Ordinal);
            var rawEditorConfig = HttpClientHelper.GetAsString(rawGitUrl, displayName).TrimEndForEmptyLines();

            rawEditorConfig += $"{Environment.NewLine}dotnet_diagnostic.IDE0058.severity = none           # Have to override this for now - to get smoke-test to run";
            File.WriteAllText(file.FullName, rawEditorConfig);
            LogFileCreated($"{CodingRulesConstants.LogFileCreated}   ", area, FileNameEditorConfig);
        }
        catch (IOException ex)
        {
            LogFileSkipped($"{CodingRulesConstants.LogError}   ", area, FileNameEditorConfig, ex.GetLastInnerMessage());
        }
    }

    private void HandleFileDirectoryBuildProps(
        DirectoryInfo path,
        string area,
        string urlPart)
    {
        var file = path.CombineFileInfo(FileNameDirectoryBuildProps);

        var rawGitUrl = string.IsNullOrEmpty(urlPart)
            ? $"{RawCodingRulesDistributionUrl}/{FileNameDirectoryBuildProps}"
            : $"{RawCodingRulesDistributionUrl}/{urlPart}/{FileNameDirectoryBuildProps}";

        try
        {
            if (!file.Directory!.Exists)
            {
                Directory.CreateDirectory(file.Directory.FullName);
            }

            var displayName = rawGitUrl.Replace(GitRawContentUrl, GitHubPrefix, StringComparison.Ordinal);
            var rawDirectoryBuildProps = HttpClientHelper.GetAsString(rawGitUrl, displayName).TrimEndForEmptyLines();
            File.WriteAllText(file.FullName, rawDirectoryBuildProps);
            LogFileCreated($"{CodingRulesConstants.LogFileCreated}   ", area, FileNameDirectoryBuildProps);
        }
        catch (IOException ex)
        {
            LogFileSkipped($"{CodingRulesConstants.LogError}   ", area, FileNameDirectoryBuildProps, ex.GetLastInnerMessage());
        }
    }
}