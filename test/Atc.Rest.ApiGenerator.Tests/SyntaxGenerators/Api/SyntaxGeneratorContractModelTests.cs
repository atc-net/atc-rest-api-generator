using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Atc.Rest.ApiGenerator.Models;
using Atc.Rest.ApiGenerator.SyntaxGenerators;
using Atc.Rest.ApiGenerator.SyntaxGenerators.Api;
using Atc.Rest.ApiGenerator.Tests.XUnitTestTypes.CodeGenerator;
using VerifyXunit;
using Xunit;

namespace Atc.Rest.ApiGenerator.Tests.SyntaxGenerators.Api
{
    [UsesVerify]
    public class SyntaxGeneratorContractModelTests : SyntaxGeneratorTestBase
    {
        public static IEnumerable<object[]> TestInput { get; } = AllTestInput
            .Where(x => x.TestDirectory.Contains("ContractModel", System.StringComparison.Ordinal))
            .Select(x => new object[] { x });

        protected override ISyntaxCodeGenerator CreateApiGenerator(ApiProjectOptions apiProject)
        {
            // Verify spec file supported for unit test
            Assert.Single(apiProject.Document.Components.Schemas);
            var schema = apiProject.Document.Components.Schemas.First();

            // Construct SUT
            return new SyntaxGeneratorContractModel(apiProject, schema.Key, schema.Value, FocusOnSegment);
        }

        [Theory(DisplayName = "Api Contract Model")]
        [MemberData(nameof(TestInput))]
        public Task ExecuteGeneratorTest(GeneratorTestInput specFile)
        {
            return ExecuteTest(specFile);
        }
    }
}