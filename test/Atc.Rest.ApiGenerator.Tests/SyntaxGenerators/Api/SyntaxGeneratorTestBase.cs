using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Atc.Rest.ApiGenerator.Models;
using Atc.Rest.ApiGenerator.SyntaxGenerators;
using Atc.Rest.ApiGenerator.Tests.Helpers.CodeGenerator;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers;
using VerifyTests;
using VerifyXunit;

namespace Atc.Rest.ApiGenerator.Tests.SyntaxGenerators
{
    public abstract class SyntaxGeneratorTestBase
    {
        protected static readonly string FocusOnSecment = "Test";
        private const string ProjectPrefix = "TestProject";
        private const string ProjectSuffix = "AtcTest";

        protected static IReadOnlyList<YamlSpecFile> AllFiles { get; } = GetYamlFiles();

        protected abstract ISyntaxCodeGenerator CreateApiGenerator(ApiProjectOptions apiProject);

        protected static Task VerifyGeneratedCode(string generatedCode, VerifySettings verifySettings)
        {
            return Verifier.Verify(generatedCode, verifySettings);
        }

        protected async Task ExecuteTest(YamlSpecFile specFile)
        {
            // Arrange
            var apiProject = await CreateApiProjectAsync(specFile);
            var verifySettings = CreateVerifySettings(specFile, apiProject);

            var sut = CreateApiGenerator(apiProject);

            // Act
            var generatedCode = sut.ToCodeAsString();

            // Assert
            await VerifyGeneratedCode(generatedCode, verifySettings);
        }

        private VerifySettings CreateVerifySettings(YamlSpecFile yamlFile, ApiProjectOptions apiOptions)
        {
            var settings = new VerifySettings();
            settings.UseDirectory(yamlFile.DirectoryName);
            settings.UseFileName(yamlFile.FileName);
            settings.UseExtension("cs");
            settings.AddScrubber(input => input.Replace(apiOptions.ToolVersion.ToString(), "x.x.x.x"));
            return settings;
        }

        private static IReadOnlyList<YamlSpecFile> GetYamlFiles([CallerFilePath] string sourceFilePath = "")
        {
            var directory = Path.GetDirectoryName(sourceFilePath);
            return Directory.EnumerateFiles(directory, "*.yaml", SearchOption.AllDirectories)
                .Select(x => new YamlSpecFile(new FileInfo(x)))
                .ToArray();
        }

        private async Task<ApiProjectOptions> CreateApiProjectAsync(YamlSpecFile specFile)
        {
            var spec = await specFile.LoadFileContentAsync();
            var document = GenerateApiDocument(spec);

            return new ApiProjectOptions(
                new DirectoryInfo("resources"),
                null,
                document,
                new FileInfo("resources/dummySpec.yaml"),
                ProjectPrefix,
                ProjectSuffix,
                new Models.ApiOptions.ApiOptions());
        }

        private OpenApiDocument GenerateApiDocument(string spec)
        {
            using var specStream = GenerateStreamFromString(spec);
            var openApiStreamReader = new OpenApiStreamReader();
            var openApiDocument = openApiStreamReader.Read(specStream, out var diagnostic);
            return openApiDocument;
        }

        public Stream GenerateStreamFromString(string s)
        {
            var stream = new MemoryStream();
            using var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
    }
}