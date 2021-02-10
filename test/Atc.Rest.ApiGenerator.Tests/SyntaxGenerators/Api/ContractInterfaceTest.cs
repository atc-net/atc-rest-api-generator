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

namespace Atc.Rest.ApiGenerator.Tests.SyntaxGenerators.Api
{
    [UsesVerify]
    public class ContractInterfaceTest : SyntaxGeneratorTestBase
    {
        public static IEnumerable<object[]> YamlFiles { get; } = AllFiles
            .Where(x => x.FilePath.Contains("ContractInterface", System.StringComparison.Ordinal))
            .Select(x => new object[] { x });

        protected override ISyntaxCodeGenerator CreateApiGenerator(ApiProjectOptions apiProject)
        {
            // Verify spec file supported for unit test
            Assert.Single(apiProject.Document.Paths);
            var urlPath = apiProject.Document.Paths.First();
            Assert.False(urlPath.IsPathStartingSegmentName(FocusOnSegment));
            Assert.Single(urlPath.Value.Operations);
            var (operationType, openApiOperation) = urlPath.Value.Operations.First();

            // Construct SUT
            return new SyntaxGeneratorContractInterface(
                        apiProject,
                        urlPath.Value.Parameters,
                        operationType,
                        openApiOperation,
                        FocusOnSegment,
                        urlPath.Value.HasParameters() || openApiOperation.HasParametersOrRequestBody());
        }

        [Theory(DisplayName = "Api Contract Interface")]
        [MemberData(nameof(YamlFiles))]
        public Task ExecuteGeneratorTest(YamlSpecFile specFile)
        {
            Assert.NotNull(specFile.FilePath);
            return ExecuteTest(specFile);
        }
    }
}