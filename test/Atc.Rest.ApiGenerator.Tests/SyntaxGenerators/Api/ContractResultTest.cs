using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Atc.Rest.ApiGenerator.Models;
using Atc.Rest.ApiGenerator.SyntaxGenerators;
using Atc.Rest.ApiGenerator.SyntaxGenerators.Api;
using Atc.Rest.ApiGenerator.Tests.Helpers.CodeGenerator;
using Microsoft.OpenApi.Models;
using VerifyXunit;
using Xunit;

namespace Atc.Rest.ApiGenerator.Tests.SyntaxGenerators
{
    [UsesVerify]
    public class ContractResultTest : SyntaxGeneratorTestBase
    {
        public static IEnumerable<object[]> YamlFiles { get; } = AllFiles
            .Where(x => x.FilePath.Contains("ContractResult", System.StringComparison.Ordinal))
            .Select(x => new object[] { x });

        protected override ISyntaxCodeGenerator CreateApiGenerator(ApiProjectOptions apiProject)
        {
            // Verify spec file suported for unit test
            Assert.Single(apiProject.Document.Paths);
            var urlPath = apiProject.Document.Paths.First();
            Assert.False(urlPath.IsPathStartingSegmentName(FocusOnSecment));
            Assert.Single(urlPath.Value.Operations);
            var urlOperation = urlPath.Value.Operations.First();

            // Construct SUT
            return new SyntaxGeneratorContractResult(
                apiProject,
                urlOperation.Key,
                urlOperation.Value,
                FocusOnSecment);
        }

        [Theory(DisplayName = "Contract Result")]
        [MemberData(nameof(YamlFiles))]
        public async Task ExecuteGeneratorTest(YamlSpecFile specFile)
        {
            Assert.NotNull(specFile?.FilePath);
            await ExecuteTest(specFile);
        }
    }
}