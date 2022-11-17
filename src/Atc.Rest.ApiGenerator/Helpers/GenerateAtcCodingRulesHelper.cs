// ReSharper disable SuggestBaseTypeForParameter
// ReSharper disable InvertIf
namespace Atc.Rest.ApiGenerator.Helpers;

public static class GenerateAtcCodingRulesHelper
{
    private const string RawCodingRulesDistribution = "https://raw.githubusercontent.com/atc-net/atc-coding-rules/main/distribution/dotnet6";
    public const string FileNameEditorConfig = ".editorconfig";
    public const string FileNameDirectoryBuildProps = "Directory.Build.props";

    public static bool Generate(
        ILogger logger,
        string outputSlnPath,
        DirectoryInfo outputSrcPath,
        DirectoryInfo? outputTestPath)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(outputSlnPath);
        ArgumentNullException.ThrowIfNull(outputSrcPath);

        var rootPath = outputSlnPath.EndsWith(".sln", StringComparison.OrdinalIgnoreCase)
            ? new FileInfo(outputSlnPath).Directory
            : new DirectoryInfo(outputSlnPath);

        if (rootPath is null)
        {
            throw new IOException("Invalid outputSlnPath");
        }

        if (IsFirstTime(rootPath))
        {
            HandleCodingRulesFiles(logger, outputSrcPath, outputTestPath, rootPath);
            HandleEditorConfigFiles(logger, outputSrcPath, outputTestPath, rootPath);
            HandleDirectoryBuildPropsFiles(logger, outputSrcPath, outputTestPath, rootPath);
        }

        return true;
    }

    private static bool IsFirstTime(
        DirectoryInfo rootPath)
    {
        var file = new FileInfo(Path.Combine(rootPath.FullName, FileNameEditorConfig));
        return !file.Exists;
    }

    private static void HandleCodingRulesFiles(
        ILogger logger,
        DirectoryInfo outputSrcPath,
        DirectoryInfo? outputTestPath,
        DirectoryInfo rootPath)
    {
        logger.LogInformation($"{AppEmojisConstants.AreaCodingRules} Working on Coding Rules files");
        HandleAtcCodingRulesJson(logger, rootPath, outputSrcPath, outputTestPath);
        HandleAtcCodingRulesPowerShell(logger, rootPath);
    }

    private static void HandleEditorConfigFiles(
        ILogger logger,
        DirectoryInfo outputSrcPath,
        DirectoryInfo? outputTestPath,
        DirectoryInfo rootPath)
    {
        logger.LogInformation($"{AppEmojisConstants.AreaEditorConfig} Working on EditorConfig files");
        HandleFileEditorConfig(logger, rootPath, "root", string.Empty);

        if (rootPath.FullName != outputSrcPath.FullName)
        {
            HandleFileEditorConfig(logger, outputSrcPath, "src", "src");
        }

        if (outputTestPath is not null &&
            rootPath.FullName != outputTestPath.FullName)
        {
            HandleFileEditorConfig(logger, outputTestPath, "test", "test");
        }
    }

    private static void HandleDirectoryBuildPropsFiles(
        ILogger logger,
        DirectoryInfo outputSrcPath,
        DirectoryInfo? outputTestPath,
        DirectoryInfo rootPath)
    {
        logger.LogInformation($"{AppEmojisConstants.AreaDirectoryBuildProps} Working on Directory.Build.props files");

        HandleFileDirectoryBuildProps(logger, rootPath, "root", string.Empty);

        if (rootPath.FullName != outputSrcPath.FullName)
        {
            HandleFileDirectoryBuildProps(logger, outputSrcPath, "src", "src");
        }

        if (outputTestPath is not null &&
            rootPath.FullName != outputTestPath.FullName)
        {
            HandleFileDirectoryBuildProps(logger, outputTestPath, "test", "test");
        }
    }

    private static void HandleAtcCodingRulesJson(
    ILogger logger,
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
            sb.AppendLine("  \"projectTarget\": \"DotNet6\",");
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
            logger.LogDebug($"{EmojisConstants.FileCreated}   root: {file} created");
        }
        catch (IOException ex)
        {
            logger.LogError($"{EmojisConstants.Error}   root: {file} skipped - {ex.Message}");
        }
    }

    private static void HandleAtcCodingRulesPowerShell(
        ILogger logger,
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
            sb.AppendLine("  -v true");
            File.WriteAllText(file, sb.ToString());
            File.WriteAllText(filePath, sb.ToString());
            logger.LogDebug($"{EmojisConstants.FileCreated}   root: {file} created");
        }
        catch (IOException ex)
        {
            logger.LogError($"{EmojisConstants.Error}   root: {file} skipped - {ex.Message}");
        }
    }

    private static void HandleFileEditorConfig(
        ILogger logger,
        DirectoryInfo path,
        string area,
        string urlPart)
    {
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

            var displayName = rawGitUrl.Replace(Constants.GitRawContentUrl, Constants.GitHubPrefix, StringComparison.Ordinal);
            var rawEditorConfig = HttpClientHelper.GetAsString(logger, rawGitUrl, displayName).TrimEndForEmptyLines();

            rawEditorConfig += $"{Environment.NewLine}dotnet_diagnostic.IDE0058.severity = none           # Have to override this for now - to get smoke-test to run";
            File.WriteAllText(file.FullName, rawEditorConfig);
            logger.LogDebug($"{EmojisConstants.FileCreated}   {area}: {FileNameEditorConfig} created");
        }
        catch (IOException ex)
        {
            logger.LogError($"{EmojisConstants.Error}   {area}: {FileNameEditorConfig} skipped - {ex.Message}");
        }
    }

    private static void HandleFileDirectoryBuildProps(
        ILogger logger,
        DirectoryInfo path,
        string area,
        string urlPart)
    {
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

            var displayName = rawGitUrl.Replace(Constants.GitRawContentUrl, Constants.GitHubPrefix, StringComparison.Ordinal);
            var rawDirectoryBuildProps = HttpClientHelper.GetAsString(logger, rawGitUrl, displayName).TrimEndForEmptyLines();
            File.WriteAllText(file.FullName, rawDirectoryBuildProps);
            logger.LogDebug($"{EmojisConstants.FileCreated}   {area}: {FileNameDirectoryBuildProps} created");
        }
        catch (IOException ex)
        {
            logger.LogError($"{EmojisConstants.Error}   {area}: {FileNameDirectoryBuildProps} skipped - {ex.Message}");
        }
    }
}