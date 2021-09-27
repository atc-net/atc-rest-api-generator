using Atc.DotNet;
using Atc.Helpers;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using VerifyTests;
using VerifyXunit;
using Xunit;

namespace Atc.Rest.ApiGenerator.Tests.IntegrationTests.Scenarios
{
    [UsesVerify]
    public class ScenariosTests : ScenarioIntegrationTestBase, IAsyncLifetime
    {
        private static readonly string WorkingDirectory = Path.Combine(Path.GetTempPath(), "atc-api-gen-integration-test");

        [Fact]
        public async Task ValidateAndBuildScenarios()
        {
            var cliExeFile = GetExecutableFileForCli(typeof(CLI.Program), "src");

            foreach (var scenario in CollectScenarios())
            {
                //----------------------------------------------------------------------------------------
                // Step 1:
                // Find and validate the yaml specification.
                //----------------------------------------------------------------------------------------
                var specificationPath = GetYamlSpecificationPath(scenario.FullName);

                var cliExecutionResult = await ProcessHelper.Execute(
                    cliExeFile,
                    $"validate schema --specificationPath {specificationPath}");

                var cliOutputLines = cliExecutionResult.output.Split(
                    Environment.NewLine,
                    StringSplitOptions.RemoveEmptyEntries);

                Assert.True(cliExecutionResult.isSuccessful);
                Assert.Equal("Schema is OK.", cliOutputLines[^1]);

                //----------------------------------------------------------------------------------------
                // Step 2:
                // Invoke the generator with yaml spec, project name and output path arguments.
                //----------------------------------------------------------------------------------------
                var outputPath = Path.Combine(WorkingDirectory, scenario.Name);

                cliExecutionResult = await ProcessHelper.Execute(
                    cliExeFile,
                    "generate server all " +
                    $"--specificationPath {specificationPath} " +
                    $"--projectPrefixName {scenario.Name} " +
                    $"--outputSlnPath {outputPath} " +
                    $"--outputSrcPath {Path.Combine(outputPath, "src")} " +
                    $"--outputTestPath {Path.Combine(outputPath, "test")} " +
                    "-v");

                Assert.True(cliExecutionResult.isSuccessful);

                //----------------------------------------------------------------------------------------
                // Step 3:
                // Iterate files in this scenario's Verify folder and compare them to the generated output.
                //----------------------------------------------------------------------------------------
                var verifyPath = Path.Combine(scenario.FullName, "Verify");
                var verifyFiles = Directory
                    .GetFiles(verifyPath, "*.verified.*", SearchOption.AllDirectories)
                    .Select(x => new FileInfo(x));

                foreach (var verifyFile in verifyFiles)
                {
                    var generatedFile = verifyFile
                        .FullName
                        .Replace(".verified", string.Empty, StringComparison.Ordinal)
                        .Replace(verifyPath, outputPath, StringComparison.Ordinal);

                    Assert.True(File.Exists(generatedFile));

                    var fileExtension = Path.GetExtension(generatedFile)[1..];
                    var settings = new VerifySettings();
                    settings.UseDirectory(verifyFile.Directory.FullName);
                    settings.UseFileName(verifyFile.Name.Replace($".verified.{fileExtension}", string.Empty, StringComparison.Ordinal));
                    settings.UseExtension(fileExtension);

                    var generatedFileContent = ReadGeneratedFile(generatedFile);

                    await Verifier.Verify(generatedFileContent, settings);
                }

                //----------------------------------------------------------------------------------------
                // Step 4:
                // Build the generated project and assert no errors.
                //----------------------------------------------------------------------------------------
                var buildErrors = await DotnetBuildHelper.BuildAndCollectErrors(
                    NullLogger.Instance,
                    new DirectoryInfo(outputPath),
                    1);

                Assert.Empty(buildErrors);
            }
        }

        public Task DisposeAsync()
        {
            if (Directory.Exists(WorkingDirectory))
            {
                Directory.Delete(WorkingDirectory, recursive: true);
            }

            return Task.CompletedTask;
        }

        public Task InitializeAsync() => Task.CompletedTask;
    }
}
