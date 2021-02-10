using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Atc.Rest.ApiGenerator.Models;
using Atc.Rest.ApiGenerator.SyntaxGenerators;
using Atc.Rest.ApiGenerator.SyntaxGenerators.Api;
using Atc.Rest.ApiGenerator.Tests.XUnitTestTypes.CodeGenerator;
using Microsoft.OpenApi.Models;
using VerifyXunit;
using Xunit;

namespace Atc.Rest.ApiGenerator.Tests.SyntaxGenerators
{
    [UsesVerify]
    public class ContractInterfaceTest : SyntaxGeneratorTestBase
    {
        public static IEnumerable<object[]> YamlFiles { get; } = AllFiles
            .Where(x => x.FilePath.Contains("ContractInterface", System.StringComparison.Ordinal))
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
            return new SyntaxGeneratorContractInterface(
                        apiProject,
                        urlPath.Value.Parameters,
                        urlOperation.Key,
                        urlOperation.Value,
                        FocusOnSecment,
                        urlPath.Value.HasParameters() || urlOperation.Value.HasParametersOrRequestBody());
        }

        [Theory(DisplayName = "Contract Interface")]
        [MemberData(nameof(YamlFiles))]
        public async Task ExecuteGeneratorTest(YamlSpecFile specFile)
        {
            Assert.NotNull(specFile?.FilePath);
            await ExecuteTest(specFile);
        }
    }
}