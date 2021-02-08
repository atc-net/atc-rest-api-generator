using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Atc.Rest.ApiGenerator.Helpers;
using Atc.Rest.ApiGenerator.Models;
using Atc.Rest.ApiGenerator.SyntaxGenerators;
using Atc.Rest.ApiGenerator.SyntaxGenerators.Api;
using Atc.Rest.ApiGenerator.Tests.Helpers.CodeGenerator;
using VerifyXunit;
using Xunit;

namespace Atc.Rest.ApiGenerator.Tests.SyntaxGenerators
{
    [UsesVerify]
    public class EndpointControllersTest : SyntaxGeneratorTestBase
    {
        public static IEnumerable<object[]> YamlFiles { get; } = AllFiles
            .Where(x => x.FilePath.Contains("EndpointControllers", System.StringComparison.Ordinal))
            .Select(x => new object[] { x });

        protected override ISyntaxCodeGenerator CreateApiGenerator(ApiProjectOptions apiProject)
        {
            // Verify spec file suported for unit test
            Assert.Single(apiProject.BasePathSegmentNames);
            var segmentName = apiProject.BasePathSegmentNames.First();
            var operationSchemaMappings = OpenApiOperationSchemaMapHelper.CollectMappings(apiProject.Document);

            // Construct SUT
            return new SyntaxGeneratorEndpointControllers(apiProject, operationSchemaMappings, segmentName);
        }

        [Theory(DisplayName = "Contract Controllers")]
        [MemberData(nameof(YamlFiles))]
        public async Task ExecuteGeneratorTest(YamlSpecFile specFile)
        {
            Assert.NotNull(specFile?.FilePath);
            await ExecuteTest(specFile);
        }
    }
}