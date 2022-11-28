namespace Atc.Rest.ApiGenerator.Tests.SyntaxGenerators.Api;

public abstract class GeneratorTestBase
{
    protected static readonly string FocusOnSegment = "Test";
    private const string ProjectPrefix = "TestProject";
    private const string ProjectSuffix = "AtcTest";

    protected static IReadOnlyList<GeneratorTestInput> AllTestInput { get; } = GetTestInput();

    protected static Task VerifyGeneratedCode(
        string generatedCode,
        VerifySettings verifySettings)
        => Verify(generatedCode, verifySettings);

    protected static VerifySettings CreateVerifySettings(
        GeneratorTestInput yamlFile,
        ApiProjectOptions apiOptions)
    {
        ArgumentNullException.ThrowIfNull(yamlFile);

        var settings = new VerifySettings();
        settings.UseDirectory(yamlFile.TestDirectory);
        settings.UseFileName(yamlFile.TestName);
        settings.UseExtension("cs");
        settings.AddScrubber(input => input.Replace(apiOptions.ApiGeneratorVersion.ToString(), "x.x.x.x"));
        return settings;
    }

    [SuppressMessage("Info Code Smell", "S4457:Split this method into two", Justification = "OK for now.")]
    protected static async Task<ApiProjectOptions> CreateApiProjectAsync(
        GeneratorTestInput testInput)
    {
        ArgumentNullException.ThrowIfNull(testInput);

        var spec = await testInput.LoadYamlSpecContentAsync();
        var options = testInput.GeneratorOptions.Value;
        var document = GenerateApiDocument(spec);

        return new ApiProjectOptions(
            new DirectoryInfo("resources"),
            projectTestGeneratePath: null,
            document,
            new FileInfo("resources/dummySpec.yaml"),
            ProjectPrefix,
            ProjectSuffix,
            options,
            usingCodingRules: false);
    }

    public static OpenApiDocument GenerateApiDocument(
        string spec)
    {
        var memoryStream = new MemoryStream();

        using var writer = new StreamWriter(memoryStream);
        writer.Write(spec);
        writer.Flush();
        memoryStream.Position = 0;

        var openApiStreamReader = new OpenApiStreamReader();
        return openApiStreamReader.Read(memoryStream, out _);
    }

    private static IReadOnlyList<GeneratorTestInput> GetTestInput(
        [CallerFilePath] string sourceFilePath = "")
    {
        var directory = Path.GetDirectoryName(sourceFilePath);
        return Directory.EnumerateFiles(directory!, "*.yaml", SearchOption.AllDirectories)
            .Select(x =>
            {
                var specFile = new FileInfo(x);
                var configFilePath = Path.Combine(specFile.DirectoryName!, Path.GetFileNameWithoutExtension(specFile.Name) + ".json");
                var configFile = File.Exists(configFilePath)
                    ? new FileInfo(configFilePath)
                    : null;
                return new GeneratorTestInput(specFile, configFile);
            })
            .ToArray();
    }
}