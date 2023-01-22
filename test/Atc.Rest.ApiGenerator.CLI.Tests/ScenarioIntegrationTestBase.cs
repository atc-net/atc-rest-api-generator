// ReSharper disable SuggestBaseTypeForParameter
namespace Atc.Rest.ApiGenerator.CLI.Tests;

[SuppressMessage("Major Code Smell", "S4457:Parameter validation in \"async\"/\"await\" methods should be wrapped", Justification = "OK.")]
public abstract class ScenarioIntegrationTestBase : IntegrationTestCliBase
{
    public static IEnumerable<DirectoryInfo> CollectScenarioPaths()
    {
        var testAssemblyName = Assembly.GetExecutingAssembly().GetName().Name;
        var baseDir = AppDomain.CurrentDomain.BaseDirectory;

        var testBasePath = new DirectoryInfo(
            baseDir.Split(
                testAssemblyName,
                StringSplitOptions.RemoveEmptyEntries)[0]);

        var scenariosPath = Path.Combine(
            testBasePath.FullName,
            Path.Combine(
                "Atc.Rest.ApiGenerator.CLI.Tests",
                "Scenarios"));

        return Directory
            .EnumerateDirectories(scenariosPath)
            .Select(x => new DirectoryInfo(x));
    }

    [SuppressMessage("Performance", "MA0023:Add RegexOptions.ExplicitCapture", Justification = "OK.")]
    public static async Task<string> ReadGeneratedFile(
        FileInfo generatedFile)
    {
        ArgumentNullException.ThrowIfNull(generatedFile);

        var content = await File.ReadAllTextAsync(generatedFile.FullName);

        var result = Regex.Replace(
            content,
            @"(ApiGenerator.*?)(\d+\.\d+\.\d+\.\d+)",
            m => $"{m.Groups[1]}x.x.x.x",
            RegexOptions.None,
            TimeSpan.FromSeconds(1));

        return result;
    }

    public static FileInfo GetYamlSpecificationPath(
        string scenarioPath)
    {
        var files = Directory.GetFiles(scenarioPath, "*.yaml");
        return files.Length switch
        {
            0 => throw new FileNotFoundException($"Could not find a .yaml specification file in path '{scenarioPath}'."),
            > 1 => throw new NotSupportedException($"Too many .yaml files found in path '{scenarioPath}'."),
            _ => new FileInfo(files[0]),
        };
    }

    public static DirectoryInfo GetOutputPath(
        DirectoryInfo workingPath,
        DirectoryInfo scenario)
    {
        ArgumentNullException.ThrowIfNull(workingPath);
        ArgumentNullException.ThrowIfNull(scenario);

        return new DirectoryInfo(Path.Combine(workingPath.FullName, scenario.Name));
    }

    public static VerifySettings GetVerifySettings(
        FileInfo verifyFile,
        FileInfo generatedFile)
    {
        ArgumentNullException.ThrowIfNull(verifyFile);
        ArgumentNullException.ThrowIfNull(generatedFile);
        var fileExtension = generatedFile.Extension[1..];
        var settings = new VerifySettings();
        settings.UseDirectory(verifyFile.Directory!.FullName);
        settings.UseFileName(verifyFile.Name.Replace($".verified.{fileExtension}", string.Empty, StringComparison.Ordinal));
        settings.UseExtension(fileExtension);
        return settings;
    }

    public static FileInfo[] GetVerifyServerAllCsFilesForScenario(
        FileSystemInfo scenarioDirectoryInfo)
        => GetVerifyCsFilesForScenario(scenarioDirectoryInfo, "VerifyServerAll");

    public static FileInfo[] GetVerifyClientCSharpCsFilesForScenario(
        FileSystemInfo scenarioDirectoryInfo)
        => GetVerifyCsFilesForScenario(scenarioDirectoryInfo, "VerifyClientCSharp");

    public static FileInfo GetServerAllGeneratedFileForScenario(
        FileSystemInfo scenarioDirectoryInfo,
        FileInfo verifyFile,
        DirectoryInfo outputPath)
        => GetGeneratedFileForScenario(scenarioDirectoryInfo, verifyFile, outputPath, "VerifyServerAll");

    public static FileInfo GetClientCSharpGeneratedFileForScenario(
        FileSystemInfo scenarioDirectoryInfo,
        FileInfo verifyFile,
        DirectoryInfo outputPath)
        => GetGeneratedFileForScenario(scenarioDirectoryInfo, verifyFile, outputPath, "VerifyClientCSharp");

    private static FileInfo[] GetVerifyCsFilesForScenario(
        FileSystemInfo scenarioDirectoryInfo,
        string area)
    {
        ArgumentNullException.ThrowIfNull(scenarioDirectoryInfo);

        var verifyPath = Path.Combine(scenarioDirectoryInfo.FullName, area);
        var verifyCsFiles = Directory
            .GetFiles(verifyPath, "*.verified.cs", SearchOption.AllDirectories)
            .Select(x => new FileInfo(x))
            .ToArray();
        return verifyCsFiles;
    }

    private static FileInfo GetGeneratedFileForScenario(
        FileSystemInfo scenarioDirectoryInfo,
        FileInfo verifyFile,
        DirectoryInfo outputPath,
        string area)
    {
        ArgumentNullException.ThrowIfNull(scenarioDirectoryInfo);
        ArgumentNullException.ThrowIfNull(verifyFile);
        ArgumentNullException.ThrowIfNull(outputPath);

        var verifyPath = Path.Combine(scenarioDirectoryInfo.FullName, area);
        var generatedFile = verifyFile
            .FullName
            .Replace(".verified", string.Empty, StringComparison.Ordinal)
            .Replace(verifyPath, outputPath.FullName, StringComparison.Ordinal);
        return new FileInfo(generatedFile);
    }
}