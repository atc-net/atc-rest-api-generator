using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Atc.Rest.ApiGenerator.Helpers;
using Atc.Rest.ApiGenerator.Models;
using Atc.Rest.ApiGenerator.SyntaxGenerators;
using Atc.Rest.ApiGenerator.SyntaxGenerators.Api;
using Atc.Rest.ApiGenerator.Tests.XUnitTestTypes.CodeGenerator;
using VerifyXunit;
using Xunit;

namespace Atc.Rest.ApiGenerator.Tests.SyntaxGenerators.Api
{
    [UsesVerify]
    public class SyntaxGeneratorEndpointControllersTests : SyntaxGeneratorTestBase
    {
        public static IEnumerable<object[]> TestInput { get; } = AllTestInput
            .Where(x => x.TestDirectory.Contains("EndpointControllers", System.StringComparison.Ordinal))
            .Select(x => new object[] { x });

        protected override ISyntaxCodeGenerator CreateApiGenerator(ApiProjectOptions apiProject)
        {
            // Verify spec file supported for unit test
            Assert.Single(apiProject.BasePathSegmentNames);
            var segmentName = apiProject.BasePathSegmentNames.First();
            var operationSchemaMappings = OpenApiOperationSchemaMapHelper.CollectMappings(apiProject.Document);

            // Construct SUT
            return new SyntaxGeneratorEndpointControllers(apiProject, operationSchemaMappings, segmentName);
        }

        [Theory(DisplayName = "Api Contract Controllers")]
        [MemberData(nameof(TestInput))]
        public Task ExecuteGeneratorTest(GeneratorTestInput input)
        {
            return VerifyGeneratedOutput(input);
        }
    }
}