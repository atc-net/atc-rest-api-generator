// ReSharper disable SuggestBaseTypeForParameter
namespace Atc.Rest.ApiGenerator.CLI.Tests;

[UsesVerify]
[Collection("Sequential-Scenarios")]
public class ScenariosTests : ScenarioIntegrationTestBase, IAsyncLifetime
{
    private static readonly DirectoryInfo WorkingPath = new(
        Path.Combine(
            Path.GetTempPath(),
            "atc-rest-api-generator-cli-test"));

    private static FileInfo? cliExeFile;

    [Theory]
    [InlineData("DemoSampleApi")]
    [InlineData("DemoUsersApi")]
    [InlineData("GenericPaginationApi")]
    [InlineData("PetStoreApi")]
    public async Task ValidateYamlSpecificationByScenario(
        string scenarioName)
    {
        // Arrange
        var scenarioPath = CollectScenarioPaths().First(x => x.Name == scenarioName);

        var specificationFile = GetYamlSpecificationPath(scenarioPath.FullName);

        // Atc
        var (isSuccessful, output) = await ProcessHelper.Execute(
            cliExeFile!,
            $"validate schema --specificationPath {specificationFile.FullName}");

        // Assert
        Assert.True(
            isSuccessful,
            $"CLI validate schema output is not successful for scenario '{scenarioPath.Name}'");

        var outputLines = output
            .EnsureEnvironmentNewLines()
            .Split(
                Environment.NewLine,
                StringSplitOptions.RemoveEmptyEntries);

        Assert.True(
            "Schema validated successfully.".Equals(outputLines[^1], StringComparison.Ordinal),
            $"CLI validate schema output is missing 'Schema validated successfully' for scenario '{scenarioPath.Name}'");
    }

    [Theory]
    [InlineData("DemoSampleApi")]
    [InlineData("DemoUsersApi")]
    [InlineData("GenericPaginationApi")]
    [InlineData("PetStoreApi")]
    public async Task GenerateVerifyAndBuildForServerAllByScenario(
        string scenarioName)
    {
        // Arrange
        var scenarioPath = CollectScenarioPaths().First(x => x.Name == scenarioName);

        var specificationFile = GetYamlSpecificationPath(scenarioPath.FullName);

        // Act & Assert
        await AssertGenerateForServerAll(WorkingPath, scenarioPath, specificationFile);
        await AssertVerifyCsFilesForServerAll(WorkingPath, scenarioPath);
        await AssertBuildForServerAll(WorkingPath, scenarioPath);
    }

    [Theory(Skip = "Remove 'skip' to test a single scenario by name without cleanup after code generations")]
    [InlineData("Dummy for Skip-Theory")]
    ////[Theory]
    ////[InlineData("DemoSampleApi")]
    ////[InlineData("DemoUsersApi")]
    ////[InlineData("GenericPaginationApi")]
    ////[InlineData("PetStoreApi")]
    public async Task GenerateVerifyAndBuildForServerAllByScenarioWithoutCleanup(
        string scenarioName)
    {
        // Arrange
        var scenarioPath = CollectScenarioPaths().First(x => x.Name == scenarioName);

        var specificationFile = GetYamlSpecificationPath(scenarioPath.FullName);

        var workingPath = new DirectoryInfo(@"c:\temp\ApiGenTemp");
        var workingPathForScenario = new DirectoryInfo(Path.Combine(workingPath.FullName, scenarioName));

        if (workingPathForScenario.Exists)
        {
            Directory.Delete(workingPathForScenario.FullName, recursive: true);
        }

        // Act & Assert
        await AssertGenerateForServerAll(workingPath, scenarioPath, specificationFile);
        await AssertVerifyCsFilesForServerAll(workingPath, scenarioPath);
        await AssertBuildForServerAll(workingPath, scenarioPath);
    }

    [Theory]
    [InlineData("DemoSampleApi")]
    [InlineData("DemoUsersApi")]
    [InlineData("GenericPaginationApi")]
    [InlineData("PetStoreApi")]
    public async Task GenerateVerifyAndBuildForClientCSharpByScenario(
        string scenarioName)
    {
        // Arrange
        var scenarioPath = CollectScenarioPaths().First(x => x.Name == scenarioName);

        var specificationFile = GetYamlSpecificationPath(scenarioPath.FullName);

        // Act & Assert
        await AssertGenerateForClientCSharp(WorkingPath, scenarioPath, specificationFile);
        await AssertVerifyCsFilesForClientCSharp(WorkingPath, scenarioPath);
    }

    [Theory(Skip = "Remove 'skip' to test a single scenario by name without cleanup after code generations")]
    [InlineData("Dummy for Skip-Theory")]
    ////[Theory]
    ////[InlineData("DemoSampleApi")]
    ////[InlineData("DemoUsersApi")]
    ////[InlineData("GenericPaginationApi")]
    ////[InlineData("PetStoreApi")]
    public async Task GenerateVerifyAndBuildForClientCSharpByScenarioWithoutCleanup(
        string scenarioName)
    {
        // Arrange
        var scenarioPath = CollectScenarioPaths().First(x => x.Name == scenarioName);

        var specificationFile = GetYamlSpecificationPath(scenarioPath.FullName);

        var workingPath = new DirectoryInfo(@"c:\temp\ApiGenTemp");
        var workingPathForScenario = new DirectoryInfo(Path.Combine(workingPath.FullName, scenarioName));

        if (workingPathForScenario.Exists)
        {
            Directory.Delete(workingPathForScenario.FullName, recursive: true);
        }

        // Act & Assert
        await AssertGenerateForClientCSharp(workingPath, scenarioPath, specificationFile);
        await AssertVerifyCsFilesForClientCSharp(workingPath, scenarioPath);
    }

