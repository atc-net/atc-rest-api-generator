using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Atc.Rest.ApiGenerator.Models;
using Atc.Rest.ApiGenerator.SyntaxGenerators.Api;
using Atc.Rest.ApiGenerator.SyntaxGenerators.Api.Interfaces;
using Atc.Rest.ApiGenerator.Tests.XUnitTestTypes.CodeGenerator;
using Microsoft.OpenApi.Models;
using VerifyXunit;
using Xunit;

namespace Atc.Rest.ApiGenerator.Tests.SyntaxGenerators.Api
{
    [UsesVerify]
    public class SyntaxGeneratorContractModelsTests : SyntaxGeneratorContractModelsTestBase
    {
        public static IEnumerable<object[]> TestInput { get; } = AllTestInput
            .Where(x => x.TestDirectory.Contains("ContractMultiModels", StringComparison.Ordinal))
            .Select(x => new object[] { x });

        protected override ISyntaxGeneratorContractModels CreateGenerator(ApiProjectOptions apiProject)
        {
            // Verify spec file supported for unit test
            Assert.True(apiProject.Document.Components.Schemas.Count > 0);

            // Construct SUT
            var apiOperationSchemaMaps = apiProject.Document.Components.Schemas
                .Select(schema => new ApiOperationSchemaMap(schema.Key, SchemaMapLocatedAreaType.Response, FocusOnSegment, OperationType.Get, parentSchemaKey: null))
                .ToList();

            return new SyntaxGeneratorContractModels(apiProject, apiOperationSchemaMaps, FocusOnSegment);
        }

        [Theory(DisplayName = "Api Contract Model")]
        [MemberData(nameof(TestInput))]
        public Task ExecuteGeneratorTest(GeneratorTestInput input)
        {
            return VerifyGeneratedOutput(input);
        }
    }
}