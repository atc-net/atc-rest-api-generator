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
                "Atc.Rest.ApiGenerator.CLI.Tests",
                "Scenarios");

        return FileHelper.GetFiles(scenariosPath, "*.yaml")
            .Select(x => x.Directory!)
            .ToList();
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

    public static FileInfo? GetApiGeneratorOptionsPath(
        string scenarioPath)
    {
        var files = Directory.GetFiles(scenarioPath, "ApiGeneratorOptions.json");
        return files.Length switch
        {
            0 => null,
            > 1 => throw new NotSupportedException($"Too many ApiGeneratorOptions.json files found in path '{scenarioPath}'."),
            _ => new FileInfo(files[0]),
        };
    }

    public static DirectoryInfo GetOutputPath(
        DirectoryInfo workingPath,
        DirectoryInfo scenario,
        AspNetOutputType? aspNetOutputType,
        bool useProblemDetailsAsDefaultResponseBody,
        bool useCustomErrorResponseModel = false)
    {
        ArgumentNullException.ThrowIfNull(workingPath);
        ArgumentNullException.ThrowIfNull(scenario);

        string suffix;
        if (useCustomErrorResponseModel)
        {
            suffix = "WithCustomErrorResponse";
        }
        else
        {
            suffix = useProblemDetailsAsDefaultResponseBody
                ? "WithProblemDetails"
                : "WithoutProblemDetails";
        }

        if (aspNetOutputType is null)
        {
            return new DirectoryInfo(
                Path.Combine(
                    workingPath.FullName,
                    $"{scenario.Name}_Client_{suffix}"));
        }

        return new DirectoryInfo(
            Path.Combine(
                workingPath.FullName,
                $"{scenario.Name}_{aspNetOutputType}_{suffix}"));
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

    public static FileInfo[] GetVerifyCsFilesForScenario(
        DirectoryInfo verifyPath)
    {
        ArgumentNullException.ThrowIfNull(verifyPath);

        var verifyCsFiles = Directory
            .GetFiles(verifyPath.FullName, "*.verified.cs", SearchOption.AllDirectories)
            .Select(x => new FileInfo(x))
            .ToArray();
        return verifyCsFiles;
    }

    public static FileInfo GetGeneratedFileForScenario(
        DirectoryInfo verifyPath,
        FileInfo verifyFile,
        DirectoryInfo outputPath)
    {
        ArgumentNullException.ThrowIfNull(verifyPath);
        ArgumentNullException.ThrowIfNull(verifyFile);
        ArgumentNullException.ThrowIfNull(outputPath);

        var generatedFile = verifyFile
            .FullName
            .Replace(".verified", string.Empty, StringComparison.Ordinal)
            .Replace(verifyPath.FullName, outputPath.FullName, StringComparison.Ordinal);
        return new FileInfo(generatedFile);
    }
}