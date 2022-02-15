namespace Atc.Rest.ApiGenerator.Tests.IntegrationTests.Scenarios;

public class ScenarioIntegrationTestBase : IntegrationTestCliBase
{
    public static IEnumerable<DirectoryInfo> CollectScenarios()
    {
        var testAssemblyName = Assembly.GetExecutingAssembly().GetName().Name;
        var baseDir = AppDomain.CurrentDomain.BaseDirectory;

        var testBasePath = new DirectoryInfo(baseDir.Split(testAssemblyName, StringSplitOptions.RemoveEmptyEntries)[0]);
        var scenariosPath = Path.Combine(testBasePath.FullName, "Atc.Rest.ApiGenerator.Tests/IntegrationTests/Scenarios");
        return Directory.EnumerateDirectories(scenariosPath).Select(x => new DirectoryInfo(x));
    }

    [SuppressMessage("Security", "MA0009:Add regex evaluation timeout", Justification = "OK. - test class")]
    public static async Task<string> ReadGeneratedFile(string filePath)
    {
        var content = await File.ReadAllTextAsync(filePath);
        return Regex.Replace(content, @"(ApiGenerator.*?)(\d+\.\d+\.\d+\.\d+)", m => $"{m.Groups[1]}x.x.x.x");
    }

    public static string GetYamlSpecificationPath(string scenarioPath)
    {
        var files = Directory.GetFiles(scenarioPath, "*.yaml");
        return files.Length switch
        {
            0 => throw new FileNotFoundException($"Could not find a .yaml specification file in path '{scenarioPath}'."),
            > 1 => throw new NotSupportedException($"Too many .yaml files found in path '{scenarioPath}'."),
            _ => files[0],
        };
    }
}