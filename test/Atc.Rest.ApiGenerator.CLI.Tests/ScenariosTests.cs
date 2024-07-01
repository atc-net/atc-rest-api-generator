// ReSharper disable SuggestBaseTypeForParameter
// ReSharper disable HeuristicUnreachableCode
namespace Atc.Rest.ApiGenerator.CLI.Tests;

[UsesVerify]
[Collection("Sequential-Scenarios")]
[Trait(Traits.Category, Traits.Categories.Integration)]
public class ScenariosTests : ScenarioIntegrationTestBase, IAsyncLifetime
{
    private static readonly DirectoryInfo WorkingPath = new(
        Path.Combine(
            @"c:\temp",
            "atc-rest-cli-tests"));

    private static FileInfo? cliExeFile;

    [Theory]
    [InlineData("DemoSample")]
    [InlineData("ExAllResponseTypes")]
    [InlineData("ExGenericPagination")]
    [InlineData("ExNsWithTask")]
    [InlineData("ExUsers")]
    [InlineData("PetStore")]
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
            outputLines[^1].Contains("Schema validated successfully.", StringComparison.Ordinal),
            $"CLI validate schema output is missing 'Schema validated successfully.' for scenario '{scenarioPath.Name}'");
    }

    [Theory]
    [InlineData("DemoSample", AspNetOutputType.Mvc, false)]
    [InlineData("DemoSample", AspNetOutputType.Mvc, true)]
    [InlineData("ExAllResponseTypes", AspNetOutputType.Mvc, false)]
    [InlineData("ExAllResponseTypes", AspNetOutputType.Mvc, true)]
    [InlineData("ExGenericPagination", AspNetOutputType.Mvc, false)]
    [InlineData("ExGenericPagination", AspNetOutputType.Mvc, true)]
    [InlineData("ExNsWithTask", AspNetOutputType.Mvc, false)]
    [InlineData("ExNsWithTask", AspNetOutputType.Mvc, true)]
    [InlineData("ExUsers", AspNetOutputType.Mvc, false)]
    [InlineData("ExUsers", AspNetOutputType.Mvc, true)]
    [InlineData("PetStore", AspNetOutputType.Mvc, false)]
    [InlineData("PetStore", AspNetOutputType.Mvc, true)]
    public async Task GenerateVerifyAndBuildForServerAllByScenario(
        string scenarioName,
        AspNetOutputType aspNetOutputType,
        bool useProblemDetailsAsDefaultResponseBody)
    {
        // Arrange
        var scenarioPath = CollectScenarioPaths().First(x => x.Name == scenarioName);

        var specificationFile = GetYamlSpecificationPath(scenarioPath.FullName);

        var outputPath = GetOutputPath(WorkingPath, scenarioPath, aspNetOutputType, useProblemDetailsAsDefaultResponseBody);
        if (Directory.Exists(outputPath.FullName))
        {
            Directory.Delete(outputPath.FullName, recursive: true);
        }

        // Act & Assert
        await AssertGenerateForServerAll(outputPath, scenarioPath, specificationFile, aspNetOutputType, useProblemDetailsAsDefaultResponseBody);
        await AssertVerifyCsFilesForServerAll(outputPath, scenarioPath, aspNetOutputType, useProblemDetailsAsDefaultResponseBody);
        await AssertBuildForServerAll(outputPath, scenarioPath);
    }

    [Theory]
    [InlineData("DemoSample", false, false)]
    [InlineData("DemoSample", true, false)]
    [InlineData("ExAllResponseTypes", false, false)]
    [InlineData("ExAllResponseTypes", true, false)]
    [InlineData("ExGenericPagination", false, false)]
    [InlineData("ExGenericPagination", true, false)]
    [InlineData("ExNsWithTask", false, false)]
    [InlineData("ExNsWithTask", true, false)]
    [InlineData("ExUsers", false, false)]
    [InlineData("ExUsers", true, false)]
    [InlineData("PetStore", false, false)]
    [InlineData("PetStore", true, false)]
    [InlineData("MontaPartner", false, true)]
    public async Task GenerateVerifyAndBuildForClientCSharpByScenario(
        string scenarioName,
        bool useProblemDetailsAsDefaultResponseBody,
        bool useCustomErrorResponseModel)
    {
        // Arrange
        var scenarioPath = CollectScenarioPaths().First(x => x.Name == scenarioName);

        var specificationFile = GetYamlSpecificationPath(scenarioPath.FullName);
        var optionsFile = GetApiGeneratorOptionsPath(scenarioPath.FullName);

        var outputPath = GetOutputPath(WorkingPath, scenarioPath, aspNetOutputType: null, useProblemDetailsAsDefaultResponseBody, useCustomErrorResponseModel);
        if (Directory.Exists(outputPath.FullName))
        {
            Directory.Delete(outputPath.FullName, recursive: true);
        }

        // Act & Assert
        await AssertGenerateForClientCSharp(outputPath, scenarioPath, specificationFile, optionsFile, useProblemDetailsAsDefaultResponseBody, useCustomErrorResponseModel);
        await AssertVerifyCsFilesForClientCSharp(outputPath, scenarioPath, useProblemDetailsAsDefaultResponseBody, useCustomErrorResponseModel);
    }

    ////[Fact]
    [Fact(Skip = "Only use it for prepare verify files")]
    [SuppressMessage("Major Bug", "S2583:Conditionally executed code should be reachable", Justification = "OK.")]
    public void PrepareVerifyServerAllCsFilesFromGeneratedOutput()
    {
        const string scenarioName = "DemoSample";
        const AspNetOutputType aspNetOutputType = AspNetOutputType.Mvc;
        const bool useProblemDetailsAsDefaultResponseBody = false;

        var scenarioPath = CollectScenarioPaths().First(x => x.Name == scenarioName);

        var outputPath = GetOutputPath(WorkingPath, scenarioPath, aspNetOutputType, useProblemDetailsAsDefaultResponseBody);
        if (!outputPath.Exists)
        {
            return;
        }

        var suffix = useProblemDetailsAsDefaultResponseBody
            ? "WPD"
            : "WOPD";

        var verifyPath = new DirectoryInfo(Path.Combine(scenarioPath.FullName, "VerifyServerAll", $"{aspNetOutputType}_{suffix}"));

        CopyAndRenameCsFilesToVerified(outputPath, verifyPath);
    }

    ////[Fact]
    [Fact(Skip = "Only use it for prepare verify files")]
    [SuppressMessage("Major Bug", "S2583:Conditionally executed code should be reachable", Justification = "OK.")]
    public void PrepareVerifySClientCsFilesFromGeneratedOutput()
    {
        const string scenarioName = "PetStore";
        const bool useProblemDetailsAsDefaultResponseBody = true;

        var scenarioPath = CollectScenarioPaths().First(x => x.Name == scenarioName);

        var outputPath = GetOutputPath(WorkingPath, scenarioPath, aspNetOutputType: null, useProblemDetailsAsDefaultResponseBody);
        if (!outputPath.Exists)
        {
            return;
        }

        var suffix = useProblemDetailsAsDefaultResponseBody
            ? "WPD"
            : "WOPD";

        var verifyPath = new DirectoryInfo(Path.Combine(scenarioPath.FullName, "VerifyClient", suffix));

        CopyAndRenameCsFilesToVerified(outputPath, verifyPath);
    }

    private static void CopyAndRenameCsFilesToVerified(
        DirectoryInfo source,
        DirectoryInfo target)
    {
        foreach (var directory in source.GetDirectories())
        {
            var targetDirectory = target.CreateSubdirectory(directory.Name);
            CopyAndRenameCsFilesToVerified(directory, targetDirectory);
        }

        foreach (var file in source.GetFiles("*.cs"))
        {
            var targetFilePath = Path.Combine(target.FullName, file.Name);
            file.CopyTo(targetFilePath, overwrite: true);

            var renamedFilePath = Path.Combine(target.FullName, Path.GetFileNameWithoutExtension(file.Name) + ".verified.cs");
            File.Move(targetFilePath, renamedFilePath, true);

            ModifyFileToReplaceVersionIfNeeded(new FileInfo(renamedFilePath));
        }
    }

    [SuppressMessage("Major Bug", "S4143:Collection elements should not be replaced unconditionally", Justification = "OK.")]
    private static void ModifyFileToReplaceVersionIfNeeded(
        FileInfo file)
    {
        var updateCount = 0;
        var lines = FileHelper.ReadAllTextToLines(file);
        for (var i = 0; i < lines.Length; i++)
        {
            if (!lines[i].Contains("ApiGenerator", StringComparison.Ordinal))
            {
                continue;
            }

            lines[i] = Regex.Replace(
                lines[i],
                pattern: @"ApiGenerator\s\d+\.\d+\.\d+\.\d+",
                replacement: "ApiGenerator x.x.x.x",
                RegexOptions.None,
                TimeSpan.FromSeconds(1));

            lines[i] = Regex.Replace(
                lines[i],
                pattern: @"\[GeneratedCode\(""ApiGenerator"",\s*""\d+\.\d+\.\d+\.\d+""\)\]",
                replacement: @"[GeneratedCode(""ApiGenerator"", ""x.x.x.x"")]",
                RegexOptions.None,
                TimeSpan.FromSeconds(1));

            updateCount++;

            if (updateCount >= 2)
            {
                break;
            }
        }

        if (updateCount > 0)
        {
            File.WriteAllLines(file.FullName, lines);
        }
    }

    private static async Task AssertGenerateForServerAll(
        DirectoryInfo outputPath,
        DirectoryInfo scenarioPath,
        FileInfo specificationFile,
        AspNetOutputType aspNetOutputType,
        bool useProblemDetailsAsDefaultResponseBody)
    {
        var sbCommands = new StringBuilder();
        sbCommands.Append("generate server all");
        sbCommands.Append(" --specificationPath ");
        sbCommands.Append(specificationFile.FullName);
        sbCommands.Append(" --projectPrefixName ");
        sbCommands.Append(scenarioPath.Name);
        sbCommands.Append(" --outputSlnPath ");
        sbCommands.Append(outputPath.FullName);
        sbCommands.Append(" --outputSrcPath ");
        sbCommands.Append(Path.Combine(outputPath.FullName, "src"));
        sbCommands.Append(" --outputTestPath ");
        sbCommands.Append(Path.Combine(outputPath.FullName, "test"));
        sbCommands.Append(" --aspnet-output-type ");
        sbCommands.Append(aspNetOutputType.ToString());
        if (useProblemDetailsAsDefaultResponseBody)
        {
            sbCommands.Append(" --useProblemDetailsAsDefaultResponseBody");
        }

        sbCommands.Append(" --verbose");

        var (isSuccessful, output) = await ProcessHelper.Execute(cliExeFile!, sbCommands.ToString());

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
        DirectoryInfo outputPath,
        DirectoryInfo scenarioPath,
        AspNetOutputType aspNetOutputType,
        bool useProblemDetailsAsDefaultResponseBody)
    {
        var suffix = useProblemDetailsAsDefaultResponseBody
            ? "WPD"
            : "WOPD";

        var verifyPath = new DirectoryInfo(Path.Combine(scenarioPath.FullName, "VerifyServerAll", $"{aspNetOutputType}_{suffix}"));

        var verifyCsFiles = GetVerifyCsFilesForScenario(verifyPath);

        foreach (var verifyFile in verifyCsFiles)
        {
            var generatedFile = GetGeneratedFileForScenario(verifyPath, verifyFile, outputPath);

            Assert.True(
                generatedFile.Exists,
                $"File not generated: {generatedFile.FullName}");

            var generatedFileContent = await ReadGeneratedFile(generatedFile);
            var settings = GetVerifySettings(verifyFile, generatedFile);

            await Verify(generatedFileContent, settings);
        }

        var outputCsFilesWithRelativePath = Directory.GetFiles(outputPath.FullName, "*.cs", SearchOption.AllDirectories)
            .Select(x => Path.GetRelativePath(outputPath.FullName, x))
            .ToArray();

        var verifyCsFilesWithRelativePath = verifyCsFiles.Select(x => x.FullName)
            .Select(x => Path.GetRelativePath(scenarioPath.CombineFileInfo("VerifyServerAll", $"{aspNetOutputType}_{suffix}").FullName, x))
            .ToArray();

        var onlyInOutput = outputCsFilesWithRelativePath
            .Except(verifyCsFilesWithRelativePath, StringComparer.Ordinal)
            .OrderBy(x => x, StringComparer.Ordinal)
            .ToArray();

        var onlyInVerify = verifyCsFilesWithRelativePath
            .Except(outputCsFilesWithRelativePath, StringComparer.Ordinal)
            .OrderBy(x => x, StringComparer.Ordinal)
            .ToArray();

        Assert.True(
            outputCsFilesWithRelativePath.Length == verifyCsFilesWithRelativePath.Length,
            $"Different count on *.cs files, " +
            $"verify.count={verifyCsFilesWithRelativePath.Length} and " +
            $"generated.count={outputCsFilesWithRelativePath.Length} for scenario '{scenarioPath.Name}'. " +
            $"\n\nFiles only in output:" +
            $"\n\t{string.Join("\n\t", onlyInOutput)}" +
            $"\n\nFiles only in verify:" +
            $"\n\t{string.Join("\n\t", onlyInVerify)}\n");
    }

    private static async Task AssertBuildForServerAll(
        DirectoryInfo outputPath,
        DirectoryInfo scenarioPath)
    {
        var buildErrors = await DotnetBuildHelper.BuildAndCollectErrors(
            NullLogger.Instance,
            outputPath,
            1);

        Assert.True(
            buildErrors.Count == 0,
            $"BuildErrors: {string.Join(" # ", buildErrors)} for scenario '{scenarioPath.Name}'");
    }

    private static async Task AssertGenerateForClientCSharp(
        DirectoryInfo outputPath,
        DirectoryInfo scenarioPath,
        FileInfo specificationFile,
        FileInfo? optionsFile,
        bool useProblemDetailsAsDefaultResponseBody,
        bool useCustomErrorResponseModel)
    {
        var sbCommands = new StringBuilder();
        sbCommands.Append("generate client csharp");
        sbCommands.Append(" --specificationPath ");
        sbCommands.Append(specificationFile.FullName);
        sbCommands.Append(" --projectPrefixName ");
        sbCommands.Append(scenarioPath.Name);
        sbCommands.Append(" --outputPath ");
        sbCommands.Append(Path.Combine(outputPath.FullName, "src"));
        if (!useCustomErrorResponseModel &&
            useProblemDetailsAsDefaultResponseBody)
        {
            sbCommands.Append(" --useProblemDetailsAsDefaultResponseBody");
        }

        if (optionsFile is not null)
        {
            sbCommands.Append(" --optionsPath ");
            sbCommands.Append(optionsFile.FullName);
        }

        sbCommands.Append(" --verbose");

        var (isSuccessful, output) = await ProcessHelper.Execute(cliExeFile!, sbCommands.ToString());

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
        DirectoryInfo outputPath,
        DirectoryInfo scenarioPath,
        bool useProblemDetailsAsDefaultResponseBody,
        bool useCustomErrorResponseModel)
    {
        var suffix = string.Empty;
        if (useCustomErrorResponseModel)
        {
            suffix = "WCEM";
        }
        else
        {
            suffix = useProblemDetailsAsDefaultResponseBody
                ? "WPD"
                : "WOPD";
        }

        var verifyPath = new DirectoryInfo(Path.Combine(scenarioPath.FullName, "VerifyClient", suffix));

        var verifyCsFiles = GetVerifyCsFilesForScenario(verifyPath);

        foreach (var verifyFile in verifyCsFiles)
        {
            var generatedFile = GetGeneratedFileForScenario(verifyPath, verifyFile, outputPath);

            Assert.True(
                generatedFile.Exists,
                $"File not generated: {generatedFile.FullName}");

            var generatedFileContent = await ReadGeneratedFile(generatedFile);
            var settings = GetVerifySettings(verifyFile, generatedFile);

            await Verify(generatedFileContent, settings);
        }

        var outputCsFilesWithRelativePath = Directory.GetFiles(outputPath.FullName, "*.cs", SearchOption.AllDirectories)
            .Select(x => Path.GetRelativePath(outputPath.FullName, x))
            .ToArray();

        var verifyCsFilesWithRelativePath = verifyCsFiles.Select(x => x.FullName)
            .Select(x => Path.GetRelativePath(scenarioPath.CombineFileInfo("VerifyServerAll", suffix).FullName, x))
            .ToArray();

        var onlyInOutput = outputCsFilesWithRelativePath.Except(verifyCsFilesWithRelativePath, StringComparer.Ordinal).ToArray();
        var onlyInVerify = verifyCsFilesWithRelativePath.Except(outputCsFilesWithRelativePath, StringComparer.Ordinal).ToArray();

        Assert.True(
            outputCsFilesWithRelativePath.Length == verifyCsFilesWithRelativePath.Length,
            $"Different count on *.cs files, " +
            $"verify.count={verifyCsFilesWithRelativePath.Length} and " +
            $"generated.count={outputCsFilesWithRelativePath.Length} for scenario '{scenarioPath.Name}'. " +
            $"\n\nFiles only in output:" +
            $"\n\t{string.Join("\n\t", onlyInOutput)}" +
            $"\n\nFiles only in verify:" +
            $"\n\t{string.Join("\n\t", onlyInVerify)}\n");
    }

    public Task InitializeAsync()
    {
        cliExeFile = GetExecutableFileForCli(typeof(Program), "test");

        return Task.CompletedTask;
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
}