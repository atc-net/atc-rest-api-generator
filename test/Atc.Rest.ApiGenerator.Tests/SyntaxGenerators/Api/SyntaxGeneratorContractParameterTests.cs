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
    public class SyntaxGeneratorContractParameterTests : SyntaxGeneratorTestBase
    {
        public static IEnumerable<object[]> TestInput { get; } = AllTestInput
            .Where(x => x.TestDirectory.Contains("ContractParameter", System.StringComparison.Ordinal))
            .Select(x => new object[] { x });

        protected override ISyntaxCodeGenerator CreateApiGenerator(ApiProjectOptions apiProject)
        {
            // Verify spec file supported for unit test
            Assert.Single(apiProject.Document.Paths);
            var urlPath = apiProject.Document.Paths.First();
            Assert.False(urlPath.IsPathStartingSegmentName(FocusOnSegment));
            Assert.Single(urlPath.Value.Operations);
            var (operationType, openApiOperation) = urlPath.Value.Operations.First();
            Assert.True(openApiOperation.HasParametersOrRequestBody() || urlPath.Value.HasParameters());

            // Construct SUT
            return new SyntaxGeneratorContractParameter(
                        apiProject,
                        urlPath.Value.Parameters,
                        operationType,
                        openApiOperation,
                        FocusOnSegment);
        }

        [Theory(DisplayName = "Api Contract Parameter")]
        [MemberData(nameof(TestInput))]
        public Task ExecuteGeneratorTest(GeneratorTestInput input)
        {
            return VerifyGeneratedOutput(input);
        }
    }
}