    private static async Task AssertGenerateForServerAll(
        DirectoryInfo workingPath,
        DirectoryInfo scenarioPath,
        FileInfo specificationFile)
    {
        var outputPath = GetOutputPath(workingPath, scenarioPath);

        var (isSuccessful, output) = await ProcessHelper.Execute(
            cliExeFile!,
            "generate server all " +
            $"--specificationPath {specificationFile.FullName} " +
            $"--projectPrefixName {scenarioPath.Name} " +
            $"--outputSlnPath {outputPath.FullName} " +
            $"--outputSrcPath {Path.Combine(outputPath.FullName, "src")} " +
            $"--outputTestPath {Path.Combine(outputPath.FullName, "test")} " +
            "-v");

        Assert.True(
            isSuccessful,
            $"CLI output is not successful for scenario '{scenarioPath.Name}'");

        var outputLines = output
            .EnsureEnvironmentNewLines()
            .Split(
                Environment.NewLine,
                StringSplitOptions.RemoveEmptyEntries);

        Assert.True(
            outputLines[^1].Contains("Done", StringComparison.Ordinal),
            $"CLI output is missing 'Done' for scenario '{scenarioPath.Name}'");
    }

    private static async Task AssertVerifyCsFilesForServerAll(
        DirectoryInfo workingPath,
        DirectoryInfo scenarioPath)
    {
        var verifyCsFiles = GetVerifyServerAllCsFilesForScenario(scenarioPath);
        var outputPath = GetOutputPath(workingPath, scenarioPath);

        foreach (var verifyFile in verifyCsFiles)
        {
            var generatedFile = GetServerAllGeneratedFileForScenario(scenarioPath, verifyFile, outputPath);

            Assert.True(
                generatedFile.Exists,
                $"File not generated: {generatedFile.FullName}");

            var generatedFileContent = await ReadGeneratedFile(generatedFile);
            var settings = GetVerifySettings(verifyFile, generatedFile);

            await Verify(generatedFileContent, settings);
        }

        var outputCsFiles = Directory.GetFiles(outputPath.FullName, "*.cs", SearchOption.AllDirectories);

        Assert.True(
            outputCsFiles.Length == verifyCsFiles.Length,
            $"Different count on *.cs files, input.count={verifyCsFiles.Length} and output.count={outputCsFiles.Length} for scenario '{scenarioPath.Name}'");
    }

    private static async Task AssertBuildForServerAll(
        DirectoryInfo workingPath,
        DirectoryInfo scenarioPath)
    {
        var outputPath = GetOutputPath(workingPath, scenarioPath);

        var buildErrors = await DotnetBuildHelper.BuildAndCollectErrors(
            NullLogger.Instance,
            outputPath,
            1);

        Assert.True(
            buildErrors.Count == 0,
            $"BuildErrors: {string.Join(" # ", buildErrors)} for scenario '{scenarioPath.Name}'");
    }

    private static async Task AssertGenerateForClientCSharp(
        DirectoryInfo workingPath,
        DirectoryInfo scenarioPath,
        FileInfo specificationFile)
    {
        var outputPath = GetOutputPath(workingPath, scenarioPath);

        var (isSuccessful, output) = await ProcessHelper.Execute(
            cliExeFile!,
            "generate client csharp " +
            $"--specificationPath {specificationFile.FullName} " +
            $"--projectPrefixName {scenarioPath.Name} " +
            $"--outputPath {Path.Combine(outputPath.FullName, "src")} " +
            "-v");

        Assert.True(
            isSuccessful,
            $"CLI output is not successful for scenario '{scenarioPath.Name}'");

        var outputLines = output
            .EnsureEnvironmentNewLines()
            .Split(
                Environment.NewLine,
                StringSplitOptions.RemoveEmptyEntries);

        Assert.True(
            outputLines[^1].Contains("Done", StringComparison.Ordinal),
            $"CLI output is missing 'Done' for scenario '{scenarioPath.Name}'");
    }

    private static async Task AssertVerifyCsFilesForClientCSharp(
        DirectoryInfo workingPath,
        DirectoryInfo scenarioPath)
    {
        var verifyCsFiles = GetVerifyClientCSharpCsFilesForScenario(scenarioPath);
        var outputPath = GetOutputPath(workingPath, scenarioPath);

        foreach (var verifyFile in verifyCsFiles)
        {
            var generatedFile = GetClientCSharpGeneratedFileForScenario(scenarioPath, verifyFile, outputPath);

            Assert.True(
                generatedFile.Exists,
                $"File not generated: {generatedFile.FullName}");

            var generatedFileContent = await ReadGeneratedFile(generatedFile);
            var settings = GetVerifySettings(verifyFile, generatedFile);

            await Verify(generatedFileContent, settings);
        }

        var outputCsFiles = Directory.GetFiles(outputPath.FullName, "*.cs", SearchOption.AllDirectories);

        Assert.True(
            outputCsFiles.Length == verifyCsFiles.Length,
            $"Different count on *.cs files, input.count={verifyCsFiles.Length} and output.count={outputCsFiles.Length} for scenario '{scenarioPath.Name}'");
    }

    public Task InitializeAsync()
    {
        cliExeFile = GetExecutableFileForCli(typeof(Program), "test");

        return Task.CompletedTask;
    }

    public Task DisposeAsync()
    {
        if (Directory.Exists(WorkingPath.FullName))
        {
            Directory.Delete(WorkingPath.FullName, recursive: true);
        }

        return Task.CompletedTask;
    }
}