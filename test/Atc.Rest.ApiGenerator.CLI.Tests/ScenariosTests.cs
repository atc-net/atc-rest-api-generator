namespace Atc.Rest.ApiGenerator.CLI.Tests;

[SuppressMessage("Design", "MA0051:Method is too long", Justification = "OK.")]
[UsesVerify]
public class ScenariosTests : ScenarioIntegrationTestBase, IAsyncLifetime
{
    private static readonly DirectoryInfo WorkingPath = new(
        Path.Combine(
            Path.GetTempPath(),
            "atc-rest-api-generator-cli-test"));

    [Theory(Skip = "Remove 'skip' to test a single scenario by name without cleanup after code generations")]
    [InlineData("Dummy for Skip-Theory")]
    //// [Theory]
    //// [InlineData("DemoSampleApi")]
    //// [InlineData("DemoUsersApi")]
    [SuppressMessage("Usage", "xUnit1004:Test methods should not be skipped", Justification = "OK - Should only be used for debugging")]
    public Task ValidateAndBuildASingleScenarioByNameWithoutCleanup(string scenarioName)
    {
        var cliExeFile = GetExecutableFileForCli(typeof(Program), "test");

        var scenarioPath = CollectScenarioPaths().FirstOrDefault(x => x.Name == scenarioName);

        Assert.NotNull(scenarioPath);

        var workingPath = new DirectoryInfo(@"c:\temp\ApiGenTemp");
        var workingPathForScenario = new DirectoryInfo(Path.Combine(workingPath.FullName, scenarioName));

        if (workingPathForScenario.Exists)
        {
            Directory.Delete(workingPathForScenario.FullName, recursive: true);
        }

        return ValidateAndBuildScenarioTestFlow(
            cliExeFile,
            scenarioPath,
            workingPath);
    }

    [Fact]
    public async Task ValidateAndBuildScenarios()
    {
        var cliExeFile = GetExecutableFileForCli(typeof(Program), "test");

        foreach (var scenario in CollectScenarioPaths())
        {
            await ValidateAndBuildScenarioTestFlow(
                cliExeFile,
                scenario,
                WorkingPath);
        }
    }

    private static async Task ValidateAndBuildScenarioTestFlow(
        FileInfo cliExeFile,
        DirectoryInfo scenarioPath,
        DirectoryInfo workingPath)
    {
        //----------------------------------------------------------------------------------------
        // Step 1:
        // Find and validate the yaml specification.
        //----------------------------------------------------------------------------------------
        var specificationFile = GetYamlSpecificationPath(scenarioPath.FullName);

        var cliExecutionResult = await ProcessHelper.Execute(
            cliExeFile,
            $"validate schema --specificationPath {specificationFile.FullName}");

        var cliOutputLines = cliExecutionResult.Output.Split(
            Environment.NewLine,
            StringSplitOptions.RemoveEmptyEntries);

        Assert.True(
            cliExecutionResult.IsSuccessful,
            $"CLI validate schema output is not successful for scenario '{scenarioPath.Name}'");

        Assert.True(
            "Schema validated successfully.".Equals(cliOutputLines[^1], StringComparison.Ordinal),
            $"CLI validate schema output is missing 'Schema validated successfully' for scenario '{scenarioPath.Name}'");

        //----------------------------------------------------------------------------------------
        // Step 2:
        // Invoke the generator with yaml spec, project name and output path arguments.
        //----------------------------------------------------------------------------------------
        var outputPath = GetOutputPath(workingPath, scenarioPath);

        cliExecutionResult = await ProcessHelper.Execute(
            cliExeFile,
            "generate server all " +
            $"--specificationPath {specificationFile.FullName} " +
            $"--projectPrefixName {scenarioPath.Name} " +
            $"--outputSlnPath {outputPath.FullName} " +
            $"--outputSrcPath {Path.Combine(outputPath.FullName, "src")} " +
            $"--outputTestPath {Path.Combine(outputPath.FullName, "test")} " +
            "-v");

        Assert.True(
            cliExecutionResult.IsSuccessful,
            $"CLI output is not successful for scenario '{scenarioPath.Name}'");

        Assert.True(
            "Schema validated successfully.".Equals(cliOutputLines[^1], StringComparison.Ordinal),
            $"CLI validate schema output is missing 'Schema validated successfully' for scenario '{scenarioPath.Name}'");

        //----------------------------------------------------------------------------------------
        // Step 3:
        // Iterate files in this scenario's Verify folder and compare them to the generated output.
        //----------------------------------------------------------------------------------------
        var verifyCsFiles = GetVerifyCsFilesForScenario(scenarioPath);

        foreach (var verifyFile in verifyCsFiles)
        {
            var generatedFile = GetGeneratedFile(scenarioPath, verifyFile, outputPath);

            Assert.True(
                generatedFile.Exists,
                $"File not generated: {generatedFile.FullName}");

            var generatedFileContent = await ReadGeneratedFile(generatedFile);
            var settings = GetVerifySettings(verifyFile, generatedFile);

            await Verify(generatedFileContent, settings);
        }

        //----------------------------------------------------------------------------------------
        // Step 4:
        // Check that all *.cs is verified.
        //----------------------------------------------------------------------------------------
        var outputCsFiles = Directory.GetFiles(outputPath.FullName, "*.cs", SearchOption.AllDirectories);

        Assert.True(
            outputCsFiles.Length == verifyCsFiles.Length,
            $"Different count on *.cs files, input.count={verifyCsFiles.Length} and output.count={outputCsFiles.Length} for scenario '{scenarioPath.Name}'");

        //----------------------------------------------------------------------------------------
        // Step 5:
        // Build the generated project and assert no errors.
        //----------------------------------------------------------------------------------------
        var buildErrors = await DotnetBuildHelper.BuildAndCollectErrors(
            NullLogger.Instance,
            outputPath,
            1);

        Assert.True(
            buildErrors.Count == 0,
            $"BuildErrors: {string.Join(" # ", buildErrors)} for scenario '{scenarioPath.Name}'");
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync()
    {
        if (WorkingPath.Exists)
        {
            Directory.Delete(WorkingPath.FullName, recursive: true);
        }

        return Task.CompletedTask;
    }
}