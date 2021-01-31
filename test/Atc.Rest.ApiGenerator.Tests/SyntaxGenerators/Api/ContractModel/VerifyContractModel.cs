using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Atc.Rest.ApiGenerator.SyntaxGenerators.Api;
using Atc.Rest.ApiGenerator.Tests.Helpers;
using Atc.Rest.ApiGenerator.Tests.Helpers.CodeGenerator;
using VerifyTests;
using VerifyXunit;
using Xunit;

namespace Atc.Rest.ApiGenerator.Tests.SyntaxGenerators
{
    [UsesVerify]
    public class VerifyContractModel
    {
        private static readonly string ProjectPrefix = "TestProject";
        private static readonly string ProjectSuffix = "AtcTest";
        private static readonly string FocusOnSecment = "Test";

        [Theory]
        [MemberData(nameof(GetFilesProxy))]
        public async Task VerifySpecAsync(YamlSpecFile file)
        {
            // Arrange
            var specFileInfo = new FileInfo(file.FilePath);
            var settings = new VerifySettings();
            settings.UseDirectory(specFileInfo.DirectoryName);
            settings.UseFileName(specFileInfo.Name);
            settings.UseExtension("cs");

            var spec = await specFileInfo.OpenText().ReadToEndAsync();
            var apiProj = GeneratorTestSetup.CreateApiProject(spec, ProjectPrefix, ProjectSuffix);
            var sut = new SyntaxGeneratorContractModel(apiProj, string.Empty, apiProj.Document.Components.Schemas.First().Value, FocusOnSecment);

            // Act
            var specOutput = sut.ToCodeAsString();

            // Assert
            await Verifier.Verify(specOutput, settings);
        }

        public static IEnumerable<object[]> GetFilesProxy() => GetFiles();

        private static IEnumerable<object[]> GetFiles([CallerFilePath] string sourceFilePath = "")
        {
            var directory = Path.GetDirectoryName(sourceFilePath);
            return Directory.EnumerateFiles(directory, "*.yaml")
                .Select(x => new object[]
                {
                    new YamlSpecFile(x, Path.GetFileName(x)),
                });
        }
    }
}
