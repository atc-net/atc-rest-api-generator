using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Atc.Rest.ApiGenerator.Models;
using Atc.Rest.ApiGenerator.Tests.XUnitTestTypes.CodeGenerator;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers;
using VerifyTests;
using VerifyXunit;

namespace Atc.Rest.ApiGenerator.Tests.SyntaxGenerators.Api
{
    public abstract class GeneratorTestBase
    {
        protected static readonly string FocusOnSegment = "Test";
        private const string ProjectPrefix = "TestProject";
        private const string ProjectSuffix = "AtcTest";

        protected static IReadOnlyList<GeneratorTestInput> AllTestInput { get; } = GetTestInput();

        protected static Task VerifyGeneratedCode(string generatedCode, VerifySettings verifySettings)
        {
            return Verifier.Verify(generatedCode, verifySettings);
        }

        protected VerifySettings CreateVerifySettings(GeneratorTestInput yamlFile, ApiProjectOptions apiOptions)
        {
            var settings = new VerifySettings();
            settings.UseDirectory(yamlFile.TestDirectory);
            settings.UseFileName(yamlFile.TestName);
            settings.UseExtension("cs");
            settings.AddScrubber(input => input
                .Replace("\r\n", Environment.NewLine)
                .Replace("\r", Environment.NewLine)
                .Replace("\n", Environment.NewLine));
            settings.AddScrubber(input => input.Replace(apiOptions.ToolVersion.ToString(), "x.x.x.x"));
            return settings;
        }

        protected async Task<ApiProjectOptions> CreateApiProjectAsync(GeneratorTestInput testInput)
        {
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
                options);
        }

        private OpenApiDocument GenerateApiDocument(string spec)
        {
            var memoryStream = new MemoryStream();

            using var writer = new StreamWriter(memoryStream);
            writer.Write(spec);
            writer.Flush();
            memoryStream.Position = 0;

            var openApiStreamReader = new OpenApiStreamReader();
            return openApiStreamReader.Read(memoryStream, out _);
        }

        private static IReadOnlyList<GeneratorTestInput> GetTestInput([CallerFilePath] string sourceFilePath = "")
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
}